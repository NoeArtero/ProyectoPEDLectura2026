using System.Text;
using ProyectoPEDLectura.extras.EstructurasPersonalizadas;
using ProyectoPEDLectura.extras.Usuarios;

namespace ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros
{
    public static class GestorLibros
    {
        private static ListaLibros libros = new ListaLibros();

        private static string codigoUsuarioCargado = "";

        static GestorLibros()
        {
            libros = new ListaLibros();
        }

        private static string ObtenerCodigoUsuarioActivo()
        {
            if (!SesionActual.HaySesionActiva || SesionActual.UsuarioActivo == null)
                throw new Exception("No hay una sesión activa. Inicie sesión antes de cargar libros.");

            return SesionActual.UsuarioActivo.Codigo;
        }

        private static string ObtenerRutaArchivo()
        {
            string codigoUsuario = ObtenerCodigoUsuarioActivo();

            GestorRutasUsuario.CrearEstructuraUsuario(codigoUsuario);

            return GestorRutasUsuario.ObtenerRutaLibrosUsuario(codigoUsuario);
        }

        private static void AsegurarUsuarioCargado()
        {
            string codigoUsuarioActual = ObtenerCodigoUsuarioActivo();

            if (!string.Equals(codigoUsuarioCargado, codigoUsuarioActual, StringComparison.OrdinalIgnoreCase))
            {
                CargarDesdeArchivo();
            }
        }

        private static void CrearArchivoSiNoExiste()
        {
            string rutaArchivo = ObtenerRutaArchivo();

            if (!File.Exists(rutaArchivo))
            {
                File.Create(rutaArchivo).Close();
            }
        }

        private static string LimpiarTexto(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "";

            return texto.Replace("|", "/").Replace(Environment.NewLine, " ");
        }

        // NUEVO: evita guardar progreso inválido
        private static int NormalizarPaginasLeidas(int paginasLeidas, int numeroPaginas)
        {
            if (paginasLeidas < 0)
                return 0;

            if (numeroPaginas > 0 && paginasLeidas > numeroPaginas)
                return numeroPaginas;

            return paginasLeidas;
        }

        public static void AgregarLibro(ArchivoAdjunto libro)
        {
            if (libro == null)
                throw new ArgumentNullException(nameof(libro));

            if (string.IsNullOrWhiteSpace(libro.Codigo))
                throw new Exception("El libro debe tener un código.");

            AsegurarUsuarioCargado();

            if (libros.ExisteCodigo(libro.Codigo))
                throw new Exception("Ya existe un libro con ese código en esta sesión.");

            libro.PaginasLeidas = NormalizarPaginasLeidas(libro.PaginasLeidas, libro.NumeroPaginas);

            libros.Agregar(libro);
            GuardarEnArchivo();
        }

        public static ArchivoAdjunto? BuscarPorCodigo(string codigo)
        {
            AsegurarUsuarioCargado();
            return libros.BuscarPorCodigo(codigo);
        }

        public static ListaLibros HistorialLibros
        {
            get
            {
                AsegurarUsuarioCargado();
                return libros;
            }
        }

        public static int TotalLibros
        {
            get
            {
                AsegurarUsuarioCargado();
                return libros.Cantidad;
            }
        }

        public static void RecorrerLibros(AccionLibro accion)
        {
            if (accion == null)
                throw new ArgumentNullException(nameof(accion));

            AsegurarUsuarioCargado();
            libros.Recorrer(accion);
        }

        // NUEVO: actualiza manualmente el progreso por páginas leídas
        public static bool ActualizarProgresoLectura(string codigo, int paginasLeidas)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new Exception("No se recibió el código del libro.");

            AsegurarUsuarioCargado();

            ArchivoAdjunto? libro = libros.BuscarPorCodigo(codigo);

            if (libro == null)
                return false;

            if (paginasLeidas < 0)
                throw new Exception("Las páginas leídas no pueden ser menores que cero.");

            if (libro.NumeroPaginas <= 0)
                throw new Exception("El libro no tiene un número de páginas válido.");

            if (paginasLeidas > libro.NumeroPaginas)
                throw new Exception("Las páginas leídas no pueden ser mayores al total de páginas del libro.");

            libro.PaginasLeidas = paginasLeidas;
            GuardarEnArchivo();

            return true;
        }

        public static bool EliminarLibro(string codigo)
        {
            AsegurarUsuarioCargado();

            bool eliminado = libros.EliminarPorCodigo(codigo);

            if (eliminado)
            {
                GuardarEnArchivo();
            }

            return eliminado;
        }

        public static void GuardarEnArchivo()
        {
            string rutaArchivo = ObtenerRutaArchivo();

            StringBuilder contenido = new StringBuilder();

            libros.Recorrer(libro =>
            {
                contenido.AppendLine(
                    $"{LimpiarTexto(libro.Codigo)}|" +
                    $"{LimpiarTexto(libro.NombreArchivo)}|" +
                    $"{LimpiarTexto(libro.RutaArchivo)}|" +
                    $"{LimpiarTexto(libro.Categoria)}|" +
                    $"{libro.NumeroPaginas}|" +
                    $"{libro.FechaAgregado:yyyy-MM-dd}|" +
                    $"{NormalizarPaginasLeidas(libro.PaginasLeidas, libro.NumeroPaginas)}"
                );
            });

            File.WriteAllText(rutaArchivo, contenido.ToString(), Encoding.UTF8);
        }

        public static void CargarDesdeArchivo()
        {
            libros.Limpiar();

            string codigoUsuarioActual = ObtenerCodigoUsuarioActivo();
            codigoUsuarioCargado = codigoUsuarioActual;

            CrearArchivoSiNoExiste();

            string rutaArchivo = ObtenerRutaArchivo();

            if (!File.Exists(rutaArchivo))
                return;

            string[] lineas = File.ReadAllLines(rutaArchivo, Encoding.UTF8);

            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea))
                    continue;

                string[] datos = linea.Split('|');

                if (datos.Length >= 6)
                {
                    ArchivoAdjunto libro = new ArchivoAdjunto
                    {
                        Codigo = datos[0],
                        NombreArchivo = datos[1],
                        RutaArchivo = datos[2],
                        Categoria = datos[3],
                        NumeroPaginas = int.TryParse(datos[4], out int paginas) ? paginas : 0,
                        FechaAgregado = DateTime.TryParse(datos[5], out DateTime fecha) ? fecha : DateTime.Now,

                        // Si el archivo viejo no tiene páginas leídas, se carga como 0
                        PaginasLeidas = datos.Length >= 7 && int.TryParse(datos[6], out int paginasLeidas) ? paginasLeidas : 0
                    };

                    libro.PaginasLeidas = NormalizarPaginasLeidas(libro.PaginasLeidas, libro.NumeroPaginas);

                    libros.Agregar(libro);
                }
            }
        }

        public static void LimpiarMemoria()
        {
            libros.Limpiar();
            codigoUsuarioCargado = "";
        }
    }
}