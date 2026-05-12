using ProyectoPEDLectura.extras;
using ProyectoPEDLectura.extras.Usuarios;

namespace ProyectoPEDLectura.Vistas.Login
{
    public partial class AgregarNuevoUsuarioUC : UserControl
    {
        private string rutaFotoSeleccionada = "";

        public AgregarNuevoUsuarioUC()
        {
            InitializeComponent();

            cmbGeneroUsuarioAgregar.Items.Clear();
            cmbGeneroUsuarioAgregar.Items.Add("Masculino");
            cmbGeneroUsuarioAgregar.Items.Add("Femenino");
            cmbGeneroUsuarioAgregar.Items.Add("Prefiero no decir");
            cmbGeneroUsuarioAgregar.SelectedIndex = 0;

            imgAgregarUsuario.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private bool Valido()
        {
            if (string.IsNullOrWhiteSpace(txtAgregarNombreUsuario.Text))
                return false;

            if (string.IsNullOrWhiteSpace(txtContra.Text))
                return false;

            if (cmbGeneroUsuarioAgregar.SelectedItem == null)
                return false;

            if (string.IsNullOrWhiteSpace(rutaFotoSeleccionada))
                return false;

            return true;
        }

        private void btnAgregarUsuario_Click(object sender, EventArgs e)
        {
            if (!Valido())
            {
                Mensaje.MostrarError(
                    "Verifique que el usuario, contraseña, género e imagen estén completos.",
                    "Error de agregado"
                );
                return;
            }

            if (Mensaje.MostrarConfirmacion("¿Seguro que quiere guardar este usuario?", "Agregar usuario") != DialogResult.Yes)
                return;

            try
            {
                string nombreUsuario = txtAgregarNombreUsuario.Text.Trim();
                string contrasena = txtContra.Text.Trim();
                string genero = cmbGeneroUsuarioAgregar.SelectedItem?.ToString() ?? "";

                Usuario nuevoUsuario = GestorUsuarios.CrearUsuario(
                    nombreUsuario,
                    contrasena,
                    genero,
                    rutaFotoSeleccionada
                );

                Mensaje.MostrarMensaje(
                    $"Usuario '{nuevoUsuario.NombreUsuario}' agregado con éxito.",
                    "Usuario agregado"
                );

                Parent?.Controls.Remove(this);
            }
            catch (Exception ex)
            {
                Mensaje.MostrarError(ex.Message, "Error al crear usuario");
            }
        }

        private void btnCancelarAgregarUsuario_Click(object sender, EventArgs e)
        {
            if (Mensaje.MostrarConfirmacion("¿Seguro que quiere cancelar la operación?", "Cancelar usuario") == DialogResult.Yes)
            {
                Parent?.Controls.Remove(this);
            }
        }

        private void btnElegirImagenUsuario_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Seleccione una foto de perfil";
            openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                rutaFotoSeleccionada = openFileDialog.FileName;

                try
                {
                    imgAgregarUsuario.Image?.Dispose();
                    imgAgregarUsuario.Image = CargarImagenSinBloquearArchivo(rutaFotoSeleccionada);
                    imgAgregarUsuario.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch
                {
                    rutaFotoSeleccionada = "";
                    Mensaje.MostrarError("No se pudo cargar la imagen seleccionada.", "Error de imagen");
                }
            }
        }

        private Image CargarImagenSinBloquearArchivo(string rutaImagen)
        {
            using Image imagenTemporal = Image.FromFile(rutaImagen);
            return new Bitmap(imagenTemporal);
        }
    }
}