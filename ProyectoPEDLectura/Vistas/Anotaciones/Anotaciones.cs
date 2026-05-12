using DocumentFormat.OpenXml.InkML;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProyectoPEDLectura.Vistas.Anotaciones
{

    public partial class AnotacionesUC : UserControl
    {
        // MODELO
        public class Anotacion
        {
            public int Pagina { get; set; }
            public string Texto { get; set; } = "";
        }

        // TAD creado por el equipo para manejar anotaciones
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

            public void Agregar(Anotacion anotacion)
            {
                Nodo nuevo = new Nodo(anotacion);

                if (inicio == null)
                {
                    inicio = nuevo;
                }
                else
                {
                    Nodo actual = inicio;

                    while (actual.Siguiente != null)
                    {
                        actual = actual.Siguiente;
                    }

                    actual.Siguiente = nuevo;
                }
            }

            public Anotacion? BuscarPorPagina(int pagina)
            {
                Nodo? actual = inicio;

                while (actual != null)
                {
                    if (actual.Dato.Pagina == pagina)
                    {
                        return actual.Dato;
                    }

                    actual = actual.Siguiente;
                }

                return null;
            }

            public bool EliminarPorPagina(int pagina)
            {
                if (inicio == null)
                {
                    return false;
                }

                if (inicio.Dato.Pagina == pagina)
                {
                    inicio = inicio.Siguiente;
                    return true;
                }

                Nodo actual = inicio;

                while (actual.Siguiente != null)
                {
                    if (actual.Siguiente.Dato.Pagina == pagina)
                    {
                        actual.Siguiente = actual.Siguiente.Siguiente;
                        return true;
                    }

                    actual = actual.Siguiente;
                }

                return false;
            }

            public void MostrarEnTabla(DataGridView tabla, int paginasMeta, int minutosMeta)
            {
                tabla.Rows.Clear();

                Nodo? actual = inicio;

                while (actual != null)
                {
                    tabla.Rows.Add(
                        "Libro actual",
                        paginasMeta + " páginas / " + minutosMeta + " minutos",
                        actual.Dato.Texto,
                        "Página " + actual.Dato.Pagina
                    );

                    actual = actual.Siguiente;
                }
            }
        }

        TADAnotaciones anotaciones = new TADAnotaciones();

        int paginasDiarias = 0;
        int minutosDiarios = 0;

        public AnotacionesUC()
        {
            InitializeComponent();
        }

        private void ActualizarTablaAnotaciones()
        {
            anotaciones.MostrarEnTabla(dgvAnotaciones, paginasDiarias, minutosDiarios);
        }

        private void btnAgregarAnotacion_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtAnotacionLibro.Text))
            {
                MessageBox.Show("Escribe una anotación.");
                return;
            }

            anotaciones.Agregar(new Anotacion
            {
                Pagina = (int)numPaginaAgregarAnotacion.Value,
                Texto = txtAnotacionLibro.Text
            });

            txtAnotacionLibro.Clear();
            ActualizarTablaAnotaciones();

            MessageBox.Show("Anotación agregada correctamente.");

        }

        private void btnAgregarMeta_Click(object sender, EventArgs e)
        {

            paginasDiarias = (int)guna2NumericUpDown2.Value;
            minutosDiarios = (int)guna2NumericUpDown3.Value;

            ActualizarTablaAnotaciones();

            MessageBox.Show($"Meta guardada:\n{paginasDiarias} páginas / {minutosDiarios} minutos diarios"
            );

        }

        private void btnLeerAnotacion_Click(object sender, EventArgs e)
        {

            int pagina = (int)numPaginaAgregarAnotacion.Value;

            var anotacion = anotaciones.BuscarPorPagina(pagina);

            if (anotacion == null)
            {
                MessageBox.Show("No hay anotación en esta página.");
                return;
            }

            txtLeerAnotacion.Text = anotacion.Texto;

        }

        private void btnEditarAnotacion_Click(object sender, EventArgs e)
        {

            int pagina = (int)numPaginaAgregarAnotacion.Value;

            var anotacion = anotaciones.BuscarPorPagina(pagina);

            if (anotacion == null)
            {
                MessageBox.Show("No existe anotación para editar.");
                return;
            }

            anotacion.Texto = txtLeerAnotacion.Text;

            ActualizarTablaAnotaciones();

            MessageBox.Show("Anotación editada correctamente.");

        }

        private void btnEliminarAnotacion_Click(object sender, EventArgs e)
        {

            int pagina = (int)numPaginaAgregarAnotacion.Value;

            bool eliminado = anotaciones.EliminarPorPagina(pagina);

            if (!eliminado)
            {
                MessageBox.Show("No existe anotación para eliminar.");
                return;
            }

            txtLeerAnotacion.Clear();

            ActualizarTablaAnotaciones();

            MessageBox.Show("Anotación eliminada.");

        }
    }
}