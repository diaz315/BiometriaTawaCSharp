using Suprema;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using UareUSampleCSharp;

namespace BiometriaTawaCSharp
{
    public partial class RegistroEmpleado : Form
    {
        public static Empleado Resultado;

        private static string BdSqlite = Program.DirectorioPrincipalProd + "UFDatabase.db";

        public RegistroEmpleado()
        {
            InitializeComponent();
            pbImageFrame.Image = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try {
                if (txtCodEmpleado.Text == "")
                {
                    throw new Exception(Mensajes.IngresarCodigo);
                }

                /*if (ObtenerEmpleado(txtCodEmpleado.Text) > 0)
                {
                    btnRegistrar.Enabled = false;
                }*/

                ConsultarApi();
                if (ObtenerEmpleado(txtCodEmpleado.Text) > 0) 
                {
                    btnRegistrar.Enabled = false;
                    MessageBox.Show(Mensajes.ColaboradorExistente);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

        }


        private int ObtenerEmpleado(string codigo)
        {
            using (var conection = Utilidad<Empleado>.ConnSqlite(BdSqlite))
            {
                var query = "Select Count(Serial) From Fingerprints where CodEmpleado="+"'"+codigo+"'";
                SQLiteCommand command = new SQLiteCommand(query, conection);

                var reader = command.ExecuteReader();
                var Empleado = 0;
                while (reader.Read())
                {
                    Empleado=(int.Parse(reader[0].ToString()));
                }
               
                reader.Close();
                conection.Close();

                return Empleado;
            }
        }

        public static void ActualizarRegistroLocalEnviado(int id)
        {
            try
            {
                using (var conection = Utilidad<Empleado>.ConnSqlite(BdSqlite))
                {
                    SQLiteCommand comm = new SQLiteCommand("UPDATE Asistencia SET Enviado=1 WHERE Id=?", conection);
                    comm.Parameters.AddWithValue("@Id", id);
                    int iResultado = comm.ExecuteNonQuery();
                    conection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        public static void EliminarHuellaLocal(string codEmpleado)
        {
            try
            {
                using (var conection = Utilidad<Empleado>.ConnSqlite(BdSqlite))
                {
                    SQLiteCommand comm = new SQLiteCommand("UPDATE FingerPrints SET Template1='',Gui_Huella='' WHERE CodEmpleado=?", conection);
                    comm.Parameters.AddWithValue("@CodEmpleado", codEmpleado);
                    int iResultado = comm.ExecuteNonQuery();
                    conection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static List<Empleado> ObtenerResgistrosSinEnviar()
        {
            var ListEmpleado = new List<Empleado>();

            try
            {
                using (var conection = Utilidad<Empleado>.ConnSqlite(BdSqlite))
                {
                    var query = "SELECT Id,EmpleadoId,Fecha,Estado,Terminal,Coordenadas FROM Asistencia WHERE Enviado=0";
                    SQLiteCommand command = new SQLiteCommand(query, conection);
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
                    
                    reader.Close();
                    conection.Close();

                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            return ListEmpleado;
        }

        private string RegistrarHuellaApi()
        {
            if(Huella.huellaBase64==null)
            {   
                var auxImg = "TempRE.png";
                if (File.Exists(auxImg)) {
                    File.Delete(auxImg);
                }

                pbImageFrame.Image.Save(auxImg, ImageFormat.Png);
                var bytes = File.ReadAllBytes(auxImg);
                Huella.huellaBase64 = Convert.ToBase64String(bytes);
            }

            var huella = Convert.ToBase64String(Utilidad<Empleado>.ExtraerTemplate(Huella.huellaBase64).Template);//Convert.ToBase64String(Resultado.huellaByte);
            var coordenada = Huella.txtCoordenada.Text;
            var terminal = Utilidad<Empleado>.GetIp() + "::" + Utilidad<Empleado>.GetMacAddress().ToString();
            var param = "empleadoId=" + Resultado.id + "&huella=" + huella + "&terminal=" + terminal + "&coordenadas=" + coordenada+"&clave="+Huella.ApiKey;             
            var empleado=Utilidad<Empleado>.GetJson(new Empleado(), Huella.Api+Constante.RegistrarHuellaApi + param);
            if (empleado.error == true)
            {
                throw new Exception(empleado.mensaje);
            }
            return empleado.guiHuella;
        }

        private string ActualizarHuellaApi(string codEmpleado,string huella)
        {
            try
            {
                var coordenada = Huella.txtCoordenada.Text;
                var terminal = Utilidad<Empleado>.GetIp() + "::" + Utilidad<Empleado>.GetMacAddress().ToString();
                var param = "codEmpleado=" + codEmpleado + "&huella=" + huella + "&terminal=" + terminal + "&coordenadas=" + coordenada + "&clave=" + Huella.ApiKey;
                var empleado=Utilidad<Empleado>.GetJson(new Empleado(), Huella.Api + Constante.ActualizarHuellaApi + param);
                if (empleado.error==true) {
                    throw new Exception(empleado.mensaje);
                }
                return empleado.guiHuella;
            }
            catch (Exception ex) {
                throw ex;
            }
        }


        public static void ProcesarDatosNoEnviados() {
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
            catch {}
        }



        public static void ProcesarDatosNoEnviadosAux()
        {
            List<Empleado> RegSinEnviar = ObtenerResgistrosSinEnviar();
            try
            {
                if (RegSinEnviar.Count > 0)
                {
                    foreach (Empleado emp in RegSinEnviar)
                    {
                        var param = "empleadoId=" + emp.idEmpleado + "&terminal=" + emp.terminal + "&coordenadas=" + emp.coordenadas + "&fecha=" + emp.fecha + "&clave=" + Huella.ApiKey;
                        string Url = Huella.Api + Constante.RegAsistenciaApi + param;
                        Empleado empleado = Utilidad<Empleado>.GetJson(new Empleado(), Url);
                        if (empleado.resultado)
                        {
                            ActualizarRegistroLocalEnviado(int.Parse(emp.ids));
                        }
                    }
                }
            }
            catch (Exception ex){
                throw ex;
            }
        }

        public static void RegistrarAsistenciaApi(int id=0)
        {
            try
            {
                int empId = 0;

                if (id > 0)
                {
                    empId = id;
                }
                else if (Resultado.id != 0)
                {
                    empId = Resultado.id;
                }

                if (empId > 0)
                {
                    var coordenada = Huella.txtCoordenada.Text;
                    var terminal = Utilidad<Empleado>.GetIp() + "::" + Utilidad<Empleado>.GetMacAddress().ToString();
                    var fecha = DateTime.Now;

                    RegistrarAsistenciaLocal(empId, fecha, 0, terminal, coordenada);
                }
                //asistencia sin enviar
                ProcesarDatosNoEnviados();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, Mensajes.Error, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        public static void RegistrarAsistenciaLocal(int empleadoId, DateTime fecha, int enviado, string terminal, string coordenadas)
        {
            try
            {
                using (var conection = Utilidad<Empleado>.ConnSqlite(BdSqlite))
                {
                    SQLiteCommand comm = new SQLiteCommand("INSERT INTO Asistencia(EmpleadoId,Fecha,Estado,Enviado,Terminal,Coordenadas) VALUES (?,?,?,?,?,?)", conection);

                    comm.Parameters.AddWithValue("@EmpleadoId",  empleadoId);
                    comm.Parameters.AddWithValue("@Fecha", fecha);
                    comm.Parameters.AddWithValue("@Estado", 1);
                    comm.Parameters.AddWithValue("@Enviado", enviado);
                    comm.Parameters.AddWithValue("@Terminal", terminal);
                    comm.Parameters.AddWithValue("@Coordenada", coordenadas);
                    int iResultado = comm.ExecuteNonQuery();
                    conection.Close();
                }
            }
            catch (Exception ex){
                throw ex;
            }
            
        }

        private void ConsultarApi() {
            try {
                Resultado = Utilidad<Empleado>.GetJson(new Empleado(), Huella.Api + Constante.ConsultarApi + txtCodEmpleado.Text + "&clave=" + Huella.ApiKey);
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
                        pbImageFrame.Image = Image.FromFile(Program.DirectorioPrincipal + "bien.png");
                        btnRegistrar.Enabled = true;
                        btnEliminarHuella.Visible = true;
                    }
                    else
                    {
                        pbImageFrame.Image = Image.FromFile(Program.DirectorioPrincipal + "mal.png");
                        btnRegistrar.Enabled = false;
                        btnEliminarHuella.Visible = false;
                    }
                }
                else
                {
                    Resultado = null;
                    MessageBox.Show(Mensajes.CodigoEmpleadoNoEncontrado, Mensajes.RegistroNoEncontrado, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }catch(Exception ex){
                throw ex;
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
            try
            {
                if (Resultado != null)
                {
                    if (Resultado.huella != null || Huella.HuellaTomada == 1)
                    {
                        try
                        {
                            if (Huella.HuellaTomada == 1)
                            {
                                Resultado.guiHuella=RegistrarHuellaApi();
                            }
                        }
                        catch (Exception x)
                        {
                            throw new Exception(x.Message + " "+Mensajes.RegistrosSinSeleccionar);
                        }

                        Huella.RegistrarEmpleado(Resultado);
                    }
                    else
                    {
                        throw new Exception(Mensajes.ColocarIndiceScan);
                    }
                }
                else
                {
                    throw new Exception(Mensajes.RegistrosSinSeleccionar);
                }
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.Message + Mensajes.PermisoAdmin, Mensajes.Error, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally {
                Resultado = null;
                btnRegistrar.Enabled = false;
                btnEliminarHuella.Visible = false;
                Limpiar();
            }
        }

        private void pbImageFrame_Click(object sender, EventArgs e)
        {
            if (SeleccionBiometrico.Biometrico == 0)
            {
                BiometricoEikon();
            }
            else
            {
                BiometricoSuprema();
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void btnEliminarHuella_Click(object sender, EventArgs e)
        {
            try
            {
                ActualizarHuellaApi(txtCodEmpleado.Text, "");
                EliminarHuellaLocal(txtCodEmpleado.Text);
                Resultado.huella = null;
                btnEliminarHuella.Visible = false;
                pbImageFrame.Image = Image.FromFile(Program.DirectorioPrincipal + "mal.png");
                MessageBox.Show(Mensajes.EliminadoHuella, Mensajes.Exito, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally {
                //Limpiar();
            }
        }

        private void BiometricoEikon()
        {
            try
            {
                new Form_Main(2);
                Huella.HuellaTomada = 1;
                btnRegistrar.Enabled = true;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }     
        
        private void BiometricoSuprema()
        {
            try
            {
                if (Resultado != null && Resultado.huella == null)
                {
                    Empleado resul = Huella.IniciarEscaneo();
                    Resultado.huellaByte = resul.huellaByte;
                    Resultado.pesoHuella = resul.pesoHuella;
                    if (resul.pesoHuella > 0)
                    {
                        btnRegistrar.Enabled = true;
                    }
                }
            }
            catch { }
        }

    }
}
