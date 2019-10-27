using Suprema;
using System;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BiometriaTawaCSharp
{
    public partial class RegistroEmpleado : Form
    {
        private static Empleado Resultado;
        public RegistroEmpleado()
        {
            InitializeComponent();
            pbImageFrame.Image = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ObtenerEmpleado(txtCodEmpleado.Text) > 0)
            {
                MessageBox.Show("Este usuario ya existe", "Registro Existente", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
                ConsultarApi();
            }

        }


        private int ObtenerEmpleado(string codigo)
        {
            using (var conection = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;" + "data source=C://Users//JD//source//repos//BiometriaTawaCSharp//BiometriaTawaCSharp//UFDatabase.mdb"))
            {
                conection.Open();
                var query = "Select Count(Serial) From Fingerprints where CodEmpleado="+"'"+codigo+"'";
                var command = new OleDbCommand(query, conection);
                var reader = command.ExecuteReader();
                var Empleado = 0;
                while (reader.Read())
                {
                    Empleado=(int.Parse(reader[0].ToString()));
                }
                return Empleado;
            }
        }

        private void RegistrarHuellaApi()
        {
            var huella = Convert.ToBase64String(Resultado.huellaByte);
            var coordenada = CLocation.GetLocationProperty();
            var terminal = Utilidad<Empleado>.GetIp() + "::" + Utilidad<Empleado>.GetMacAddress().ToString();
            var param = "empleadoId=" + Resultado.id + "&huella=" + huella + "&terminal=" + terminal + "&coordenadas=" + coordenada;
            Utilidad<Empleado>.GetJson(new Empleado(), "https://localhost:44396/api/tawa/registroHuella/?" + param);
        }

        private void ConsultarApi() {
            Resultado = Utilidad<Empleado>.GetJson(new Empleado(), "https://localhost:44396/api/tawa/empleado/?codigo=" + txtCodEmpleado.Text);
            if (Resultado != null)
            {
                txtNombres.Text = Resultado.nombres;
                txtDoc.Text = Resultado.tipoDoc;
                txtNroDoc.Text = Resultado.nroDoc;
                txtCodEmp.Text = Resultado.codigo;

                if (Resultado.huella != null)
                {
                    var huellaByte = Convert.FromBase64String(Resultado.huella);
                    Resultado.huellaByte = huellaByte;
                    pbImageFrame.Image = Image.FromFile("C:/Users/JD/source/repos/BiometriaTawaCSharp/BiometriaTawaCSharp/bien.png");
                    btnRegistrar.Enabled = true;
                }
                else {
                    pbImageFrame.Image = Image.FromFile("C:/Users/JD/source/repos/BiometriaTawaCSharp/BiometriaTawaCSharp/mal.png");
                    btnRegistrar.Enabled = false;
                }

            }
            else
            {
                Resultado = null;
                MessageBox.Show("No se ha encontrado el codigo de empleado", "Registro no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Limpiar()
        {
            txtNombres.Text = "";
            txtDoc.Text = "";
            txtNroDoc.Text = "";
            txtCodEmp.Text = "";
            txtCodEmpleado.Text = "";
            pbImageFrame.Image = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Resultado != null)
            {
                if (Resultado.huella!=null || Huella.HuellaTomada == 1)
                {
                    try {
                        if (Huella.HuellaTomada == 1)
                        {
                            RegistrarHuellaApi();
                        }
                    }
                    catch (Exception x) {
                        Console.WriteLine(x.Message);
                    }

                    Huella.RegistrarEmpleado(Resultado);
                    Huella.UpdateDatabaseList();
                    Limpiar();
                }
                else {
                    MessageBox.Show("Por favor ingrese la huella", "Ingrese huella", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            else {
                MessageBox.Show("No se ha seleccionado ningun registro", "Registro no seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void pbImageFrame_Click(object sender, EventArgs e)
        {
            if (Resultado != null && Resultado.huella==null)
            {
                Empleado resul=Huella.IniciarEscaneo();
                Resultado.huellaByte = resul.huellaByte;
                Resultado.pesoHuella = resul.pesoHuella;
                if (resul.pesoHuella>0) {
                    btnRegistrar.Enabled = true;
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Close();
        }
    }
}
