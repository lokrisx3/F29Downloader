namespace F29Downloader
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblArchivo = new Label();
            dgvContribuyentes = new DataGridView();
            btnIniciarProceso = new Button();
            txtLog = new TextBox();
            label1 = new Label();
            progressBarProceso = new ProgressBar();
            btnSeleccionarExcel = new Button();
            btnCargarExcel = new Button();
            btnProbarNavegador = new Button();
            forceClose = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            textBox1 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dgvContribuyentes).BeginInit();
            SuspendLayout();
            // 
            // lblArchivo
            // 
            lblArchivo.AutoSize = true;
            lblArchivo.Location = new Point(130, 20);
            lblArchivo.Name = "lblArchivo";
            lblArchivo.Size = new Size(16, 15);
            lblArchivo.TabIndex = 1;
            lblArchivo.Text = "...";
            // 
            // dgvContribuyentes
            // 
            dgvContribuyentes.AllowUserToAddRows = false;
            dgvContribuyentes.AllowUserToDeleteRows = false;
            dgvContribuyentes.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            dgvContribuyentes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvContribuyentes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvContribuyentes.Location = new Point(12, 87);
            dgvContribuyentes.MultiSelect = false;
            dgvContribuyentes.Name = "dgvContribuyentes";
            dgvContribuyentes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvContribuyentes.Size = new Size(582, 150);
            dgvContribuyentes.TabIndex = 3;
            // 
            // btnIniciarProceso
            // 
            btnIniciarProceso.Location = new Point(12, 243);
            btnIniciarProceso.Name = "btnIniciarProceso";
            btnIniciarProceso.Size = new Size(100, 23);
            btnIniciarProceso.TabIndex = 4;
            btnIniciarProceso.Text = "Iniciar Descarga";
            btnIniciarProceso.UseVisualStyleBackColor = true;
            btnIniciarProceso.Click += btnIniciarProceso_Click;
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Location = new Point(12, 316);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Both;
            txtLog.Size = new Size(582, 150);
            txtLog.TabIndex = 6;
            txtLog.WordWrap = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 298);
            label1.Name = "label1";
            label1.Size = new Size(51, 15);
            label1.TabIndex = 7;
            label1.Text = "Detalles:";
            // 
            // progressBarProceso
            // 
            progressBarProceso.Location = new Point(12, 272);
            progressBarProceso.Name = "progressBarProceso";
            progressBarProceso.Size = new Size(240, 23);
            progressBarProceso.TabIndex = 8;
            // 
            // btnSeleccionarExcel
            // 
            btnSeleccionarExcel.Location = new Point(12, 16);
            btnSeleccionarExcel.Name = "btnSeleccionarExcel";
            btnSeleccionarExcel.Size = new Size(112, 23);
            btnSeleccionarExcel.TabIndex = 9;
            btnSeleccionarExcel.Text = "Seleccionar Excel";
            btnSeleccionarExcel.UseVisualStyleBackColor = true;
            btnSeleccionarExcel.Click += btnSeleccionarExcel_Click;
            // 
            // btnCargarExcel
            // 
            btnCargarExcel.Location = new Point(12, 48);
            btnCargarExcel.Name = "btnCargarExcel";
            btnCargarExcel.Size = new Size(122, 23);
            btnCargarExcel.TabIndex = 10;
            btnCargarExcel.Text = "Cargar Excel";
            btnCargarExcel.UseVisualStyleBackColor = true;
            btnCargarExcel.Click += btnCargarExcel_Click;
            // 
            // btnProbarNavegador
            // 
            btnProbarNavegador.Location = new Point(140, 48);
            btnProbarNavegador.Name = "btnProbarNavegador";
            btnProbarNavegador.Size = new Size(116, 23);
            btnProbarNavegador.TabIndex = 11;
            btnProbarNavegador.Text = "Probar navegador";
            btnProbarNavegador.UseVisualStyleBackColor = true;
            btnProbarNavegador.Click += btnProbarNavegador_Click;
            // 
            // forceClose
            // 
            forceClose.Location = new Point(262, 48);
            forceClose.Name = "forceClose";
            forceClose.Size = new Size(137, 23);
            forceClose.TabIndex = 13;
            forceClose.Text = "Forzar Cierre";
            forceClose.UseVisualStyleBackColor = true;
            forceClose.Click += forceClose_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(264, 273);
            label2.Name = "label2";
            label2.Size = new Size(0, 15);
            label2.TabIndex = 15;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(257, 275);
            label3.Name = "label3";
            label3.Size = new Size(51, 15);
            label3.TabIndex = 16;
            label3.Text = "0/0 (0%)";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(150, 246);
            label4.Name = "label4";
            label4.Size = new Size(182, 15);
            label4.TabIndex = 17;
            label4.Text = "Espera entre contribuyentes (seg)";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(120, 243);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(26, 23);
            textBox1.TabIndex = 18;
            textBox1.Text = "30";
            textBox1.TextAlign = HorizontalAlignment.Right;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(606, 481);
            Controls.Add(textBox1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(forceClose);
            Controls.Add(btnProbarNavegador);
            Controls.Add(btnCargarExcel);
            Controls.Add(btnSeleccionarExcel);
            Controls.Add(progressBarProceso);
            Controls.Add(label1);
            Controls.Add(txtLog);
            Controls.Add(btnIniciarProceso);
            Controls.Add(dgvContribuyentes);
            Controls.Add(lblArchivo);
            Name = "Form1";
            Text = "F29 Downloader";
            ((System.ComponentModel.ISupportInitialize)dgvContribuyentes).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lblArchivo;
        private DataGridView dgvContribuyentes;
        private Button btnIniciarProceso;
        private TextBox txtLog;
        private Label label1;
        private ProgressBar progressBarProceso;
        private Button btnSeleccionarExcel;
        private Button btnCargarExcel;
        private Button btnProbarNavegador;
        private Button forceClose;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox textBox1;
    }
}
