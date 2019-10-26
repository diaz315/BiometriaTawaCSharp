using System;
using System.Drawing;
using System.Reflection.Emit;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

public class UserInfoForms : Form
{
    private System.ComponentModel.IContainer components;
    private Label label1;
    private Label label2;
    private Label label3;
    private TextBox tbxUserID;
    private TextBox tbxFingerIndex;
    private TextBox tbxMemo;
    private Button btnOK;
    private Button btnCancel;
    public string UserID
    {
        get
        {
            return this.tbxUserID.Text;
        }
        set
        {
            this.tbxUserID.Text = value;
        }
    }
    public int FingerIndex
    {
        get
        {
            return Convert.ToInt32(this.tbxFingerIndex.Text);
        }
        set
        {
            this.tbxFingerIndex.Text = Convert.ToString(value);
        }
    }
    public string Memo
    {
        get
        {
            return this.tbxMemo.Text;
        }
        set
        {
            this.tbxMemo.Text = Convert.ToString(value);
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbxUserID = new System.Windows.Forms.TextBox();
            this.tbxFingerIndex = new System.Windows.Forms.TextBox();
            this.tbxMemo = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Empleado";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "FingerIndex";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Descripcion";
            // 
            // tbxUserID
            // 
            this.tbxUserID.Location = new System.Drawing.Point(96, 12);
            this.tbxUserID.MaxLength = 50;
            this.tbxUserID.Name = "tbxUserID";
            this.tbxUserID.Size = new System.Drawing.Size(96, 20);
            this.tbxUserID.TabIndex = 3;
            // 
            // tbxFingerIndex
            // 
            this.tbxFingerIndex.Location = new System.Drawing.Point(96, 40);
            this.tbxFingerIndex.MaxLength = 1;
            this.tbxFingerIndex.Name = "tbxFingerIndex";
            this.tbxFingerIndex.Size = new System.Drawing.Size(40, 20);
            this.tbxFingerIndex.TabIndex = 4;
            // 
            // tbxMemo
            // 
            this.tbxMemo.Location = new System.Drawing.Point(96, 68);
            this.tbxMemo.MaxLength = 100;
            this.tbxMemo.Multiline = true;
            this.tbxMemo.Name = "tbxMemo";
            this.tbxMemo.Size = new System.Drawing.Size(144, 100);
            this.tbxMemo.TabIndex = 5;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(12, 180);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(108, 20);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(132, 180);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(108, 20);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // UserInfoForms
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(254, 211);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbxMemo);
            this.Controls.Add(this.tbxFingerIndex);
            this.Controls.Add(this.tbxUserID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserInfoForms";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Información de usuario";
            this.ResumeLayout(false);
            this.PerformLayout();

    }
    public UserInfoForms(bool Update)
    {
        this.InitializeComponent();
        this.tbxUserID.Text = "UserID";
        this.tbxFingerIndex.Text = "0";
        this.tbxMemo.Text = "Memo";
        if (Update)
        {
            this.tbxUserID.ReadOnly = true;
            this.tbxFingerIndex.ReadOnly = true;
        }
    }
    private void btnOK_Click(object sender, EventArgs e)
    {
    }
}