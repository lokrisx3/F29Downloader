namespace F29Downloader
{
    public class CredencialesDialog : Form
    {
        private readonly TextBox txtRut = new();
        private readonly TextBox txtClave = new();
        private readonly Button btnMostrarClave = new();
        private readonly ComboBox cboMes = new();
        private readonly ComboBox cboAnio = new();

        public string Rut => txtRut.Text.Trim();

        public string Clave => txtClave.Text;

        public string Periodo => $"{cboMes.SelectedItem}/{cboAnio.SelectedItem}";

        public CredencialesDialog(
            string rutPredeterminado = "",
            string clavePredeterminada = "")
        {
            Text = "Iniciar sesión";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(365, 236);

            Label lblRut = new()
            {
                Text = "RUT:",
                AutoSize = true,
                Location = new Point(20, 24)
            };

            txtRut.Location = new Point(20, 45);
            txtRut.Size = new Size(325, 23);
            txtRut.Text = rutPredeterminado;
            txtRut.TabIndex = 0;

            Label lblClave = new()
            {
                Text = "Contraseña:",
                AutoSize = true,
                Location = new Point(20, 80)
            };

            txtClave.Location = new Point(20, 101);
            txtClave.Size = new Size(275, 23);
            txtClave.Text = clavePredeterminada;
            txtClave.UseSystemPasswordChar = true;
            txtClave.TabIndex = 1;

            btnMostrarClave.Text = "Mostrar";
            btnMostrarClave.Location = new Point(300, 100);
            btnMostrarClave.Size = new Size(45, 25);
            btnMostrarClave.TabIndex = 2;
            btnMostrarClave.Click += btnMostrarClave_Click;

            Label lblPeriodo = new()
            {
                Text = "Período:",
                AutoSize = true,
                Location = new Point(20, 137)
            };

            cboMes.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMes.Location = new Point(20, 158);
            cboMes.Size = new Size(70, 23);
            cboMes.Items.AddRange(Enumerable.Range(1, 12)
                .Select(mes => mes.ToString("00"))
                .Cast<object>()
                .ToArray());
            cboMes.SelectedItem = "05";

            cboAnio.DropDownStyle = ComboBoxStyle.DropDownList;
            cboAnio.Location = new Point(100, 158);
            cboAnio.Size = new Size(90, 23);
            cboAnio.Items.AddRange(Enumerable.Range(2020, DateTime.Now.Year - 2019)
                .Select(anio => anio.ToString())
                .Cast<object>()
                .ToArray());
            cboAnio.SelectedItem = "2025";

            Button btnAceptar = new()
            {
                Text = "Aceptar",
                DialogResult = DialogResult.OK,
                Location = new Point(189, 198),
                Size = new Size(75, 26),
                TabIndex = 3
            };
            btnAceptar.Click += btnAceptar_Click;

            Button btnCancelar = new()
            {
                Text = "Cancelar",
                DialogResult = DialogResult.Cancel,
                Location = new Point(270, 198),
                Size = new Size(75, 26),
                TabIndex = 4
            };

            AcceptButton = btnAceptar;
            CancelButton = btnCancelar;
            Controls.AddRange([lblRut, txtRut, lblClave, txtClave, lblPeriodo, cboMes, cboAnio,
                btnMostrarClave, btnAceptar, btnCancelar]);
        }

        private void btnMostrarClave_Click(object? sender, EventArgs e)
        {
            txtClave.UseSystemPasswordChar = !txtClave.UseSystemPasswordChar;
            btnMostrarClave.Text = txtClave.UseSystemPasswordChar ? "Mostrar" : "Ocultar";
        }

        private void btnAceptar_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Rut) && !string.IsNullOrWhiteSpace(Clave))
                return;

            MessageBox.Show(
                "Ingrese el RUT y la contraseña.",
                "Datos incompletos",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            DialogResult = DialogResult.None;
        }
    }
}
