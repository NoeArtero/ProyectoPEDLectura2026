using ProyectoPEDLectura.extras;
using ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros; // Para usar la pila y ArchivoAdjunto
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO; // Para usar Path y manejo de archivos
using iText.Kernel.Pdf; // Permite leer la cantidad de páginas de un PDF
using DocumentFormat.OpenXml.Packaging; // Permite abrir archivos Word .docx

namespace ProyectoPEDLectura.Vistas.Libros
{
    public partial class AgregarLibroUC : UserControl
    {

        //Variable para guardar la ruta del archivo seleccionado
        private string rutaArchivoSeleccionado = string.Empty;

        public AgregarLibroUC()
        {
            InitializeComponent();

            txtCodigo.Enabled = true;
            txtCodigo.ReadOnly = false;
            txtPaginas.Enabled = false;
            txtPaginas.Clear();

            CargarCategorias();
        }

        //método para poder cargar archivos
        private void CargarCategorias()
        {
            cmbAgregarProductoCategoria.Items.Clear();

            cmbAgregarProductoCategoria.Items.Add("PDF");
            cmbAgregarProductoCategoria.Items.Add("TXT");
            cmbAgregarProductoCategoria.Items.Add("DOCX");
            cmbAgregarProductoCategoria.Items.Add("Imagen");

            cmbAgregarProductoCategoria.SelectedIndex = 0;
        }

        // Método general para obtener el número de páginas según el tipo de archivo
        private int ObtenerNumeroPaginas(string rutaArchivo)
        {
            string extension = Path.GetExtension(rutaArchivo).ToLower();

            try
            {
                // Si es PDF, obtiene el número real de páginas
                if (extension == ".pdf")
                {
                    return ObtenerPaginasPdf(rutaArchivo);
                }

                // Si es Word, intenta obtener el número de páginas guardado en las propiedades del documento
                if (extension == ".docx")
                {
                    return ObtenerPaginasWord(rutaArchivo);
                }

                // En TXT e imágenes se deja en 0 porque no manejan páginas reales como un PDF o Word
                return 0;
            }
            catch
            {
                // Si ocurre cualquier error al leer el archivo, devuelve 0
                return 0;
            }
        }

        // Método para obtener la cantidad de páginas de un archivo PDF
        private int ObtenerPaginasPdf(string rutaArchivo)
        {
            using (PdfReader reader = new PdfReader(rutaArchivo))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                // Devuelve el total de páginas del PDF
                return pdfDoc.GetNumberOfPages();
            }
        }

        // Método para obtener la cantidad de páginas de un archivo Word (.docx)
        private int ObtenerPaginasWord(string rutaArchivo)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open(rutaArchivo, false))
            {
                // Se obtienen las propiedades extendidas del documento
                var props = doc.ExtendedFilePropertiesPart?.Properties;

                // Si el documento tiene guardada la propiedad Pages, se devuelve ese valor
                if (props != null && props.Pages != null)
                {
                    if (int.TryParse(props.Pages.Text, out int paginas))
                    {
                        return paginas;
                    }
                }
            }

            // Si no se pudo leer la propiedad Pages, devuelve 0
            return 0;
        }

        private void CargarArchivo()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos soportados|*.pdf;*.txt;*.docx;*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string rutaArchivo = openFileDialog.FileName;

                // Guarda la ruta para usarla después
                rutaArchivoSeleccionado = rutaArchivo;

                // Llena automáticamente el nombre del libro
                txtNombreProductoAgregar.Text = Path.GetFileNameWithoutExtension(rutaArchivo);

                // Llena automáticamente la categoría
                cmbAgregarProductoCategoria.Text = Path.GetExtension(rutaArchivo).Replace(".", "").ToUpper();

                // Intenta obtener automáticamente el número de páginas del archivo
                int paginasDetectadas = ObtenerNumeroPaginas(rutaArchivo);

                // Si se pudieron detectar páginas, se muestran y el TextBox queda deshabilitado
                if (paginasDetectadas > 0)
                {
                    txtPaginas.Text = paginasDetectadas.ToString();
                    txtPaginas.Enabled = false;
                }
                else
                {
                    // Si no se pudieron detectar, se limpia y se habilita para ingreso manual
                    txtPaginas.Clear();
                    txtPaginas.Enabled = true;
                }

                Image preview = ObtenerVistaPreviaArchivo(rutaArchivo);

                imgAgregarProd.Image = preview;
                imgAgregarProd.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }


        private Image ObtenerVistaPreviaArchivo(string rutaArchivo)
        {
            string extension = Path.GetExtension(rutaArchivo).ToLower();

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                    return Image.FromFile(rutaArchivo);

                case ".txt":
                    return GenerarPreviewTexto(rutaArchivo);

                case ".pdf":
                    return GenerarPreviewPdf(rutaArchivo);

                case ".docx":
                    return GenerarPreviewDocx(rutaArchivo);

                default:
                    return CrearImagenGenerica("Archivo no compatible");
            }
        }

        private Image GenerarPreviewTexto(string rutaArchivo)
        {
            string contenido = File.ReadAllText(rutaArchivo);
            string vista = contenido.Length > 200 ? contenido.Substring(0, 200) + "..." : contenido;

            Bitmap bmp = new Bitmap(300, 400);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.DrawRectangle(Pens.Gray, 0, 0, bmp.Width - 1, bmp.Height - 1);
                g.DrawString("TXT Preview", new Font("Arial", 14, FontStyle.Bold), Brushes.Black, new PointF(10, 10));
                g.DrawString(vista, new Font("Arial", 10), Brushes.Black, new RectangleF(10, 40, 280, 340));
            }

            return bmp;
        }

        private Image GenerarPreviewDocx(string rutaArchivo)
        {
            return CrearImagenGenerica("DOCX");
        }

        private Image CrearImagenGenerica(string texto)
        {
            Bitmap bmp = new Bitmap(300, 400);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                g.DrawRectangle(Pens.DimGray, 0, 0, bmp.Width - 1, bmp.Height - 1);
                g.DrawString(texto, new Font("Bahnschrift", 18, FontStyle.Bold), Brushes.Black, new PointF(90, 180));
            }

            return bmp;
        }

        private Image GenerarPreviewPdf(string rutaArchivo)
        {
            // Aquí tendría que ir una librería de renderizado PDF
            // Ejemplo :
            // using (var documento = PdfDocument.Load(rutaArchivo))
            // {
            //     return documento.Render(0, 300, 400, true);
            // }

            return CrearImagenGenerica("PDF");
        }

        private void btnAgregarLibro_Click(object sender, EventArgs e)
        {
            // Verifica que se haya seleccionado un archivo
            if (string.IsNullOrWhiteSpace(rutaArchivoSeleccionado))
            {
                Mensaje.MostrarError("Debe seleccionar un archivo antes de agregar el libro.", "Error");
                return;
            }

            // Verifica que los campos principales estén llenos
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                string.IsNullOrWhiteSpace(txtNombreProductoAgregar.Text) ||
                string.IsNullOrWhiteSpace(cmbAgregarProductoCategoria.Text))
            {
                Mensaje.MostrarError("Complete Código, Nombre y Categoría.", "Error");
                return;
            }

            // Obtiene nuevamente el número de páginas detectado automáticamente
            int numeroPaginas = ObtenerNumeroPaginas(rutaArchivoSeleccionado);

            // Si no se pudo detectar automáticamente, se obliga a ingresar el número manualmente
            if (numeroPaginas <= 0)
            {
                if (string.IsNullOrWhiteSpace(txtPaginas.Text))
                {
                    Mensaje.MostrarError("No se pudo determinar el número de páginas automáticamente. Ingréselo manualmente.", "Error");
                    return;
                }

                // Valida que lo escrito sea un número entero válido y mayor que 0
                if (!int.TryParse(txtPaginas.Text, out numeroPaginas) || numeroPaginas <= 0)
                {
                    Mensaje.MostrarError("Ingrese un número de páginas válido.", "Error");
                    return;
                }
            }

            // Confirmación del usuario
            if (Mensaje.MostrarConfirmacion("¿Está seguro de que desea agregar el libro?", "Confirmación") == DialogResult.Yes)
            {
                // Se crea el objeto libro con todos sus datos
                ArchivoAdjunto nuevoLibro = new ArchivoAdjunto
                {
                    Codigo = txtCodigo.Text,
                    NombreArchivo = txtNombreProductoAgregar.Text,
                    RutaArchivo = rutaArchivoSeleccionado,
                    Extension = Path.GetExtension(rutaArchivoSeleccionado),
                    Categoria = cmbAgregarProductoCategoria.Text,
                    NumeroPaginas = numeroPaginas,
                    VistaPrevia = imgAgregarProd.Image,
                    FechaAgregado = DateTime.Now
                };

                // Agrega el libro a la pila
                GestorLibros.AgregarLibro(nuevoLibro);

                // Si el control padre es LibrosUC, actualiza la tabla de inmediato
                if (this.Parent is LibrosUC vistaLibros)
                {
                    vistaLibros.ActualizarLibros();
                }

                Mensaje.MostrarMensaje("Libro agregado exitosamente.", "Éxito");
                Parent?.Controls.Remove(this);
            }
        }


        // Permite escribir únicamente números en txtPaginas
        private void txtPaginas_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permite números y la tecla de retroceso
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnElegirLibro_Click(object sender, EventArgs e)
        {
            CargarArchivo();
        }

        private void btnCancelarAgregarProducto_Click(object sender, EventArgs e)
        {
            Parent?.Controls.Remove(this);
        }
    }
}