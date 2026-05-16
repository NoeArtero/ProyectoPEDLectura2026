using System;

namespace ProyectoPEDLectura.extras.LecturaDiaria
{
    public class LecturaDiaria
    {
        public string CodigoLibro { get; set; } = "";
        public DateTime Fecha { get; set; }
        public int PaginasLeidasHoy { get; set; }
        public bool MetaNotificada { get; set; }
    }
}