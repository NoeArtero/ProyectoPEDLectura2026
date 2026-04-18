using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros
{
    public class ArchivoAdjunto
    {
        public string? NombreArchivo { get; set; }
        public string? RutaArchivo { get; set; }
        public string? Extension { get; set; }
        public Image? VistaPrevia { get; set; }
    }
}
