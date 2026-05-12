using ProyectoPEDLectura.extras;
using ProyectoPEDLectura.extras.Usuarios;
using ProyectoPEDLectura.Vistas.Login;

namespace ProyectoPEDLectura.Vistas
{
    public partial class LoginPrincipal : Form
    {
        public LoginPrincipal()
        {
            InitializeComponent();
        }

        // apartado para métodos
        private void CentrarControl(Control control, Control contenedor)
        {
            control.Left = (contenedor.Width - control.Width) / 2;
            control.Top = (contenedor.Height - control.Height) / 2;
        }

        private void CentrarBotones()
        {
            int espacioEntreBotones = 15;

            int anchoTotal = btnIngresar.Width + btnCancelar.Width + btnNuevoUsuario.Width + espacioEntreBotones;

            int xInicial = (PanelInferiorBotonesCentro.Width - anchoTotal) / 2;
            int y = (PanelInferiorBotonesCentro.Height - btnIngresar.Height) / 2;

            btnIngresar.Left = xInicial;
            btnIngresar.Top = y;

            btnCancelar.Left = btnIngresar.Right + espacioEntreBotones;
            btnCancelar.Top = y;

            btnNuevoUsuario.Left = btnCancelar.Right + espacioEntreBotones;
            btnNuevoUsuario.Top = y;
        }

        private bool Valido()
        {
            if (string.IsNullOrWhiteSpace(txtNombreIngreso.Text))
                return false;

            if (string.IsNullOrWhiteSpace(txtContra.Text))
                return false;

            return true;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            CentrarControl(lblTitulo, panelCentro);
            CentrarControl(lblInicioDeSesion, panelInicioSesionFinal);
            CentrarBotones();
        }

        private void Login_Resize(object sender, EventArgs e)
        {
            CentrarControl(lblTitulo, panelCentro);
            CentrarControl(lblInicioDeSesion, panelInicioSesionFinal);
            CentrarBotones();
        }

        private void panelCentro_Resize(object sender, EventArgs e)
        {
            CentrarControl(lblTitulo, panelCentro);
            CentrarControl(lblInicioDeSesion, panelInicioSesionFinal);
            CentrarBotones();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (!Valido())
            {
                Mensaje.MostrarError( "Ingrese usuario y contraseña.", "Error de inicio de sesión" );
                return;
            }

            try
            {
                string nombreUsuario = txtNombreIngreso.Text.Trim();
                string contrasena = txtContra.Text.Trim();

                Usuario? usuario = GestorUsuarios.ValidarLogin(nombreUsuario, contrasena);

                if (usuario == null)
                {
                    Mensaje.MostrarError(
                        "Usuario o contraseña incorrectos.",
                        "Acceso denegado"
                    );
                    return;
                }

                SesionActual.IniciarSesion(usuario);

                DashBoard inicio = new DashBoard();

                inicio.FormClosed += (s, args) =>
                {
                    SesionActual.CerrarSesion();
                    this.Close();
                };

                inicio.Show();

                txtContra.Clear();
                txtNombreIngreso.Clear();

                this.Hide();
            }
            catch (Exception ex)
            {
                Mensaje.MostrarError(
                    ex.Message,
                    "Error al iniciar sesión"
                );
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNuevoUsuario_Click(object sender, EventArgs e)
        {
            AgregarNuevoUsuarioUC agregarNuevoUsuarioUC = new AgregarNuevoUsuarioUC();

            this.Controls.Add(agregarNuevoUsuarioUC);

            CentrarControl(agregarNuevoUsuarioUC, this);

            agregarNuevoUsuarioUC.BringToFront();
        }
    }
}