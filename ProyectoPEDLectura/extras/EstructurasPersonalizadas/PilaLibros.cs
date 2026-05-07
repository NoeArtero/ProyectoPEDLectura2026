using ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoPEDLectura.extras.EstructurasPersonalizadas
{
    public class PilaLibros
    {
        private NodoLibros? cima;
        private int cantidad;

        public int Cantidad
        {
            get { return cantidad; }
        }

        public bool EstaVacia()
        {
            return cima == null;
        }

        public void ApilarLibro(ArchivoAdjunto libro)
        {
            if (libro == null)
                throw new ArgumentNullException(nameof(libro));

            NodoLibros nuevoNodo = new NodoLibros(libro);
            nuevoNodo.Siguiente = cima;
            cima = nuevoNodo;
            cantidad++;
        }

        public ArchivoAdjunto? DesapilarLibro()
        {
            if (EstaVacia())
                return null;

            ArchivoAdjunto libro = cima!.Libro;
            cima = cima.Siguiente;
            cantidad--;

            return libro;
        }

        public ArchivoAdjunto? VerCima()
        {
            if (EstaVacia())
                return null;

            return cima!.Libro;
        }

        public ArchivoAdjunto? BuscarPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            NodoLibros? actual = cima;

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

        public bool EliminarPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            PilaLibros auxiliar = new PilaLibros();
            bool eliminado = false;

            while (!EstaVacia())
            {
                ArchivoAdjunto? libro = DesapilarLibro();

                if (libro == null)
                    continue;

                if (!eliminado &&
                    string.Equals(libro.Codigo, codigo, StringComparison.OrdinalIgnoreCase))
                {
                    eliminado = true;
                    continue;
                }

                auxiliar.ApilarLibro(libro);
            }

            while (!auxiliar.EstaVacia())
            {
                ArchivoAdjunto? libro = auxiliar.DesapilarLibro();

                if (libro != null)
                {
                    ApilarLibro(libro);
                }
            }

            return eliminado;
        }

        public void Recorrer(AccionLibro accion)
        {
            if (accion == null)
                throw new ArgumentNullException(nameof(accion));

            NodoLibros? actual = cima;

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

            NodoLibros? actual = cima;
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

        public ArchivoAdjunto[] ObtenerComoArreglo()
        {
            ArchivoAdjunto[] arreglo = new ArchivoAdjunto[cantidad];

            NodoLibros? actual = cima;
            int indice = 0;

            while (actual != null)
            {
                arreglo[indice] = actual.Libro;
                indice++;
                actual = actual.Siguiente;
            }

            return arreglo;
        }

        public void Limpiar()
        {
            cima = null;
            cantidad = 0;
        }
    }
}
