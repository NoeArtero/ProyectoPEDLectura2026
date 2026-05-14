using Guna.Charts.WinForms;
using ProyectoPEDLectura.extras.LibrosAgregados.ClaseAgregarLibros;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ProyectoPEDLectura.Vistas.Inicio
{
    public partial class InicioUC : UserControl
    {
        private readonly System.Windows.Forms.Timer clockTimer;

        // Constructor: inicializa componentes, configura el temporizador del reloj y carga la vista inicio
        public InicioUC()
        {
            InitializeComponent();

            clockTimer = new System.Windows.Forms.Timer();
            clockTimer.Interval = 1000;
            clockTimer.Tick += (_, __) => UpdateClock();
            clockTimer.Start();

            components?.Add(clockTimer);

            dgvResumen.CellPainting += dgvResumen_CellPainting;

            CargarDatosInicio();
        }

        // Carga los datos iniciales y prepara los gráficos y el resumen de libros
        private void CargarDatosInicio()
        {
            try
            {
                GestorLibros.CargarDesdeArchivo();

                CargarGraficoLineaProgreso();
                CargarGraficoBarrasPaginasLeidas();
                CargarGraficoCircularProgreso();
                CargarResumenLibros();
            }
            catch
            {
                LimpiarVistaInicio();
            }
        }

        // Configura el gráfico de línea con el progreso (%) de los libros (máximo 6)
        private void CargarGraficoLineaProgreso()
        {
            GunaLineDataset ds = new GunaLineDataset
            {
                Label = "Progreso de lectura (%)"
            };

            int contador = 0;

            GestorLibros.RecorrerLibros(libro =>
            {
                if (contador >= 6)
                    return;

                string nombre = ObtenerNombreCorto(libro.NombreArchivo);
                ds.DataPoints.Add(nombre, libro.ProgresoPorcentaje);
                contador++;
            });

            if (contador == 0)
            {
                ds.DataPoints.Add("Sin libros", 0);
            }

            ChartVentasDia.Datasets.Clear();
            ChartVentasDia.Datasets.Add(ds);
            ChartVentasDia.Update();
        }

        // Configura el gráfico de barras con las páginas leídas por libro (máximo 6)
        private void CargarGraficoBarrasPaginasLeidas()
        {
            GunaBarDataset dsInv = new GunaBarDataset
            {
                Label = "Páginas leídas"
            };

            int contador = 0;

            GestorLibros.RecorrerLibros(libro =>
            {
                if (contador >= 6)
                    return;

                string nombre = ObtenerNombreCorto(libro.NombreArchivo);
                dsInv.DataPoints.Add(nombre, libro.PaginasLeidas);
                contador++;
            });

            if (contador == 0)
            {
                dsInv.DataPoints.Add("Sin libros", 0);
            }

            ChartInvDia.Datasets.Clear();
            ChartInvDia.Datasets.Add(dsInv);
            ChartInvDia.Update();
        }

        // Configura el gráfico circular mostrando la distribución del progreso entre libros (máximo 6)
        private void CargarGraficoCircularProgreso()
        {
            GunaPieDataset dsCat = new GunaPieDataset
            {
                Label = "Distribución del progreso"
            };

            int contador = 0;
            int totalProgreso = 0;

            GestorLibros.RecorrerLibros(libro =>
            {
                if (contador >= 6)
                    return;

                if (libro.ProgresoPorcentaje > 0)
                {
                    string nombre = ObtenerNombreCorto(libro.NombreArchivo);
                    dsCat.DataPoints.Add(nombre, libro.ProgresoPorcentaje);

                    totalProgreso += libro.ProgresoPorcentaje;
                    contador++;
                }
            });

            if (contador == 0 || totalProgreso == 0)
            {
                dsCat.DataPoints.Add("Sin avance", 100);
            }

            ChartCreAlDia.Datasets.Clear();
            ChartCreAlDia.Datasets.Add(dsCat);
            ChartCreAlDia.Update();
        }

        // Llena el DataGridView con un resumen de los libros recientes (máximo 6)
        private void CargarResumenLibros()
        {
            dgvResumen.Rows.Clear();

            int contador = 0;

            GestorLibros.RecorrerLibros(libro =>
            {
                if (contador >= 6)
                    return;

                dgvResumen.Rows.Add(
                    libro.Codigo,
                    libro.NombreArchivo,
                    "0",
                    libro.FechaAgregado.ToString("dd/MM/yyyy"),
                    libro.ProgresoPorcentaje
                );

                contador++;
            });

            dgvResumen.Invalidate();
        }

        // Acorta el nombre de un libro para mostrar en los gráficos/resumen
        private string ObtenerNombreCorto(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return "Sin nombre";

            if (nombre.Length <= 15)
                return nombre;

            return nombre.Substring(0, 15) + "...";
        }

        // Limpia los gráficos y el resumen cuando no hay datos disponibles
        private void LimpiarVistaInicio()
        {
            ChartVentasDia.Datasets.Clear();
            ChartInvDia.Datasets.Clear();
            ChartCreAlDia.Datasets.Clear();

            dgvResumen.Rows.Clear();
        }

        // Dibuja manualmente la barra de progreso en la celda correspondiente del DataGridView
        private void dgvResumen_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvResumen.Columns[e.ColumnIndex].Name != "ProgresoLibro")
                return;

            e.Paint(e.CellBounds, e.PaintParts & ~DataGridViewPaintParts.ContentForeground);

            int porcentaje = 0;
            object? valor = dgvResumen.Rows[e.RowIndex].Cells["ProgresoLibro"].Value;

            if (valor != null)
            {
                int.TryParse(valor.ToString(), out porcentaje);
            }

            if (porcentaje < 0)
                porcentaje = 0;

            if (porcentaje > 100)
                porcentaje = 100;

            Rectangle barraFondo = new Rectangle(
                e.CellBounds.X + 8,
                e.CellBounds.Y + 8,
                e.CellBounds.Width - 16,
                e.CellBounds.Height - 16
            );

            int anchoProgreso = (barraFondo.Width * porcentaje) / 100;

            Rectangle barraProgreso = new Rectangle(
                barraFondo.X,
                barraFondo.Y,
                anchoProgreso,
                barraFondo.Height
            );

            using (SolidBrush fondo = new SolidBrush(Color.FromArgb(235, 235, 235)))
            using (SolidBrush progreso = new SolidBrush(ObtenerColorProgreso(porcentaje)))
            using (Pen borde = new Pen(Color.FromArgb(180, 180, 180)))
            {
                e.Graphics.FillRectangle(fondo, barraFondo);

                if (anchoProgreso > 0)
                {
                    e.Graphics.FillRectangle(progreso, barraProgreso);
                }

                e.Graphics.DrawRectangle(borde, barraFondo);
            }

            TextRenderer.DrawText(
                e.Graphics,
                $"{porcentaje}%",
                e.CellStyle.Font,
                e.CellBounds,
                Color.FromArgb(55, 55, 55),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );

            e.Handled = true;
        }

        // Devuelve un color según el porcentaje de progreso (rojo, naranja, verde)
        private Color ObtenerColorProgreso(int porcentaje)
        {
            if (porcentaje < 35)
                return Color.FromArgb(210, 85, 60);

            if (porcentaje < 75)
                return Color.FromArgb(230, 150, 40);

            return Color.FromArgb(70, 160, 90);
        }

        // método para actualizar la fecha y hora en los labels
        private void UpdateClock()
        {
            DateTime now = DateTime.Now;

            lblFecha.Text = now.ToString("dddd dd 'de' MMMM yyyy", new CultureInfo("es-ES"));
            lblHora.Text = now.ToString("HH:mm:ss");
        }

        //para actualizar datos
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            CargarDatosInicio();
        }
    }
}