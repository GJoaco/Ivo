
namespace KioscoWF
{
    partial class ModificarCategoria
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
            this.ddlEstadoC = new System.Windows.Forms.ComboBox();
            this.btnModificarC = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txtDetalleC = new System.Windows.Forms.TextBox();
            this.txtNombreC = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ddlEstadoC
            // 
            this.ddlEstadoC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlEstadoC.FormattingEnabled = true;
            this.ddlEstadoC.Location = new System.Drawing.Point(23, 144);
            this.ddlEstadoC.Margin = new System.Windows.Forms.Padding(4);
            this.ddlEstadoC.Name = "ddlEstadoC";
            this.ddlEstadoC.Size = new System.Drawing.Size(499, 24);
            this.ddlEstadoC.TabIndex = 28;
            // 
            // btnModificarC
            // 
            this.btnModificarC.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnModificarC.Location = new System.Drawing.Point(23, 249);
            this.btnModificarC.Margin = new System.Windows.Forms.Padding(4);
            this.btnModificarC.Name = "btnModificarC";
            this.btnModificarC.Size = new System.Drawing.Size(500, 28);
            this.btnModificarC.TabIndex = 27;
            this.btnModificarC.Text = "Modificar";
            this.btnModificarC.UseVisualStyleBackColor = true;
            this.btnModificarC.Click += new System.EventHandler(this.btnModificarC_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(16, 174);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(92, 29);
            this.label11.TabIndex = 25;
            this.label11.Text = "Detalle";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(16, 110);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(92, 29);
            this.label12.TabIndex = 24;
            this.label12.Text = "Estado";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.label15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label15.Location = new System.Drawing.Point(16, 47);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(103, 29);
            this.label15.TabIndex = 23;
            this.label15.Text = "Nombre";
            // 
            // txtDetalleC
            // 
            this.txtDetalleC.Location = new System.Drawing.Point(23, 209);
            this.txtDetalleC.Margin = new System.Windows.Forms.Padding(4);
            this.txtDetalleC.Name = "txtDetalleC";
            this.txtDetalleC.Size = new System.Drawing.Size(499, 22);
            this.txtDetalleC.TabIndex = 21;
            // 
            // txtNombreC
            // 
            this.txtNombreC.Location = new System.Drawing.Point(23, 81);
            this.txtNombreC.Margin = new System.Windows.Forms.Padding(4);
            this.txtNombreC.Name = "txtNombreC";
            this.txtNombreC.Size = new System.Drawing.Size(499, 22);
            this.txtNombreC.TabIndex = 20;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label16.Location = new System.Drawing.Point(16, 11);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(227, 29);
            this.label16.TabIndex = 19;
            this.label16.Text = "Modificar categoria";
            // 
            // Modificar_Categoria
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.ddlEstadoC);
            this.Controls.Add(this.btnModificarC);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txtDetalleC);
            this.Controls.Add(this.txtNombreC);
            this.Controls.Add(this.label16);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Modificar_Categoria";
            this.Text = "Modificar Categoria";
            this.Load += new System.EventHandler(this.Modificar_Categoria_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ddlEstadoC;
        private System.Windows.Forms.Button btnModificarC;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtDetalleC;
        private System.Windows.Forms.TextBox txtNombreC;
        private System.Windows.Forms.Label label16;
    }
}