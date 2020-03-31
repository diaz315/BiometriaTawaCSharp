namespace BiometriaTawaCSharp
{
    partial class SeleccionBiometrico
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
            this.pbxEikon = new System.Windows.Forms.PictureBox();
            this.pbxSuprema = new System.Windows.Forms.PictureBox();
            this.pbxEmpresa = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbxEikon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxSuprema)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxEmpresa)).BeginInit();
            this.SuspendLayout();
            // 
            // pbxEikon
            // 
            this.pbxEikon.Location = new System.Drawing.Point(25, 65);
            this.pbxEikon.Name = "pbxEikon";
            this.pbxEikon.Size = new System.Drawing.Size(281, 232);
            this.pbxEikon.TabIndex = 3;
            this.pbxEikon.TabStop = false;
            this.pbxEikon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbxEikon_MouseClick);
            // 
            // pbxSuprema
            // 
            this.pbxSuprema.Location = new System.Drawing.Point(366, 65);
            this.pbxSuprema.Name = "pbxSuprema";
            this.pbxSuprema.Size = new System.Drawing.Size(281, 232);
            this.pbxSuprema.TabIndex = 4;
            this.pbxSuprema.TabStop = false;
            this.pbxSuprema.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbxSuprema_MouseClick);
            // 
            // pbxEmpresa
            // 
            this.pbxEmpresa.Location = new System.Drawing.Point(507, 317);
            this.pbxEmpresa.Name = "pbxEmpresa";
            this.pbxEmpresa.Size = new System.Drawing.Size(207, 148);
            this.pbxEmpresa.TabIndex = 5;
            this.pbxEmpresa.TabStop = false;
            // 
            // SeleccionBiometrico
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(236)))), ((int)(((byte)(244)))));
            this.ClientSize = new System.Drawing.Size(726, 477);
            this.Controls.Add(this.pbxEmpresa);
            this.Controls.Add(this.pbxSuprema);
            this.Controls.Add(this.pbxEikon);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "SeleccionBiometrico";
            this.Text = "Seleccionar Biometrico";
            ((System.ComponentModel.ISupportInitialize)(this.pbxEikon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxSuprema)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxEmpresa)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pbxEikon;
        private System.Windows.Forms.PictureBox pbxSuprema;
        private System.Windows.Forms.PictureBox pbxEmpresa;
    }
}