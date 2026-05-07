using ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoPEDLectura.extras.EstructurasPersonalizadas
{
    public class ColaLibros
    {
        private NodoLibros? frente;
        private NodoLibros? final;
        private int cantidad;

        public int Cantidad
        {
            get { return cantidad; }
        }

        public bool EstaVacia()
        {
            return frente == null;
        }

        public void EncolarLibro(ArchivoAdjunto libro)
        {
            if (libro == null)
                throw new ArgumentNullException(nameof(libro));

            NodoLibros nuevoNodo = new NodoLibros(libro);

            if (EstaVacia())
            {
                frente = nuevoNodo;
                final = nuevoNodo;
            }
            else
            {
                final!.Siguiente = nuevoNodo;
                final = nuevoNodo;
            }

            cantidad++;
        }

        public ArchivoAdjunto? DesencolarLibro()
        {
            if (EstaVacia())
                return null;

            ArchivoAdjunto libro = frente!.Libro;
            frente = frente.Siguiente;

            if (frente == null)
            {
                final = null;
            }

            cantidad--;
            return libro;
        }

        public ArchivoAdjunto? VerFrente()
        {
            if (EstaVacia())
                return null;

            return frente!.Libro;
        }

        public ArchivoAdjunto? BuscarPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            NodoLibros? actual = frente;

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

            NodoLibros? actual = frente;
            NodoLibros? anterior = null;

            while (actual != null)
            {
                if (string.Equals(actual.Libro.Codigo, codigo, StringComparison.OrdinalIgnoreCase))
                {
                    if (anterior == null)
                    {
                        frente = actual.Siguiente;
                    }
                    else
                    {
                        anterior.Siguiente = actual.Siguiente;
                    }

                    if (actual == final)
                    {
                        final = anterior;
                    }

                    cantidad--;
                    return true;
                }

                anterior = actual;
                actual = actual.Siguiente;
            }

            return false;
        }

        public void Recorrer(AccionLibro accion)
        {
            if (accion == null)
                throw new ArgumentNullException(nameof(accion));

            NodoLibros? actual = frente;

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

            NodoLibros? actual = frente;
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

            NodoLibros? actual = frente;
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
            frente = null;
            final = null;
            cantidad = 0;
        }
    }
}
