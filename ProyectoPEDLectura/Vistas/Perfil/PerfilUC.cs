using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProyectoPEDLectura.extras.Recomendaciones;

namespace ProyectoPEDLectura.Vistas.Perfil
{

    // Clase: PerfilUC
    // Control de usuario que gestiona la visualización de recomendaciones.
    public partial class PerfilUC : UserControl
    {
        // Campo: recomendacionCargando
        // Indica si ya se está cargando una recomendación para evitar llamadas simultáneas.
        private bool recomendacionCargando = false;

        // Constructor: PerfilUC
        // Inicializa componentes del control y prepara la caja de recomendaciones.
        public PerfilUC()
        {
            InitializeComponent();
            PrepararCajaRecomendaciones();
        }

        // Método: PrepararCajaRecomendaciones
        // Configura las propiedades visuales y de comportamiento del TextBox
        // que mostrará las recomendaciones (lectura, multilinea, colores, etc.).
        private void PrepararCajaRecomendaciones()
        {
            txtRecomendaciones.ReadOnly = true;
            txtRecomendaciones.Multiline = true;
            txtRecomendaciones.WordWrap = true;
            txtRecomendaciones.ScrollBars = ScrollBars.Vertical;
            txtRecomendaciones.BackColor = Color.White;
            txtRecomendaciones.ForeColor = Color.Black;

            txtRecomendaciones.Text = "La recomendación aparecerá aquí al cargar tu perfil.";
        }

        // Método público: ActualizarRecomendacionAsync
        // Este método inicia la actualización de la recomendación de forma asíncrona.
        // Comentario sobre 'async' y 'await':
        // - 'async' en la firma de un método indica que el método puede contener 'await'
        //   y normalmente devuelve Task o Task<T>. No crea automáticamente un hilo nuevo.
        // - 'await' se usa dentro de un método async para esperar de forma asíncrona
        //   a que una operación termine sin bloquear el hilo (por ejemplo, la interfaz).
        public async Task ActualizarRecomendacionAsync()
        {
            // Aquí usamos 'await' para llamar al método que muestra la recomendación.
            // Esto suspende la ejecución de este método hasta que MostrarRecomendacionAsync
            // complete, pero no bloquea la UI.
            await MostrarRecomendacionAsync();
        }

        // Método privado: MostrarRecomendacionAsync
        // Realiza la lógica de obtención y presentación de la recomendación.
        // Está marcado como 'async' porque realiza operaciones asíncronas (espera una tarea).
        private async Task MostrarRecomendacionAsync()
        {
            // Evita que se inicie otra carga mientras ya se está cargando una recomendación.
            if (recomendacionCargando)
                return;

            try
            {
                recomendacionCargando = true;

                txtRecomendaciones.Text = "Analizando los nombres de tus archivos guardados...";

                // Llamada asíncrona a RecomendadorLibros.GenerarRecomendacionAsync.
                // 'await' aquí espera de forma asíncrona el resultado de la tarea que genera
                // la recomendación. Mientras tanto, el hilo de la interfaz puede seguir respondiendo.
                string recomendacion = await RecomendadorLibros.GenerarRecomendacionAsync();

                if (string.IsNullOrWhiteSpace(recomendacion))
                {
                    txtRecomendaciones.Text = "No se pudo generar una recomendación en este momento.";
                    return;
                }

                txtRecomendaciones.Text = recomendacion;
            }
            catch
            {
                // En caso de error (por ejemplo, problemas de red), se muestra un mensaje genérico.
                txtRecomendaciones.Text = "No se pudo generar la recomendación. Verifica tu conexión o intenta entrar nuevamente al perfil.";
            }
            finally
            {
                // Asegura que el indicador de carga se restablezca aunque ocurra una excepción.
                recomendacionCargando = false;
            }
        }
    }
}