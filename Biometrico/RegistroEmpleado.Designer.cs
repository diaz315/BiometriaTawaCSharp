﻿namespace BiometriaTawaCSharp
{
    partial class RegistroEmpleado
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtCodEmpleado = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtNombres = new System.Windows.Forms.TextBox();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDoc = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNroDoc = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCodEmp = new System.Windows.Forms.TextBox();
            pbImageFrame = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btnEliminarHuella = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(pbImageFrame)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtCodEmpleado
            // 
            this.txtCodEmpleado.Location = new System.Drawing.Point(124, 12);
            this.txtCodEmpleado.Name = "txtCodEmpleado";
            this.txtCodEmpleado.Size = new System.Drawing.Size(250, 20);
            this.txtCodEmpleado.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Código Colaborador:";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(156)))), ((int)(((byte)(203)))));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(293, 38);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Buscar";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtNombres
            // 
            this.txtNombres.Location = new System.Drawing.Point(124, 118);
            this.txtNombres.Name = "txtNombres";
            this.txtNombres.ReadOnly = true;
            this.txtNombres.Size = new System.Drawing.Size(250, 20);
            this.txtNombres.TabIndex = 3;
            // 
            // btnRegistrar
            // 
            this.btnRegistrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(156)))), ((int)(((byte)(203)))));
            this.btnRegistrar.Enabled = false;
            this.btnRegistrar.ForeColor = System.Drawing.Color.White;
            this.btnRegistrar.Location = new System.Drawing.Point(209, 374);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(75, 23);
            this.btnRegistrar.TabIndex = 4;
            this.btnRegistrar.Text = "Registrar";
            this.btnRegistrar.UseVisualStyleBackColor = false;
            this.btnRegistrar.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Colaborador:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 151);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Tipo de Documento:";
            // 
            // txtDoc
            // 
            this.txtDoc.Location = new System.Drawing.Point(124, 144);
            this.txtDoc.Name = "txtDoc";
            this.txtDoc.ReadOnly = true;
            this.txtDoc.Size = new System.Drawing.Size(250, 20);
            this.txtDoc.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 177);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Nro. Documento:";
            // 
            // txtNroDoc
            // 
            this.txtNroDoc.Location = new System.Drawing.Point(124, 170);
            this.txtNroDoc.Name = "txtNroDoc";
            this.txtNroDoc.ReadOnly = true;
            this.txtNroDoc.Size = new System.Drawing.Size(250, 20);
            this.txtNroDoc.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 203);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Código Colaborador:";
            // 
            // txtCodEmp
            // 
            this.txtCodEmp.Location = new System.Drawing.Point(124, 196);
            this.txtCodEmp.Name = "txtCodEmp";
            this.txtCodEmp.ReadOnly = true;
            this.txtCodEmp.Size = new System.Drawing.Size(250, 20);
            this.txtCodEmp.TabIndex = 10;
            // 
            // pbImageFrame
            // 
            pbImageFrame.Location = new System.Drawing.Point(6, 18);
            pbImageFrame.Name = "pbImageFrame";
            pbImageFrame.Size = new System.Drawing.Size(94, 101);
            pbImageFrame.TabIndex = 14;
            pbImageFrame.TabStop = false;
            pbImageFrame.Click += new System.EventHandler(pbImageFrame_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(pbImageFrame);
            this.groupBox1.Location = new System.Drawing.Point(118, 233);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(111, 129);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Huella";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(156)))), ((int)(((byte)(203)))));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(293, 373);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 24);
            this.button2.TabIndex = 16;
            this.button2.Text = "Cerrar";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // btnEliminarHuella
            // 
            this.btnEliminarHuella.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(156)))), ((int)(((byte)(203)))));
            this.btnEliminarHuella.ForeColor = System.Drawing.Color.White;
            this.btnEliminarHuella.Location = new System.Drawing.Point(12, 329);
            this.btnEliminarHuella.Name = "btnEliminarHuella";
            this.btnEliminarHuella.Size = new System.Drawing.Size(96, 23);
            this.btnEliminarHuella.TabIndex = 17;
            this.btnEliminarHuella.Text = "Eliminar Huella";
            this.btnEliminarHuella.UseVisualStyleBackColor = false;
            this.btnEliminarHuella.Visible = false;
            this.btnEliminarHuella.Click += new System.EventHandler(this.btnEliminarHuella_Click);
            // 
            // RegistroEmpleado
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(236)))), ((int)(((byte)(244)))));
            this.ClientSize = new System.Drawing.Size(416, 409);
            this.Controls.Add(this.btnEliminarHuella);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtCodEmp);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtNroDoc);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDoc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnRegistrar);
            this.Controls.Add(this.txtNombres);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCodEmpleado);
            this.Name = "RegistroEmpleado";
            this.Text = "Registrar Colaborador";
            ((System.ComponentModel.ISupportInitialize)(pbImageFrame)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCodEmpleado;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtNombres;
        private System.Windows.Forms.Button btnRegistrar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDoc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNroDoc;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCodEmp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnEliminarHuella;
        public static System.Windows.Forms.PictureBox pbImageFrame;
    }
}