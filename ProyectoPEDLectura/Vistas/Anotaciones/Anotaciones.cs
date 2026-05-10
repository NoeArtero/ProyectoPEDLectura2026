using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
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

        List<Anotacion> anotaciones = new List<Anotacion>();

        int paginasDiarias = 0;
        int minutosDiarios = 0;

        public AnotacionesUC()
        {
            InitializeComponent();
        }


        private void btnAgregarAnotacion_Click(object sender, EventArgs e)
            {

                if (string.IsNullOrWhiteSpace(txtAnotacionLibro.Text))
                {
                    MessageBox.Show("Escribe una anotación.");
                    return;
                }

                anotaciones.Add(new Anotacion
                {
                    Pagina = (int)guna2NumericUpDown2.Value,
                    Texto = txtAnotacionLibro.Text
                });

                txtAnotacionLibro.Clear();
                MessageBox.Show("Anotación agregada correctamente.");


            }

            private void btnAgregarMeta_Click(object sender, EventArgs e)
            {

                paginasDiarias = (int)guna2NumericUpDown2.Value;
                minutosDiarios = (int)guna2NumericUpDown3.Value;

                MessageBox.Show($"Meta guardada:\n{paginasDiarias} páginas / {minutosDiarios} minutos diarios"
                );

            }

            private void btnLeerAnotacion_Click(object sender, EventArgs e)
            {

                int pagina = (int)guna2NumericUpDown2.Value;

                var anotacion = anotaciones.FirstOrDefault(a => a.Pagina == pagina);

                if (anotacion == null)
                {
                    MessageBox.Show("No hay anotación en esta página.");
                    return;
                }

                txtLeerAnotacion.Text = anotacion.Texto;

            }

            private void btnEditarAnotacion_Click(object sender, EventArgs e)
            {

                int pagina = (int)guna2NumericUpDown2.Value;

                var anotacion = anotaciones.FirstOrDefault(a => a.Pagina == pagina);

                if (anotacion == null)
                {
                    MessageBox.Show("No existe anotación para editar.");
                    return;
                }

                anotacion.Texto = txtLeerAnotacion.Text;
                MessageBox.Show("Anotación editada correctamente.");

            }

            private void btnEliminarAnotacion_Click(object sender, EventArgs e)
            {

                int pagina = (int)guna2NumericUpDown2.Value;

                var anotacion = anotaciones.FirstOrDefault(a => a.Pagina == pagina);

                if (anotacion == null)
                {
                    MessageBox.Show("No existe anotación para eliminar.");
                    return;
                }

                anotaciones.Remove(anotacion);
                txtLeerAnotacion.Clear();
                MessageBox.Show("Anotación eliminada.");

            }
        }
 }


