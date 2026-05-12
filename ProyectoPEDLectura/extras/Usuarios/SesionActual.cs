namespace ProyectoPEDLectura.extras.Usuarios
{
    public static class SesionActual
    {
        public static Usuario? UsuarioActivo { get; private set; }

        public static bool HaySesionActiva
        {
            get { return UsuarioActivo != null; }
        }

        public static void IniciarSesion(Usuario usuario)
        {
            UsuarioActivo = usuario;
        }

        public static void CerrarSesion()
        {
            UsuarioActivo = null;
        }

        public static string ObtenerCodigoUsuarioActivo()
        {
            if (UsuarioActivo == null)
                return "";

            return UsuarioActivo.Codigo;
        }
    }
}
