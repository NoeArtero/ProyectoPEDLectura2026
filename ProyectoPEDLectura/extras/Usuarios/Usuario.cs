
namespace ProyectoPEDLectura.extras.Usuarios
{
    public class Usuario
    {
        public string Codigo { get; set; } = "";
        public string NombreUsuario { get; set; } = "";
        public string ContrasenaHash { get; set; } = "";
        public string Genero { get; set; } = "";
        public string RutaFotoPerfil { get; set; } = "";
        public DateTime FechaCreacion { get; set; }
    }


}