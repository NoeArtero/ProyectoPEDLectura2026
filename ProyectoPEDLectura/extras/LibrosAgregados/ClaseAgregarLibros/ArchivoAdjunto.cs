using System;
using System.Drawing;

namespace ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros
{
    public class ArchivoAdjunto
    {
        public string? Codigo { get; set; }
        public string? NombreArchivo { get; set; }
        public string? RutaArchivo { get; set; }
        public string? Extension { get; set; }
        public string? Categoria { get; set; }
        public int NumeroPaginas { get; set; }

        // NUEVO: páginas que el usuario ya leyó
        public int PaginasLeidas { get; set; }

        public Image? VistaPrevia { get; set; }
        public DateTime FechaAgregado { get; set; }

        // NUEVO: porcentaje calculado automáticamente
        public int ProgresoPorcentaje
        {
            get
            {
                if (NumeroPaginas <= 0)
                    return 0;

                int porcentaje = (PaginasLeidas * 100) / NumeroPaginas;

                if (porcentaje < 0)
                    return 0;

                if (porcentaje > 100)
                    return 100;

                return porcentaje;
            }
        }
    }
}