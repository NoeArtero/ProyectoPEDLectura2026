using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros
{
    public static class GestorLibros
    {
        // Propiedad estática existe una sola pila para toda la aplicación
        // Permite que diferentes vistas (UserControls) accedan a la misma pila
        public static PilaLibros HistorialLibros { get; } = new PilaLibros();
    }
}
