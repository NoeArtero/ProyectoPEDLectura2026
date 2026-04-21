using ProyectoPEDLectura.extras;
using ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros;
using System.Diagnostics;
using System.IO;

namespace ProyectoPEDLectura.Vistas.Libros
{
    public partial class LibrosUC : UserControl
    {
        public LibrosUC()
        {
            InitializeComponent();

            this.VisibleChanged += LibrosUC_VisibleChanged;
            txtBuscarProd.TextChanged += txtBuscarProd_TextChanged;
            cmbCategoriaProducto.SelectedIndexChanged += cmbCategoriaProducto_SelectedIndexChanged;

            CargarCategoriasFiltro();

            // Cargar desde el TXT al abrir la vista
            GestorLibros.CargarDesdeArchivo();

            CargarLibrosEnTabla();
            ContarLibros();
        }

        private void ContarLibros()
        {
            int totalLibros = dgvLibros.Rows.Count;
            lblTotalLibros.Text = $"N° de libros: {totalLibros}";
        }

        private void CargarCategoriasFiltro()
        {
            cmbCategoriaProducto.Items.Clear();

            cmbCategoriaProducto.Items.Add("Todas");
            cmbCategoriaProducto.Items.Add("PDF");
            cmbCategoriaProducto.Items.Add("TXT");
            cmbCategoriaProducto.Items.Add("DOCX");
            cmbCategoriaProducto.Items.Add("Imagen");

            cmbCategoriaProducto.SelectedIndex = 0;
        }

        private void CargarLibrosEnTabla()
        {
            dgvLibros.Rows.Clear();

            List<ArchivoAdjunto> libros = GestorLibros.ObtenerLibros();

            string textoBuscar = txtBuscarProd.Text.Trim().ToLower();
            string categoriaSeleccionada = cmbCategoriaProducto.Text.Trim();

            if (!string.IsNullOrWhiteSpace(textoBuscar))
            {
                libros = libros.Where(libro =>
                    (libro.Codigo != null && libro.Codigo.ToLower().Contains(textoBuscar)) ||
                    (libro.NombreArchivo != null && libro.NombreArchivo.ToLower().Contains(textoBuscar))
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(categoriaSeleccionada) && categoriaSeleccionada != "Todas")
            {
                libros = libros.Where(libro =>
                    libro.Categoria != null &&
                    libro.Categoria.Equals(categoriaSeleccionada, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            foreach (ArchivoAdjunto libro in libros)
            {
                int fila = dgvLibros.Rows.Add();

                dgvLibros.Rows[fila].Cells[0].Value = libro.VistaPrevia;
                dgvLibros.Rows[fila].Cells[1].Value = libro.Codigo;
                dgvLibros.Rows[fila].Cells[2].Value = libro.NombreArchivo;
                dgvLibros.Rows[fila].Cells[3].Value = libro.Categoria;
                dgvLibros.Rows[fila].Cells[4].Value = libro.NumeroPaginas;
                dgvLibros.Rows[fila].Cells[5].Value = libro.FechaAgregado.ToShortDateString();
                dgvLibros.Rows[fila].Cells[6].Value = "";
                dgvLibros.Rows[fila].Cells[7].Value = "";
            }

            ContarLibros();
        }


        public void ActualizarLibros()
        {
            GestorLibros.CargarDesdeArchivo();
            CargarLibrosEnTabla();
        }

        private void btnAgregarLibro_Click(object sender, EventArgs e)
        {
            AgregarLibroUC agregarLibroUCv = new AgregarLibroUC();
            agregarLibroUCv.Show();
            this.Controls.Add(agregarLibroUCv);
            agregarLibroUCv.BringToFront();
            agregarLibroUCv.Location = new Point((this.Width - agregarLibroUCv.Width) / 2,
                                                 (this.Height - agregarLibroUCv.Height) / 2);
        }

        private void LibrosUC_VisibleChanged(object? sender, EventArgs e)
        {
            if (this.Visible)
            {
                GestorLibros.CargarDesdeArchivo();
                CargarLibrosEnTabla();
            }
        }

        private void txtBuscarProd_TextChanged(object? sender, EventArgs e)
        {
            CargarLibrosEnTabla();
        }

        private void cmbCategoriaProducto_SelectedIndexChanged(object? sender, EventArgs e)
        {
            CargarLibrosEnTabla();
        }

        private void btnEliminarLibro_Click(object sender, EventArgs e)
        {
            if (dgvLibros.CurrentRow == null)
            {
                Mensaje.MostrarError("Seleccione un libro para eliminar.", "Error");
                return;
            }

            string? codigo = Convert.ToString(dgvLibros.CurrentRow.Cells[1].Value);

            if (string.IsNullOrWhiteSpace(codigo))
            {
                Mensaje.MostrarError("No se pudo obtener el código del libro seleccionado.", "Error");
                return;
            }

            if (Mensaje.MostrarConfirmacion("¿Está seguro de que desea eliminar este libro?", "Confirmación") == DialogResult.Yes)
            {
                bool eliminado = GestorLibros.EliminarLibro(codigo);

                if (eliminado)
                {
                    Mensaje.MostrarMensaje("Libro eliminado exitosamente.", "Éxito");
                    GestorLibros.CargarDesdeArchivo();
                    CargarLibrosEnTabla();
                }
                else
                {
                    Mensaje.MostrarError("No se encontró el libro.", "Error");
                }
            }
        }


        // Permite abrir el libro al hacer doble click en la fila del DataGridView
        private void dgvLibros_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            string? codigo = Convert.ToString(dgvLibros.Rows[e.RowIndex].Cells[1].Value);

            if (string.IsNullOrWhiteSpace(codigo))
            {
                Mensaje.MostrarError("No se pudo obtener el código del libro seleccionado.", "Error");
                return;
            }

            ArchivoAdjunto? libro = GestorLibros.BuscarPorCodigo(codigo);

            if (libro == null)
            {
                Mensaje.MostrarError("No se encontró la información del libro.", "Error");
                return;
            }

            if (string.IsNullOrWhiteSpace(libro.RutaArchivo) || !File.Exists(libro.RutaArchivo))
            {
                Mensaje.MostrarError("El archivo no existe o la ruta guardada no es válida.", "Archivo no encontrado");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = libro.RutaArchivo,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Mensaje.MostrarError($"No se pudo abrir el archivo.\nDetalle: {ex.Message}", "Error");
            }
        }
    }
}