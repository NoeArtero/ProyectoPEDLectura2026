using ProyectoPEDLectura.extras;
using ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ProyectoPEDLectura.extras.Usuarios;
using System.Text;


namespace ProyectoPEDLectura.Vistas.Libros
{
    public partial class LibrosUC : UserControl
    {
        //variables constantes para mostrar cada columna del dbv de libro agregado
        private const string ColumnaFoto = "FotoProducto";
        private const string ColumnaCodigo = "codigo";
        private const string ColumnaNombre = "nProducto";
        private const string ColumnaCategoria = "catProd";
        private const string ColumnaNumeroPaginas = "NumPaginas";
        private const string ColumnaPaginasLeidas = "PaginasLeidas";
        private const string ColumnaFechaAgregado = "FechaAgregado";
        private const string ColumnaAnotaciones = "NAnotaciones";
        private const string ColumnaProgreso = "Progreso";

        // Constructor: inicializa componentes, configura eventos y carga los libros y categorías iniciales.
        public LibrosUC()
        {
            InitializeComponent();


            this.VisibleChanged += LibrosUC_VisibleChanged;
            txtBuscarProd.TextChanged += txtBuscarProd_TextChanged;
            cmbCategoriaProducto.SelectedIndexChanged += cmbCategoriaProducto_SelectedIndexChanged;

            ConfigurarTablaProgreso();

            dgvLibros.CellEndEdit += dgvLibros_CellEndEdit;
            dgvLibros.CellPainting += dgvLibros_CellPainting;
            dgvLibros.EditingControlShowing += dgvLibros_EditingControlShowing;

            CargarCategoriasFiltro();

            GestorLibros.CargarDesdeArchivo();

            CargarLibrosEnTabla();
            ContarLibros();
        }

        // Configura las propiedades de la tabla para permitir edición del campo de páginas leídas y mostrar el progreso.
        private void ConfigurarTablaProgreso()
        {
            dgvLibros.ReadOnly = false;
            dgvLibros.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

            foreach (DataGridViewColumn columna in dgvLibros.Columns)
            {
                columna.ReadOnly = true;
            }

            if (!dgvLibros.Columns.Contains(ColumnaPaginasLeidas))
            {
                DataGridViewTextBoxColumn columnaPaginasLeidas = new DataGridViewTextBoxColumn
                {
                    Name = ColumnaPaginasLeidas,
                    HeaderText = "Páginas leídas",
                    MinimumWidth = 6,
                    ReadOnly = false,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };

                int posicion = dgvLibros.Columns[ColumnaNumeroPaginas].Index + 1;
                dgvLibros.Columns.Insert(posicion, columnaPaginasLeidas);
            }

            dgvLibros.Columns[ColumnaPaginasLeidas].ReadOnly = false;
            dgvLibros.Columns[ColumnaPaginasLeidas].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvLibros.Columns[ColumnaProgreso].ReadOnly = true;
            dgvLibros.Columns[ColumnaProgreso].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // Actualiza el contador de libros mostrado en la interfaz.
        private void ContarLibros()
        {
            int totalLibros = dgvLibros.Rows.Count;
            lblTotalLibros.Text = $"N° de libros: {totalLibros}";
        }

        // Carga las opciones de filtro de categorías en el combobox.
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

        // Carga los libros desde el gestor en el DataGridView aplicando filtros de búsqueda y categoría.
        private void CargarLibrosEnTabla()
        {
            dgvLibros.Rows.Clear();

            string textoBuscar = txtBuscarProd.Text.Trim();
            string categoriaSeleccionada = cmbCategoriaProducto.Text.Trim();

            GestorLibros.RecorrerLibros(libro =>
            {
                bool coincideBusqueda = true;

                if (!string.IsNullOrWhiteSpace(textoBuscar))
                {
                    bool coincideCodigo =
                        !string.IsNullOrWhiteSpace(libro.Codigo) &&
                        libro.Codigo.Contains(textoBuscar, StringComparison.OrdinalIgnoreCase);

                    bool coincideNombre =
                        !string.IsNullOrWhiteSpace(libro.NombreArchivo) &&
                        libro.NombreArchivo.Contains(textoBuscar, StringComparison.OrdinalIgnoreCase);

                    coincideBusqueda = coincideCodigo || coincideNombre;
                }

                bool coincideCategoria = true;

                if (!string.IsNullOrWhiteSpace(categoriaSeleccionada) &&
                    !categoriaSeleccionada.Equals("Todas", StringComparison.OrdinalIgnoreCase))
                {
                    coincideCategoria =
                        !string.IsNullOrWhiteSpace(libro.Categoria) &&
                        libro.Categoria.Equals(categoriaSeleccionada, StringComparison.OrdinalIgnoreCase);
                }

                if (!coincideBusqueda || !coincideCategoria)
                    return;

                int fila = dgvLibros.Rows.Add();
                DataGridViewRow row = dgvLibros.Rows[fila];

                row.Cells[ColumnaFoto].Value = libro.VistaPrevia;
                row.Cells[ColumnaCodigo].Value = libro.Codigo;
                row.Cells[ColumnaNombre].Value = libro.NombreArchivo;
                row.Cells[ColumnaCategoria].Value = libro.Categoria;
                row.Cells[ColumnaNumeroPaginas].Value = libro.NumeroPaginas;
                row.Cells[ColumnaPaginasLeidas].Value = libro.PaginasLeidas;
                row.Cells[ColumnaFechaAgregado].Value = libro.FechaAgregado.ToShortDateString();
                row.Cells[ColumnaAnotaciones].Value = ContarAnotacionesPorLibro(libro.Codigo ?? "");
                row.Cells[ColumnaProgreso].Value = libro.ProgresoPorcentaje;
            });

            ContarLibros();
        }

        // Obtiene el código del usuario que inició sesión
        private string ObtenerCodigoUsuarioActivo()
        {
            if (!SesionActual.HaySesionActiva || SesionActual.UsuarioActivo == null)
            {
                throw new Exception("No hay una sesión activa.");
            }

            return SesionActual.UsuarioActivo.Codigo;
        }

        // Obtiene la ruta donde están guardadas las anotaciones del usuario actual
        private string ObtenerRutaAnotaciones()
        {
            string codigoUsuario = ObtenerCodigoUsuarioActivo();

            GestorRutasUsuario.CrearEstructuraUsuario(codigoUsuario);

            return GestorRutasUsuario.ObtenerRutaAnotacionesUsuario(codigoUsuario);
        }

        // Cuenta cuántas anotaciones tiene un libro usando su código
        private int ContarAnotacionesPorLibro(string codigoLibro)
        {
            if (string.IsNullOrWhiteSpace(codigoLibro))
                return 0;

            string ruta = ObtenerRutaAnotaciones();

            if (!File.Exists(ruta))
                return 0;

            int total = 0;

            string[] lineas = File.ReadAllLines(ruta, Encoding.UTF8);

            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea))
                    continue;

                string[] datos = linea.Split('|');

                if (datos.Length < 1)
                    continue;

                string codigoGuardado = datos[0];

                if (string.Equals(codigoGuardado, codigoLibro, StringComparison.OrdinalIgnoreCase))
                {
                    total++;
                }
            }

            return total;
        }

        // Fuerza la recarga de datos desde el almacenamiento y actualiza la tabla.
        public void ActualizarLibros()
        {
            GestorLibros.CargarDesdeArchivo();
            CargarLibrosEnTabla();
        }

        // Muestra el control para agregar un nuevo libro centrado en el UserControl actual.
        private void btnAgregarLibro_Click(object sender, EventArgs e)
        {
            AgregarLibroUC agregarLibroUCv = new AgregarLibroUC();
            agregarLibroUCv.Show();
            this.Controls.Add(agregarLibroUCv);
            agregarLibroUCv.BringToFront();
            agregarLibroUCv.Location = new Point((this.Width - agregarLibroUCv.Width) / 2,
                                                 (this.Height - agregarLibroUCv.Height) / 2);
        }

        // Evento que se dispara al cambiar la visibilidad del control; recarga datos cuando se muestra.
        private void LibrosUC_VisibleChanged(object? sender, EventArgs e)
        {
            if (this.Visible)
            {
                GestorLibros.CargarDesdeArchivo();
                CargarLibrosEnTabla();
            }
        }

        // Evento para recargar la tabla cuando cambia el texto de búsqueda.
        private void txtBuscarProd_TextChanged(object? sender, EventArgs e)
        {
            CargarLibrosEnTabla();
        }

        // Evento para recargar la tabla al cambiar la categoría seleccionada.
        private void cmbCategoriaProducto_SelectedIndexChanged(object? sender, EventArgs e)
        {
            CargarLibrosEnTabla();
        }

        // Controla la visualización del control de edición para permitir solo números en la columna de páginas leídas.
        private void dgvLibros_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox cajaTexto)
            {
                cajaTexto.KeyPress -= SoloNumeros_KeyPress;

                if (dgvLibros.CurrentCell != null &&
                    dgvLibros.Columns[dgvLibros.CurrentCell.ColumnIndex].Name == ColumnaPaginasLeidas)
                {
                    cajaTexto.KeyPress += SoloNumeros_KeyPress;
                }
            }
        }

        // Valida la entrada de teclas para permitir únicamente dígitos y teclas de control.
        private void SoloNumeros_KeyPress(object? sender, KeyPressEventArgs e)
        {
            bool esNumero = char.IsDigit(e.KeyChar);
            bool esTeclaControl = char.IsControl(e.KeyChar);

            if (!esNumero && !esTeclaControl)
            {
                e.Handled = true;
            }
        }

        // Maneja la finalización de edición en la columna de páginas leídas, validando y actualizando el progreso.
        private void dgvLibros_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string nombreColumna = dgvLibros.Columns[e.ColumnIndex].Name;

            if (nombreColumna != ColumnaPaginasLeidas)
                return;

            DataGridViewRow fila = dgvLibros.Rows[e.RowIndex];

            string? codigo = Convert.ToString(fila.Cells[ColumnaCodigo].Value);

            if (string.IsNullOrWhiteSpace(codigo))
            {
                Mensaje.MostrarError("No se pudo obtener el código del libro seleccionado.", "Error");
                CargarLibrosEnTabla();
                return;
            }

            ArchivoAdjunto? libroAntesDeEditar = GestorLibros.BuscarPorCodigo(codigo);

            if (libroAntesDeEditar == null)
            {
                Mensaje.MostrarError("No se encontró el libro seleccionado.", "Error");
                CargarLibrosEnTabla();
                return;
            }

            string textoPaginasLeidas = Convert.ToString(fila.Cells[ColumnaPaginasLeidas].Value)?.Trim() ?? "";

            if (!int.TryParse(textoPaginasLeidas, out int paginasLeidas))
            {
                Mensaje.MostrarError("Debe ingresar un número válido en páginas leídas.", "Error");
                fila.Cells[ColumnaPaginasLeidas].Value = libroAntesDeEditar.PaginasLeidas;
                fila.Cells[ColumnaProgreso].Value = libroAntesDeEditar.ProgresoPorcentaje;
                dgvLibros.InvalidateRow(e.RowIndex);
                return;
            }

            try
            {
                bool actualizado = GestorLibros.ActualizarProgresoLectura(codigo, paginasLeidas);

                if (!actualizado)
                {
                    Mensaje.MostrarError("No se pudo actualizar el progreso del libro.", "Error");
                    CargarLibrosEnTabla();
                    return;
                }

                ArchivoAdjunto? libroActualizado = GestorLibros.BuscarPorCodigo(codigo);

                if (libroActualizado == null)
                {
                    CargarLibrosEnTabla();
                    return;
                }

                fila.Cells[ColumnaPaginasLeidas].Value = libroActualizado.PaginasLeidas;
                fila.Cells[ColumnaProgreso].Value = libroActualizado.ProgresoPorcentaje;

                // Verifica si el usuario alcanzó o superó la meta diaria
                VerificarMetaDiariaCumplida(
                    libroActualizado.Codigo ?? "",
                    libroAntesDeEditar.PaginasLeidas,
                    libroActualizado.PaginasLeidas
                );

                dgvLibros.InvalidateRow(e.RowIndex);
            }
            catch (Exception ex)
            {
                Mensaje.MostrarError(ex.Message, "Error");

                fila.Cells[ColumnaPaginasLeidas].Value = libroAntesDeEditar.PaginasLeidas;
                fila.Cells[ColumnaProgreso].Value = libroAntesDeEditar.ProgresoPorcentaje;

                dgvLibros.InvalidateRow(e.RowIndex);
            }
        }

        // Verifica si el usuario alcanzó o superó la meta diaria
        private void VerificarMetaDiariaCumplida(string codigoLibro, int paginasAntes, int paginasAhora)
        {
            int metaDiaria = ObtenerMetaDiariaPorLibro(codigoLibro);

            // Si no hay meta registrada para este libro, no se muestra mensaje
            if (metaDiaria <= 0)
                return;

            // Si antes ya había cumplido la meta, evitamos mostrar el mensaje repetidamente
            if (paginasAntes >= metaDiaria)
                return;

            // Si ahora llegó o superó la meta, mostramos felicitación
            if (paginasAhora >= metaDiaria)
            {
                Mensaje.MostrarMensaje(
                    $"¡Felicidades! Has cumplido tu meta diaria de {metaDiaria} páginas leídas.",
                    "Meta diaria cumplida"
                );
            }
        }

        // Busca la meta diaria de páginas asociada al libro dentro del archivo de metas
        private int ObtenerMetaDiariaPorLibro(string codigoLibro)
        {
            if (string.IsNullOrWhiteSpace(codigoLibro))
                return 0;

            string ruta = ObtenerRutaMetas();

            if (!File.Exists(ruta))
                return 0;

            string[] lineas = File.ReadAllLines(ruta, Encoding.UTF8);

            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea))
                    continue;

                string[] datos = linea.Split('|');

                // Formato esperado:
                // CodigoLibro | PaginasDiarias | MinutosDiarios
                if (datos.Length < 2)
                    continue;

                string codigoGuardado = datos[0];

                if (string.Equals(codigoGuardado, codigoLibro, StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(datos[1], out int paginasMeta))
                    {
                        return paginasMeta;
                    }
                }
            }

            return 0;
        }

        // Obtiene la ruta donde se guardan las metas del usuario actual
        private string ObtenerRutaMetas()
        {
            if (!SesionActual.HaySesionActiva || SesionActual.UsuarioActivo == null)
            {
                throw new Exception("No hay una sesión activa.");
            }

            string codigoUsuario = SesionActual.UsuarioActivo.Codigo;

            GestorRutasUsuario.CrearEstructuraUsuario(codigoUsuario);

            return GestorRutasUsuario.ObtenerRutaMetasUsuario(codigoUsuario);
        }

        // Dibuja la barra de progreso personalizada en la columna de progreso del DataGridView.
        private void dgvLibros_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string nombreColumna = dgvLibros.Columns[e.ColumnIndex].Name;

            if (nombreColumna != ColumnaProgreso)
                return;

            e.Paint(e.CellBounds, e.PaintParts & ~DataGridViewPaintParts.ContentForeground);

            int porcentaje = 0;
            object? valor = dgvLibros.Rows[e.RowIndex].Cells[ColumnaProgreso].Value;

            if (valor != null)
            {
                int.TryParse(valor.ToString(), out porcentaje);
            }

            if (porcentaje < 0)
                porcentaje = 0;

            if (porcentaje > 100)
                porcentaje = 100;

            Rectangle barraFondo = new Rectangle(
                e.CellBounds.X + 8,
                e.CellBounds.Y + 9,
                e.CellBounds.Width - 16,
                e.CellBounds.Height - 18
            );

            int anchoProgreso = (barraFondo.Width * porcentaje) / 100;

            Rectangle barraProgreso = new Rectangle(
                barraFondo.X,
                barraFondo.Y,
                anchoProgreso,
                barraFondo.Height
            );

            using (SolidBrush fondo = new SolidBrush(Color.FromArgb(235, 235, 235)))
            using (SolidBrush progreso = new SolidBrush(ObtenerColorProgreso(porcentaje)))
            using (Pen borde = new Pen(Color.FromArgb(180, 180, 180)))
            {
                e.Graphics.FillRectangle(fondo, barraFondo);

                if (anchoProgreso > 0)
                {
                    e.Graphics.FillRectangle(progreso, barraProgreso);
                }

                e.Graphics.DrawRectangle(borde, barraFondo);
            }

            TextRenderer.DrawText(
                e.Graphics,
                $"{porcentaje}%",
                e.CellStyle.Font,
                e.CellBounds,
                Color.FromArgb(55, 55, 55),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );

            e.Handled = true;
        }

        // Devuelve el color que corresponde al nivel de progreso (rojo, naranja o verde).
        private Color ObtenerColorProgreso(int porcentaje)
        {
            if (porcentaje < 35)
                return Color.FromArgb(210, 85, 60);

            if (porcentaje < 75)
                return Color.FromArgb(230, 150, 40);

            return Color.FromArgb(70, 160, 90);
        }

        // Elimina el libro seleccionado después de confirmar con el usuario y actualiza la tabla.
        private void btnEliminarLibro_Click(object sender, EventArgs e)
        {
            if (dgvLibros.CurrentRow == null)
            {
                Mensaje.MostrarError("Seleccione un libro para eliminar.", "Error");
                return;
            }

            string? codigo = Convert.ToString(dgvLibros.CurrentRow.Cells[ColumnaCodigo].Value);

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
                    EliminarAnotacionesDelLibro(codigo);
                    EliminarMetasDelLibro(codigo);

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

        private void EliminarAnotacionesDelLibro(string codigoLibro)
        {
            string ruta = ObtenerRutaAnotaciones();

            if (!File.Exists(ruta))
                return;

            StringBuilder nuevoContenido = new StringBuilder();

            string[] lineas = File.ReadAllLines(ruta, Encoding.UTF8);

            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea))
                    continue;

                string[] datos = linea.Split('|');

                if (datos.Length < 1)
                    continue;

                string codigoGuardado = datos[0];

                if (!string.Equals(codigoGuardado, codigoLibro, StringComparison.OrdinalIgnoreCase))
                {
                    nuevoContenido.AppendLine(linea);
                }
            }

            File.WriteAllText(ruta, nuevoContenido.ToString(), Encoding.UTF8);
        }

        private void EliminarMetasDelLibro(string codigoLibro)
        {
            string ruta = ObtenerRutaMetas();

            if (!File.Exists(ruta))
                return;

            StringBuilder nuevoContenido = new StringBuilder();

            string[] lineas = File.ReadAllLines(ruta, Encoding.UTF8);

            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea))
                    continue;

                string[] datos = linea.Split('|');

                if (datos.Length < 1)
                    continue;

                string codigoGuardado = datos[0];

                if (!string.Equals(codigoGuardado, codigoLibro, StringComparison.OrdinalIgnoreCase))
                {
                    nuevoContenido.AppendLine(linea);
                }
            }

            File.WriteAllText(ruta, nuevoContenido.ToString(), Encoding.UTF8);
        }

        // Abre el archivo asociado al libro en el sistema si existe; gestiona errores si no es posible abrirlo.
        private void dgvLibros_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex >= 0 &&
                dgvLibros.Columns[e.ColumnIndex].Name == ColumnaPaginasLeidas)
            {
                return;
            }

            string? codigo = Convert.ToString(dgvLibros.Rows[e.RowIndex].Cells[ColumnaCodigo].Value);

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