using ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros;


namespace ProyectoPEDLectura.extras.EstructurasPersonalizadas
{
    public delegate void AccionLibro(ArchivoAdjunto libro);

    public class ListaLibros
    {
        private NodoLibros? primero;
        private NodoLibros? ultimo;
        private int cantidad;

        public int Cantidad
        {
            get { return cantidad; }
        }

        public bool EstaVacia()
        {
            return primero == null;
        }

        public void Agregar(ArchivoAdjunto libro)
        {
            if (libro == null)
                throw new ArgumentNullException(nameof(libro));

            NodoLibros nuevoNodo = new NodoLibros(libro);

            if (EstaVacia())
            {
                primero = nuevoNodo;
                ultimo = nuevoNodo;
            }
            else
            {
                ultimo!.Siguiente = nuevoNodo;
                ultimo = nuevoNodo;
            }

            cantidad++;
        }

        public ArchivoAdjunto? BuscarPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            NodoLibros? actual = primero;

            while (actual != null)
            {
                if (string.Equals(actual.Libro.Codigo, codigo, StringComparison.OrdinalIgnoreCase))
                {
                    return actual.Libro;
                }

                actual = actual.Siguiente;
            }

            return null;
        }

        public bool ExisteCodigo(string codigo)
        {
            return BuscarPorCodigo(codigo) != null;
        }

        public bool EliminarPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            NodoLibros? actual = primero;
            NodoLibros? anterior = null;

            while (actual != null)
            {
                if (string.Equals(actual.Libro.Codigo, codigo, StringComparison.OrdinalIgnoreCase))
                {
                    if (anterior == null)
                    {
                        primero = actual.Siguiente;
                    }
                    else
                    {
                        anterior.Siguiente = actual.Siguiente;
                    }

                    if (actual == ultimo)
                    {
                        ultimo = anterior;
                    }

                    cantidad--;
                    return true;
                }

                anterior = actual;
                actual = actual.Siguiente;
            }

            return false;
        }

        public void Limpiar()
        {
            primero = null;
            ultimo = null;
            cantidad = 0;
        }

        public void Recorrer(AccionLibro accion)
        {
            if (accion == null)
                throw new ArgumentNullException(nameof(accion));

            NodoLibros? actual = primero;

            while (actual != null)
            {
                accion(actual.Libro);
                actual = actual.Siguiente;
            }
        }

        public ArchivoAdjunto? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidad)
                return null;

            NodoLibros? actual = primero;
            int contador = 0;

            while (actual != null)
            {
                if (contador == indice)
                {
                    return actual.Libro;
                }

                contador++;
                actual = actual.Siguiente;
            }

            return null;
        }
    }
}
