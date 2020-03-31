using Suprema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BiometriaTawaCSharp
{
    static class Program
    {
        public static string DirectorioPrincipalDev = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar; //Desarrollo
        public static string DirectorioPrincipalProd = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar; //Produccion
        public static double coindicendiaHuella = 0.50;

        //Setear el ambiente correspondiente
        public static string DirectorioPrincipal = DirectorioPrincipalDev;

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
