using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros
{
    public static class GestorLibros
    {
        // Propiedad estática existe una sola pila para toda la aplicación
        // Permite que diferentes vistas (UserControls) accedan a la misma pila
        public static PilaLibros HistorialLibros { get; } = new PilaLibros();


        private static string rutaArchivo = "HistorialLibros.txt";

        //Agregar libro y guardar
        public static void AgregarLibro(ArchivoAdjunto libro)
        {
            HistorialLibros.ApilarLibro(libro);
            GuardarEnArchivo();
        }


        // Guardar en TXT
        public static void GuardarEnArchivo()
        {
            List<string> lineas = new List<string>();

            foreach (var libro in HistorialLibros.ObtenerLibros())
            {
                lineas.Add($"{libro.Codigo}|{libro.NombreArchivo}");
            }

            File.WriteAllLines(rutaArchivo, lineas);
        }


      public static void CargarDesdeArchivo()
        {
            if (!File.Exists(rutaArchivo))
                return;

            var lineas = File.ReadAllLines(rutaArchivo);

            foreach (var linea in lineas)
            {
                var datos = linea.Split('|');

                ArchivoAdjunto libro = new ArchivoAdjunto
                {
                    Codigo = datos[0],
                    NombreArchivo = datos[1]
                };

                HistorialLibros.ApilarLibro(libro);
            }
        }
    }
}
