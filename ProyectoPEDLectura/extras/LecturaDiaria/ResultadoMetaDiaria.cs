namespace ProyectoPEDLectura.extras.LecturaDiaria
{
    public class ResultadoMetaDiaria
    {
        public bool DebeMostrarFelicitacion { get; set; }
        public int PaginasLeidasHoy { get; set; }
        public int MetaPaginas { get; set; }
        public string Mensaje { get; set; } = "";
    }
}