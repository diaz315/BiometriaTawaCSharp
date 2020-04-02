using Suprema;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BiometriaTawaCSharp
{
    public partial class SeleccionBiometrico : Form
    {
        public static int Biometrico;

        public SeleccionBiometrico()
        {
            InitializeComponent();
            LoadImg();
        }
        private string opt;

        public SeleccionBiometrico(string opt)
        {
            InitializeComponent();
            LoadImg();
            this.opt = opt;
        }

        private void LoadImg() {
            pbxEikon.Image=Image.FromFile(Program.DirectorioPrincipal+ "Imagen" + Path.DirectorySeparatorChar + "EikonImg.png");
            pbxEikon.SizeMode = PictureBoxSizeMode.StretchImage;

            pbxSuprema.Image = Image.FromFile(Program.DirectorioPrincipal + "Imagen" + Path.DirectorySeparatorChar + "SupremaImg.png");
            pbxSuprema.SizeMode = PictureBoxSizeMode.StretchImage;

            pbxEmpresa.Image = Image.FromFile(Program.DirectorioPrincipal + "Imagen" + Path.DirectorySeparatorChar + "RomImg.png");
            pbxEmpresa.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void pbxEikon_MouseClick(object sender, MouseEventArgs e)
        {
            Biometrico = 0;

            if (opt == "Huella")
            {
                this.Hide();
            }
            else
            {
                new Huella().Show();
                this.Hide();
            }
        }

        private void pbxSuprema_MouseClick(object sender, MouseEventArgs e)
        {
            Biometrico = 1;

            if (opt == "Huella")
            {
                this.Hide();
            }
            else
            {
                new Huella().Show();
                this.Hide();
            }
        }
    }
 }

