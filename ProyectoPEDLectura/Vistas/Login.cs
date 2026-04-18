

using ProyectoPEDLectura.Vistas.Inicio;

namespace ProyectoPEDLectura.Vistas
{
    public partial class Login : Form
    {
        public Login()
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

        private bool valido()
        {
            bool valido = false;

            if (txtNombreIngreso.Text.IsWhiteSpace() || txtContra.Text.IsWhiteSpace())
            {
                valido = false;
            }
            else
            {
                valido = true;
            }

            return valido;
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
            if (!valido())
            {
                MessageBox.Show("Uno de los espacios está vacío, ingrese sus credenciales completas", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DashBoard inicio = new DashBoard();
                inicio.Show();
                txtContra.Clear();
                txtNombreIngreso.Clear();
            }

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNuevoUsuario_Click(object sender, EventArgs e)
        {

        }
    }
}
