using ProyectoPEDLectura.extras.EstructurasPersonalizadas;

namespace ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros
{
    public class PilaLibros
    {
        // Usa la pila personalizada, no Stack<T> de C#
        private ProyectoPEDLectura.extras.EstructurasPersonalizadas.PilaLibros pilaLibros =
            new ProyectoPEDLectura.extras.EstructurasPersonalizadas.PilaLibros();

        public int Cantidad
        {
            get { return pilaLibros.Cantidad; }
        }

        public bool EstaVacia()
        {
            return pilaLibros.EstaVacia();
        }

        public void ApilarLibro(ArchivoAdjunto libro)
        {
            pilaLibros.ApilarLibro(libro);
        }

        public ArchivoAdjunto? DesapilarLibro()
        {
            return pilaLibros.DesapilarLibro();
        }

        public ArchivoAdjunto? VerCima()
        {
            return pilaLibros.VerCima();
        }

        public ArchivoAdjunto? BuscarPorCodigo(string codigo)
        {
            return pilaLibros.BuscarPorCodigo(codigo);
        }

        public bool EliminarPorCodigo(string codigo)
        {
            return pilaLibros.EliminarPorCodigo(codigo);
        }

        public void Recorrer(AccionLibro accion)
        {
            pilaLibros.Recorrer(accion);
        }

        public ArchivoAdjunto? ObtenerPorIndice(int indice)
        {
            return pilaLibros.ObtenerPorIndice(indice);
        }

        public ArchivoAdjunto[] ObtenerLibros()
        {
            return pilaLibros.ObtenerComoArreglo();
        }

        public void Limpiar()
        {
            pilaLibros.Limpiar();
        }
    }
}