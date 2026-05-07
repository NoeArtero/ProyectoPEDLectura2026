using ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros;


namespace ProyectoPEDLectura.extras.EstructurasPersonalizadas
{
    public class NodoLibros
    {
        public ArchivoAdjunto Libro { get; set; }
        public NodoLibros? Siguiente { get; set; }

        public NodoLibros(ArchivoAdjunto libro)
        {
            Libro = libro ?? throw new ArgumentNullException(nameof(libro));
            Siguiente = null;
        }
    }
}
