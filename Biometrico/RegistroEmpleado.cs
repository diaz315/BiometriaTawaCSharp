using Suprema;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BiometriaTawaCSharp
{
    public partial class RegistroEmpleado : Form
    {
        private static Empleado Resultado;
        //private static string DirectorioPrincipal = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar;
        private static string DirectorioPrincipal = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar;
        public RegistroEmpleado()
        {
            InitializeComponent();
            pbImageFrame.Image = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtCodEmpleado.Text=="") {
                MessageBox.Show("Por favor ingrese un codigo", "Validar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

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
            using (var conection = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;" + "data source="+DirectorioPrincipal+"UFDatabase.mdb"))
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

        public static void ActualizarRegistroLocalEnviado(int id)
        {
            try
            {
                using (var conection = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;" + "data source=" + DirectorioPrincipal + "UFDatabase.mdb"))
                {
                    OleDbCommand comm = new OleDbCommand("UPDATE Asistencia SET Enviado=1 WHERE Id=?", conection);
                    comm.Parameters.Add("@Id", OleDbType.Integer).Value = id;
                    conection.Open();
                    int iResultado = comm.ExecuteNonQuery();
                    conection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private static List<Empleado> ObtenerResgistrosSinEnviar()
        {
            var ListEmpleado = new List<Empleado>();

            try
            {
                using (var conection = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;" + "data source=" + DirectorioPrincipal + "UFDatabase.mdb"))
                {
                    conection.Open();
                    var query = "SELECT Id,EmpleadoId,Fecha,Estado,Terminal,Coordenadas FROM Asistencia WHERE Enviado=0";
                    var command = new OleDbCommand(query, conection);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Empleado empleado = new Empleado();
                        empleado.ids = reader[0].ToString();
                        empleado.idEmpleado = reader[1].ToString();
                        empleado.fecha = reader[2].ToString();
                        empleado.estado = reader[3].ToString();
                        empleado.terminal = reader[4].ToString();
                        empleado.coordenadas = reader[5].ToString();
                        ListEmpleado.Add(empleado);
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            return ListEmpleado;
        }

        private void RegistrarHuellaApi()
        {
            var huella = Convert.ToBase64String(Resultado.huellaByte);
            var coordenada = Huella.txtCoordenada.Text;
            var terminal = Utilidad<Empleado>.GetIp() + "::" + Utilidad<Empleado>.GetMacAddress().ToString();
            var param = "empleadoId=" + Resultado.id + "&huella=" + huella + "&terminal=" + terminal + "&coordenadas=" + coordenada+"&clave="+Huella.ApiKey;             
            Utilidad<Empleado>.GetJson(new Empleado(), Huella.Api+Constante.RegistrarHuellaApi + param);
        }

        private static void ProcesarDatosNoEnviados() {
            List<Empleado> RegSinEnviar = ObtenerResgistrosSinEnviar();
            try
            {
                if (RegSinEnviar.Count > 0)
                {
                    foreach (Empleado emp in RegSinEnviar)
                    {
                        var param = "empleadoId=" + emp.idEmpleado + "&terminal=" + emp.terminal + "&coordenadas=" + emp.coordenadas + "&fecha=" + emp.fecha + "&clave=" + Huella.ApiKey;
                        string Url = Huella.Api+Constante.RegAsistenciaApi + param;
                        Empleado empleado = Utilidad<Empleado>.GetJson(new Empleado(), Url);
                        if (empleado.resultado) {
                            ActualizarRegistroLocalEnviado(int.Parse(emp.ids));
                        }
                    }
                }
            }
            catch(Exception Ex) {
                MessageBox.Show(Ex.Message, "Registro no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        public static void RegistrarAsistenciaApi(int id=0)
        {
      
            int empId =0;

            if (id > 0)
            {
                empId = id;
            }
            else if (Resultado.id!=0) {
                empId = Resultado.id;
            }

            if (empId>0) {
                var coordenada = Huella.txtCoordenada.Text;
                var terminal = Utilidad<Empleado>.GetIp() + "::" + Utilidad<Empleado>.GetMacAddress().ToString();
                var fecha = DateTime.Now;
                
                RegistrarAsistenciaLocal(empId, fecha, 0, terminal, coordenada);
            }
            //asistencia sin enviar
            ProcesarDatosNoEnviados();
        }

        public static void RegistrarAsistenciaLocal(int empleadoId, DateTime fecha, int enviado, string terminal, string coordenadas)
        {
            try
            {
                using (var conection = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;" + "data source="+DirectorioPrincipal+"UFDatabase.mdb"))
                {
                    OleDbCommand comm = new OleDbCommand("INSERT INTO Asistencia(EmpleadoId,Fecha,Estado,Enviado,Terminal,Coordenadas) VALUES (?,?,?,?,?,?)", conection);

                    comm.Parameters.Add("@EmpleadoId", OleDbType.VarChar).Value = empleadoId;
                    comm.Parameters.Add("@Fecha", OleDbType.VarChar).Value = fecha;
                    comm.Parameters.Add("@Estado", OleDbType.Integer).Value = 1;
                    comm.Parameters.Add("@Enviado", OleDbType.VarChar).Value = enviado;
                    comm.Parameters.Add("@Terminal", OleDbType.VarChar).Value = terminal;
                    comm.Parameters.Add("@Coordenada", OleDbType.VarChar).Value = coordenadas;
                    conection.Open();
                    int iResultado = comm.ExecuteNonQuery();
                    conection.Close();
                }
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }
            
        }

        private void ConsultarApi() {
            Resultado = Utilidad<Empleado>.GetJson(new Empleado(), Huella.Api+Constante.ConsultarApi + txtCodEmpleado.Text+"&clave=" + Huella.ApiKey);
            if (Resultado != null)
            {
                txtNombres.Text = Resultado.nombres;
                txtDoc.Text = Resultado.tipoDoc;
                txtNroDoc.Text = Resultado.nroDoc;
                txtCodEmp.Text = Resultado.codigo;

                if (Resultado.huella != null)
                {
                    string dummyData = Resultado.huella.Trim().Replace(" ", "+");
                    if (dummyData.Length % 4 > 0)
                        dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                    byte[] huellaByte = Convert.FromBase64String(dummyData);

                    Resultado.huellaByte = huellaByte;
                    pbImageFrame.Image = Image.FromFile(DirectorioPrincipal+"bien.png");
                    btnRegistrar.Enabled = true;
                }
                else {
                    pbImageFrame.Image = Image.FromFile(DirectorioPrincipal+"mal.png");
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
            try {
                if (Resultado != null)
                {
                    if (Resultado.huella != null || Huella.HuellaTomada == 1)
                    {
                        try
                        {
                            if (Huella.HuellaTomada == 1)
                            {
                                RegistrarHuellaApi();
                            }
                        }
                        catch (Exception x)
                        {
                            MessageBox.Show(x.Message, "Registro no seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        Huella.RegistrarEmpleado(Resultado);
                        Huella.UpdateDatabaseList();

                        try
                        {
                            RegistrarAsistenciaApi();
                        }
                        catch (Exception y)
                        {
                            MessageBox.Show(y.Message, "Registro no seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        Limpiar();
                    }
                    else
                    {
                        MessageBox.Show("Por favor ingrese la huella", "Ingrese huella", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                else
                {
                    MessageBox.Show("No se ha seleccionado ningun registro", "Registro no seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ec) {
                MessageBox.Show(ec.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
