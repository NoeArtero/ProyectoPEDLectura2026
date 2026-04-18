using Guna.UI2.WinForms;
using ProyectoPEDLectura.Vistas.Inicio;
using ProyectoPEDLectura.Vistas.Libros;

namespace ProyectoPEDLectura
{
    public partial class DashBoard : Form
    {
        public DashBoard()
        {
            InitializeComponent();
            ShowView<InicioUC>();
            BotonSeleccionado(btnInicio);
        }

        // metodos para poder ver las vistas (van a ser user controls)

        private readonly Dictionary<Type, UserControl> _views = new();

        private T EnsureView<T>() where T : UserControl, new()
        {
            if (!_views.TryGetValue(typeof(T), out var view))
            {
                view = new T
                {
                    Dock = DockStyle.Fill
                };
                _views.Add(typeof(T), view);
                ViewHost.Controls.Add(view);
            }
            return (T)view;
        }

        private void ShowView<T>() where T : UserControl, new()
        {
            var targetView = EnsureView<T>();
            foreach (Control c in ViewHost.Controls)
            {
                c.Visible = false;
            }

            targetView.Visible = true;
            targetView.BringToFront();
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
            ShowView<InicioUC>();
        }

        private void btnLibros_Click(object sender, EventArgs e)
        {
            BotonSeleccionado(btnLibros);
            ShowView<LibrosUC>();
        }

        private void btnAnotaciones_Click(object sender, EventArgs e)
        {
            BotonSeleccionado(btnAnotaciones);
        }

        private void btnPerfil_Click(object sender, EventArgs e)
        {
            BotonSeleccionado(btnPerfil);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }
    }
}
