using ProyectoPEDLectura.extras.LecturaDiaria;

namespace ProyectoPEDLectura.extras.EstructurasPersonalizadas
{
    public delegate void AccionLecturaDiaria(LecturaDiaria.LecturaDiaria lectura);

    public class ListaLecturaDiaria
    {
        private class NodoLecturaDiaria
        {
            public LecturaDiaria.LecturaDiaria Dato { get; set; }
            public NodoLecturaDiaria? Siguiente { get; set; }

            public NodoLecturaDiaria(LecturaDiaria.LecturaDiaria dato)
            {
                Dato = dato;
                Siguiente = null;
            }
        }

        private NodoLecturaDiaria? inicio;
        private NodoLecturaDiaria? fin;
        private int cantidad;

        public int Cantidad
        {
            get { return cantidad; }
        }

        public void Agregar(LecturaDiaria.LecturaDiaria lectura)
        {
            NodoLecturaDiaria nuevo = new NodoLecturaDiaria(lectura);

            if (inicio == null)
            {
                inicio = nuevo;
                fin = nuevo;
            }
            else
            {
                fin!.Siguiente = nuevo;
                fin = nuevo;
            }

            cantidad++;
        }

        public LecturaDiaria.LecturaDiaria? BuscarPorLibroYFecha(string codigoLibro, DateTime fecha)
        {
            NodoLecturaDiaria? actual = inicio;

            while (actual != null)
            {
                bool mismoLibro = string.Equals(
                    actual.Dato.CodigoLibro,
                    codigoLibro,
                    StringComparison.OrdinalIgnoreCase
                );

                bool mismaFecha = actual.Dato.Fecha.Date == fecha.Date;

                if (mismoLibro && mismaFecha)
                {
                    return actual.Dato;
                }

                actual = actual.Siguiente;
            }

            return null;
        }

        public void Recorrer(AccionLecturaDiaria accion)
        {
            NodoLecturaDiaria? actual = inicio;

            while (actual != null)
            {
                accion(actual.Dato);
                actual = actual.Siguiente;
            }
        }

        public void Limpiar()
        {
            inicio = null;
            fin = null;
            cantidad = 0;
        }
    }
}