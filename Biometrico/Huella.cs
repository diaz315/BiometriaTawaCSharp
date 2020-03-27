using BiometriaTawaCSharp;
using SourceAFIS.Simple;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using UareUSampleCSharp;

[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]

namespace Suprema
{
    public class Huella : Form
    {
        private UFScannerManager m_ScannerManager;
        private static UFScanner m_Scanner;
        private static UFDatabase m_Database;
        private UFMatcher m_Matcher;
        private static string m_strError;
        private int m_Serial;
        private string m_UserID;
        private int m_FingerIndex;
        private static byte[] m_Template1;
        private static int m_Template1Size;
        private byte[] m_Template2;
        private int m_Template2Size;
        private string m_Memo;
        private IContainer components;
        private Button btnInit;
        private Button btnUninit;
        private GroupBox groupBox1;
        private Button btnSelectionDelete;
        private Button btnSelectionVerify;
        private Button btnSelectionUpdateUserInfo;
        private Button btnDeleteAll;
        public static ListView lvDatabaseList;
        public static TextBox tbxMessage;
        private Button btnClear;
        public  static PictureBox pbImageFrame;
        private Label label1;
        private ComboBox cbScanTemplateType;
        private Button btnSelectionUpdateTemplate;
        public static int form;
        public static int HuellaTomada = 0;
        private Label label2;
        private GroupBox groupBox2;
        public static int BdIniciada = 0;
        public static TextBox txtCoordenada;
        public static string Api;
        public static string ApiKey;


        //private static string DirectorioPrincipal = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar+".."+ Path.DirectorySeparatorChar+".."+ Path.DirectorySeparatorChar;
        private static string DirectorioPrincipal = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar;
        private static string BdSqlite = DirectorioPrincipal + "UFDatabase.db";
        private MenuStrip menuStrip1;
        private ToolStripMenuItem menuToolStripMenuItem;
        private ToolStripMenuItem marcarHuellaToolStripMenuItem;
        private ToolStripMenuItem agregarColaboradorToolStripMenuItem;
        private ToolStripMenuItem sincronizarToolStripMenuItem;
        private ToolStripMenuItem asistenciaToolStripMenuItem;
        private ToolStripMenuItem huellasToolStripMenuItem;
        private AfisEngine Afis = new AfisEngine();
        private ToolStripMenuItem huelleroEikonToolStripMenuItem;
        public static string huellaBase64;

        public Huella()
        {
            try
            {
                this.InitializeComponent();
                txtCoordenada.Visible = false;
                new CLocation().GetLocationProperty();
                Api = LeerArchivo(@"C:\Api.txt");
                ApiKey = LeerArchivo(@"C:\ApiKey.txt");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static float FixScore(float score)
        {
            double ScoreScaling = 75;
            double BendingThreshold = 0.67;
            double BendingFactor = 5;

            double similarity = score / ScoreScaling;
            if (similarity > BendingThreshold)
                similarity = (similarity - BendingThreshold) / BendingFactor + BendingThreshold;
            if (similarity > 1)
                similarity = 1;
            if (similarity < 0)
                similarity = 0;
            return (float)similarity;
        }


        private string LeerArchivo(string ruta)
        {
            return File.ReadAllText(ruta);
        }

        private void Huella_Load(object sender, EventArgs e)
        {
            m_ScannerManager = new UFScannerManager(this);
            m_Scanner = null;
            m_Database = null;
            this.m_Matcher = null;
            m_Template1 = new byte[1024];
            this.m_Template2 = new byte[1024];
            lvDatabaseList.Columns.Add("N#", 50, HorizontalAlignment.Left);
            lvDatabaseList.Columns.Add("Código", 80, HorizontalAlignment.Left);
            lvDatabaseList.Columns.Add("Nombres", 170, HorizontalAlignment.Left);
            lvDatabaseList.Columns.Add("Tipo. Doc", 80, HorizontalAlignment.Left);
            lvDatabaseList.Columns.Add("Nro. Doc", 80, HorizontalAlignment.Left);
        }
        private void Huella_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnUninit_Click(sender, e);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbxMessage.Clear();
        }
        private static void AddRow(int numero, string Nombres, string TipoDoc, string NroDoc, string CodColaborador)
        {
            ListViewItem  listViewItem = lvDatabaseList.Items.Add(Convert.ToString(numero));
            listViewItem.SubItems.Add(CodColaborador);
            listViewItem.SubItems.Add(Nombres);
            listViewItem.SubItems.Add(TipoDoc);
            listViewItem.SubItems.Add(NroDoc);
        }

        public static void UpdateDatabaseList()
        {
            var Empleados = ObtenerEmpleado();
            int i = 1;
            foreach (Empleado obj in Empleados)
            {
                AddRow(i, obj.nombres, obj.tipoDoc, obj.nroDoc, obj.codigo);
                i++;
            }
        }

        private static void GetDrawCapturedImage(UFScanner Scanner)
        {
            Bitmap bitmap;
            int resolucion;

            Scanner.GetCaptureImageBuffer(out bitmap, out resolucion);

            Bitmap bImage = bitmap;  // Your Bitmap Image
            MemoryStream ms = new MemoryStream();
            bImage.Save(ms, ImageFormat.Png);
            byte[] byteImage = ms.ToArray();

            huellaBase64 = Convert.ToBase64String(byteImage);
        }

        private static void DrawCapturedImage(UFScanner Scanner)
        {
            if (form == 1)
            {
                Graphics graphics = pbImageFrame.CreateGraphics();

                Rectangle rect = new Rectangle(0, 0, pbImageFrame.Width, pbImageFrame.Height);
                try
                {
                    Scanner.DrawCaptureImageBuffer(graphics, rect, false);
                }
                finally
                {
                    graphics.Dispose();
                }
            }
            else
            {
                Graphics graphics2 = RegistroEmpleado.pbImageFrame.CreateGraphics();

                Rectangle rect = new Rectangle(0, 0, RegistroEmpleado.pbImageFrame.Width, RegistroEmpleado.pbImageFrame.Height);
                try
                {
                    Scanner.DrawCaptureImageBuffer(graphics2, rect, false);
                    HuellaTomada = 1;
                }
                finally
                {
                    graphics2.Dispose();
                }
            }

            GetDrawCapturedImage(Scanner);

        }
        private void btnInit_Click(object sender, EventArgs e)
        {
            InicializarBD();
            btnUninit.Visible = true;
            btnInit.Visible = false;
        }

        public void InicializarBD()
        {
            if (BdIniciada == 0)
            {
                BdIniciada = 1;
                Cursor.Current = Cursors.WaitCursor;
                UFS_STATUS uFS_STATUS = m_ScannerManager.Init();
                Cursor.Current = this.Cursor;
                if (uFS_STATUS != UFS_STATUS.OK)
                {
                    UFScanner.GetErrorString(uFS_STATUS, out m_strError);
                    tbxMessage.AppendText("UFScanner Init: " + m_strError + "\r\n");
                    return;
                }
                tbxMessage.AppendText("UFScanner Init: OK\r\n");
                int count = m_ScannerManager.Scanners.Count;
                tbxMessage.AppendText("UFScanner GetScannerNumber: " + count + "\r\n");
                if (count == 0)
                {
                    tbxMessage.AppendText("There's no available scanner\r\n");
                    return;
                }
                tbxMessage.AppendText("First scanner will be used\r\n");
                m_Scanner = m_ScannerManager.Scanners[0];
                UpdateDatabaseList();
            }

        }

        public void Desconectar()
        {
            Cursor.Current = Cursors.WaitCursor;
            UFS_STATUS uFS_STATUS = m_ScannerManager.Uninit();
            Cursor.Current = this.Cursor;
            if (uFS_STATUS == UFS_STATUS.OK)
            {
                tbxMessage.AppendText("UFScanner Uninit: OK\r\n");
            }
            else
            {
                UFScanner.GetErrorString(uFS_STATUS, out m_strError);
                tbxMessage.AppendText("UFScanner Uninit: " + m_strError + "\r\n");
            }
           
            lvDatabaseList.Items.Clear();
            BdIniciada = 0;
        }

        private void btnUninit_Click(object sender, EventArgs e)
        {
            Desconectar();
        }
        private static bool ExtractTemplate(byte[] Template, out int TemplateSize)
        {
            m_Scanner.ClearCaptureImageBuffer();
            tbxMessage.AppendText("Colocar dedo\r\n");
            TemplateSize = 0;
            UFS_STATUS uFS_STATUS;
            while (true)
            {
                uFS_STATUS = m_Scanner.CaptureSingleImage();
                if (uFS_STATUS != UFS_STATUS.OK)
                {
                    break;
                }
                int num;
                uFS_STATUS = m_Scanner.ExtractEx(1024, Template, out TemplateSize, out num);
                if (uFS_STATUS == UFS_STATUS.OK)
                {
                    goto IL_A4;
                }
                UFScanner.GetErrorString(uFS_STATUS, out m_strError);
                tbxMessage.AppendText("UFScanner Extracto: " + m_strError + "\r\n");
            }
            UFScanner.GetErrorString(uFS_STATUS, out m_strError);
            tbxMessage.AppendText("UFScanner CaptureUnicaImage: " + m_strError + "\r\n");
            MessageBox.Show(Mensajes.IntentarNuevamente, Mensajes.IdentificaionFallida, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        IL_A4:
            tbxMessage.AppendText("UFScanner Extracto: OK\r\n");
            return true;
        }

        public static Empleado IniciarEscaneo()
        {
            MessageBox.Show(Mensajes.ColocarIndiceScan, Mensajes.Error, MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (!ExtractTemplate(m_Template1, out m_Template1Size))
            {
                return null;
            }
            else
            {
                DrawCapturedImage(m_Scanner);
                var res = new Empleado();
                res.huellaByte = m_Template1;
                res.pesoHuella = m_Template1Size;

                return res;
            }

        }

        public static Empleado CompararHuellas(List<Empleado> empleados)
        {
            Empleado resultado = new Empleado();

            try
            {
                if (huellaBase64 == null) {
                    pbImageFrame.Image.Save("testing.png", ImageFormat.Png);
                    var x = File.ReadAllBytes("testing.png");
                    huellaBase64= Convert.ToBase64String(x);
                }


                var huellaOriginal = Utilidad<Empleado>.ExtraerTemplate(huellaBase64);

                Fingerprint fingers = new Fingerprint();

                if (empleados.Count() > 0)
                {
                    foreach (Empleado empleado in empleados)
                    {
                        fingers = new Fingerprint { Template = Utilidad<Empleado>.convertStringToByte(empleado.huella) };
                        AfisEngine afis = new AfisEngine();
                        Person probe = new Person(huellaOriginal);

                        var huella = new List<Fingerprint>();
                        huella.Add(fingers);

                        Person candidate = new Person { Fingerprints= huella };
                        afis.Threshold = 0;
                        float score = afis.Verify(probe, candidate);

                        resultado = empleado;
                        resultado.coincidencia= FixScore(score);

                        if (resultado.coincidencia >= 0.50) {
                            break;
                        }
                    }

                }
                else
                {
                    throw new Exception("Aun no hay registros.");
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return resultado;
        }

        private void btnEnroll_Click(object sender, EventArgs e)
        {
            IniciarEscaneo();
            UserInfoForms userInfoForm = new UserInfoForms(false);
            tbxMessage.AppendText("Ingrese data de usuario\r\n");
            if (userInfoForm.ShowDialog(this) != DialogResult.OK)
            {
                tbxMessage.AppendText("El ingreso de data ha sido cancelada por el usuario\r\n");
                return;
            }
            UFD_STATUS uFD_STATUS = m_Database.AddData(userInfoForm.UserID, userInfoForm.FingerIndex, m_Template1, m_Template1Size, null, 0, userInfoForm.Memo);
            if (uFD_STATUS != UFD_STATUS.OK)
            {
                UFDatabase.GetErrorString(uFD_STATUS, out m_strError);
                tbxMessage.AppendText("UFDatabase AgregarData: " + m_strError + "\r\n");
            }
            else
            {
                this.cbScanTemplateType.Enabled = false;
            }
            UpdateDatabaseList();
        }

        public static void MarcarHuellaDos() {
            var listaHuellaBd = ObtenerEmpleadoHuella();
            var coincidencia = CompararHuellas(listaHuellaBd);

            if (coincidencia.coincidencia >= 0.50)
            {
                var Empleado = ObtenerEmpleado(int.Parse(coincidencia.idEmpleado));
                MessageBox.Show(Empleado[0] + " " + Empleado[1], Mensajes.MarcacionExitosa, MessageBoxButtons.OK, MessageBoxIcon.Information);
                RegistroEmpleado.RegistrarAsistenciaApi(int.Parse(Empleado[2]));
            }
            else
            {
                MessageBox.Show("Registro no encontrado", Mensajes.MarcacionExitosa, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void MarcarHuella()
        {
            InicializarBD();
            form = 1;
            byte[] array = new byte[1024];

            int template1Size;
            MessageBox.Show(Mensajes.ColocarIndiceScan, Mensajes.IngresarHuella, MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (!ExtractTemplate(array, out template1Size))
            {
                return;
            }
            DrawCapturedImage(m_Scanner);
            Cursor.Current = Cursors.WaitCursor;

            var listaHuellaBd = ObtenerEmpleadoHuella();
            var coincidencia = CompararHuellas(listaHuellaBd);

            if (coincidencia.coincidencia >= 0.50)
            {
                var Empleado = ObtenerEmpleado(int.Parse(coincidencia.idEmpleado));
                MessageBox.Show(Empleado[0] + " " + Empleado[1], Mensajes.MarcacionExitosa, MessageBoxButtons.OK, MessageBoxIcon.Information);
                RegistroEmpleado.RegistrarAsistenciaApi(int.Parse(Empleado[2]));
            }
            else {
                MessageBox.Show("Registro no encontrado", Mensajes.MarcacionExitosa, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static List<string> ObtenerEmpleado(int id)
        {
            using (var conection = Utilidad<Empleado>.ConnSqlite(BdSqlite))
            {
                var query = "Select Nombres,CodEmpleado,EmpleadoId From Fingerprints where Serial=" + id;
                SQLiteCommand command = new SQLiteCommand(query, conection);
                var reader = command.ExecuteReader();
                var Empleado = new List<string>();
                while (reader.Read())
                {
                    Empleado.Add(reader[0].ToString());
                    Empleado.Add(reader[1].ToString());
                    Empleado.Add(reader[2].ToString());
                }
                return Empleado;

            }
        }

        private static List<Empleado> ObtenerEmpleadoHuella()
        {
            using (var conection = Utilidad<Empleado>.ConnSqlite(BdSqlite))
            {
                var query = "Select Nombres,CodEmpleado,EmpleadoId,Template1 From Fingerprints where Estado=1";
                SQLiteCommand command = new SQLiteCommand(query, conection);
                var reader = command.ExecuteReader();
                var Empleado = new List<Empleado>();
                while (reader.Read())
                {
                    Empleado.Add(new Empleado { nombres = reader[0].ToString(), codigo = reader[1].ToString(), idEmpleado = reader[2].ToString(), huella = reader[3].ToString() });
                }
                conection.Close();
                return Empleado;

            }
        }

        private static List<Empleado> ObtenerEmpleado()
        {
            using (var conection = Utilidad<Empleado>.ConnSqlite(BdSqlite))
            {
                var query = "Select Nombres,CodEmpleado,Documento,NroDocumento From Fingerprints where Estado=1";
                SQLiteCommand command = new SQLiteCommand(query, conection);
                var reader = command.ExecuteReader();
                var Empleado = new List<Empleado>();

                while (reader.Read())
                {
                    Empleado.Add(new Empleado() { nombres = reader[0].ToString(), codigo = reader[1].ToString(), tipoDoc = reader[2].ToString(), nroDoc = reader[3].ToString() });
                }
                return Empleado;

            }
        }

        private static int ContadorEmpleadoPorCodigo()
        {

            using (var conection = Utilidad<Empleado>.ConnSqlite(BdSqlite))
            {
                var query = "Select count(CodEmpleado) From Fingerprints where CodEmpleado='" + RegistroEmpleado.Resultado.codigo + "'";
                SQLiteCommand command = new SQLiteCommand(query, conection);
                var reader = command.ExecuteReader();
                reader.Read();
                var count = Convert.ToInt32(reader[0].ToString());
                conection.Close();
                return count;
            }

        }

        public static void RegistrarEmpleado(Empleado obj)
        {

            int count = ContadorEmpleadoPorCodigo();

            if (count == 0)
            {
                RegistrarEmpleadoLocal(obj);
            }
            else
            {
                ActualizarEmpleadoLocal(obj);
            }
        }

        private void test()
        {

            using (var ctx = Utilidad<Empleado>.ConnSqlite(BdSqlite))
            {
                string query = "SELECT count(*) FROM sqlite_master WHERE type = 'table' AND name != 'android_metadata' AND name != 'sqlite_sequence';";

                using (var command = new SQLiteCommand(query, ctx))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        MessageBox.Show(reader[0].ToString());

                    }
                }

            }

        }

        private static void ActualizarEmpleadoLocal(Empleado obj, bool mensaje = true)
        {
            using (var conection = Utilidad<Empleado>.ConnSqlite(BdSqlite))
            {
                SQLiteCommand comm = new SQLiteCommand("UPDATE Fingerprints set Template1=?,Gui_Huella=? where CodEmpleado=?", conection);
                SQLiteParameter parImagen = new SQLiteParameter("@imagen", Utilidad<Empleado>.convertByteToString(Utilidad<Empleado>.ExtraerTemplate(huellaBase64).Template));
                parImagen.Value = obj.huellaByte;
                comm.Parameters.Add(parImagen);
                comm.Parameters.AddWithValue("@Gui_Huella", obj.guiHuella);
                comm.Parameters.AddWithValue("@CodEmpleado", obj.codigo);
                int iResultado = comm.ExecuteNonQuery();
                conection.Close();
                HuellaTomada = 0;
                huellaBase64 = null;
                if (mensaje)
                {
                    MessageBox.Show(Mensajes.RegistroExitoso);
                }
            }
        }

        private static void RegistrarEmpleadoLocal(Empleado obj)
        {

            string query = "INSERT INTO Fingerprints(Nombres,Documento,FingerIndex,Template1,NroDocumento,CodEmpleado,Estado,EmpleadoId,Gui_Huella) VALUES (?,?,?,?,?,?,?,?,?)";

            using (var connection = Utilidad<Empleado>.ConnSqlite(BdSqlite))
            {

                SQLiteCommand comm = new SQLiteCommand(query, connection);

                SQLiteParameter parImagen = new SQLiteParameter("@imagen", Utilidad<Empleado>.convertByteToString(Utilidad<Empleado>.ExtraerTemplate(huellaBase64).Template));

                comm.Parameters.AddWithValue("@Nombres", obj.nombres.Trim());
                comm.Parameters.AddWithValue("@TipoDoc", obj.tipoDoc.Trim());
                comm.Parameters.AddWithValue("@FingerIndex", obj.dedo);
                comm.Parameters.Add(parImagen);
                comm.Parameters.AddWithValue("@NroDocumento", obj.nroDoc);
                comm.Parameters.AddWithValue("@CodEmpleado", obj.codigo);
                comm.Parameters.AddWithValue("@Estado", 1);
                comm.Parameters.AddWithValue("@EmpleadoId", obj.id);
                comm.Parameters.AddWithValue("@Gui_Huella", obj.guiHuella);

                int iResultado = comm.ExecuteNonQuery();
                connection.Close();

                HuellaTomada = 0;
                huellaBase64 = null;
                MessageBox.Show(Mensajes.RegistroExitoso);

            }

        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            UFD_STATUS uFD_STATUS = m_Database.RemoveAllData();
            if (uFD_STATUS == UFD_STATUS.OK)
            {
                tbxMessage.AppendText("UFDatabase RemoveAllData: OK\r\n");
                UpdateDatabaseList();
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out m_strError);
            tbxMessage.AppendText("UFDatabase RemoveAllData: " + m_strError + "\r\n");
        }
        private void btnSelectionDelete_Click(object sender, EventArgs e)
        {
            if (lvDatabaseList.SelectedIndices.Count == 0)
            {
                tbxMessage.AppendText("Seleccione el registro\r\n");
                return;
            }
            int serial = Convert.ToInt32(lvDatabaseList.SelectedItems[0].SubItems[0].Text);
            UFD_STATUS uFD_STATUS = m_Database.RemoveDataBySerial(serial);
            if (uFD_STATUS == UFD_STATUS.OK)
            {
                tbxMessage.AppendText("UFDatabase RemoveDataBySerial: OK\r\n");
                UpdateDatabaseList();
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out m_strError);
            tbxMessage.AppendText("UFDatabase RemoveDataBySerial: " + m_strError + "\r\n");
        }
        private void btnSelectionUpdateUserInfo_Click(object sender, EventArgs e)
        {
            UserInfoForms userInfoForm = new UserInfoForms(true);
            if (lvDatabaseList.SelectedIndices.Count == 0)
            {
                tbxMessage.AppendText("Seleccione el registro\r\n");
                return;
            }
            int serial = Convert.ToInt32(lvDatabaseList.SelectedItems[0].SubItems[0].Text);
            userInfoForm.UserID = lvDatabaseList.SelectedItems[0].SubItems[1].Text;
            userInfoForm.FingerIndex = Convert.ToInt32(lvDatabaseList.SelectedItems[0].SubItems[2].Text);
            userInfoForm.Memo = lvDatabaseList.SelectedItems[0].SubItems[5].Text;
            tbxMessage.AppendText("Datos de usuario actualizado\r\n");
            tbxMessage.AppendText("Id usuario y dedo indice no seran actualizados\r\n");
            if (userInfoForm.ShowDialog(this) != DialogResult.OK)
            {
                tbxMessage.AppendText("El ingreso de datos ha sido cancelado por el usuario\r\n");
                return;
            }
            UFD_STATUS uFD_STATUS = m_Database.UpdateDataBySerial(serial, null, 0, null, 0, userInfoForm.Memo);
            if (uFD_STATUS == UFD_STATUS.OK)
            {
                tbxMessage.AppendText("UFD_UpdateDataBySerial: OK\r\n");
                UpdateDatabaseList();
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out m_strError);
            tbxMessage.AppendText("UFDatabase UpdateDataBySerial: " + m_strError + "\r\n");
        }
        private void btnSelectionUpdateTemplate_Click(object sender, EventArgs e)
        {
            if (lvDatabaseList.SelectedIndices.Count == 0)
            {
                tbxMessage.AppendText("Seleccione el registro\r\n");
                return;
            }
            int serial = Convert.ToInt32(lvDatabaseList.SelectedItems[0].SubItems[0].Text);
            if (!ExtractTemplate(m_Template1, out m_Template1Size))
            {
                return;
            }
            DrawCapturedImage(m_Scanner);
            UFD_STATUS uFD_STATUS = m_Database.UpdateDataBySerial(serial, m_Template1, m_Template1Size, null, 0, null);
            if (uFD_STATUS == UFD_STATUS.OK)
            {
                tbxMessage.AppendText("UFD_UpdateDataBySerial: OK\r\n");
                UpdateDatabaseList();
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out m_strError);
            tbxMessage.AppendText("UFDatabase UpdateDataBySerial: " + m_strError + "\r\n");
        }
        private void btnSelectionVerify_Click(object sender, EventArgs e)
        {
            byte[] array = new byte[1024];
            if (lvDatabaseList.SelectedIndices.Count == 0)
            {
                tbxMessage.AppendText("Seleccione el registro\r\n");
                return;
            }
            int num = Convert.ToInt32(lvDatabaseList.SelectedItems[0].SubItems[0].Text);
            UFD_STATUS dataBySerial = m_Database.GetDataBySerial(num, out this.m_UserID, out this.m_FingerIndex, m_Template1, out m_Template1Size, this.m_Template2, out this.m_Template2Size, out this.m_Memo);
            if (dataBySerial != UFD_STATUS.OK)
            {
                UFDatabase.GetErrorString(dataBySerial, out m_strError);
                tbxMessage.AppendText("UFDatabase UpdateDataBySerial: " + m_strError + "\r\n");
                return;
            }
            int template1Size;
            if (!ExtractTemplate(array, out template1Size))
            {
                return;
            }
            DrawCapturedImage(m_Scanner);
            bool flag;
            UFM_STATUS uFM_STATUS = this.m_Matcher.Verify(array, template1Size, m_Template1, m_Template1Size, out flag);
            if (uFM_STATUS != UFM_STATUS.OK)
            {
                UFMatcher.GetErrorString(uFM_STATUS, out m_strError);
                tbxMessage.AppendText("UFMatcher Verify: " + m_strError + "\r\n");
                return;
            }
            if (flag)
            {
                tbxMessage.AppendText("Verificación exitosa (Serial = " + num + ")\r\n");
                return;
            }
            tbxMessage.AppendText("Verificación fallo\r\n");
        }
        private void bScanTemplateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Scanner == null)
            {
                tbxMessage.AppendText("Sin instancias de escaner\r\n");
                return;
            }
            switch (this.cbScanTemplateType.SelectedIndex)
            {
                case 0:
                    m_Scanner.nTemplateType = 2001;

                    this.m_Matcher.nTemplateType = 2001;

                    return;
                case 1:
                    m_Scanner.nTemplateType = 2002;

                    this.m_Matcher.nTemplateType = 2002;

                    return;
                case 2:
                    m_Scanner.nTemplateType = 2003;

                    this.m_Matcher.nTemplateType = 2003;

                    return;
                default:
                    return;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.btnInit = new System.Windows.Forms.Button();
            this.btnUninit = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbScanTemplateType = new System.Windows.Forms.ComboBox();
            this.btnSelectionUpdateTemplate = new System.Windows.Forms.Button();
            this.btnSelectionVerify = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelectionUpdateUserInfo = new System.Windows.Forms.Button();
            this.btnSelectionDelete = new System.Windows.Forms.Button();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            lvDatabaseList = new System.Windows.Forms.ListView();
            tbxMessage = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            pbImageFrame = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            txtCoordenada = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.marcarHuellaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.agregarColaboradorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sincronizarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asistenciaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.huellasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.huelleroEikonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(pbImageFrame)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnInit
            // 
            this.btnInit.AccessibleDescription = "";
            this.btnInit.Location = new System.Drawing.Point(6, 183);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(84, 24);
            this.btnInit.TabIndex = 0;
            this.btnInit.Text = "Iniciar";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // btnUninit
            // 
            this.btnUninit.AccessibleDescription = "";
            this.btnUninit.Location = new System.Drawing.Point(5, 213);
            this.btnUninit.Name = "btnUninit";
            this.btnUninit.Size = new System.Drawing.Size(84, 24);
            this.btnUninit.TabIndex = 1;
            this.btnUninit.Text = "Desconectar";
            this.btnUninit.UseVisualStyleBackColor = true;
            this.btnUninit.Click += new System.EventHandler(this.btnUninit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbScanTemplateType);
            this.groupBox1.Controls.Add(this.btnSelectionUpdateTemplate);
            this.groupBox1.Controls.Add(this.btnSelectionVerify);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnSelectionUpdateUserInfo);
            this.groupBox1.Controls.Add(this.btnSelectionDelete);
            this.groupBox1.Controls.Add(this.btnDeleteAll);
            this.groupBox1.Controls.Add(this.btnUninit);
            this.groupBox1.Controls.Add(this.btnInit);
            this.groupBox1.Location = new System.Drawing.Point(701, 415);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(75, 19);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Seleccion";
            this.groupBox1.Visible = false;
            // 
            // cbScanTemplateType
            // 
            this.cbScanTemplateType.BackColor = System.Drawing.Color.White;
            this.cbScanTemplateType.FormattingEnabled = true;
            this.cbScanTemplateType.Items.AddRange(new object[] {
            "suprema",
            "iso19479_2",
            "ansi378"});
            this.cbScanTemplateType.Location = new System.Drawing.Point(199, 23);
            this.cbScanTemplateType.Name = "cbScanTemplateType";
            this.cbScanTemplateType.Size = new System.Drawing.Size(124, 21);
            this.cbScanTemplateType.TabIndex = 11;
            this.cbScanTemplateType.SelectedIndexChanged += new System.EventHandler(this.bScanTemplateType_SelectedIndexChanged);
            // 
            // btnSelectionUpdateTemplate
            // 
            this.btnSelectionUpdateTemplate.AccessibleDescription = "";
            this.btnSelectionUpdateTemplate.Location = new System.Drawing.Point(6, 137);
            this.btnSelectionUpdateTemplate.Name = "btnSelectionUpdateTemplate";
            this.btnSelectionUpdateTemplate.Size = new System.Drawing.Size(68, 40);
            this.btnSelectionUpdateTemplate.TabIndex = 7;
            this.btnSelectionUpdateTemplate.Text = "Actualizar Plantilla";
            this.btnSelectionUpdateTemplate.UseVisualStyleBackColor = true;
            this.btnSelectionUpdateTemplate.Click += new System.EventHandler(this.btnSelectionUpdateTemplate_Click);
            // 
            // btnSelectionVerify
            // 
            this.btnSelectionVerify.AccessibleDescription = "";
            this.btnSelectionVerify.Location = new System.Drawing.Point(8, 94);
            this.btnSelectionVerify.Name = "btnSelectionVerify";
            this.btnSelectionVerify.Size = new System.Drawing.Size(68, 24);
            this.btnSelectionVerify.TabIndex = 8;
            this.btnSelectionVerify.Text = "Verificar";
            this.btnSelectionVerify.UseVisualStyleBackColor = true;
            this.btnSelectionVerify.Click += new System.EventHandler(this.btnSelectionVerify_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(95, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Tipo de plantilla";
            // 
            // btnSelectionUpdateUserInfo
            // 
            this.btnSelectionUpdateUserInfo.AccessibleDescription = "";
            this.btnSelectionUpdateUserInfo.Location = new System.Drawing.Point(8, 48);
            this.btnSelectionUpdateUserInfo.Name = "btnSelectionUpdateUserInfo";
            this.btnSelectionUpdateUserInfo.Size = new System.Drawing.Size(68, 40);
            this.btnSelectionUpdateUserInfo.TabIndex = 6;
            this.btnSelectionUpdateUserInfo.Text = "Actualizar Usuario";
            this.btnSelectionUpdateUserInfo.UseVisualStyleBackColor = true;
            this.btnSelectionUpdateUserInfo.Click += new System.EventHandler(this.btnSelectionUpdateUserInfo_Click);
            // 
            // btnSelectionDelete
            // 
            this.btnSelectionDelete.AccessibleDescription = "";
            this.btnSelectionDelete.Location = new System.Drawing.Point(8, 20);
            this.btnSelectionDelete.Name = "btnSelectionDelete";
            this.btnSelectionDelete.Size = new System.Drawing.Size(68, 24);
            this.btnSelectionDelete.TabIndex = 5;
            this.btnSelectionDelete.Text = "Eliminar";
            this.btnSelectionDelete.UseVisualStyleBackColor = true;
            this.btnSelectionDelete.Click += new System.EventHandler(this.btnSelectionDelete_Click);
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.AccessibleDescription = "";
            this.btnDeleteAll.Location = new System.Drawing.Point(98, 56);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(84, 24);
            this.btnDeleteAll.TabIndex = 5;
            this.btnDeleteAll.Text = "Eliminar Todo";
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // lvDatabaseList
            // 
            lvDatabaseList.Activation = System.Windows.Forms.ItemActivation.OneClick;
            lvDatabaseList.FullRowSelect = true;
            lvDatabaseList.GridLines = true;
            lvDatabaseList.HideSelection = false;
            lvDatabaseList.Location = new System.Drawing.Point(257, 42);
            lvDatabaseList.MultiSelect = false;
            lvDatabaseList.Name = "lvDatabaseList";
            lvDatabaseList.Size = new System.Drawing.Size(420, 252);
            lvDatabaseList.TabIndex = 6;
            lvDatabaseList.UseCompatibleStateImageBehavior = false;
            lvDatabaseList.View = System.Windows.Forms.View.Details;
            // 
            // tbxMessage
            // 
            tbxMessage.Location = new System.Drawing.Point(12, 350);
            tbxMessage.Multiline = true;
            tbxMessage.Name = "tbxMessage";
            tbxMessage.Size = new System.Drawing.Size(609, 84);
            tbxMessage.TabIndex = 7;
            // 
            // btnClear
            // 
            this.btnClear.AccessibleDescription = "";
            this.btnClear.Location = new System.Drawing.Point(629, 350);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 84);
            this.btnClear.TabIndex = 8;
            this.btnClear.Text = "Limpiar";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // pbImageFrame
            // 
            pbImageFrame.Location = new System.Drawing.Point(6, 19);
            pbImageFrame.Name = "pbImageFrame";
            pbImageFrame.Size = new System.Drawing.Size(207, 227);
            pbImageFrame.TabIndex = 9;
            pbImageFrame.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(400, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Lista de Colaboradores";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(pbImageFrame);
            this.groupBox2.Location = new System.Drawing.Point(12, 42);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(219, 252);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Visualización de Huella";
            // 
            // txtCoordenada
            // 
            txtCoordenada.Location = new System.Drawing.Point(18, 306);
            txtCoordenada.Name = "txtCoordenada";
            txtCoordenada.ReadOnly = true;
            txtCoordenada.Size = new System.Drawing.Size(207, 20);
            txtCoordenada.TabIndex = 13;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(706, 24);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.marcarHuellaToolStripMenuItem,
            this.huelleroEikonToolStripMenuItem,
            this.agregarColaboradorToolStripMenuItem,
            this.sincronizarToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // marcarHuellaToolStripMenuItem
            // 
            this.marcarHuellaToolStripMenuItem.Name = "marcarHuellaToolStripMenuItem";
            this.marcarHuellaToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.marcarHuellaToolStripMenuItem.Text = "Huellero Suprema";
            this.marcarHuellaToolStripMenuItem.Click += new System.EventHandler(this.marcarHuellaToolStripMenuItem_Click);
            // 
            // agregarColaboradorToolStripMenuItem
            // 
            this.agregarColaboradorToolStripMenuItem.Name = "agregarColaboradorToolStripMenuItem";
            this.agregarColaboradorToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.agregarColaboradorToolStripMenuItem.Text = "Agregar Colaborador";
            this.agregarColaboradorToolStripMenuItem.Click += new System.EventHandler(this.agregarColaboradorToolStripMenuItem_Click);
            // 
            // sincronizarToolStripMenuItem
            // 
            this.sincronizarToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.asistenciaToolStripMenuItem,
            this.huellasToolStripMenuItem});
            this.sincronizarToolStripMenuItem.Name = "sincronizarToolStripMenuItem";
            this.sincronizarToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.sincronizarToolStripMenuItem.Text = "Sincronizar";
            // 
            // asistenciaToolStripMenuItem
            // 
            this.asistenciaToolStripMenuItem.Name = "asistenciaToolStripMenuItem";
            this.asistenciaToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.asistenciaToolStripMenuItem.Text = "Asistencia";
            this.asistenciaToolStripMenuItem.Click += new System.EventHandler(this.asistenciaToolStripMenuItem_Click);
            // 
            // huellasToolStripMenuItem
            // 
            this.huellasToolStripMenuItem.Name = "huellasToolStripMenuItem";
            this.huellasToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.huellasToolStripMenuItem.Text = "Huellas";
            this.huellasToolStripMenuItem.Click += new System.EventHandler(this.huellasToolStripMenuItem_Click);
            // 
            // huelleroEikonToolStripMenuItem
            // 
            this.huelleroEikonToolStripMenuItem.Name = "huelleroEikonToolStripMenuItem";
            this.huelleroEikonToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.huelleroEikonToolStripMenuItem.Text = "Huellero Eikon";
            this.huelleroEikonToolStripMenuItem.Click += new System.EventHandler(this.huelleroEikonToolStripMenuItem_Click);
            // 
            // Huella
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(706, 338);
            this.Controls.Add(txtCoordenada);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(tbxMessage);
            this.Controls.Add(lvDatabaseList);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Huella";
            this.Text = "Sistema de Marcación Biométrica";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Huella_FormClosing);
            this.Load += new System.EventHandler(this.Huella_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(pbImageFrame)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void AgregarColaborador()
        {
            InicializarBD();
            form = 2;
            new RegistroEmpleado().Show();
        }

        private void SincronizarAsistencia()
        {

            var registrosSinActualizar = RegistroEmpleado.ObtenerResgistrosSinEnviar().Count();

            if (registrosSinActualizar == 0)
            {
                MessageBox.Show(Mensajes.MarcacionesDeRegistrosActualizadas);
                return;
            }
            else
            {
                var confirmResult = MessageBox.Show(Mensajes.ConteoRegistrosSinActualizar(registrosSinActualizar),
                             Mensajes.ConfirmandoActualizacion,
                             MessageBoxButtons.OK);
                if (confirmResult == DialogResult.OK)
                {
                    try
                    {
                        RegistroEmpleado.ProcesarDatosNoEnviadosAux();
                        MessageBox.Show(Mensajes.RegistrosMarcacionesProcesadas);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }

        }


        private List<Empleado> ObtenerEmpleadosActivos()
        {
            using (var conection = Utilidad<Empleado>.ConnSqlite(BdSqlite))
            {
                var query = "Select CodEmpleado,Gui_Huella From Fingerprints where Estado=1";
                SQLiteCommand command = new SQLiteCommand(query, conection);
                var reader = command.ExecuteReader();
                var Empleado = new List<Empleado>();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Empleado.Add(new Empleado() { codigo = reader[0].ToString(), guiHuella = reader[1].ToString() });
                    }
                }

                return Empleado;

            }
        }

        private void SincronizarHuellas()
        {

            try
            {
                var empleadosActivos = ObtenerEmpleadosActivos();

                if (empleadosActivos.Count() > 0)
                {
                    int count = 0;
                    foreach (Empleado empleado in empleadosActivos)
                    {
                        var Emp = Utilidad<Empleado>.GetJson(new Empleado(), Api + Constante.ConsultarApi + empleado.codigo + "&clave=" + ApiKey);
                        if (Emp.guiHuella != empleado.guiHuella && Emp.huella != null && Emp.huella.Length > 0)
                        {
                            string dummyData = Emp.huella.Trim().Replace(" ", "+");
                            if (dummyData.Length % 4 > 0)
                                dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                            byte[] huellaByte = Convert.FromBase64String(dummyData);

                            Emp.huellaByte = huellaByte;

                            ActualizarEmpleadoLocal(Emp, false);
                            count++;
                        }
                    }
                    if (count > 0)
                    {
                        MessageBox.Show(Mensajes.ConteoActualizacionHuellas(count));
                    }
                    else
                    {
                        MessageBox.Show(Mensajes.SinRegistrosDeHuellasPorActualizar);
                    }
                }
                else
                {
                    MessageBox.Show(Mensajes.SinRegistrosDeHuellas);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void marcarHuellaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MarcarHuella();
        }

        private void asistenciaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SincronizarAsistencia();
        }

        private void huellasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SincronizarHuellas();
        }

        private void agregarColaboradorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AgregarColaborador();
        }

        private void huelleroEikonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form_Main(1).Show();
        }
    }

}