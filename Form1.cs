using DocumentFormat.OpenXml.Office2010.ExcelAc;
using F29Downloader.Models;
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

        private bool ConsultarReintento(
    Contribuyente contribuyente)
        {
            DialogResult resultado =
                MessageBox.Show(
                    $"No fue posible iniciar sesión para:\r\n\r\n" +
                    $"{contribuyente.Rut}\r\n\r\n" +
                    "¿Desea intentar nuevamente?",
                    "Inicio de sesión",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            return resultado == DialogResult.Yes;
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

            if (excel.Errores.Count > 0)
            {
                MessageBox.Show(
                    "Se encontraron errores en el archivo Excel.\r\n\r\n" +
                    string.Join(
                        "\r\n",
                        excel.Errores) +
                    "\r\n\r\nCorrija el archivo y vuelva a cargar el Excel.",
                    "Validación del Excel",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DatabaseService db =
                new DatabaseService();

            db.LimpiarContribuyentes();

            db.InsertarContribuyentes(
                lista);

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
                ActualizarGrilla,
                ConsultarReintento);
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

            if (!int.TryParse(
                textBox1.Text,
                out segundosEspera))
            {
                MessageBox.Show(
                    "Ingrese un tiempo de espera válido.");

                textBox1.Focus();

                return;
            }

            progressBarProceso.Value = 0;

            await download.IniciarDescarga(
                lista,
                segundosEspera,
                EscribirLog,
                ActualizarGrilla,
                ActualizarEstado,
                ActualizarProgreso,
                ConsultarReintento);
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
                "⚠ Cancelando proceso...\r\n");


            download.ProcesoCancelado();


            await download.ForzarCierre();


            EscribirLog(
                "✓ Navegador cerrado.\r\n");


            RestablecerInterfaz();
        }

        private void ActualizarEstado(
    string texto)
        {
            if (InvokeRequired)
            {
                Invoke(() =>
                    ActualizarEstado(texto));

                return;
            }

            label4.Text = texto;
        }

        private void ActualizarProgreso(
    int actual,
    int total)
        {
            if (InvokeRequired)
            {
                Invoke(() =>
                    ActualizarProgreso(
                        actual,
                        total));

                return;
            }


            if (total <= 0)
                return;


            progressBarProceso.Minimum = 0;

            progressBarProceso.Maximum = total;


            progressBarProceso.Value =
                Math.Min(
                    actual,
                    total);
        }

        private void RestablecerInterfaz()
        {
            if (InvokeRequired)
            {
                Invoke(RestablecerInterfaz);
                return;
            }


            label.Text =
                "Listo";


            progressBarProceso.Value =
                0;


            label4.Text =
                "0 / 0 (0%)";


            txtLog.AppendText(
                "✓ Proceso detenido por usuario.\r\n");
        }
    }
}