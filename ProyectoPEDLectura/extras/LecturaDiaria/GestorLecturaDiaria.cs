using ProyectoPEDLectura.extras.EstructurasPersonalizadas;
using ProyectoPEDLectura.extras.Usuarios;
using System;
using System.IO;
using System.Text;

namespace ProyectoPEDLectura.extras.LecturaDiaria
{
    public static class GestorLecturaDiaria
    {
        private static ListaLecturaDiaria lecturas = new ListaLecturaDiaria();
        private static string codigoUsuarioCargado = "";

        // Devuelve el código del usuario que tiene la sesión activa. Lanza excepción si no hay sesión.
        private static string ObtenerCodigoUsuarioActivo()
        {
            if (!SesionActual.HaySesionActiva || SesionActual.UsuarioActivo == null)
                throw new Exception("No hay una sesión activa.");

            return SesionActual.UsuarioActivo.Codigo;
        }

        // Construye y devuelve la ruta del archivo de lecturas diarias para el usuario activo,
        // creando la estructura de carpetas si es necesario.
        private static string ObtenerRutaArchivo()
        {
            string codigoUsuario = ObtenerCodigoUsuarioActivo();

            GestorRutasUsuario.CrearEstructuraUsuario(codigoUsuario);

            return GestorRutasUsuario.ObtenerRutaLecturaDiariaUsuario(codigoUsuario);
        }

        // Asegura que las lecturas en memoria correspondan al usuario activo actual;
        // si no, carga desde archivo.
        private static void AsegurarUsuarioCargado()
        {
            string codigoUsuarioActual = ObtenerCodigoUsuarioActivo();

            if (!string.Equals(codigoUsuarioCargado, codigoUsuarioActual, StringComparison.OrdinalIgnoreCase))
            {
                CargarDesdeArchivo();
            }
        }

        // Registra el avance de lectura entre dos posiciones, actualiza el registro del día,
        // verifica si se alcanzó la meta diaria y devuelve el resultado para mostrar felicitación si aplica.
        public static ResultadoMetaDiaria RegistrarAvanceYVerificarMeta(
            string codigoLibro,
            int paginasAntes,
            int paginasAhora,
            int metaPaginas
        )
        {
            ResultadoMetaDiaria resultado = new ResultadoMetaDiaria
            {
                DebeMostrarFelicitacion = false,
                PaginasLeidasHoy = 0,
                MetaPaginas = metaPaginas,
                Mensaje = ""
            };

            if (string.IsNullOrWhiteSpace(codigoLibro))
                return resultado;

            int paginasAvanzadas = paginasAhora - paginasAntes;

            // Si el usuario bajó páginas o no avanzó, no se cuenta como lectura diaria.
            if (paginasAvanzadas <= 0)
                return resultado;

            // Si no hay meta válida, tampoco se muestra felicitación.
            if (metaPaginas <= 0)
                return resultado;

            AsegurarUsuarioCargado();

            DateTime fechaHoy = DateTime.Today;

            LecturaDiaria? lecturaHoy = lecturas.BuscarPorLibroYFecha(codigoLibro, fechaHoy);

            if (lecturaHoy == null)
            {
                lecturaHoy = new LecturaDiaria
                {
                    CodigoLibro = codigoLibro,
                    Fecha = fechaHoy,
                    PaginasLeidasHoy = 0,
                    MetaNotificada = false
                };

                lecturas.Agregar(lecturaHoy);
            }

            int paginasAntesHoy = lecturaHoy.PaginasLeidasHoy;

            lecturaHoy.PaginasLeidasHoy += paginasAvanzadas;

            resultado.PaginasLeidasHoy = lecturaHoy.PaginasLeidasHoy;

            if (!lecturaHoy.MetaNotificada &&
                paginasAntesHoy < metaPaginas &&
                lecturaHoy.PaginasLeidasHoy >= metaPaginas)
            {
                lecturaHoy.MetaNotificada = true;

                resultado.DebeMostrarFelicitacion = true;
                resultado.Mensaje =
                    $"¡Felicidades! Hoy has leído {lecturaHoy.PaginasLeidasHoy} páginas y cumpliste tu meta diaria de {metaPaginas} páginas.";
            }

            GuardarEnArchivo();

            return resultado;
        }

        // Devuelve cuántas páginas se han leido hoy para el libro indicado.
        public static int ObtenerPaginasLeidasHoy(string codigoLibro)
        {
            if (string.IsNullOrWhiteSpace(codigoLibro))
                return 0;

            AsegurarUsuarioCargado();

            LecturaDiaria? lecturaHoy = lecturas.BuscarPorLibroYFecha(codigoLibro, DateTime.Today);

            if (lecturaHoy == null)
                return 0;

            return lecturaHoy.PaginasLeidasHoy;
        }

        // Carga las lecturas diarias del archivo del usuario activo a la memoria.
        // Si el archivo no existe, lo crea vacío.
        public static void CargarDesdeArchivo()
        {
            lecturas.Limpiar();

            string codigoUsuarioActual = ObtenerCodigoUsuarioActivo();
            codigoUsuarioCargado = codigoUsuarioActual;

            string ruta = ObtenerRutaArchivo();

            if (!File.Exists(ruta))
            {
                File.Create(ruta).Close();
                return;
            }

            string[] lineas = File.ReadAllLines(ruta, Encoding.UTF8);

            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea))
                    continue;

                string[] datos = linea.Split('|');

                if (datos.Length < 4)
                    continue;

                LecturaDiaria lectura = new LecturaDiaria
                {
                    CodigoLibro = datos[0],
                    Fecha = DateTime.TryParse(datos[1], out DateTime fecha) ? fecha : DateTime.Today,
                    PaginasLeidasHoy = int.TryParse(datos[2], out int paginas) ? paginas : 0,
                    MetaNotificada = bool.TryParse(datos[3], out bool notificada) && notificada
                };

                lecturas.Agregar(lectura);
            }
        }
            
        // Guarda en el archivo del usuario todas las lecturas diarias almacenadas en memoria.
        public static void GuardarEnArchivo()
        {
            string ruta = ObtenerRutaArchivo();

            StringBuilder contenido = new StringBuilder();

            lecturas.Recorrer(lectura =>
            {
                contenido.AppendLine(
                    $"{lectura.CodigoLibro}|" +
                    $"{lectura.Fecha:yyyy-MM-dd}|" +
                    $"{lectura.PaginasLeidasHoy}|" +
                    $"{lectura.MetaNotificada}"
                );
            });

            File.WriteAllText(ruta, contenido.ToString(), Encoding.UTF8);
        }

        // Limpia la memoria de lecturas y resetea el código de usuario cargado.
        public static void LimpiarMemoria()
        {
            lecturas.Limpiar();
            codigoUsuarioCargado = "";
        }
    }
}