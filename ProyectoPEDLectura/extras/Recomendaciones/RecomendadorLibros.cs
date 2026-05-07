using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros;

namespace ProyectoPEDLectura.extras.Recomendaciones
{
    /// <summary>
    /// Genera una recomendación de lectura a partir de los nombres de los archivos guardados.
    /// Punto de entrada: GenerarRecomendacionAsync() — devuelve un texto listo para mostrar al usuario.
    /// Detecta un "tema" contando palabras relevantes en los nombres (limpia extensiones, símbolos y elimina palabras no útiles),
    /// intenta obtener una recomendación desde la API de Open Library y, si no hay resultado o falla, devuelve una sugerencia local.
    /// Evita recomendar libros ya presentes en el historial comparando títulos normalizados.
    /// Incluye utilidades para limpiar/normalizar texto, parseo seguro de JSON y uso de un HttpClient estático.
    /// </summary>
    public static class RecomendadorLibros
    {
        // HttpClient estático reutilizado para evitar overhead de sockets; se usa en las consultas HTTP externas.
        private static readonly HttpClient clienteHttp = new HttpClient();

        // Método público principal: genera una recomendación combinando detección de tema y consulta externa o fallback local.
        public static async Task<string> GenerarRecomendacionAsync()
        {
            if (GestorLibros.TotalLibros == 0)
            {
                return "Todavía no hay archivos suficientes para generar una recomendación. Agrega algunos documentos desde el apartado Libros.";
            }

            string temaDetectado = ObtenerTemaDesdeNombresArchivos();

            if (string.IsNullOrWhiteSpace(temaDetectado))
            {
                return "No se pudo detectar un tema claro a partir de los nombres de los archivos guardados.";
            }

            string recomendacionExterna = await BuscarRecomendacionEnOpenLibraryAsync(temaDetectado);

            if (!string.IsNullOrWhiteSpace(recomendacionExterna))
            {
                return recomendacionExterna;
            }

            return GenerarRecomendacionLocal(temaDetectado);
        }

        // Recorre todos los nombres de archivo, limpia y normaliza cada palabra, filtra palabras no útiles y cuenta ocurrencias.
        // Devuelve una cadena con las 1-3 palabras más frecuentes como tema.
        private static string ObtenerTemaDesdeNombresArchivos()
        {
            string[] palabras = new string[300];
            int[] conteos = new int[300];
            int cantidadPalabras = 0;

            GestorLibros.RecorrerLibros(libro =>
            {
                if (libro == null || string.IsNullOrWhiteSpace(libro.NombreArchivo))
                    return;

                string nombreLimpio = LimpiarNombreArchivo(libro.NombreArchivo);
                string[] partes = nombreLimpio.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (string parte in partes)
                {
                    string palabra = NormalizarPalabra(parte);

                    if (string.IsNullOrWhiteSpace(palabra))
                        continue;

                    if (EsPalabraNoUtil(palabra))
                        continue;

                    int indiceExistente = BuscarIndicePalabra(palabras, cantidadPalabras, palabra);

                    if (indiceExistente >= 0)
                    {
                        conteos[indiceExistente]++;
                    }
                    else
                    {
                        if (cantidadPalabras < palabras.Length)
                        {
                            palabras[cantidadPalabras] = palabra;
                            conteos[cantidadPalabras] = 1;
                            cantidadPalabras++;
                        }
                    }
                }
            });

            return ConstruirTemaPrincipal(palabras, conteos, cantidadPalabras);
        }

        // Busca si una palabra ya fue registrada en el arreglo; devuelve el índice o -1 si no existe.
        private static int BuscarIndicePalabra(string[] palabras, int cantidad, string palabra)
        {
            for (int i = 0; i < cantidad; i++)
            {
                if (palabras[i].Equals(palabra, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        // Ordena por frecuencia (sin modificar los arreglos) y construye una frase con hasta tres palabras más frecuentes.
        private static string ConstruirTemaPrincipal(string[] palabras, int[] conteos, int cantidad)
        {
            string primera = "";
            string segunda = "";
            string tercera = "";

            int conteoPrimera = 0;
            int conteoSegunda = 0;
            int conteoTercera = 0;

            for (int i = 0; i < cantidad; i++)
            {
                if (conteos[i] > conteoPrimera)
                {
                    tercera = segunda;
                    conteoTercera = conteoSegunda;

                    segunda = primera;
                    conteoSegunda = conteoPrimera;

                    primera = palabras[i];
                    conteoPrimera = conteos[i];
                }
                else if (conteos[i] > conteoSegunda)
                {
                    tercera = segunda;
                    conteoTercera = conteoSegunda;

                    segunda = palabras[i];
                    conteoSegunda = conteos[i];
                }
                else if (conteos[i] > conteoTercera)
                {
                    tercera = palabras[i];
                    conteoTercera = conteos[i];
                }
            }

            string tema = primera;

            if (!string.IsNullOrWhiteSpace(segunda))
                tema += " " + segunda;

            if (!string.IsNullOrWhiteSpace(tercera))
                tema += " " + tercera;

            return tema.Trim();
        }

        // Consulta la API de Open Library buscando títulos relacionados con el tema; filtra libros ya presentes y formatea la recomendación.
        private static async Task<string> BuscarRecomendacionEnOpenLibraryAsync(string tema)
        {
            try
            {
                string consulta = Uri.EscapeDataString(tema);

                string url = $"https://openlibrary.org/search.json?q={consulta}&limit=10&fields=title,author_name,first_publish_year";

                string respuesta = await clienteHttp.GetStringAsync(url);

                using JsonDocument documento = JsonDocument.Parse(respuesta);

                if (!documento.RootElement.TryGetProperty("docs", out JsonElement resultados))
                {
                    return "";
                }

                foreach (JsonElement libroEncontrado in resultados.EnumerateArray())
                {
                    string titulo = ObtenerTexto(libroEncontrado, "title");
                    string autor = ObtenerPrimerAutor(libroEncontrado);
                    string anio = ObtenerNumeroComoTexto(libroEncontrado, "first_publish_year");

                    if (string.IsNullOrWhiteSpace(titulo))
                        continue;

                    if (LibroYaExisteEnHistorial(titulo))
                        continue;

                    if (string.IsNullOrWhiteSpace(autor))
                        autor = "autor no especificado";

                    string textoAnio = string.IsNullOrWhiteSpace(anio)
                        ? ""
                        : $" Publicado por primera vez en {anio}.";

                    return $"Según los nombres de tus archivos guardados, detectamos interés en \"{tema}\". Te recomendamos leer \"{titulo}\" de {autor}.{textoAnio}";
                }

                return "";
            }
            catch
            {
                return "";
            }
        }

        // Extrae de forma segura una propiedad string de un elemento JSON, si existe y es string.
        private static string ObtenerTexto(JsonElement elemento, string propiedad)
        {
            if (elemento.TryGetProperty(propiedad, out JsonElement valor))
            {
                if (valor.ValueKind == JsonValueKind.String)
                    return valor.GetString() ?? "";
            }

            return "";
        }

        // Extrae de forma segura una propiedad numérica y la devuelve como texto; útil para años.
        private static string ObtenerNumeroComoTexto(JsonElement elemento, string propiedad)
        {
            if (elemento.TryGetProperty(propiedad, out JsonElement valor))
            {
                if (valor.ValueKind == JsonValueKind.Number)
                    return valor.GetInt32().ToString();
            }

            return "";
        }

        // Devuelve el primer autor encontrado en el arreglo "author_name" si existe y es de tipo string.
        private static string ObtenerPrimerAutor(JsonElement elemento)
        {
            if (elemento.TryGetProperty("author_name", out JsonElement autores))
            {
                if (autores.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement autor in autores.EnumerateArray())
                    {
                        if (autor.ValueKind == JsonValueKind.String)
                        {
                            return autor.GetString() ?? "";
                        }
                    }
                }
            }

            return "";
        }

        // Recorre el historial de archivos y compara títulos normalizados para evitar recomendar algo que ya tengas.
        private static bool LibroYaExisteEnHistorial(string titulo)
        {
            bool existe = false;
            string tituloNormalizado = NormalizarTexto(titulo);

            GestorLibros.RecorrerLibros(libro =>
            {
                if (existe)
                    return;

                string nombreNormalizado = NormalizarTexto(libro.NombreArchivo);

                if (nombreNormalizado.Contains(tituloNormalizado, StringComparison.OrdinalIgnoreCase) ||
                    tituloNormalizado.Contains(nombreNormalizado, StringComparison.OrdinalIgnoreCase))
                {
                    existe = true;
                }
            });

            return existe;
        }

        // Fallback local si no se obtiene respuesta externa: sugiere buscar un libro sobre el tema detectado.
        private static string GenerarRecomendacionLocal(string tema)
        {
            return $"Según los nombres de tus archivos guardados, parece que estás trabajando temas relacionados con \"{tema}\". Te recomendamos buscar un libro introductorio o avanzado sobre ese tema para complementar tus documentos.";
        }

        // Limpia el nombre de archivo quitando extensiones comunes y reemplazando símbolos por espacios.
        private static string LimpiarNombreArchivo(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return "";

            string limpio = nombre;

            limpio = limpio.Replace(".pdf", "", StringComparison.OrdinalIgnoreCase);
            limpio = limpio.Replace(".docx", "", StringComparison.OrdinalIgnoreCase);
            limpio = limpio.Replace(".txt", "", StringComparison.OrdinalIgnoreCase);
            limpio = limpio.Replace(".jpg", "", StringComparison.OrdinalIgnoreCase);
            limpio = limpio.Replace(".png", "", StringComparison.OrdinalIgnoreCase);

            limpio = limpio.Replace("_", " ");
            limpio = limpio.Replace("-", " ");
            limpio = limpio.Replace(".", " ");
            limpio = limpio.Replace(",", " ");
            limpio = limpio.Replace("(", " ");
            limpio = limpio.Replace(")", " ");

            return limpio.Trim();
        }

        // Normaliza una palabra a minúsculas y elimina caracteres no alfanuméricos.
        private static string NormalizarPalabra(string palabra)
        {
            if (string.IsNullOrWhiteSpace(palabra))
                return "";

            palabra = palabra.Trim().ToLower();

            string limpia = "";

            foreach (char caracter in palabra)
            {
                if (char.IsLetterOrDigit(caracter))
                {
                    limpia += caracter;
                }
            }

            return limpia;
        }

        // Normaliza texto completo usando la limpieza de nombre de archivo y minúsculas.
        private static string NormalizarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "";

            return LimpiarNombreArchivo(texto).Trim().ToLower();
        }

        // Determina si una palabra es irrelevante para el tema (muy corta o está en la lista de palabras no útiles).
        private static bool EsPalabraNoUtil(string palabra)
        {
            if (palabra.Length <= 2)
                return true;

            string[] palabrasNoUtiles =
            {
                    "guia", "guía", "practica", "práctica", "tarea", "trabajo",
                    "documento", "archivo", "final", "parcial", "clase", "semana",
                    "unidad", "tema", "resumen", "actividad", "laboratorio",
                    "capitulo", "capítulo", "pdf", "docx", "txt", "imagen",
                    "nuevo", "copia", "version", "versión", "archivo"
                };

            foreach (string palabraNoUtil in palabrasNoUtiles)
            {
                if (palabra.Equals(palabraNoUtil, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}