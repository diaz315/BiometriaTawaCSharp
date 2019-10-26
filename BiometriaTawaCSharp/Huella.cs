using BiometriaTawaCSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]

namespace Suprema
{
    public class Huella : Form
    {
        private UFScannerManager m_ScannerManager;
        private static UFScanner m_Scanner;
        private UFDatabase m_Database;
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
        private Button btnIdentify;
        private GroupBox groupBox1;
        private Button btnSelectionDelete;
        private Button btnSelectionVerify;
        private Button btnSelectionUpdateUserInfo;
        private Button btnDeleteAll;
        private ListView lvDatabaseList;
        public static TextBox tbxMessage;
        private Button btnClear;
        public static PictureBox pbImageFrame;
        private Label label1;
        private Button button1;
        private ComboBox cbScanTemplateType;
        private Button btnSelectionUpdateTemplate;
        public static int form;
        public static int HuellaTomada=0;
        public static int BdIniciada=0;
        public Huella()
        {
            this.InitializeComponent();
        }
        private void Huella_Load(object sender, EventArgs e)
        {
            m_ScannerManager = new UFScannerManager(this);
            m_Scanner = null;
            this.m_Database = null;
            this.m_Matcher = null;
            m_Template1 = new byte[1024];
            this.m_Template2 = new byte[1024];
            this.lvDatabaseList.Columns.Add("Serial", 50, HorizontalAlignment.Left);
            this.lvDatabaseList.Columns.Add("UserID", 60, HorizontalAlignment.Left);
            this.lvDatabaseList.Columns.Add("FingerIndex", 80, HorizontalAlignment.Left);
            this.lvDatabaseList.Columns.Add("Template1", 80, HorizontalAlignment.Left);
            this.lvDatabaseList.Columns.Add("Template2", 80, HorizontalAlignment.Left);
            this.lvDatabaseList.Columns.Add("Memo", 60, HorizontalAlignment.Left);
        }
        private void Huella_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnUninit_Click(sender, e);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbxMessage.Clear();
        }
        private void AddRow(int Serial, string UserID, int FingerIndex, bool bTemplate1, bool bTemplate2, string Memo)
        {
            ListViewItem listViewItem = this.lvDatabaseList.Items.Add(Convert.ToString(Serial));
            listViewItem.SubItems.Add(UserID);
            listViewItem.SubItems.Add(Convert.ToString(FingerIndex));
            listViewItem.SubItems.Add(bTemplate1 ? "O" : "X");
            listViewItem.SubItems.Add(bTemplate2 ? "O" : "X");
            listViewItem.SubItems.Add(Memo);
        }

        private void UpdateDatabaseList()
        {
            if (this.m_Database == null)
            {
                return;
            }
            int num;
            UFD_STATUS uFD_STATUS = this.m_Database.GetDataNumber(out num);
            if (uFD_STATUS == UFD_STATUS.OK)
            {
                tbxMessage.AppendText("UFDatabase GetDataNumber: " + num + "\r\n");
                this.lvDatabaseList.Items.Clear();
                for (int i = 0; i < num; i++)
                {
                    uFD_STATUS = this.m_Database.GetDataByIndex(i, out this.m_Serial, out this.m_UserID, out this.m_FingerIndex, m_Template1, out m_Template1Size, this.m_Template2, out this.m_Template2Size, out this.m_Memo);
                    if (uFD_STATUS != UFD_STATUS.OK)
                    {
                        UFDatabase.GetErrorString(uFD_STATUS, out m_strError);
                        tbxMessage.AppendText("UFDatabase GetDataByIndex: " + m_strError + "\r\n");
                        return;
                    }
                    this.AddRow(this.m_Serial, this.m_UserID, this.m_FingerIndex, m_Template1Size != 0, this.m_Template2Size != 0, this.m_Memo);
                }
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out m_strError);
            tbxMessage.AppendText("UFDatabase GetDataNumber: " + m_strError + "\r\n");
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
            else {
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

        }
        private void btnInit_Click(object sender, EventArgs e)
        {
            InicializarBD();
            btnUninit.Visible = true;
            btnInit.Visible = false;
        }

        private void InicializarBD() {
            if (BdIniciada==0) {
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
                this.m_Database = new UFDatabase();
                string fileName = "C://Users//JD//Desktop//NagaSkaki_512//UFDatabase.mdb";

                string connection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";";
                UFD_STATUS uFD_STATUS = this.m_Database.Open(connection, null, null);
                if (uFD_STATUS == UFD_STATUS.OK)
                {
                    tbxMessage.AppendText("UFDatabase Open: OK\r\n");
                    this.UpdateDatabaseList();
                    this.m_Matcher = new UFMatcher();
                    if (this.m_Matcher.InitResult == UFM_STATUS.OK)
                    {
                        tbxMessage.AppendText("UFMatcher Init: OK\r\n");
                    }
                    else
                    {
                        UFMatcher.GetErrorString(m_Matcher.InitResult, out m_strError);
                        tbxMessage.AppendText("UFMatcher Init: " + m_strError + "\r\n");
                    }
                    this.cbScanTemplateType.SelectedIndex = 0;
                    return;
                }
                UFDatabase.GetErrorString(uFD_STATUS, out m_strError);
                tbxMessage.AppendText("UFDatabase Open: " + m_strError + "\r\n");
                BdIniciada = 1;
            }
            
        }

        private void btnUninit_Click(object sender, EventArgs e)
        {
            btnUninit.Visible = false;
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
            if (this.m_Database != null)
            {
                UFD_STATUS uFD_STATUS = this.m_Database.Close();
                if (uFD_STATUS == UFD_STATUS.OK)
                {
                    tbxMessage.AppendText("UFDatabase Close: OK\r\n");
                }
                else
                {
                    UFDatabase.GetErrorString(uFD_STATUS, out m_strError);
                    tbxMessage.AppendText("UFDatabase Close: " + m_strError + "\r\n");
                }
            }
            this.lvDatabaseList.Items.Clear();
            btnInit.Visible = true;
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
            return false;
        IL_A4:
            tbxMessage.AppendText("UFScanner Extracto: OK\r\n");
            return true;
        }

        public static Empleado IniciarEscaneo() {
            if (!ExtractTemplate(m_Template1, out m_Template1Size))
            {
                return null;
            }
            else {
                DrawCapturedImage(m_Scanner);
                var res = new Empleado();
                res.huellaByte = m_Template1;
                res.pesoHuella = m_Template1Size;

                return res;
            }

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
            UFD_STATUS uFD_STATUS = this.m_Database.AddData(userInfoForm.UserID, userInfoForm.FingerIndex, m_Template1, m_Template1Size, null, 0, userInfoForm.Memo);
            if (uFD_STATUS != UFD_STATUS.OK)
            {
                UFDatabase.GetErrorString(uFD_STATUS, out m_strError);
                tbxMessage.AppendText("UFDatabase AgregarData: " + m_strError + "\r\n");
            }
            else
            {
                this.cbScanTemplateType.Enabled = false;
            }
            this.UpdateDatabaseList();
        }
        private void btnIdentify_Click(object sender, EventArgs e)
        {
            InicializarBD();
            form = 1;
            byte[] array = new byte[1024];
            byte[][] template2Array = null;
            int[] template2SizeArray = null;
            int[] array2 = null;
            int template2Num;
            UFD_STATUS templateListWithSerial = this.m_Database.GetTemplateListWithSerial(out template2Array, out template2SizeArray, out template2Num, out array2);
            if (templateListWithSerial != UFD_STATUS.OK)
            {
                UFDatabase.GetErrorString(templateListWithSerial, out m_strError);
                tbxMessage.AppendText("UFD_GetTemplateListWithSerial: " + m_strError + "\r\n");
                return;
            }
            int template1Size;
            if (!ExtractTemplate(array, out template1Size))
            {
                return;
            }
            DrawCapturedImage(m_Scanner);
            Cursor.Current = Cursors.WaitCursor;
            int num;
            UFM_STATUS uFM_STATUS = this.m_Matcher.Identify(array, template1Size, template2Array, template2SizeArray, template2Num, 5000, out num);
            Cursor.Current = this.Cursor;
            if (uFM_STATUS != UFM_STATUS.OK)
            {
                UFMatcher.GetErrorString(uFM_STATUS, out m_strError);
                tbxMessage.AppendText("UFMatcher Identify: " + m_strError + "\r\n");
                return;
            }
            if (num != -1)
            {
                var Empleado= ObtenerEmpleado(array2[num]);
                //tbxMessage.AppendText("Identificación exitosa (Empleado = " + Empleado[0]+" "+ Empleado[1]+ ")\r\n");
                MessageBox.Show(Empleado[0]+" " +Empleado[1], "Identificación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            tbxMessage.AppendText("Identificación fallida\r\n");
        }

        private List<string> ObtenerEmpleado(int id) {
            using (var conection = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;" + "data source=C://Users//JD//Desktop//NagaSkaki_512//UFDatabase.mdb"))
            {
                conection.Open();
                var query = "Select Nombres,CodEmpleado From Fingerprints where Serial="+id;
                var command = new OleDbCommand(query, conection);
                var reader = command.ExecuteReader();
                var Empleado = new List<string>();
                while (reader.Read()) {
                    Empleado.Add(reader[0].ToString());
                    Empleado.Add(reader[1].ToString());
                }
                return Empleado;

            }
        }

        public static void RegistrarEmpleado(Empleado obj) {
            using (var conection = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;" + "data source=C://Users//JD//Desktop//NagaSkaki_512//UFDatabase.mdb"))
            {
                OleDbCommand comm = new OleDbCommand("INSERT INTO Fingerprints(Nombres,Documento,FingerIndex,Template1,NroDocumento,CodEmpleado) VALUES (?,?,?,?,?,?)", conection);
                OleDbParameter parImagen = new OleDbParameter("@imagen", OleDbType.VarBinary, obj.huellaByte.Length);
                parImagen.Value = obj.huellaByte;
                comm.Parameters.Add("@Nombres", OleDbType.VarChar).Value = obj.nombres.Trim();
                comm.Parameters.Add("@TipoDoc", OleDbType.VarChar).Value = obj.tipoDoc.Trim();
                comm.Parameters.Add("@FingerIndex", OleDbType.Integer).Value = obj.dedo;
                comm.Parameters.Add(parImagen);
                comm.Parameters.Add("@NroDocumento", OleDbType.VarChar).Value = obj.nroDoc;
                comm.Parameters.Add("@CodEmpleado", OleDbType.VarChar).Value = obj.codigo;
                conection.Open();
                int iResultado = comm.ExecuteNonQuery();
                conection.Close();
                HuellaTomada = 0;
                MessageBox.Show("Guardo con exito");
            }
        }
        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            UFD_STATUS uFD_STATUS = this.m_Database.RemoveAllData();
            if (uFD_STATUS == UFD_STATUS.OK)
            {
                tbxMessage.AppendText("UFDatabase RemoveAllData: OK\r\n");
                this.UpdateDatabaseList();
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out m_strError);
            tbxMessage.AppendText("UFDatabase RemoveAllData: " + m_strError + "\r\n");
        }
        private void btnSelectionDelete_Click(object sender, EventArgs e)
        {
            if (this.lvDatabaseList.SelectedIndices.Count == 0)
            {
                tbxMessage.AppendText("Seleccione el registro\r\n");
                return;
            }
            int serial = Convert.ToInt32(this.lvDatabaseList.SelectedItems[0].SubItems[0].Text);
            UFD_STATUS uFD_STATUS = this.m_Database.RemoveDataBySerial(serial);
            if (uFD_STATUS == UFD_STATUS.OK)
            {
                tbxMessage.AppendText("UFDatabase RemoveDataBySerial: OK\r\n");
                this.UpdateDatabaseList();
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out m_strError);
            tbxMessage.AppendText("UFDatabase RemoveDataBySerial: " + m_strError + "\r\n");
        }
        private void btnSelectionUpdateUserInfo_Click(object sender, EventArgs e)
        {
            UserInfoForms userInfoForm = new UserInfoForms(true);
            if (this.lvDatabaseList.SelectedIndices.Count == 0)
            {
                tbxMessage.AppendText("Seleccione el registro\r\n");
                return;
            }
            int serial = Convert.ToInt32(this.lvDatabaseList.SelectedItems[0].SubItems[0].Text);
            userInfoForm.UserID = this.lvDatabaseList.SelectedItems[0].SubItems[1].Text;
            userInfoForm.FingerIndex = Convert.ToInt32(this.lvDatabaseList.SelectedItems[0].SubItems[2].Text);
            userInfoForm.Memo = this.lvDatabaseList.SelectedItems[0].SubItems[5].Text;
            tbxMessage.AppendText("Datos de usuario actualizado\r\n");
            tbxMessage.AppendText("Id usuario y dedo indice no seran actualizados\r\n");
            if (userInfoForm.ShowDialog(this) != DialogResult.OK)
            {
                tbxMessage.AppendText("El ingreso de datos ha sido cancelado por el usuario\r\n");
                return;
            }
            UFD_STATUS uFD_STATUS = this.m_Database.UpdateDataBySerial(serial, null, 0, null, 0, userInfoForm.Memo);
            if (uFD_STATUS == UFD_STATUS.OK)
            {
                tbxMessage.AppendText("UFD_UpdateDataBySerial: OK\r\n");
                this.UpdateDatabaseList();
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out m_strError);
            tbxMessage.AppendText("UFDatabase UpdateDataBySerial: " + m_strError + "\r\n");
        }
        private void btnSelectionUpdateTemplate_Click(object sender, EventArgs e)
        {
            if (this.lvDatabaseList.SelectedIndices.Count == 0)
            {
                tbxMessage.AppendText("Seleccione el registro\r\n");
                return;
            }
            int serial = Convert.ToInt32(this.lvDatabaseList.SelectedItems[0].SubItems[0].Text);
            if (!ExtractTemplate(m_Template1, out m_Template1Size))
            {
                return;
            }
            DrawCapturedImage(m_Scanner);
            UFD_STATUS uFD_STATUS = this.m_Database.UpdateDataBySerial(serial, m_Template1, m_Template1Size, null, 0, null);
            if (uFD_STATUS == UFD_STATUS.OK)
            {
                tbxMessage.AppendText("UFD_UpdateDataBySerial: OK\r\n");
                this.UpdateDatabaseList();
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out m_strError);
            tbxMessage.AppendText("UFDatabase UpdateDataBySerial: " + m_strError + "\r\n");
        }
        private void btnSelectionVerify_Click(object sender, EventArgs e)
        {
            byte[] array = new byte[1024];
            if (this.lvDatabaseList.SelectedIndices.Count == 0)
            {
                tbxMessage.AppendText("Seleccione el registro\r\n");
                return;
            }
            int num = Convert.ToInt32(this.lvDatabaseList.SelectedItems[0].SubItems[0].Text);
            UFD_STATUS dataBySerial = this.m_Database.GetDataBySerial(num, out this.m_UserID, out this.m_FingerIndex, m_Template1, out m_Template1Size, this.m_Template2, out this.m_Template2Size, out this.m_Memo);
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
            this.btnIdentify = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbScanTemplateType = new System.Windows.Forms.ComboBox();
            this.btnSelectionUpdateTemplate = new System.Windows.Forms.Button();
            this.btnSelectionVerify = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelectionUpdateUserInfo = new System.Windows.Forms.Button();
            this.btnSelectionDelete = new System.Windows.Forms.Button();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.lvDatabaseList = new System.Windows.Forms.ListView();
            this.tbxMessage = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.pbImageFrame = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImageFrame)).BeginInit();
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
            // btnIdentify
            // 
            this.btnIdentify.AccessibleDescription = "";
            this.btnIdentify.Location = new System.Drawing.Point(11, 41);
            this.btnIdentify.Name = "btnIdentify";
            this.btnIdentify.Size = new System.Drawing.Size(84, 24);
            this.btnIdentify.TabIndex = 3;
            this.btnIdentify.Text = "Identificar";
            this.btnIdentify.UseVisualStyleBackColor = true;
            this.btnIdentify.Click += new System.EventHandler(this.btnIdentify_Click);
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
            this.groupBox1.Location = new System.Drawing.Point(11, 385);
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
            this.lvDatabaseList.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lvDatabaseList.FullRowSelect = true;
            this.lvDatabaseList.GridLines = true;
            this.lvDatabaseList.HideSelection = false;
            this.lvDatabaseList.Location = new System.Drawing.Point(347, 12);
            this.lvDatabaseList.MultiSelect = false;
            this.lvDatabaseList.Name = "lvDatabaseList";
            this.lvDatabaseList.Size = new System.Drawing.Size(420, 296);
            this.lvDatabaseList.TabIndex = 6;
            this.lvDatabaseList.UseCompatibleStateImageBehavior = false;
            this.lvDatabaseList.View = System.Windows.Forms.View.Details;
            // 
            // tbxMessage
            // 
            this.tbxMessage.Location = new System.Drawing.Point(102, 320);
            this.tbxMessage.Multiline = true;
            this.tbxMessage.Name = "tbxMessage";
            this.tbxMessage.Size = new System.Drawing.Size(609, 84);
            this.tbxMessage.TabIndex = 7;
            // 
            // btnClear
            // 
            this.btnClear.AccessibleDescription = "";
            this.btnClear.Location = new System.Drawing.Point(719, 320);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(48, 84);
            this.btnClear.TabIndex = 8;
            this.btnClear.Text = "Limpiar";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // pbImageFrame
            // 
            this.pbImageFrame.Location = new System.Drawing.Point(102, 12);
            this.pbImageFrame.Name = "pbImageFrame";
            this.pbImageFrame.Size = new System.Drawing.Size(228, 252);
            this.pbImageFrame.TabIndex = 9;
            this.pbImageFrame.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Registrar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Huella
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(793, 428);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pbImageFrame);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.tbxMessage);
            this.Controls.Add(this.lvDatabaseList);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnIdentify);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Huella";
            this.Text = "Biometrico Tawa";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Huella_FormClosing);
            this.Load += new System.EventHandler(this.Huella_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImageFrame)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            InicializarBD();
            form = 2;
            new RegistroEmpleado().Show();
            //MessageBox.Show(CLocation.GetLocationProperty(), "Identificación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
 
}
