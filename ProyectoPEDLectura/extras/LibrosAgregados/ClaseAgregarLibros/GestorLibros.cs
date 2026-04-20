using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros
{
    public static class GestorLibros
    {
        // Archivo TXT en la raíz de la aplicación
        private static readonly string rutaArchivo = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "HistorialLibros.txt"
        );

        // Lista en memoria
        private static List<ArchivoAdjunto> libros = new List<ArchivoAdjunto>();

        // Se ejecuta una sola vez al usar la clase
        static GestorLibros()
        {
            CrearArchivoSiNoExiste();
            CargarDesdeArchivo();
        }

        // Crear archivo vacío si no existe
        private static void CrearArchivoSiNoExiste()
        {
            if (!File.Exists(rutaArchivo))
            {
                File.Create(rutaArchivo).Close();
            }
        }

        // Método para limpiar separadores y evitar errores en el TXT
        private static string LimpiarTexto(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "";

            return texto.Replace("|", "/").Replace(Environment.NewLine, " ");
        }

        // Agregar libro
        public static void AgregarLibro(ArchivoAdjunto libro)
        {
            if (libro == null)
                throw new ArgumentNullException(nameof(libro));

            // Evitar códigos repetidos
            if (libros.Any(l => l.Codigo == libro.Codigo))
                throw new Exception("Ya existe un libro con ese código.");

            libros.Add(libro);
            GuardarEnArchivo();
        }

        // Obtener todos los libros
        public static List<ArchivoAdjunto> ObtenerLibros()
        {
            return new List<ArchivoAdjunto>(libros);
        }

        // Buscar un libro por código
        public static ArchivoAdjunto? BuscarPorCodigo(string codigo)
        {
            return libros.FirstOrDefault(l => l.Codigo == codigo);
        }

        public static List<ArchivoAdjunto> HistorialLibros
        {
            get { return libros; }
        }

        // Eliminar libro por código
        public static bool EliminarLibro(string codigo)
        {
            var libro = libros.FirstOrDefault(l => l.Codigo == codigo);

            if (libro == null)
                return false;

            libros.Remove(libro);
            GuardarEnArchivo();
            return true;
        }

        // Guardar toda la lista en el TXT
        public static void GuardarEnArchivo()
        {
            List<string> lineas = new List<string>();

            foreach (var libro in libros)
            {
                lineas.Add(
                    $"{LimpiarTexto(libro.Codigo)}|" +
                    $"{LimpiarTexto(libro.NombreArchivo)}|" +
                    $"{LimpiarTexto(libro.RutaArchivo)}|" +
                    $"{LimpiarTexto(libro.Categoria)}|" +
                    $"{libro.NumeroPaginas}|" +
                    $"{libro.FechaAgregado:yyyy-MM-dd}"
                );
            }

            File.WriteAllLines(rutaArchivo, lineas);
        }

        // Leer desde el TXT
        public static void CargarDesdeArchivo()
        {
            libros.Clear();

            if (!File.Exists(rutaArchivo))
                return;

            var lineas = File.ReadAllLines(rutaArchivo);

            foreach (var linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea))
                    continue;

                var datos = linea.Split('|');

                if (datos.Length >= 6)
                {
                    ArchivoAdjunto libro = new ArchivoAdjunto
                    {
                        Codigo = datos[0],
                        NombreArchivo = datos[1],
                        RutaArchivo = datos[2],
                        Categoria = datos[3],
                        NumeroPaginas = int.TryParse(datos[4], out int paginas) ? paginas : 0,
                        FechaAgregado = DateTime.TryParse(datos[5], out DateTime fecha) ? fecha : DateTime.Now
                    };

                    libros.Add(libro);
                }
            }
        }
    }
}