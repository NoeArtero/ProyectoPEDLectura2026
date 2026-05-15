
using ProyectoPEDLectura.extras;
using ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros;
using ProyectoPEDLectura.extras.Recomendaciones;
using ProyectoPEDLectura.extras.Usuarios;
using System.Text;

namespace ProyectoPEDLectura.Vistas.Perfil
{
    // UserControl que muestra y permite editar el perfil del usuario,
    // sus últimos libros y una recomendación generada.
    public partial class PerfilUC : UserControl
    {
        // Indica si la recomendación se está cargando para evitar llamadas concurrentes.
        private bool recomendacionCargando = false;
        // Indica si el usuario está en modo edición de nombre/contraseña.
        private bool modoEdicionDatos = false;
        // Ruta de la foto seleccionada pero aún no guardada.
        private string rutaFotoPendiente = "";

        // Constructor: inicializa componentes y prepara UI y eventos.
        public PerfilUC()
        {
            InitializeComponent();

            PrepararCajaRecomendaciones();
            PrepararPerfil();
            ConectarEventos();
        }

        // Conecta los manejadores de eventos a los controles del UserControl.

        private void ConectarEventos()
        {
            this.Load += PerfilUC_Load;

            btnCambiarFoto.Click += btnCambiarFoto_Click;
            btnCambiarUsContra.Click += btnCambiarUsContra_Click;
            btnGuardarCambios.Click += btnGuardarCambios_Click;
            btnActualizar.Click += btnActualizar_Click;

            // NUEVO: pinta la columna de progreso como barra visual
            dgvResumen.CellPainting += dgvResumen_CellPainting;
        }


        // Manejador del evento Load: carga datos de usuario, libros y recomendación.
        private async void PerfilUC_Load(object? sender, EventArgs e)
        {
            CargarDatosUsuario();
            CargarUltimosLibros();
            await MostrarRecomendacionAsync();
        }

        // Configura visual y comportamiento inicial de los controles del perfil.
        private void PrepararPerfil()
        {
            imgPerfil.SizeMode = PictureBoxSizeMode.StretchImage;

            txtNombreIngreso.ReadOnly = true;
            guna2TextBox1.ReadOnly = true;

            guna2TextBox1.PasswordChar = '*';

            btnGuardarCambios.Enabled = false;
            btnGuardarCambios.Visible = true;
        }

        // Inicializa la caja de texto donde se muestran las recomendaciones.
        private void PrepararCajaRecomendaciones()
        {
            txtRecomendaciones.ReadOnly = true;
            txtRecomendaciones.Multiline = true;
            txtRecomendaciones.WordWrap = true;
            txtRecomendaciones.ScrollBars = ScrollBars.Vertical;
            txtRecomendaciones.BackColor = Color.White;
            txtRecomendaciones.ForeColor = Color.Black;
            txtRecomendaciones.Text = "La recomendación aparecerá aquí al cargar tu perfil.";
        }

        // Carga los datos del usuario actual en los controles del formulario.
        private void CargarDatosUsuario()
        {
            if (!SesionActual.HaySesionActiva || SesionActual.UsuarioActivo == null)
            {
                Mensaje.MostrarError("No hay una sesión activa.", "Perfil");
                return;
            }

            Usuario usuario = SesionActual.UsuarioActivo;

            txtNombreIngreso.Text = usuario.NombreUsuario;
            guna2TextBox1.Text = "********";

            txtNombreIngreso.ReadOnly = true;
            guna2TextBox1.ReadOnly = true;

            modoEdicionDatos = false;
            rutaFotoPendiente = "";

            btnGuardarCambios.Enabled = false;

            CargarFoto(usuario.RutaFotoPerfil);
        }

        // Carga la imagen de perfil desde una ruta, manejando excepciones y liberando recursos.
        private void CargarFoto(string rutaFoto)
        {
            try
            {
                imgPerfil.Image?.Dispose();
                imgPerfil.Image = null;

                if (!string.IsNullOrWhiteSpace(rutaFoto) && File.Exists(rutaFoto))
                {
                    imgPerfil.Image = CargarImagenSinBloquearArchivo(rutaFoto);
                }
            }
            catch
            {
                imgPerfil.Image = null;
            }
        }

        // Abre la imagen y devuelve una copia para evitar bloquear el archivo original.
        private Image CargarImagenSinBloquearArchivo(string rutaImagen)
        {
            using Image imagenTemporal = Image.FromFile(rutaImagen);
            return new Bitmap(imagenTemporal);
        }

        // Manejador para cambiar la foto de perfil: abre diálogo y muestra la imagen seleccionada.
        private void btnCambiarFoto_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Seleccione una nueva foto de perfil";
            openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                rutaFotoPendiente = openFileDialog.FileName;

                try
                {
                    imgPerfil.Image?.Dispose();
                    imgPerfil.Image = CargarImagenSinBloquearArchivo(rutaFotoPendiente);
                    btnGuardarCambios.Enabled = true;
                }
                catch
                {
                    rutaFotoPendiente = "";
                    Mensaje.MostrarError("No se pudo cargar la imagen seleccionada.", "Foto de perfil");
                }
            }
        }

        // Activa el modo de edición para cambiar nombre de usuario y contraseña.
        private void btnCambiarUsContra_Click(object? sender, EventArgs e)
        {
            if (!SesionActual.HaySesionActiva || SesionActual.UsuarioActivo == null)
            {
                Mensaje.MostrarError("No hay una sesión activa.", "Perfil");
                return;
            }

            modoEdicionDatos = true;

            txtNombreIngreso.ReadOnly = false;
            guna2TextBox1.ReadOnly = false;

            txtNombreIngreso.Text = SesionActual.UsuarioActivo.NombreUsuario;
            guna2TextBox1.Clear();
            guna2TextBox1.PlaceholderText = "Nueva contraseña";

            btnGuardarCambios.Enabled = true;
            txtNombreIngreso.Focus();
        }

        // Guarda los cambios realizados en el perfil: nombre, contraseña y/o foto.
        private void btnGuardarCambios_Click(object? sender, EventArgs e)
        {
            if (!SesionActual.HaySesionActiva || SesionActual.UsuarioActivo == null)
            {
                Mensaje.MostrarError("No hay una sesión activa.", "Perfil");
                return;
            }

            Usuario usuarioActivo = SesionActual.UsuarioActivo;
            bool huboCambios = false;

            try
            {
                if (modoEdicionDatos)
                {
                    string nuevoNombre = txtNombreIngreso.Text.Trim();
                    string nuevaContrasena = guna2TextBox1.Text.Trim();

                    if (string.IsNullOrWhiteSpace(nuevoNombre))
                    {
                        Mensaje.MostrarError("El nombre de usuario no puede estar vacío.", "Datos de usuario");
                        return;
                    }

                    if (GestorUsuarios.ExisteOtroUsuarioConEseNombre(usuarioActivo.Codigo, nuevoNombre))
                    {
                        Mensaje.MostrarError("Ya existe otro usuario con ese nombre.", "Datos de usuario");
                        return;
                    }

                    usuarioActivo.NombreUsuario = nuevoNombre;

                    if (!string.IsNullOrWhiteSpace(nuevaContrasena))
                    {
                        usuarioActivo.ContrasenaHash = SeguridadUsuario.CrearHashContrasena(nuevaContrasena);
                    }

                    huboCambios = true;
                }

                if (!string.IsNullOrWhiteSpace(rutaFotoPendiente))
                {
                    string nuevaRutaFoto = GestorUsuarios.ActualizarFotoPerfil(
                        usuarioActivo.Codigo,
                        rutaFotoPendiente
                    );

                    usuarioActivo.RutaFotoPerfil = nuevaRutaFoto;
                    huboCambios = true;
                }

                if (!huboCambios)
                {
                    Mensaje.MostrarMensaje("No hay cambios pendientes para guardar.", "Perfil");
                    return;
                }

                bool actualizado = GestorUsuarios.ActualizarUsuario(usuarioActivo);

                if (!actualizado)
                {
                    Mensaje.MostrarError("No se pudo actualizar el usuario.", "Perfil");
                    return;
                }

                SesionActual.IniciarSesion(usuarioActivo);

                Mensaje.MostrarMensaje("Cambios guardados correctamente.", "Perfil actualizado");

                CargarDatosUsuario();
                CargarUltimosLibros();
            }
            catch (Exception ex)
            {
                Mensaje.MostrarError(ex.Message, "Error al guardar cambios");
            }
        }

        // Manejador para refrescar los datos del perfil y la recomendación.
        private async void btnActualizar_Click(object? sender, EventArgs e)
        {
            CargarDatosUsuario();
            CargarUltimosLibros();
            await MostrarRecomendacionAsync();
        }

        // Carga los últimos libros agregados y los muestra en el DataGridView resumen.
        private void CargarUltimosLibros()
        {
            dgvResumen.Rows.Clear();

            ArchivoAdjunto?[] ultimosLibros = new ArchivoAdjunto?[5];
            int cantidad = 0;

            try
            {
                GestorLibros.RecorrerLibros(libro =>
                {
                    if (cantidad < ultimosLibros.Length)
                    {
                        ultimosLibros[cantidad] = libro;
                        cantidad++;
                    }
                    else
                    {
                        for (int i = 0; i < ultimosLibros.Length - 1; i++)
                        {
                            ultimosLibros[i] = ultimosLibros[i + 1];
                        }

                        ultimosLibros[ultimosLibros.Length - 1] = libro;
                    }
                });

                for (int i = cantidad - 1; i >= 0; i--)
                {
                    ArchivoAdjunto? libro = ultimosLibros[i];

                    if (libro == null)
                        continue;

                    dgvResumen.Rows.Add(
                        libro.Codigo,
                        libro.NombreArchivo,
                        ContarAnotacionesPorLibro(libro.Codigo ?? "").ToString(),
                        libro.FechaAgregado.ToString("dd/MM/yyyy"),
                        libro.ProgresoPorcentaje
                    );
                }

                dgvResumen.Invalidate();
            }
            catch
            {
                dgvResumen.Rows.Clear();
            }
        }

        // Obtiene la ruta del archivo donde se guardan las anotaciones del usuario actual
        private string ObtenerRutaAnotaciones()
        {
            if (!SesionActual.HaySesionActiva || SesionActual.UsuarioActivo == null)
            {
                throw new Exception("No hay una sesión activa.");
            }

            string codigoUsuario = SesionActual.UsuarioActivo.Codigo;

            GestorRutasUsuario.CrearEstructuraUsuario(codigoUsuario);

            return GestorRutasUsuario.ObtenerRutaAnotacionesUsuario(codigoUsuario);
        }

        // Cuenta cuántas anotaciones tiene un libro según su código
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

        // Método público para disparar la actualización de la recomendación desde otros componentes.
        public async Task ActualizarRecomendacionAsync()
        {
            await MostrarRecomendacionAsync();
        }

        // Genera y muestra una recomendación de libros de forma asíncrona,
        // evitando reentradas mientras una solicitud está en curso.
        private async Task MostrarRecomendacionAsync()
        {
            if (recomendacionCargando)
                return;

            try
            {
                recomendacionCargando = true;

                txtRecomendaciones.Text = "Analizando los nombres de tus archivos guardados...";

                string recomendacion = await RecomendadorLibros.GenerarRecomendacionAsync();

                if (string.IsNullOrWhiteSpace(recomendacion))
                {
                    txtRecomendaciones.Text = "No se pudo generar una recomendación en este momento.";
                    return;
                }

                txtRecomendaciones.Text = recomendacion;
            }
            catch
            {
                txtRecomendaciones.Text = "No se pudo generar la recomendación. Verifica tu conexión o intenta entrar nuevamente al perfil.";
            }
            finally
            {
                recomendacionCargando = false;
            }
        }

        //NUEVO: para ver el progreso de lectura de los libros

        private void dgvResumen_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvResumen.Columns[e.ColumnIndex].Name != "ProgresoLibro")
                return;

            e.Paint(e.CellBounds, e.PaintParts & ~DataGridViewPaintParts.ContentForeground);

            int porcentaje = 0;
            object? valor = dgvResumen.Rows[e.RowIndex].Cells["ProgresoLibro"].Value;

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