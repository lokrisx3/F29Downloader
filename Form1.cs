using System.Windows.Forms.Design;
using F29Downloader.Services;
using Microsoft.Playwright;

namespace F29Downloader
{
    public partial class Form1 : Form
    {
        private string rutaExcel = "";


        public Form1()
        {
            InitializeComponent();

            DatabaseService db = new DatabaseService();
            db.CrearBaseDatos();
        }

        private void btnSeleccionarExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialogo = new OpenFileDialog
            {
                Filter = "Archivos Excel (*.xlsx)|*.xlsx"
            };

            if (dialogo.ShowDialog() == DialogResult.OK)
            {
                rutaExcel = dialogo.FileName;

                lblArchivo.Text = rutaExcel;

                txtLog.AppendText(
                    "Archivo seleccionado: " + rutaExcel + "\r\n");
            }
        }

        private void btnCargarExcel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rutaExcel))
            {
                MessageBox.Show("Seleccione un archivo Excel.");
                return;
            }

            ExcelService excel = new ExcelService();

            List<Models.Contribuyente> lista =
                excel.LeerExcel(rutaExcel);

            DatabaseService db = new DatabaseService();

            db.LimpiarContribuyentes();
            db.InsertarContribuyentes(lista);

            dgvContribuyentes.DataSource =
                db.ObtenerContribuyentes();

            txtLog.AppendText(
                $"Se cargaron {lista.Count} registros.\r\n");
        }
        private void EscribirLog(string mensaje)
        {
            txtLog.AppendText(mensaje);
        }
        private async void btnProbarNavegador_Click(
    object sender,
    EventArgs e)
        {
            Models.Contribuyente contribuyente =
                new Models.Contribuyente
                {
                    Rut = "76869422-2",
                    Clave = "Patricio2026@",
                    Periodo = "05/2025"
                };

            DownloadService download =
                new DownloadService();

            await download.ProcesarContribuyente(
                contribuyente,
                chkHeadless.Checked,
                EscribirLog);
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

            int segundosEspera = 0;

            if (!int.TryParse(textBox1.Text, out segundosEspera))
            {
                MessageBox.Show(
                    "Ingrese un tiempo de espera válido.");

                textBox1.Focus();

                return;
            }

            DownloadService download =
                new DownloadService();

            for (int i = 0; i < lista.Count; i++)
            {
                await download.ProcesarContribuyente(
                    lista[i],
                    chkHeadless.Checked,
                    EscribirLog);

                if (i < lista.Count - 1 &&
                    segundosEspera > 0)
                {
                    EscribirLog(
                        $"Esperando {segundosEspera} segundos...\r\n");

                    await Task.Delay(
                        TimeSpan.FromSeconds(segundosEspera));
                }
            }

            EscribirLog(
                "\r\nProceso finalizado.\r\n");
        }
    }
}
