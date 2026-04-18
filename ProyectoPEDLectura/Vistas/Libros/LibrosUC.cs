using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProyectoPEDLectura.Vistas.Libros
{
    public partial class LibrosUC : UserControl
    {
        public LibrosUC()
        {
            InitializeComponent();
            ContarLibros();

        }

        // método para poder cotar los libros existentes en el dgv y mostrarlo en el lblTotalLibros
        private void ContarLibros()
        {
            int totalLibros = dgvLibros.Rows.Count;
            lblTotalLibros.Text = $"N° de libros: {totalLibros.ToString()}";
        }

        private void btnAgregarLibro_Click(object sender, EventArgs e)
        {
            AgregarLibroUC agregarLibroUCv = new AgregarLibroUC();
            agregarLibroUCv.Show();
            this.Controls.Add(agregarLibroUCv);
            agregarLibroUCv.BringToFront();
            agregarLibroUCv.Location = new Point((this.Width - agregarLibroUCv.Width) / 2
                                                    , (this.Height - agregarLibroUCv.Height) / 2);
        }
    }
}
