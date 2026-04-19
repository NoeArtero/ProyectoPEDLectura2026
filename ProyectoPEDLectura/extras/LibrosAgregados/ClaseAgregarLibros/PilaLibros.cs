using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros
{
    public class PilaLibros
    {
        // Pila principal donde se guardan los libros
        private Stack<ArchivoAdjunto> pilaLibros = new Stack<ArchivoAdjunto>();

        // Push: agrega un libro a la pila
        public void ApilarLibro(ArchivoAdjunto libro)
        {
            if (libro != null)
            {
                pilaLibros.Push(libro);
            }
        }

        // Devuelve todos los libros en forma de lista
        public List<ArchivoAdjunto> ObtenerLibros()
        {
            return pilaLibros.ToList();
        }

        // Elimina un libro por código
        // Como Stack no elimina elementos del medio directamente,
        // se usa una pila auxiliar
        public bool EliminarPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            Stack<ArchivoAdjunto> auxiliar = new Stack<ArchivoAdjunto>();
            bool eliminado = false;

            while (pilaLibros.Count > 0)
            {
                ArchivoAdjunto libro = pilaLibros.Pop();

                if (!eliminado && libro.Codigo == codigo)
                {
                    eliminado = true;
                    continue;
                }

                auxiliar.Push(libro);
            }

            while (auxiliar.Count > 0)
            {
                pilaLibros.Push(auxiliar.Pop());
            }

            return eliminado;
        }
    }
}