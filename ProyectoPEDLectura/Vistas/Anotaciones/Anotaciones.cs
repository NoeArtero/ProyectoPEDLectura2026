using ProyectoPEDLectura.extras;
using ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros;
using ProyectoPEDLectura.extras.Usuarios;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProyectoPEDLectura.Vistas.Anotaciones
{
    public partial class AnotacionesUC : UserControl
    {
        private const string ColumnaNombreLibro = "NombreLibro";
        private const string ColumnaMetasLibro = "MetasLibro";
        private const string ColumnaAnotacionesLibro = "AnotacionesLibro";
        private const string ColumnaProgresoLibro = "ProgresoLibro";

        private string codigoLibroSeleccionado = "";
        private bool modoEdicionAnotacion = false;
        private Anotacion? anotacionEnEdicion = null;

        // MODELO DE ANOTACIÓN
        public class Anotacion
        {
            public string CodigoLibro { get; set; } = "";
            public int Pagina { get; set; }
            public string Texto { get; set; } = "";
            public DateTime Fecha { get; set; } = DateTime.Now;
        }

        // MODELO DE META
        public class MetaLectura
        {
            public string CodigoLibro { get; set; } = "";
            public int PaginasDiarias { get; set; }
            public int MinutosDiarios { get; set; }
        }

        // DELEGADOS PARA RECORRER TADs SIN USAR LIST<T>
        public delegate void AccionAnotacion(Anotacion anotacion);
        public delegate void AccionMeta(MetaLectura meta);

        // TAD PERSONALIZADO PARA ANOTACIONES
        public class TADAnotaciones
        {
            private class Nodo
            {
                public Anotacion Dato;
                public Nodo? Siguiente;

                public Nodo(Anotacion dato)
                {
                    Dato = dato;
                    Siguiente = null;
                }
            }

            private Nodo? inicio;
            private Nodo? fin;
            private int cantidad;

            public int Cantidad
            {
                get { return cantidad; }
            }

            public void Agregar(Anotacion anotacion)
            {
                Nodo nuevo = new Nodo(anotacion);

                if (inicio == null)
                {
                    inicio = nuevo;
                    fin = nuevo;
                }
                else
                {
                    fin!.Siguiente = nuevo;
                    fin = nuevo;
                }

                cantidad++;
            }

            public void Limpiar()
            {
                inicio = null;
                fin = null;
                cantidad = 0;
            }

            public void Recorrer(AccionAnotacion accion)
            {
                Nodo? actual = inicio;

                while (actual != null)
                {
                    accion(actual.Dato);
                    actual = actual.Siguiente;
                }
            }

            public int ContarPorLibro(string codigoLibro)
            {
                int total = 0;

                Nodo? actual = inicio;

                while (actual != null)
                {
                    if (string.Equals(actual.Dato.CodigoLibro, codigoLibro, StringComparison.OrdinalIgnoreCase))
                    {
                        total++;
                    }

                    actual = actual.Siguiente;
                }

                return total;
            }

            public Anotacion? ObtenerPorIndiceDeLibro(string codigoLibro, int indice)
            {
                if (indice < 0)
                    return null;

                int contador = 0;
                Nodo? actual = inicio;

                while (actual != null)
                {
                    if (string.Equals(actual.Dato.CodigoLibro, codigoLibro, StringComparison.OrdinalIgnoreCase))
                    {
                        if (contador == indice)
                        {
                            return actual.Dato;
                        }

                        contador++;
                    }

                    actual = actual.Siguiente;
                }

                return null;
            }

            public bool Eliminar(Anotacion anotacion)
            {
                if (anotacion == null || inicio == null)
                    return false;

                if (ReferenceEquals(inicio.Dato, anotacion))
                {
                    inicio = inicio.Siguiente;

                    if (inicio == null)
                    {
                        fin = null;
                    }

                    cantidad--;
                    return true;
                }

                Nodo? actual = inicio;

                while (actual.Siguiente != null)
                {
                    if (ReferenceEquals(actual.Siguiente.Dato, anotacion))
                    {
                        actual.Siguiente = actual.Siguiente.Siguiente;

                        if (actual.Siguiente == null)
                        {
                            fin = actual;
                        }

                        cantidad--;
                        return true;
                    }

                    actual = actual.Siguiente;
                }

                return false;
            }
        }

        // TAD PERSONALIZADO PARA METAS
        public class TADMetas
        {
            private class Nodo
            {
                public MetaLectura Dato;
                public Nodo? Siguiente;

                public Nodo(MetaLectura dato)
                {
                    Dato = dato;
                    Siguiente = null;
                }
            }

            private Nodo? inicio;
            private Nodo? fin;

            public void AgregarOActualizar(MetaLectura meta)
            {
                MetaLectura? existente = BuscarPorLibro(meta.CodigoLibro);

                if (existente != null)
                {
                    existente.PaginasDiarias = meta.PaginasDiarias;
                    existente.MinutosDiarios = meta.MinutosDiarios;
                    return;
                }

                Nodo nuevo = new Nodo(meta);

                if (inicio == null)
                {
                    inicio = nuevo;
                    fin = nuevo;
                }
                else
                {
                    fin!.Siguiente = nuevo;
                    fin = nuevo;
                }
            }

            public MetaLectura? BuscarPorLibro(string codigoLibro)
            {
                Nodo? actual = inicio;

                while (actual != null)
                {
                    if (string.Equals(actual.Dato.CodigoLibro, codigoLibro, StringComparison.OrdinalIgnoreCase))
                    {
                        return actual.Dato;
                    }

                    actual = actual.Siguiente;
                }

                return null;
            }

            public void Limpiar()
            {
                inicio = null;
                fin = null;
            }

            public void Recorrer(AccionMeta accion)
            {
                Nodo? actual = inicio;

                while (actual != null)
                {
                    accion(actual.Dato);
                    actual = actual.Siguiente;
                }
            }
        }

        private readonly TADAnotaciones anotaciones = new TADAnotaciones();
        private readonly TADMetas metas = new TADMetas();

        public AnotacionesUC()
        {
            InitializeComponent();

            ConectarEventos();
            CargarCategoriasFiltro();
            CargarDatosDesdeArchivos();
            ActualizarPantallaCompleta();
        }

        private void ConectarEventos()
        {
            this.Load += AnotacionesUC_Load;
            this.VisibleChanged += AnotacionesUC_VisibleChanged;

            txtBuscarProd.TextChanged += txtBuscarProd_TextChanged;
            cmbCategoriaProducto.SelectedIndexChanged += cmbCategoriaProducto_SelectedIndexChanged;

            dgvAnotaciones.SelectionChanged += dgvAnotaciones_SelectionChanged;
            dgvAnotaciones.CellPainting += dgvAnotaciones_CellPainting;

            lstListaAnotaciones.SelectedIndexChanged += lstListaAnotaciones_SelectedIndexChanged;
        }

        private void AnotacionesUC_Load(object? sender, EventArgs e)
        {
            BloquearEdicionAnotacion();
            CargarDatosDesdeArchivos();
            ActualizarPantallaCompleta();
        }

        private void AnotacionesUC_VisibleChanged(object? sender, EventArgs e)
        {
            if (this.Visible)
            {
                CargarDatosDesdeArchivos();
                ActualizarPantallaCompleta();
            }
        }

        private void txtBuscarProd_TextChanged(object? sender, EventArgs e)
        {
            ActualizarResumenLibros();
        }

        private void cmbCategoriaProducto_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ActualizarResumenLibros();
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

        private void CargarDatosDesdeArchivos()
        {
            try
            {
                GestorLibros.CargarDesdeArchivo();
                CargarAnotacionesDesdeArchivo();
                CargarMetasDesdeArchivo();
            }
            catch (Exception ex)
            {
                Mensaje.MostrarError($"No se pudieron cargar los datos.\nDetalle: {ex.Message}", "Error");
            }
        }

        private void ActualizarPantallaCompleta()
        {
            string codigoAnterior = codigoLibroSeleccionado;

            ActualizarResumenLibros();
            SeleccionarFilaPorCodigo(codigoAnterior);

            if (string.IsNullOrWhiteSpace(codigoLibroSeleccionado))
            {
                SeleccionarPrimeraFilaDisponible();
            }

            ActualizarTotalAnotaciones();
        }

        private void ActualizarResumenLibros()
        {
            dgvAnotaciones.Rows.Clear();

            string textoBuscar = txtBuscarProd.Text.Trim();
            string categoriaSeleccionada = cmbCategoriaProducto.Text.Trim();

            try
            {
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

                    string codigoLibro = libro.Codigo ?? "";
                    MetaLectura? meta = metas.BuscarPorLibro(codigoLibro);
                    int cantidadAnotaciones = anotaciones.ContarPorLibro(codigoLibro);

                    int fila = dgvAnotaciones.Rows.Add(
                        libro.NombreArchivo,
                        FormatearMeta(meta),
                        cantidadAnotaciones.ToString(),
                        libro.ProgresoPorcentaje
                    );

                    dgvAnotaciones.Rows[fila].Tag = codigoLibro;
                });

                dgvAnotaciones.Invalidate();
            }
            catch (Exception ex)
            {
                Mensaje.MostrarError($"No se pudo actualizar el resumen de libros.\nDetalle: {ex.Message}", "Error");
            }
        }

        private string FormatearMeta(MetaLectura? meta)
        {
            if (meta == null)
                return "Sin meta";

            return $"{meta.PaginasDiarias} páginas diarias, {meta.MinutosDiarios} minutos diarios";
        }

        private void SeleccionarPrimeraFilaDisponible()
        {
            if (dgvAnotaciones.Rows.Count == 0)
            {
                codigoLibroSeleccionado = "";
                lstListaAnotaciones.Items.Clear();
                txtLeerAnotacion.Clear();
                txtAnotacionLibro.Clear();
                return;
            }

            dgvAnotaciones.ClearSelection();
            dgvAnotaciones.Rows[0].Selected = true;
            dgvAnotaciones.CurrentCell = dgvAnotaciones.Rows[0].Cells[0];

            CargarDatosDelLibroSeleccionado();
        }

        private void SeleccionarFilaPorCodigo(string codigoLibro)
        {
            if (string.IsNullOrWhiteSpace(codigoLibro))
                return;

            for (int i = 0; i < dgvAnotaciones.Rows.Count; i++)
            {
                string codigoFila = dgvAnotaciones.Rows[i].Tag?.ToString() ?? "";

                if (string.Equals(codigoFila, codigoLibro, StringComparison.OrdinalIgnoreCase))
                {
                    dgvAnotaciones.ClearSelection();
                    dgvAnotaciones.Rows[i].Selected = true;
                    dgvAnotaciones.CurrentCell = dgvAnotaciones.Rows[i].Cells[0];

                    codigoLibroSeleccionado = codigoLibro;
                    CargarDatosDelLibroSeleccionado();
                    return;
                }
            }

            codigoLibroSeleccionado = "";
        }

        private void dgvAnotaciones_SelectionChanged(object? sender, EventArgs e)
        {
            CargarDatosDelLibroSeleccionado();
        }

        private void CargarDatosDelLibroSeleccionado()
        {
            if (dgvAnotaciones.CurrentRow == null)
                return;

            string codigo = dgvAnotaciones.CurrentRow.Tag?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(codigo))
                return;

            if (!string.Equals(codigoLibroSeleccionado, codigo, StringComparison.OrdinalIgnoreCase))
            {
                modoEdicionAnotacion = false;
                anotacionEnEdicion = null;
                btnEditarAnotacion.Text = "Editar anotación";
            }

            codigoLibroSeleccionado = codigo;

            CargarMetaDelLibroSeleccionado();
            CargarAnotacionesDelLibroSeleccionado();
            ConfigurarMaximoPaginaDelLibro();
        }

        private void ConfigurarMaximoPaginaDelLibro()
        {
            ArchivoAdjunto? libro = GestorLibros.BuscarPorCodigo(codigoLibroSeleccionado);

            if (libro == null)
                return;

            decimal maximo = libro.NumeroPaginas > 0 ? libro.NumeroPaginas : 9999;

            if (numPaginaAgregarAnotacion.Value > maximo)
            {
                numPaginaAgregarAnotacion.Value = maximo;
            }

            numPaginaAgregarAnotacion.Maximum = maximo;
        }

        private void CargarMetaDelLibroSeleccionado()
        {
            MetaLectura? meta = metas.BuscarPorLibro(codigoLibroSeleccionado);

            if (meta == null)
            {
                guna2NumericUpDown2.Value = 1;
                guna2NumericUpDown3.Value = 1;
                return;
            }

            guna2NumericUpDown2.Value = meta.PaginasDiarias;
            guna2NumericUpDown3.Value = meta.MinutosDiarios;
        }

        private void CargarAnotacionesDelLibroSeleccionado()
        {
            lstListaAnotaciones.Items.Clear();
            txtLeerAnotacion.Clear();

            anotaciones.Recorrer(anotacion =>
            {
                if (string.Equals(anotacion.CodigoLibro, codigoLibroSeleccionado, StringComparison.OrdinalIgnoreCase))
                {
                    lstListaAnotaciones.Items.Add(FormatearAnotacionLista(anotacion));
                }
            });
        }

        private string FormatearAnotacionLista(Anotacion anotacion)
        {
            string vistaPrevia = ObtenerVistaPrevia(anotacion.Texto);

            return $"Página {anotacion.Pagina}, {vistaPrevia}";
        }

        private string ObtenerVistaPrevia(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "Sin contenido";

            string limpio = texto.Replace(Environment.NewLine, " ").Trim();

            if (limpio.Length <= 45)
                return limpio;

            return limpio.Substring(0, 45) + "...";
        }

        // Obtiene la página más alta registrada en las anotaciones de un libro.
        // Se usa para evitar repetir páginas o retroceder en el progreso.
        private int ObtenerUltimaPaginaAnotada(string codigoLibro)
        {
            int ultimaPagina = 0;

            anotaciones.Recorrer(anotacion =>
            {
                if (string.Equals(anotacion.CodigoLibro, codigoLibro, StringComparison.OrdinalIgnoreCase))
                {
                    if (anotacion.Pagina > ultimaPagina)
                    {
                        ultimaPagina = anotacion.Pagina;
                    }
                }
            });

            return ultimaPagina;
        }

        private Anotacion? ObtenerAnotacionSeleccionada()
        {
            if (string.IsNullOrWhiteSpace(codigoLibroSeleccionado))
                return null;

            if (lstListaAnotaciones.SelectedIndex < 0)
                return null;

            return anotaciones.ObtenerPorIndiceDeLibro(codigoLibroSeleccionado, lstListaAnotaciones.SelectedIndex);
        }

        private void lstListaAnotaciones_SelectedIndexChanged(object? sender, EventArgs e)
        {
            Anotacion? anotacion = ObtenerAnotacionSeleccionada();

            if (anotacion == null)
                return;

            txtLeerAnotacion.Text = anotacion.Texto;
            numPaginaAgregarAnotacion.Value = anotacion.Pagina;

            BloquearEdicionAnotacion();
        }

        private void btnAgregarMeta_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(codigoLibroSeleccionado))
            {
                Mensaje.MostrarError("Seleccione primero un libro para agregar o actualizar su meta.", "Error");
                return;
            }

            int paginasDiarias = (int)guna2NumericUpDown2.Value;
            int minutosDiarios = (int)guna2NumericUpDown3.Value;

            MetaLectura meta = new MetaLectura
            {
                CodigoLibro = codigoLibroSeleccionado,
                PaginasDiarias = paginasDiarias,
                MinutosDiarios = minutosDiarios
            };

            metas.AgregarOActualizar(meta);
            GuardarMetasEnArchivo();

            ActualizarPantallaCompleta();

            Mensaje.MostrarMensaje(
                $"Meta guardada correctamente:\n{paginasDiarias} páginas diarias, {minutosDiarios} minutos diarios.",
                "Meta guardada"
            );
        }

        private void btnAgregarAnotacion_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(codigoLibroSeleccionado))
            {
                Mensaje.MostrarError("Seleccione primero un libro para agregar una anotación.", "Error");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAnotacionLibro.Text))
            {
                Mensaje.MostrarError("Escriba una anotación antes de guardarla.", "Error");
                return;
            }

            int pagina = (int)numPaginaAgregarAnotacion.Value;
            int ultimaPaginaAntes = ObtenerUltimaPaginaAnotada(codigoLibroSeleccionado);

            // Obtiene la última página registrada para este libro
            int ultimaPaginaRegistrada = ObtenerUltimaPaginaAnotada(codigoLibroSeleccionado);

            // No permite repetir páginas ni retroceder
            if (pagina <= ultimaPaginaRegistrada)
            {
                Mensaje.MostrarError(
                    $"La página debe ser mayor que la última registrada ({ultimaPaginaRegistrada}).",
                    "Página inválida"
                );
                return;
            }

            ArchivoAdjunto? libro = GestorLibros.BuscarPorCodigo(codigoLibroSeleccionado);

            if (libro != null && libro.NumeroPaginas > 0 && pagina > libro.NumeroPaginas)
            {
                Mensaje.MostrarError("La página de la anotación no puede ser mayor al total de páginas del libro.", "Error");
                return;
            }

            Anotacion nueva = new Anotacion
            {
                CodigoLibro = codigoLibroSeleccionado,
                Pagina = pagina,
                Texto = txtAnotacionLibro.Text.Trim(),
                Fecha = DateTime.Now
            };

            anotaciones.Agregar(nueva);
            GuardarAnotacionesEnArchivo();

            // Actualiza el progreso del libro usando la página donde se agregó la anotación.
            // Ejemplo: si el libro tiene 100 páginas y la anotación se agregó en la página 10,
            // el progreso será 10%.
            try
            {
                ArchivoAdjunto? libroAntes = GestorLibros.BuscarPorCodigo(codigoLibroSeleccionado);
                int paginasAntes = libroAntes?.PaginasLeidas ?? 0;

                GestorLibros.ActualizarProgresoLectura(codigoLibroSeleccionado, pagina);

                VerificarMetaDiariaCumplida(
                    codigoLibroSeleccionado,
                    ultimaPaginaAntes,
                    pagina
                );
            }
            catch (Exception ex)
            {
                Mensaje.MostrarError(ex.Message, "Error al actualizar progreso");
                return;
            }

            txtAnotacionLibro.Clear();

            ActualizarPantallaCompleta();

            Mensaje.MostrarMensaje("Anotación agregada correctamente.", "Éxito");
        }

        // Verifica si el usuario alcanzó o superó la meta diaria del libro
        private void VerificarMetaDiariaCumplida(string codigoLibro, int ultimaPaginaAntes, int pagina)
        {

            MetaLectura? meta = metas.BuscarPorLibro(codigoLibro);

            // Si el libro no tiene meta registrada, no se muestra mensaje
            if (meta == null)
                return;
            // Si la meta de páginas no es válida, no se muestra mensaje

            if (meta.PaginasDiarias <= 0)
                return;
            // Calcula cuántas páginas avanzó desde la última anotación

            int paginasAvanzadas = pagina - ultimaPaginaAntes;

            // Si ahora alcanzó o superó la meta, muestra felicitación
            if (paginasAvanzadas >= meta.PaginasDiarias)
            {
                Mensaje.MostrarMensaje(
                    $"¡Felicidades! Has leído {paginasAvanzadas} páginas y cumpliste tu meta diaria de {meta.PaginasDiarias} páginas.",
                    "Meta diaria cumplida"
                );
            }
        }

        private void btnLeerAnotacion_Click(object sender, EventArgs e)
        {
            Anotacion? anotacion = ObtenerAnotacionSeleccionada();

            if (anotacion == null)
            {
                Mensaje.MostrarError("Seleccione una anotación de la lista para leerla.", "Error");
                return;
            }

            txtLeerAnotacion.Text = anotacion.Texto;
            numPaginaAgregarAnotacion.Value = anotacion.Pagina;
        }

        // Bloquea los controles de edición de anotaciones
        private void BloquearEdicionAnotacion()
        {
            // Bloquea la edición del texto donde se lee/edita la anotación
            txtLeerAnotacion.ReadOnly = true;
            txtLeerAnotacion.Enabled = true;

            // Bloquea edición de página
            numPaginaAgregarAnotacion.Enabled = false;

            modoEdicionAnotacion = false;
            anotacionEnEdicion = null;
            btnEditarAnotacion.Text = "Editar anotación";
        }

        private void btnEditarAnotacion_Click(object sender, EventArgs e)
        {
            // Verifica que haya un libro seleccionado
            if (string.IsNullOrWhiteSpace(codigoLibroSeleccionado))
            {
                Mensaje.MostrarError("Seleccione primero un libro.", "Error");
                return;
            }

            // PRIMER CLIC:
            // Entra en modo edición y carga la anotación seleccionada
            if (!modoEdicionAnotacion)
            {
                // Obtiene la anotación seleccionada de la lista
                Anotacion? seleccionada = ObtenerAnotacionSeleccionada();

                // Valida que sí exista una anotación seleccionada
                if (seleccionada == null)
                {
                    Mensaje.MostrarError("Seleccione una anotación para editarla.", "Error");
                    return;
                }

                // Guarda la anotación actual para editarla después
                anotacionEnEdicion = seleccionada;

                // Muestra el contenido en los controles
                txtLeerAnotacion.Text = seleccionada.Texto;
                txtAnotacionLibro.Text = seleccionada.Texto;
                numPaginaAgregarAnotacion.Value = seleccionada.Pagina;

                // Habilita edición en txtLeerAnotacion
                txtLeerAnotacion.ReadOnly = false;
                txtLeerAnotacion.Enabled = true;
                // Habilita edición de página
                numPaginaAgregarAnotacion.Enabled = true;

                // Activa el modo edición
                modoEdicionAnotacion = true;

                // Cambia el texto del botón
                btnEditarAnotacion.Text = "Guardar cambios";
                txtLeerAnotacion.Focus();

                Mensaje.MostrarMensaje(
                    "Ahora puede modificar el texto o la página de la anotación y presionar nuevamente el botón para guardar.",
                    "Modo edición"
                );

                return;
            }

            // SEGUNDO CLIC:
            // Guarda los cambios realizados

            // Verifica que exista una anotación cargada para editar
            if (anotacionEnEdicion == null)
            {
                modoEdicionAnotacion = false;
                btnEditarAnotacion.Text = "Editar anotación";
                return;
            }

            // Obtiene el nuevo texto escrito por el usuario
            string nuevoTexto = txtLeerAnotacion.Text.Trim();

            // Valida que la anotación no quede vacía
            if (string.IsNullOrWhiteSpace(nuevoTexto))
            {
                Mensaje.MostrarError("La anotación no puede quedar vacía.", "Error");
                return;
            }

            // Obtiene la nueva página escrita por el usuario
            int nuevaPagina = (int)numPaginaAgregarAnotacion.Value;

            // Busca el libro asociado a la anotación
            ArchivoAdjunto? libro = GestorLibros.BuscarPorCodigo(codigoLibroSeleccionado);

            // Verifica que el libro exista
            if (libro == null)
            {
                Mensaje.MostrarError("No se encontró el libro asociado a la anotación.", "Error");
                return;
            }

            // Verifica que la página no supere el total del libro
            if (libro.NumeroPaginas > 0 && nuevaPagina > libro.NumeroPaginas)
            {
                Mensaje.MostrarError("La página de la anotación no puede ser mayor al total de páginas del libro.", "Error");
                return;
            }

            // Actualiza los datos de la anotación
            anotacionEnEdicion.Pagina = nuevaPagina;
            anotacionEnEdicion.Texto = nuevoTexto;
            anotacionEnEdicion.Fecha = DateTime.Now;

            // Guarda las anotaciones en el archivo
            GuardarAnotacionesEnArchivo();

            // Actualiza el progreso de lectura del libro
            try
            {
                GestorLibros.ActualizarProgresoLectura(codigoLibroSeleccionado, nuevaPagina);
            }
            catch (Exception ex)
            {
                Mensaje.MostrarError(ex.Message, "Error al actualizar progreso");
                return;
            }

            // Sale del modo edición
            modoEdicionAnotacion = false;
            anotacionEnEdicion = null;

            // Restaura el texto original del botón
            btnEditarAnotacion.Text = "Editar anotación";

            // Limpia los controles
            txtAnotacionLibro.Clear();
            txtLeerAnotacion.Clear();

            // Actualiza toda la interfaz
            ActualizarPantallaCompleta();

            Mensaje.MostrarMensaje("Anotación editada correctamente.", "Éxito");
        }

        private void btnEliminarAnotacion_Click(object sender, EventArgs e)
        {
            Anotacion? anotacion = ObtenerAnotacionSeleccionada();

            if (anotacion == null)
            {
                Mensaje.MostrarError("Seleccione una anotación para eliminarla.", "Error");
                return;
            }

            DialogResult respuesta = Mensaje.MostrarConfirmacion(
                "¿Está seguro de que desea eliminar esta anotación?",
                "Confirmación"
            );

            if (respuesta != DialogResult.Yes)
                return;

            bool eliminado = anotaciones.Eliminar(anotacion);

            if (!eliminado)
            {
                Mensaje.MostrarError("No se pudo eliminar la anotación seleccionada.", "Error");
                return;
            }

            GuardarAnotacionesEnArchivo();

            txtLeerAnotacion.Clear();
            modoEdicionAnotacion = false;
            anotacionEnEdicion = null;
            btnEditarAnotacion.Text = "Editar anotación";

            ActualizarPantallaCompleta();

            Mensaje.MostrarMensaje("Anotación eliminada correctamente.", "Éxito");
        }

        private void ActualizarTotalAnotaciones()
        {
            lblTotalAnotaciones.Text = $"N° de anotaciones: {anotaciones.Cantidad}";
        }

        private string ObtenerCodigoUsuarioActivo()
        {
            if (!SesionActual.HaySesionActiva || SesionActual.UsuarioActivo == null)
            {
                throw new Exception("No hay una sesión activa.");
            }

            return SesionActual.UsuarioActivo.Codigo;
        }

        private string ObtenerRutaAnotaciones()
        {
            string codigoUsuario = ObtenerCodigoUsuarioActivo();
            GestorRutasUsuario.CrearEstructuraUsuario(codigoUsuario);

            return GestorRutasUsuario.ObtenerRutaAnotacionesUsuario(codigoUsuario);
        }

        private string ObtenerRutaMetas()
        {
            string codigoUsuario = ObtenerCodigoUsuarioActivo();
            GestorRutasUsuario.CrearEstructuraUsuario(codigoUsuario);

            return GestorRutasUsuario.ObtenerRutaMetasUsuario(codigoUsuario);
        }

        private string LimpiarTextoParaArchivo(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "";

            return texto
                .Replace("|", "/")
                .Replace("\r\n", "\\n")
                .Replace("\n", "\\n")
                .Replace("\r", "\\n");
        }

        private string RestaurarTextoDesdeArchivo(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "";

            return texto.Replace("\\n", Environment.NewLine);
        }

        private void GuardarAnotacionesEnArchivo()
        {
            string ruta = ObtenerRutaAnotaciones();
            StringBuilder contenido = new StringBuilder();

            anotaciones.Recorrer(anotacion =>
            {
                contenido.AppendLine(
                    $"{anotacion.CodigoLibro}|" +
                    $"{anotacion.Pagina}|" +
                    $"{LimpiarTextoParaArchivo(anotacion.Texto)}|" +
                    $"{anotacion.Fecha:yyyy-MM-dd HH:mm:ss}"
                );
            });

            File.WriteAllText(ruta, contenido.ToString(), Encoding.UTF8);
        }

        private void CargarAnotacionesDesdeArchivo()
        {
            anotaciones.Limpiar();

            string ruta = ObtenerRutaAnotaciones();

            if (!File.Exists(ruta))
                return;

            string[] lineas = File.ReadAllLines(ruta, Encoding.UTF8);

            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea))
                    continue;

                string[] datos = linea.Split('|');

                if (datos.Length < 3)
                    continue;

                Anotacion anotacion = new Anotacion
                {
                    CodigoLibro = datos[0],
                    Pagina = int.TryParse(datos[1], out int pagina) ? pagina : 1,
                    Texto = RestaurarTextoDesdeArchivo(datos[2]),
                    Fecha = datos.Length >= 4 && DateTime.TryParse(datos[3], out DateTime fecha) ? fecha : DateTime.Now
                };

                anotaciones.Agregar(anotacion);
            }
        }

        private void GuardarMetasEnArchivo()
        {
            string ruta = ObtenerRutaMetas();
            StringBuilder contenido = new StringBuilder();

            metas.Recorrer(meta =>
            {
                contenido.AppendLine(
                    $"{meta.CodigoLibro}|" +
                    $"{meta.PaginasDiarias}|" +
                    $"{meta.MinutosDiarios}"
                );
            });

            File.WriteAllText(ruta, contenido.ToString(), Encoding.UTF8);
        }

        private void CargarMetasDesdeArchivo()
        {
            metas.Limpiar();

            string ruta = ObtenerRutaMetas();

            if (!File.Exists(ruta))
                return;

            string[] lineas = File.ReadAllLines(ruta, Encoding.UTF8);

            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea))
                    continue;

                string[] datos = linea.Split('|');

                if (datos.Length < 3)
                    continue;

                MetaLectura meta = new MetaLectura
                {
                    CodigoLibro = datos[0],
                    PaginasDiarias = int.TryParse(datos[1], out int paginas) ? paginas : 1,
                    MinutosDiarios = int.TryParse(datos[2], out int minutos) ? minutos : 1
                };

                metas.AgregarOActualizar(meta);
            }
        }

        private void dgvAnotaciones_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvAnotaciones.Columns[e.ColumnIndex].Name != ColumnaProgresoLibro)
                return;

            e.Paint(e.CellBounds, e.PaintParts & ~DataGridViewPaintParts.ContentForeground);

            int porcentaje = 0;
            object? valor = dgvAnotaciones.Rows[e.RowIndex].Cells[ColumnaProgresoLibro].Value;

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
                e.CellBounds.Y + 8,
                e.CellBounds.Width - 16,
                e.CellBounds.Height - 16
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

        private Color ObtenerColorProgreso(int porcentaje)
        {
            if (porcentaje < 35)
                return Color.FromArgb(210, 85, 60);

            if (porcentaje < 75)
                return Color.FromArgb(230, 150, 40);

            return Color.FromArgb(70, 160, 90);
        }
    }
}