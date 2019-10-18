using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]

namespace Suprema
{
    public class UFE30_DatabaseDemo : Form
    {
        private const int MAX_USERID_SIZE = 50;
        private const int MAX_TEMPLATE_SIZE = 1024;
        private const int MAX_MEMO_SIZE = 100;
        private const int DATABASE_COL_SERIAL = 0;
        private const int DATABASE_COL_USERID = 1;
        private const int DATABASE_COL_FINGERINDEX = 2;
        private const int DATABASE_COL_TEMPLATE1 = 3;
        private const int DATABASE_COL_TEMPLATE2 = 4;
        private const int DATABASE_COL_MEMO = 5;
        private UFScannerManager m_ScannerManager;
        private UFScanner m_Scanner;
        private UFDatabase m_Database;
        private UFMatcher m_Matcher;
        private string m_strError;
        private int m_Serial;
        private string m_UserID;
        private int m_FingerIndex;
        private byte[] m_Template1;
        private int m_Template1Size;
        private byte[] m_Template2;
        private int m_Template2Size;
        private string m_Memo;
        private System.ComponentModel.IContainer components;
        private Button btnInit;
        private Button btnUninit;
        private Button btnEnroll;
        private Button btnIdentify;
        private GroupBox groupBox1;
        private Button btnSelectionDelete;
        private Button btnSelectionVerify;
        private Button btnSelectionUpdateTemplate;
        private Button btnSelectionUpdateUserInfo;
        private Button btnDeleteAll;
        private ListView lvDatabaseList;
        private TextBox tbxMessage;
        private Button btnClear;
        private PictureBox pbImageFrame;
        private Label label1;
        private ComboBox cbScanTemplateType;
        public UFE30_DatabaseDemo()
        {
            this.InitializeComponent();
        }
        private void UFE30_DatabaseDemo_Load(object sender, EventArgs e)
        {
            this.m_ScannerManager = new UFScannerManager(this);
            this.m_Scanner = null;
            this.m_Database = null;
            this.m_Matcher = null;
            this.m_Template1 = new byte[1024];
            this.m_Template2 = new byte[1024];
            this.lvDatabaseList.Columns.Add("Serial", 50, HorizontalAlignment.Left);
            this.lvDatabaseList.Columns.Add("UserID", 60, HorizontalAlignment.Left);
            this.lvDatabaseList.Columns.Add("FingerIndex", 80, HorizontalAlignment.Left);
            this.lvDatabaseList.Columns.Add("Template1", 80, HorizontalAlignment.Left);
            this.lvDatabaseList.Columns.Add("Template2", 80, HorizontalAlignment.Left);
            this.lvDatabaseList.Columns.Add("Memo", 60, HorizontalAlignment.Left);
        }
        private void UFE30_DatabaseDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.btnUninit_Click(sender, e);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.tbxMessage.Clear();
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
                this.tbxMessage.AppendText("UFDatabase GetDataNumber: " + num + "\r\n");
                this.lvDatabaseList.Items.Clear();
                for (int i = 0; i < num; i++)
                {
                    uFD_STATUS = this.m_Database.GetDataByIndex(i, out this.m_Serial, out this.m_UserID, out this.m_FingerIndex, this.m_Template1, out this.m_Template1Size, this.m_Template2, out this.m_Template2Size, out this.m_Memo);
                    if (uFD_STATUS != UFD_STATUS.OK)
                    {
                        UFDatabase.GetErrorString(uFD_STATUS, out this.m_strError);
                        this.tbxMessage.AppendText("UFDatabase GetDataByIndex: " + this.m_strError + "\r\n");
                        return;
                    }
                    this.AddRow(this.m_Serial, this.m_UserID, this.m_FingerIndex, this.m_Template1Size != 0, this.m_Template2Size != 0, this.m_Memo);
                }
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out this.m_strError);
            this.tbxMessage.AppendText("UFDatabase GetDataNumber: " + this.m_strError + "\r\n");
        }
        private void DrawCapturedImage(UFScanner Scanner)
        {
            Graphics graphics = this.pbImageFrame.CreateGraphics();
            Rectangle rect = new Rectangle(0, 0, this.pbImageFrame.Width, this.pbImageFrame.Height);
            try
            {
                Scanner.DrawCaptureImageBuffer(graphics, rect, false);
            }
            finally
            {
                graphics.Dispose();
            }
        }
        private void btnInit_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            UFS_STATUS uFS_STATUS = this.m_ScannerManager.Init();
            Cursor.Current = this.Cursor;
            if (uFS_STATUS != UFS_STATUS.OK)
            {
                UFScanner.GetErrorString(uFS_STATUS, out this.m_strError);
                this.tbxMessage.AppendText("UFScanner Init: " + this.m_strError + "\r\n");
                return;
            }
            this.tbxMessage.AppendText("UFScanner Init: OK\r\n");
            int count = this.m_ScannerManager.Scanners.Count;
            this.tbxMessage.AppendText("UFScanner GetScannerNumber: " + count + "\r\n");
            if (count == 0)
            {
                this.tbxMessage.AppendText("There's no available scanner\r\n");
                return;
            }
            this.tbxMessage.AppendText("First scanner will be used\r\n");
            this.m_Scanner = this.m_ScannerManager.Scanners[0];
            this.m_Database = new UFDatabase();
            this.tbxMessage.AppendText("Select a database file\r\n");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = "UFDatabase.mdb";
            openFileDialog.Filter = "Database Files (*.mdb)|*.mdb";
            openFileDialog.DefaultExt = "mdb";
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                return;
            }
            string fileName = openFileDialog.FileName;
            string connection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";";
            UFD_STATUS uFD_STATUS = this.m_Database.Open(connection, null, null);
            if (uFD_STATUS == UFD_STATUS.OK)
            {
                this.tbxMessage.AppendText("UFDatabase Open: OK\r\n");
                this.UpdateDatabaseList();
                this.m_Matcher = new UFMatcher();
                if (this.m_Matcher.InitResult == UFM_STATUS.OK)
                {
                    this.tbxMessage.AppendText("UFMatcher Init: OK\r\n");
                }
                else
                {
                    UFMatcher.GetErrorString(this.m_Matcher.InitResult, out this.m_strError);
                    this.tbxMessage.AppendText("UFMatcher Init: " + this.m_strError + "\r\n");
                }
                this.cbScanTemplateType.SelectedIndex = 0;
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out this.m_strError);
            this.tbxMessage.AppendText("UFDatabase Open: " + this.m_strError + "\r\n");
        }
        private void btnUninit_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            UFS_STATUS uFS_STATUS = this.m_ScannerManager.Uninit();
            Cursor.Current = this.Cursor;
            if (uFS_STATUS == UFS_STATUS.OK)
            {
                this.tbxMessage.AppendText("UFScanner Uninit: OK\r\n");
            }
            else
            {
                UFScanner.GetErrorString(uFS_STATUS, out this.m_strError);
                this.tbxMessage.AppendText("UFScanner Uninit: " + this.m_strError + "\r\n");
            }
            if (this.m_Database != null)
            {
                UFD_STATUS uFD_STATUS = this.m_Database.Close();
                if (uFD_STATUS == UFD_STATUS.OK)
                {
                    this.tbxMessage.AppendText("UFDatabase Close: OK\r\n");
                }
                else
                {
                    UFDatabase.GetErrorString(uFD_STATUS, out this.m_strError);
                    this.tbxMessage.AppendText("UFDatabase Close: " + this.m_strError + "\r\n");
                }
            }
            this.lvDatabaseList.Items.Clear();
        }
        private bool ExtractTemplate(byte[] Template, out int TemplateSize)
        {
            this.m_Scanner.ClearCaptureImageBuffer();
            this.tbxMessage.AppendText("Colocar dedo\r\n");
            TemplateSize = 0;
            UFS_STATUS uFS_STATUS;
            while (true)
            {
                uFS_STATUS = this.m_Scanner.CaptureSingleImage();
                if (uFS_STATUS != UFS_STATUS.OK)
                {
                    break;
                }
                int num;
                uFS_STATUS = this.m_Scanner.ExtractEx(1024, Template, out TemplateSize, out num);
                if (uFS_STATUS == UFS_STATUS.OK)
                {
                    goto IL_A4;
                }
                UFScanner.GetErrorString(uFS_STATUS, out this.m_strError);
                this.tbxMessage.AppendText("UFScanner Extracto: " + this.m_strError + "\r\n");
            }
            UFScanner.GetErrorString(uFS_STATUS, out this.m_strError);
            this.tbxMessage.AppendText("UFScanner CaptureUnicaImage: " + this.m_strError + "\r\n");
            return false;
        IL_A4:
            this.tbxMessage.AppendText("UFScanner Extracto: OK\r\n");
            return true;
        }
        private void btnEnroll_Click(object sender, EventArgs e)
        {
            if (!this.ExtractTemplate(this.m_Template1, out this.m_Template1Size))
            {
                return;
            }
            this.DrawCapturedImage(this.m_Scanner);
            UserInfoForms userInfoForm = new UserInfoForms(false);
            this.tbxMessage.AppendText("Ingrese data de usuario\r\n");
            if (userInfoForm.ShowDialog(this) != DialogResult.OK)
            {
                this.tbxMessage.AppendText("El ingreso de data ha sido cancelada por el usuario\r\n");
                return;
            }
            UFD_STATUS uFD_STATUS = this.m_Database.AddData(userInfoForm.UserID, userInfoForm.FingerIndex, this.m_Template1, this.m_Template1Size, null, 0, userInfoForm.Memo);
            if (uFD_STATUS != UFD_STATUS.OK)
            {
                UFDatabase.GetErrorString(uFD_STATUS, out this.m_strError);
                this.tbxMessage.AppendText("UFDatabase AgregarData: " + this.m_strError + "\r\n");
            }
            else
            {
                this.cbScanTemplateType.Enabled = false;
            }
            this.UpdateDatabaseList();
        }
        private void btnIdentify_Click(object sender, EventArgs e)
        {
            byte[] array = new byte[1024];
            byte[][] template2Array = null;
            int[] template2SizeArray = null;
            int[] array2 = null;
            int template2Num;
            UFD_STATUS templateListWithSerial = this.m_Database.GetTemplateListWithSerial(out template2Array, out template2SizeArray, out template2Num, out array2);
            if (templateListWithSerial != UFD_STATUS.OK)
            {
                UFDatabase.GetErrorString(templateListWithSerial, out this.m_strError);
                this.tbxMessage.AppendText("UFD_GetTemplateListWithSerial: " + this.m_strError + "\r\n");
                return;
            }
            int template1Size;
            if (!this.ExtractTemplate(array, out template1Size))
            {
                return;
            }
            this.DrawCapturedImage(this.m_Scanner);
            Cursor.Current = Cursors.WaitCursor;
            int num;
            UFM_STATUS uFM_STATUS = this.m_Matcher.Identify(array, template1Size, template2Array, template2SizeArray, template2Num, 5000, out num);
            Cursor.Current = this.Cursor;
            if (uFM_STATUS != UFM_STATUS.OK)
            {
                UFMatcher.GetErrorString(uFM_STATUS, out this.m_strError);
                this.tbxMessage.AppendText("UFMatcher Identify: " + this.m_strError + "\r\n");
                return;
            }
            if (num != -1)
            {
                this.tbxMessage.AppendText("Identificación exitos (Serial = " + array2[num] + ")\r\n");
                return;
            }
            this.tbxMessage.AppendText("Identificación fallida\r\n");
        }
        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            UFD_STATUS uFD_STATUS = this.m_Database.RemoveAllData();
            if (uFD_STATUS == UFD_STATUS.OK)
            {
                this.tbxMessage.AppendText("UFDatabase RemoveAllData: OK\r\n");
                this.UpdateDatabaseList();
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out this.m_strError);
            this.tbxMessage.AppendText("UFDatabase RemoveAllData: " + this.m_strError + "\r\n");
        }
        private void btnSelectionDelete_Click(object sender, EventArgs e)
        {
            if (this.lvDatabaseList.SelectedIndices.Count == 0)
            {
                this.tbxMessage.AppendText("Seleccione el registro\r\n");
                return;
            }
            int serial = Convert.ToInt32(this.lvDatabaseList.SelectedItems[0].SubItems[0].Text);
            UFD_STATUS uFD_STATUS = this.m_Database.RemoveDataBySerial(serial);
            if (uFD_STATUS == UFD_STATUS.OK)
            {
                this.tbxMessage.AppendText("UFDatabase RemoveDataBySerial: OK\r\n");
                this.UpdateDatabaseList();
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out this.m_strError);
            this.tbxMessage.AppendText("UFDatabase RemoveDataBySerial: " + this.m_strError + "\r\n");
        }
        private void btnSelectionUpdateUserInfo_Click(object sender, EventArgs e)
        {
            UserInfoForms userInfoForm = new UserInfoForms(true);
            if (this.lvDatabaseList.SelectedIndices.Count == 0)
            {
                this.tbxMessage.AppendText("Seleccione el registro\r\n");
                return;
            }
            int serial = Convert.ToInt32(this.lvDatabaseList.SelectedItems[0].SubItems[0].Text);
            userInfoForm.UserID = this.lvDatabaseList.SelectedItems[0].SubItems[1].Text;
            userInfoForm.FingerIndex = Convert.ToInt32(this.lvDatabaseList.SelectedItems[0].SubItems[2].Text);
            userInfoForm.Memo = this.lvDatabaseList.SelectedItems[0].SubItems[5].Text;
            this.tbxMessage.AppendText("Datos de usuario actualizado\r\n");
            this.tbxMessage.AppendText("Id usuario y dedo indice no seran actualizados\r\n");
            if (userInfoForm.ShowDialog(this) != DialogResult.OK)
            {
                this.tbxMessage.AppendText("El ingreso de datos ha sido cancelado por el usuario\r\n");
                return;
            }
            UFD_STATUS uFD_STATUS = this.m_Database.UpdateDataBySerial(serial, null, 0, null, 0, userInfoForm.Memo);
            if (uFD_STATUS == UFD_STATUS.OK)
            {
                this.tbxMessage.AppendText("UFD_UpdateDataBySerial: OK\r\n");
                this.UpdateDatabaseList();
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out this.m_strError);
            this.tbxMessage.AppendText("UFDatabase UpdateDataBySerial: " + this.m_strError + "\r\n");
        }
        private void btnSelectionUpdateTemplate_Click(object sender, EventArgs e)
        {
            if (this.lvDatabaseList.SelectedIndices.Count == 0)
            {
                this.tbxMessage.AppendText("Seleccione el registro\r\n");
                return;
            }
            int serial = Convert.ToInt32(this.lvDatabaseList.SelectedItems[0].SubItems[0].Text);
            if (!this.ExtractTemplate(this.m_Template1, out this.m_Template1Size))
            {
                return;
            }
            this.DrawCapturedImage(this.m_Scanner);
            UFD_STATUS uFD_STATUS = this.m_Database.UpdateDataBySerial(serial, this.m_Template1, this.m_Template1Size, null, 0, null);
            if (uFD_STATUS == UFD_STATUS.OK)
            {
                this.tbxMessage.AppendText("UFD_UpdateDataBySerial: OK\r\n");
                this.UpdateDatabaseList();
                return;
            }
            UFDatabase.GetErrorString(uFD_STATUS, out this.m_strError);
            this.tbxMessage.AppendText("UFDatabase UpdateDataBySerial: " + this.m_strError + "\r\n");
        }
        private void btnSelectionVerify_Click(object sender, EventArgs e)
        {
            byte[] array = new byte[1024];
            if (this.lvDatabaseList.SelectedIndices.Count == 0)
            {
                this.tbxMessage.AppendText("Seleccione el registro\r\n");
                return;
            }
            int num = Convert.ToInt32(this.lvDatabaseList.SelectedItems[0].SubItems[0].Text);
            UFD_STATUS dataBySerial = this.m_Database.GetDataBySerial(num, out this.m_UserID, out this.m_FingerIndex, this.m_Template1, out this.m_Template1Size, this.m_Template2, out this.m_Template2Size, out this.m_Memo);
            if (dataBySerial != UFD_STATUS.OK)
            {
                UFDatabase.GetErrorString(dataBySerial, out this.m_strError);
                this.tbxMessage.AppendText("UFDatabase UpdateDataBySerial: " + this.m_strError + "\r\n");
                return;
            }
            int template1Size;
            if (!this.ExtractTemplate(array, out template1Size))
            {
                return;
            }
            this.DrawCapturedImage(this.m_Scanner);
            bool flag;
            UFM_STATUS uFM_STATUS = this.m_Matcher.Verify(array, template1Size, this.m_Template1, this.m_Template1Size, out flag);
            if (uFM_STATUS != UFM_STATUS.OK)
            {
                UFMatcher.GetErrorString(uFM_STATUS, out this.m_strError);
                this.tbxMessage.AppendText("UFMatcher Verify: " + this.m_strError + "\r\n");
                return;
            }
            if (flag)
            {
                this.tbxMessage.AppendText("Verificación exitosa (Serial = " + num + ")\r\n");
                return;
            }
            this.tbxMessage.AppendText("Verificación fallo\r\n");
        }
        private void bScanTemplateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.m_Scanner == null)
            {
                tbxMessage.AppendText("Sin instancias de escaner\r\n");
                return;
            }
            switch (this.cbScanTemplateType.SelectedIndex)
            {
                case 0:
                    this.m_Scanner.nTemplateType = 2001;

                    this.m_Matcher.nTemplateType = 2001;

                    return;
                case 1:
                    this.m_Scanner.nTemplateType = 2002;

                    this.m_Matcher.nTemplateType = 2002;

                    return;
                case 2:
                    this.m_Scanner.nTemplateType = 2003;

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
            this.btnInit = new Button();
            this.btnUninit = new Button();
            this.btnEnroll = new Button();
            this.btnIdentify = new Button();
            this.groupBox1 = new GroupBox();
            this.btnSelectionVerify = new Button();
            this.btnSelectionUpdateTemplate = new Button();
            this.btnSelectionUpdateUserInfo = new Button();
            this.btnSelectionDelete = new Button();
            this.btnDeleteAll = new Button();
            this.lvDatabaseList = new ListView();
            this.tbxMessage = new TextBox();
            this.btnClear = new Button();
            this.pbImageFrame = new PictureBox();
            this.label1 = new Label();
            this.cbScanTemplateType = new ComboBox();
            this.groupBox1.SuspendLayout();
            ((ISupportInitialize)(pbImageFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // btnInit
            // 
            this.btnInit.AccessibleDescription = "";
            this.btnInit.Location = new Point(12, 12);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new Size(84, 24);
            this.btnInit.TabIndex = 0;
            this.btnInit.Text = "Iniciar";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // btnUninit
            // 
            this.btnUninit.AccessibleDescription = "";
            this.btnUninit.Location = new Point(12, 40);
            this.btnUninit.Name = "btnUninit";
            this.btnUninit.Size = new Size(84, 24);
            this.btnUninit.TabIndex = 1;
            this.btnUninit.Text = "Desconectar";
            this.btnUninit.UseVisualStyleBackColor = true;
            this.btnUninit.Click += new EventHandler(this.btnUninit_Click);
            // 
            // btnEnroll
            // 
            this.btnEnroll.AccessibleDescription = "";
            this.btnEnroll.Location = new Point(12, 84);
            this.btnEnroll.Name = "btnEnroll";
            this.btnEnroll.Size = new Size(84, 24);
            this.btnEnroll.TabIndex = 2;
            this.btnEnroll.Text = "Registrar";
            this.btnEnroll.UseVisualStyleBackColor = true;
            this.btnEnroll.Click += new EventHandler(btnEnroll_Click);
            // 
            // btnIdentify
            // 
            this.btnIdentify.AccessibleDescription = "";
            this.btnIdentify.Location = new Point(12, 112);
            this.btnIdentify.Name = "btnIdentify";
            this.btnIdentify.Size = new Size(84, 24);
            this.btnIdentify.TabIndex = 3;
            this.btnIdentify.Text = "Identificar";
            this.btnIdentify.UseVisualStyleBackColor = true;
            this.btnIdentify.Click += new EventHandler(btnIdentify_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(btnSelectionVerify);
            this.groupBox1.Controls.Add(btnSelectionUpdateTemplate);
            this.groupBox1.Controls.Add(btnSelectionUpdateUserInfo);
            this.groupBox1.Controls.Add(btnSelectionDelete);
            this.groupBox1.Location = new Point(12, 208);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(84, 168);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Selection";
            // 
            // btnSelectionVerify
            // 
            this.btnSelectionVerify.AccessibleDescription = "";
            this.btnSelectionVerify.Location = new Point(8, 136);
            this.btnSelectionVerify.Name = "btnSelectionVerify";
            this.btnSelectionVerify.Size = new Size(68, 24);
            this.btnSelectionVerify.TabIndex = 8;
            this.btnSelectionVerify.Text = "Verificar";
            this.btnSelectionVerify.UseVisualStyleBackColor = true;
            this.btnSelectionVerify.Click += new EventHandler(btnSelectionVerify_Click);
            // 
            // btnSelectionUpdateTemplate
            // 
            this.btnSelectionUpdateTemplate.AccessibleDescription = "";
            this.btnSelectionUpdateTemplate.Location = new Point(8, 92);
            this.btnSelectionUpdateTemplate.Name = "btnSelectionUpdateTemplate";
            this.btnSelectionUpdateTemplate.Size = new Size(68, 40);
            this.btnSelectionUpdateTemplate.TabIndex = 7;
            this.btnSelectionUpdateTemplate.Text = "Actualizar Plantilla";
            this.btnSelectionUpdateTemplate.UseVisualStyleBackColor = true;
            this.btnSelectionUpdateTemplate.Click += new EventHandler(this.btnSelectionUpdateTemplate_Click);
            // 
            // btnSelectionUpdateUserInfo
            // 
            this.btnSelectionUpdateUserInfo.AccessibleDescription = "";
            this.btnSelectionUpdateUserInfo.Location = new Point(8, 48);
            this.btnSelectionUpdateUserInfo.Name = "btnSelectionUpdateUserInfo";
            this.btnSelectionUpdateUserInfo.Size = new Size(68, 40);
            this.btnSelectionUpdateUserInfo.TabIndex = 6;
            this.btnSelectionUpdateUserInfo.Text = "Actualizar Usuario";
            this.btnSelectionUpdateUserInfo.UseVisualStyleBackColor = true;
            this.btnSelectionUpdateUserInfo.Click += new EventHandler(this.btnSelectionUpdateUserInfo_Click);
            // 
            // btnSelectionDelete
            // 
            this.btnSelectionDelete.AccessibleDescription = "";
            this.btnSelectionDelete.Location = new Point(8, 20);
            this.btnSelectionDelete.Name = "btnSelectionDelete";
            this.btnSelectionDelete.Size = new Size(68, 24);
            this.btnSelectionDelete.TabIndex = 5;
            this.btnSelectionDelete.Text = "Eliminar";
            this.btnSelectionDelete.UseVisualStyleBackColor = true;
            this.btnSelectionDelete.Click += new EventHandler(btnSelectionDelete_Click);
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.AccessibleDescription = "";
            this.btnDeleteAll.Location = new Point(12, 152);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new Size(84, 24);
            this.btnDeleteAll.TabIndex = 5;
            this.btnDeleteAll.Text = "Eliminar Todo";
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new EventHandler(this.btnDeleteAll_Click);
            // 
            // lvDatabaseList
            // 
            this.lvDatabaseList.Activation = ItemActivation.OneClick;
            this.lvDatabaseList.FullRowSelect = true;
            this.lvDatabaseList.GridLines = true;
            this.lvDatabaseList.HideSelection = false;
            this.lvDatabaseList.Location = new Point(347, 12);
            this.lvDatabaseList.MultiSelect = false;
            this.lvDatabaseList.Name = "lvDatabaseList";
            this.lvDatabaseList.Size = new Size(420, 296);
            this.lvDatabaseList.TabIndex = 6;
            this.lvDatabaseList.UseCompatibleStateImageBehavior = false;
            this.lvDatabaseList.View = View.Details;
            // 
            // tbxMessage
            // 
            this.tbxMessage.Location = new Point(102, 320);
            this.tbxMessage.Multiline = true;
            this.tbxMessage.Name = "tbxMessage";
            this.tbxMessage.Size = new Size(609, 84);
            this.tbxMessage.TabIndex = 7;
            // 
            // btnClear
            // 
            this.btnClear.AccessibleDescription = "";
            this.btnClear.Location = new Point(719, 320);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new Size(48, 84);
            this.btnClear.TabIndex = 8;
            this.btnClear.Text = "Limpiar";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // pbImageFrame
            // 
            this.pbImageFrame.Location = new Point(102, 12);
            this.pbImageFrame.Name = "pbImageFrame";
            this.pbImageFrame.Size = new Size(228, 252);
            this.pbImageFrame.TabIndex = 9;
            this.pbImageFrame.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(101, 278);
            this.label1.Name = "label1";
            this.label1.Size = new Size(81, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Tipo de plantilla";
            // 
            // cbScanTemplateType
            // 
            this.cbScanTemplateType.BackColor = Color.White;
            this.cbScanTemplateType.FormattingEnabled = true;
            this.cbScanTemplateType.Items.AddRange(new object[] {
            "suprema",
            "iso19479_2",
            "ansi378"});
            this.cbScanTemplateType.Location = new Point(205, 276);
            this.cbScanTemplateType.Name = "cbScanTemplateType";
            this.cbScanTemplateType.Size = new Size(124, 21);
            this.cbScanTemplateType.TabIndex = 11;
            this.cbScanTemplateType.SelectedIndexChanged += new System.EventHandler(this.bScanTemplateType_SelectedIndexChanged);
            // 
            // UFE30_DatabaseDemo
            // 
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(779, 414);
            this.Controls.Add(cbScanTemplateType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbImageFrame);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.tbxMessage);
            this.Controls.Add(this.lvDatabaseList);
            this.Controls.Add(this.btnDeleteAll);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnIdentify);
            this.Controls.Add(this.btnEnroll);
            this.Controls.Add(this.btnUninit);
            this.Controls.Add(this.btnInit);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "UFE30_DatabaseDemo";
            this.Text = "Suprema PC SDK Database Demo (VC#)";
            this.FormClosing += new FormClosingEventHandler(this.UFE30_DatabaseDemo_FormClosing);
            this.Load += new System.EventHandler(this.UFE30_DatabaseDemo_Load);
            this.groupBox1.ResumeLayout(false);
            ((ISupportInitialize)(this.pbImageFrame)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
 
}
