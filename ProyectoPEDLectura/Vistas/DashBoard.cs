using Guna.UI2.WinForms;
using ProyectoPEDLectura.Vistas.Inicio;
using ProyectoPEDLectura.Vistas.Libros;
using ProyectoPEDLectura.Vistas.Anotaciones;
using ProyectoPEDLectura.Vistas.Perfil;

namespace ProyectoPEDLectura
{
    public partial class DashBoard : Form
    {
        public DashBoard()
        {
            InitializeComponent();
            MostrarInicio();
            BotonSeleccionado(btnInicio);
        }

        // metodos para poder ver las vistas (van a ser user controls)

        private readonly Dictionary<Type, UserControl> _views = new();

        private InicioUC? inicioUC;
        private LibrosUC? librosUC;
        private AnotacionesUC? anotacionesUC;
        private PerfilUC? perfilUC;

        private void OcultarVistas()
        {
            foreach (Control control in ViewHost.Controls)
            {
                control.Visible = false;
            }
        }

        private void MostrarVista(UserControl vista)
        {
            OcultarVistas();
            vista.Visible = true;
            vista.BringToFront();
        }

        private void MostrarInicio()
        {
            if (inicioUC == null)
            {
                inicioUC = new InicioUC();
                inicioUC.Dock = DockStyle.Fill;
                ViewHost.Controls.Add(inicioUC);
            }

            MostrarVista(inicioUC);
        }

        private void MostrarLibros()
        {
            if (librosUC == null)
            {
                librosUC = new LibrosUC();
                librosUC.Dock = DockStyle.Fill;
                ViewHost.Controls.Add(librosUC);
            }

            MostrarVista(librosUC);
        }

        private void MostrarAnotaciones()
        {
            if (anotacionesUC == null)
            {
                anotacionesUC = new AnotacionesUC();
                anotacionesUC.Dock = DockStyle.Fill;
                ViewHost.Controls.Add(anotacionesUC);
            }

            MostrarVista(anotacionesUC);
        }

        private void MostrarPerfil()
        {
            if (perfilUC == null)
            {
                perfilUC = new PerfilUC();
                perfilUC.Dock = DockStyle.Fill;
                ViewHost.Controls.Add(perfilUC);
            }

            MostrarVista(perfilUC);
        }
        // fin metodos para poder ver las vistas (van a ser user controls)

        // funcion para poder mostrar los botones seleccionados
        private Guna2GradientButton _actBtn;
        private void BotonSeleccionado(Guna2GradientButton btnColor)
        {
            if (_actBtn != null)
            {
                _actBtn.FillColor = Color.FromArgb(150, 51, 3);
                _actBtn.FillColor2 = Color.FromArgb(150, 51, 3);
            }

            _actBtn = btnColor;
            _actBtn.FillColor = Color.FromArgb(221, 74, 4);
            _actBtn.FillColor2 = Color.FromArgb(221, 74, 4);
        }

        // fin funcion para poder mostrar los botones seleccionados



        private void btnInicio_Click(object sender, EventArgs e)
        {
            BotonSeleccionado(btnInicio);
            MostrarInicio();
        }

        private void btnLibros_Click(object sender, EventArgs e)
        {
            BotonSeleccionado(btnLibros);
            MostrarLibros();
        }

        private void btnAnotaciones_Click(object sender, EventArgs e)
        {
            BotonSeleccionado(btnAnotaciones);
            MostrarAnotaciones();
        }

        private void btnPerfil_Click(object sender, EventArgs e)
        {
            BotonSeleccionado(btnPerfil);
            MostrarPerfil();
        }


        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }
    }
}
