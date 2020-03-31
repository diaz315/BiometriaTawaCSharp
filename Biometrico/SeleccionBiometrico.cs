using Suprema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BiometriaTawaCSharp
{
    public partial class SeleccionBiometrico : Form
    {
        public static int Biometrico;

        public SeleccionBiometrico()
        {
            InitializeComponent();
            //ComboBiometrico(cmbBiometrico);
            LoadImg();
        }
        private string opt;

        public SeleccionBiometrico(string opt)
        {
            InitializeComponent();
            //ComboBiometrico(cmbBiometrico);
            LoadImg();
            this.opt = opt;
        }

        private void LoadImg() {
            pbxEikon.Image=Image.FromFile(Program.DirectorioPrincipal + "EikonImg.png");
            pbxEikon.SizeMode = PictureBoxSizeMode.StretchImage;

            pbxSuprema.Image = Image.FromFile(Program.DirectorioPrincipal + "SupremaImg.png");
            pbxSuprema.SizeMode = PictureBoxSizeMode.StretchImage;

            pbxEmpresa.Image = Image.FromFile(Program.DirectorioPrincipal + "RomImg.png");
            pbxEmpresa.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void ComboBiometrico(ComboBox comboBox)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Value");

            DataRow row = dt.NewRow();
            row[0] = 0;
            row[1] = "Eikon";
            dt.Rows.InsertAt(row, 0);

            row = dt.NewRow();
            row[0] = 1;
            row[1] = "Suprema";
            dt.Rows.InsertAt(row, 1);

            //Assign DataTable as DataSource.
            comboBox.DataSource = dt;
            comboBox.ValueMember = "Id";
            comboBox.DisplayMember = "Value";
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

