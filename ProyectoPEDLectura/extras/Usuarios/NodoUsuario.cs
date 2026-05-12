namespace ProyectoPEDLectura.extras.Usuarios
{
    public class NodoUsuario
    {
        public Usuario Usuario { get; set; }
        public NodoUsuario? Siguiente { get; set; }

        public NodoUsuario(Usuario usuario)
        {
            Usuario = usuario;
            Siguiente = null;
        }
    }
}