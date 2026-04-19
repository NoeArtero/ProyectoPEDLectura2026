using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros
{
    public class ArchivoAdjunto
    {
        public string? Codigo { get; set; }
        public string? NombreArchivo { get; set; }
        public string? RutaArchivo { get; set; }
        public string? Extension { get; set; }
        public string? Categoria { get; set; }
        public int NumeroPaginas { get; set; }   //NUEVO
        public Image? VistaPrevia { get; set; }
    }
}