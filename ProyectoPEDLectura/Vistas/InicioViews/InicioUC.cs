using Guna.Charts.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace ProyectoPEDLectura.Vistas.Inicio
{
    public partial class InicioUC : UserControl
    {
        private readonly System.Windows.Forms.Timer clockTimer;
        public InicioUC()
        {
            InitializeComponent();

            clockTimer = new System.Windows.Forms.Timer();
            clockTimer.Interval = 1000;
            clockTimer.Tick += (_, __) => UpdateClock();
            clockTimer.Start();

            components?.Add(clockTimer);


            //SOLO SIGNIFICATIVO (AÚN NO ES OFICIAL QUE ESTE ES EL CÓDIGO)
            //// Dataset del línea de tiempo leído total
            var ds = new GunaLineDataset
            {
                Label = "Tiempo leído (minutos)",
            };

            ds.DataPoints.Add("El principito", 120);
            ds.DataPoints.Add("Biblia", 240);
            ds.DataPoints.Add("Cien años de soledad", 380);
            ds.DataPoints.Add("Don Quijote", 450);
            ds.DataPoints.Add("La sombra del viento", 520);
            ds.DataPoints.Add("1984", 610);
            ChartVentasDia.Datasets.Clear();
            ChartVentasDia.Datasets.Add(ds);
            ChartVentasDia.Update();

            // Dataset de barras para libros más leídos
            var dsInv = new GunaBarDataset
            {
                Label = "Libro más leído",

            };
            dsInv.DataPoints.Add("El principito", 150);
            dsInv.DataPoints.Add("Biblia", 87);
            dsInv.DataPoints.Add("Cien años de soledad", 50);
            dsInv.DataPoints.Add("Don Quijote", 78);
            dsInv.DataPoints.Add("La sombra del viento", 63);

            ChartInvDia.Datasets.Clear();
            ChartInvDia.Datasets.Add(dsInv);
            ChartInvDia.Update();

            // dataset cuanto se ha leído de cada libro (en porcentaje)
            var dsCat = new GunaPieDataset
            {
                Label = "cuánto has leído",

            };
            dsCat.DataPoints.Add("Biblia", 40);
            dsCat.DataPoints.Add("Principito", 4);
            dsCat.DataPoints.Add("1984", 10);
            dsCat.DataPoints.Add("Don Quijote", 30);

            ChartCreAlDia.Datasets.Clear();
            ChartCreAlDia.Datasets.Add(dsCat);
            ChartCreAlDia.Update();


            //datos para el datagrid (provisionales)
            dgvResumen.Rows.Add("El principito", "120 min", "4%");
            dgvResumen.Rows.Add("Biblia", "240 min", "40%");
            dgvResumen.Rows.Add("Cien años de soledad", "380 min", "0% ");

        }

        // método para actualizar la fecha y hora en los labels
        private void UpdateClock()
        {
            var now = DateTime.Now;

            lblFecha.Text = now.ToString("dddd dd 'de' MMMM yyyy", new CultureInfo("es-ES"));
            lblHora.Text = now.ToString("HH:mm:ss");
        }
    
    }
}
