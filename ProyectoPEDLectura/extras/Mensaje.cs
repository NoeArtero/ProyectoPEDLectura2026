using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoPEDLectura.extras
{
    public class Mensaje
    {
        public static void MostrarMensaje(string mensaje, string titulo)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void MostrarError(string mensaje, string titulo)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static DialogResult MostrarConfirmacion(string mensaje, string titulo)
        {
            return MessageBox.Show(mensaje, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
        }
    }
}
