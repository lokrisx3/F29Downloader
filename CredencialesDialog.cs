namespace F29Downloader
{
    public class CredencialesDialog : Form
    {
        private readonly TextBox txtRut = new();
        private readonly TextBox txtClave = new();
        private readonly Button btnMostrarClave = new();

        public string Rut => txtRut.Text.Trim();

        public string Clave => txtClave.Text;

        public CredencialesDialog()
        {
            Text = "Iniciar sesión";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(365, 180);

            Label lblRut = new()
            {
                Text = "RUT:",
                AutoSize = true,
                Location = new Point(20, 24)
            };

            txtRut.Location = new Point(20, 45);
            txtRut.Size = new Size(325, 23);
            txtRut.Text = "76869422-2";
            txtRut.TabIndex = 0;

            Label lblClave = new()
            {
                Text = "Contraseña:",
                AutoSize = true,
                Location = new Point(20, 80)
            };

            txtClave.Location = new Point(20, 101);
            txtClave.Size = new Size(275, 23);
            txtClave.Text = "Patricio2026@";
            txtClave.UseSystemPasswordChar = true;
            txtClave.TabIndex = 1;

            btnMostrarClave.Text = "Mostrar";
            btnMostrarClave.Location = new Point(300, 100);
            btnMostrarClave.Size = new Size(45, 25);
            btnMostrarClave.TabIndex = 2;
            btnMostrarClave.Click += btnMostrarClave_Click;

            Button btnAceptar = new()
            {
                Text = "Aceptar",
                DialogResult = DialogResult.OK,
                Location = new Point(189, 142),
                Size = new Size(75, 26),
                TabIndex = 3
            };
            btnAceptar.Click += btnAceptar_Click;

            Button btnCancelar = new()
            {
                Text = "Cancelar",
                DialogResult = DialogResult.Cancel,
                Location = new Point(270, 142),
                Size = new Size(75, 26),
                TabIndex = 4
            };

            AcceptButton = btnAceptar;
            CancelButton = btnCancelar;
            Controls.AddRange([lblRut, txtRut, lblClave, txtClave,
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
