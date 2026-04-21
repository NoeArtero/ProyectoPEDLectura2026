using ProyectoPEDLectura.extras;

namespace ProyectoPEDLectura.Vistas.Login
{
    public partial class AgregarNuevoUsuarioUC : UserControl
    {
        public AgregarNuevoUsuarioUC()
        {
            InitializeComponent();
            cmbGeneroUsuarioAgregar.Items.Add("Masculino");
            cmbGeneroUsuarioAgregar.Items.Add("Femenino");
            cmbGeneroUsuarioAgregar.Items.Add("Prefiero no decir");
            cmbGeneroUsuarioAgregar.SelectedIndex = 0;
        }

        // función bool para validar si se han llenado todos los campos
        private bool Valido()
        {
            if (string.IsNullOrWhiteSpace(txtAgregarNombreUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtContra.Text))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void btnAgregarUsuario_Click(object sender, EventArgs e)
        {
            if (!Valido())
            {
                Mensaje.MostrarError("Verifique que los espacios estén correctamente llenados", "Error de agregado");
            }
            else
            {
                if (Mensaje.MostrarConfirmacion("¿Seguro que quiere guardar este usuario?", "Agregar usuario") == DialogResult.Yes)
                {
                    Parent?.Controls.Remove(this);
                    Mensaje.MostrarMensaje("Usuario agregado con éxito.", "Usuario agregado");
                }
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string rutaImagen = openFileDialog.FileName;
                imgAgregarUsuario.Image = Image.FromFile(rutaImagen);
                imgAgregarUsuario.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }
}

