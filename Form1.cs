using F29Downloader.Services;

namespace F29Downloader
{
    public partial class Form1 : Form
    {
        private string rutaExcel = "";

        private readonly DownloadService download =
            new DownloadService();

        private readonly GridService grid =
            new GridService();

        public Form1()
        {
            InitializeComponent();

            DatabaseService db =
                new DatabaseService();

            db.CrearBaseDatos();

            dgvContribuyentes.CellContentClick +=
                dgvContribuyentes_CellContentClick;

            
        }

        private void dgvContribuyentes_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            grid.AbrirPdf(
                dgvContribuyentes,
                e);
        }

        private void btnSeleccionarExcel_Click(
            object sender,
            EventArgs e)
        {
            OpenFileDialog dialogo =
                new OpenFileDialog
                {
                    Filter =
                        "Archivos Excel (*.xlsx)|*.xlsx"
                };

            if (dialogo.ShowDialog() == DialogResult.OK)
            {
                rutaExcel =
                    dialogo.FileName;

                lblArchivo.Text =
                    rutaExcel;

                txtLog.AppendText(
                    $"Archivo seleccionado: {rutaExcel}\r\n");
            }
        }

        private void btnCargarExcel_Click(
            object sender,
            EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rutaExcel))
            {
                MessageBox.Show(
                    "Seleccione un archivo Excel.");

                return;
            }

            ExcelService excel =
                new ExcelService();

            List<Models.Contribuyente> lista =
                excel.LeerExcel(rutaExcel);

            DatabaseService db =
                new DatabaseService();

            db.LimpiarContribuyentes();

            db.InsertarContribuyentes(lista);

            grid.ActualizarGrilla(
                dgvContribuyentes);

            txtLog.AppendText(
                $"Se cargaron {lista.Count} registros.\r\n");
        }

        private void EscribirLog(
            string mensaje)
        {
            txtLog.AppendText(mensaje);
        }

        private async void btnProbarNavegador_Click(
            object sender,
            EventArgs e)
        {
            download.IniciarProceso();

            Models.Contribuyente contribuyente =
                new Models.Contribuyente
                {
                    Rut = "76869422-2",
                    Clave = "Patricio2026@",
                    Periodo = "05/2025"
                };

            await download.ProcesarContribuyente(
                contribuyente,
                EscribirLog,
                ActualizarGrilla);
        }

        private async void btnIniciarProceso_Click(
            object sender,
            EventArgs e)
        {
            DatabaseService db =
                new DatabaseService();

            List<Models.Contribuyente> lista =
                db.ObtenerContribuyentes();

            if (lista.Count == 0)
            {
                MessageBox.Show(
                    "No existen contribuyentes cargados.");

                return;
            }

            if (!int.TryParse(
                    textBox1.Text,
                    out int segundosEspera))
            {
                MessageBox.Show(
                    "Ingrese un tiempo de espera válido.");

                textBox1.Focus();

                return;
            }

            await download.IniciarDescarga(
                lista,
                segundosEspera,
                EscribirLog,
                ActualizarGrilla);
        }

        private void ActualizarGrilla()
        {
            if (InvokeRequired)
            {
                Invoke(ActualizarGrilla);
                return;
            }

            grid.ActualizarGrilla(
                dgvContribuyentes);
        }

        private async void forceClose_Click(
            object sender,
            EventArgs e)
        {
            EscribirLog(
                "⚠ Cerrando navegador forzadamente...\r\n");

            await download.ForzarCierre();

            EscribirLog(
                "✓ Navegador cerrado.\r\n");
        }
    }
}