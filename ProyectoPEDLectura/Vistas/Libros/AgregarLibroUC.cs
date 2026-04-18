using ProyectoPEDLectura.extras;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProyectoPEDLectura.Vistas.Libros
{
    public partial class AgregarLibroUC : UserControl
    {
        public AgregarLibroUC()
        {
            InitializeComponent();
        }

        //método para poder cargar archivos

        private void CargarArchivo()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos soportados|*.pdf;*.txt;*.docx;*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string rutaArchivo = openFileDialog.FileName;

                Image preview = ObtenerVistaPreviaArchivo(rutaArchivo);

                imgAgregarProd.Image = preview;
                imgAgregarProd.SizeMode = PictureBoxSizeMode.StretchImage;

                // Más adelante esto se puede guardar en una clase ArchivoAdjunto
                // txtRuta.Text = rutaArchivo;  o variable global/campo
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
            if (Mensaje.MostrarConfirmacion("¿Está seguro de que desea agregar el libro?", "Confirmación") == DialogResult.Yes)
            {
                Mensaje.MostrarMensaje("Libro agregado exitosamente.", "Éxito");
                Parent?.Controls.Remove(this);
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
