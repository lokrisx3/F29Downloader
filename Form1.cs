using System.Windows.Forms.Design;
using F29Downloader.Services;
using Microsoft.Playwright;

namespace F29Downloader
{
    public partial class Form1 : Form
    {
        private string rutaExcel = "";

        private const string UrlPdfF29Prueba =
            "https://www4.sii.cl/rfiInternet/formCompacto?folio=9071226716&rut=76869422&form=029&codInt=872677028";

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

        private async void btnProbarNavegador_Click(
            object sender,
            EventArgs e)
        {
            SiiService sii =
                new SiiService();

            IPage pagina =
                await sii.AbrirLogin(false);

            LoginService login =
                new LoginService();

            Models.Contribuyente contribuyente =
                new Models.Contribuyente
                {
                    Rut = "76869422-2",
                    Clave = "PATRICIO2018"
                };

            bool resultado =
                await login.IniciarSesion(
                    pagina,
                    contribuyente);

            if (resultado)
            {
                txtLog.AppendText(
                    "✓ Inicio de sesión exitoso.\r\n");

                IBrowserContext contexto =
                    sii.ObtenerContexto();

                // Servicio encargado del PDF
                PdfService pdf =
                    new PdfService();

                string rutaPdf =
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Downloads",
                        "F29_9071226716.pdf");

                bool guardado =
                    await pdf.GuardarPdfDesdeUrl(
                        contexto,
                        UrlPdfF29Prueba,
                        rutaPdf);

                if (guardado)
                {
                    txtLog.AppendText(
                        "PDF guardado correctamente: " +
                        rutaPdf +
                        "\r\n");
                }
                else
                {
                    txtLog.AppendText(
                        "Error al guardar PDF.\r\n");
                }
            }
            else
            {
                txtLog.AppendText(
                    "✗ Error al iniciar sesión.\r\n");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
