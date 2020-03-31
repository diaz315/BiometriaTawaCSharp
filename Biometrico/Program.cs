using Suprema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BiometriaTawaCSharp
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Huella());
                Application.Run(new SeleccionBiometrico());
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
