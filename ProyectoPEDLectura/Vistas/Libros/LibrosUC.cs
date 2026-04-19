using ProyectoPEDLectura.extras;
using ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros; // NUEVO: para usar ArchivoAdjunto y la pila compartida
using System;
using System.Collections.Generic; // NUEVO: para usar List<ArchivoAdjunto>
using System.Drawing;
using System.Windows.Forms;

namespace ProyectoPEDLectura.Vistas.Libros
{
    public partial class LibrosUC : UserControl
    {
        public LibrosUC()
        {
            InitializeComponent();

            // Cuando la vista se vuelve visible, recarga los libros
            this.VisibleChanged += LibrosUC_VisibleChanged;

            // Eventos para filtrar automáticamente
            txtBuscarProd.TextChanged += txtBuscarProd_TextChanged;
            cmbCategoriaProducto.SelectedIndexChanged += cmbCategoriaProducto_SelectedIndexChanged;

            // Carga categorías en el combo
            CargarCategoriasFiltro();

            // Carga la tabla
            CargarLibrosEnTabla();

            // Cuenta los libros
            ContarLibros();
        }

        // método para poder contar los libros existentes en el dgv y mostrarlo en el lblTotalLibros
        private void ContarLibros()
        {
            int totalLibros = dgvLibros.Rows.Count;
            lblTotalLibros.Text = $"N° de libros: {totalLibros}";
        }

        // Carga las categorías en el ComboBox del filtro
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

        // Carga todos los libros o los libros filtrados
        private void CargarLibrosEnTabla()
        {
            dgvLibros.Rows.Clear();

            List<ArchivoAdjunto> libros = GestorLibros.HistorialLibros.ObtenerLibros();

            string textoBuscar = txtBuscarProd.Text.Trim().ToLower();
            string categoriaSeleccionada = cmbCategoriaProducto.Text.Trim();

            // Filtra por búsqueda
            if (!string.IsNullOrWhiteSpace(textoBuscar))
            {
                libros = libros.Where(libro =>
                    (libro.Codigo != null && libro.Codigo.ToLower().Contains(textoBuscar)) ||
                    (libro.NombreArchivo != null && libro.NombreArchivo.ToLower().Contains(textoBuscar))
                ).ToList();
            }

            // Filtra por categoría
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

                dgvLibros.Rows[fila].Cells[0].Value = libro.VistaPrevia;                    // Fotografía
                dgvLibros.Rows[fila].Cells[1].Value = libro.Codigo;                         // Código
                dgvLibros.Rows[fila].Cells[2].Value = libro.NombreArchivo;                  // Nombre
                dgvLibros.Rows[fila].Cells[3].Value = libro.Categoria;                      // Categoría
                dgvLibros.Rows[fila].Cells[4].Value = libro.NumeroPaginas;                  // N° Páginas
                dgvLibros.Rows[fila].Cells[5].Value = DateTime.Now.ToShortDateString();     // Fecha de agregado
                dgvLibros.Rows[fila].Cells[6].Value = "";                                   // Anotaciones
                dgvLibros.Rows[fila].Cells[7].Value = "";                                   // Progreso
            }

            ContarLibros();
        }

        // Método público para actualizar la vista de libros
        public void ActualizarLibros()
        {
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

        // Cuando vuelves a la vista, refresca la tabla
        private void LibrosUC_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                CargarLibrosEnTabla();
            }
        }

        // Busca automáticamente al escribir
        private void txtBuscarProd_TextChanged(object sender, EventArgs e)
        {
            CargarLibrosEnTabla();
        }

        // Filtra automáticamente al cambiar categoría
        private void cmbCategoriaProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarLibrosEnTabla();
        }

        // Elimina el libro seleccionado
        private void btnEliminarLibro_Click(object sender, EventArgs e)
        {
            if (dgvLibros.CurrentRow == null)
            {
                Mensaje.MostrarError("Seleccione un libro para eliminar.", "Error");
                return;
            }

            string codigo = Convert.ToString(dgvLibros.CurrentRow.Cells[1].Value);

            if (string.IsNullOrWhiteSpace(codigo))
            {
                Mensaje.MostrarError("No se pudo obtener el código del libro seleccionado.", "Error");
                return;
            }

            if (Mensaje.MostrarConfirmacion("¿Está seguro de que desea eliminar este libro?", "Confirmación") == DialogResult.Yes)
            {
                bool eliminado = GestorLibros.HistorialLibros.EliminarPorCodigo(codigo);

                if (eliminado)
                {
                    Mensaje.MostrarMensaje("Libro eliminado exitosamente.", "Éxito");
                    CargarLibrosEnTabla();
                }
                else
                {
                    Mensaje.MostrarError("No se encontró el libro en la pila.", "Error");
                }
            }
        }
    }
}