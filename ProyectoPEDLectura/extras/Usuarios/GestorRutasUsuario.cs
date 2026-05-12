

namespace ProyectoPEDLectura.extras.Usuarios
{
    public static class GestorRutasUsuario
    {
        public static string CarpetaDatos
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos");
            }
        }

        public static string ArchivoUsuarios
        {
            get
            {
                return Path.Combine(CarpetaDatos, "Usuarios.txt");
            }
        }

        public static string CarpetaUsuarios
        {
            get
            {
                return Path.Combine(CarpetaDatos, "Usuarios");
            }
        }

        public static string ObtenerCarpetaUsuario(string codigoUsuario)
        {
            return Path.Combine(CarpetaUsuarios, codigoUsuario);
        }

        public static string ObtenerRutaLibrosUsuario(string codigoUsuario)
        {
            return Path.Combine(ObtenerCarpetaUsuario(codigoUsuario), "libros.txt");
        }

        public static string ObtenerRutaAnotacionesUsuario(string codigoUsuario)
        {
            return Path.Combine(ObtenerCarpetaUsuario(codigoUsuario), "anotaciones.txt");
        }

        public static string ObtenerRutaMetasUsuario(string codigoUsuario)
        {
            return Path.Combine(ObtenerCarpetaUsuario(codigoUsuario), "metas.txt");
        }

        public static void CrearEstructuraBase()
        {
            Directory.CreateDirectory(CarpetaDatos);
            Directory.CreateDirectory(CarpetaUsuarios);

            if (!File.Exists(ArchivoUsuarios))
            {
                File.Create(ArchivoUsuarios).Close();
            }
        }

        public static void CrearEstructuraUsuario(string codigoUsuario)
        {
            string carpetaUsuario = ObtenerCarpetaUsuario(codigoUsuario);

            Directory.CreateDirectory(carpetaUsuario);

            CrearArchivoSiNoExiste(ObtenerRutaLibrosUsuario(codigoUsuario));
            CrearArchivoSiNoExiste(ObtenerRutaAnotacionesUsuario(codigoUsuario));
            CrearArchivoSiNoExiste(ObtenerRutaMetasUsuario(codigoUsuario));
        }

        private static void CrearArchivoSiNoExiste(string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
            {
                File.Create(rutaArchivo).Close();
            }
        }
    }
}