using System;
using System.IO;
using System.Text;
using ProyectoPEDLectura.extras.EstructurasPersonalizadas;

namespace ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros
{

    public static class GestorLibros
    {
        // Archivo TXT en la raíz de la aplicación
        private static readonly string rutaArchivo = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "HistorialLibros.txt"
        );

        // Lista propia en memoria
        private static ListaLibros libros = new ListaLibros();

        // Se ejecuta una sola vez al usar la clase
        static GestorLibros()
        {
            CrearArchivoSiNoExiste();
            CargarDesdeArchivo();
        }

        private static void CrearArchivoSiNoExiste()
        {
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

        public static void AgregarLibro(ArchivoAdjunto libro)
        {
            if (libro == null)
                throw new ArgumentNullException(nameof(libro));

            if (string.IsNullOrWhiteSpace(libro.Codigo))
                throw new Exception("El libro debe tener un código.");

            // Evitar códigos repetidos usando nuestra lista propia
            if (libros.ExisteCodigo(libro.Codigo))
                throw new Exception("Ya existe un libro con ese código.");

            libros.Agregar(libro);
            GuardarEnArchivo();
        }

        public static ArchivoAdjunto? BuscarPorCodigo(string codigo)
        {
            return libros.BuscarPorCodigo(codigo);
        }

        public static ListaLibros HistorialLibros
        {
            get { return libros; }
        }

        public static int TotalLibros
        {
            get { return libros.Cantidad; }
        }

        public static void RecorrerLibros(AccionLibro accion)
        {
            libros.Recorrer(accion);
        }

        public static bool EliminarLibro(string codigo)
        {
            bool eliminado = libros.EliminarPorCodigo(codigo);

            if (eliminado)
            {
                GuardarEnArchivo();
            }

            return eliminado;
        }

        public static void GuardarEnArchivo()
        {
            StringBuilder contenido = new StringBuilder();

            libros.Recorrer(libro =>
            {
                contenido.AppendLine(
                    $"{LimpiarTexto(libro.Codigo)}|" +
                    $"{LimpiarTexto(libro.NombreArchivo)}|" +
                    $"{LimpiarTexto(libro.RutaArchivo)}|" +
                    $"{LimpiarTexto(libro.Categoria)}|" +
                    $"{libro.NumeroPaginas}|" +
                    $"{libro.FechaAgregado:yyyy-MM-dd}"
                );
            });

            File.WriteAllText(rutaArchivo, contenido.ToString(), Encoding.UTF8);
        }

        public static void CargarDesdeArchivo()
        {
            libros.Limpiar();

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
                        FechaAgregado = DateTime.TryParse(datos[5], out DateTime fecha) ? fecha : DateTime.Now
                    };

                    libros.Agregar(libro);
                }
            }
        }
    }
}