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

        private async Task ProcesarContribuyente(
            Models.Contribuyente contribuyente)
        {
            SiiService sii =
                new SiiService();

            IPage pagina =
                await sii.AbrirLogin(false);

            LoginService login =
                new LoginService();

            txtLog.AppendText(
                "\r\n" +
                "--------------------------------------------------\r\n" +
                "Procesando contribuyente\r\n" +
                $"RUT     : {contribuyente.Rut}\r\n" +
                $"Periodo : {contribuyente.Periodo}\r\n" +
                "--------------------------------------------------\r\n");


            bool resultado =
                await login.IniciarSesion(
                    pagina,
                    contribuyente);


            if (!resultado)
            {
                txtLog.AppendText(
                    "✗ Error al iniciar sesión.\r\n");

                return;
            }


            txtLog.AppendText(
                "✓ Inicio de sesión exitoso.\r\n");


            await sii.IrAConsultaF29(pagina);

            txtLog.AppendText(
                "✓ Consulta F29 abierta.\r\n");


            await sii.SeleccionarFormulario29(pagina);

            txtLog.AppendText(
                "✓ Formulario 29 seleccionado.\r\n");


            await sii.SeleccionarMes(
                pagina,
                "Mayo");

            txtLog.AppendText(
                "✓ Mes seleccionado.\r\n");


            await sii.BuscarDatosIngresados(pagina);

            txtLog.AppendText(
                "✓ Datos ingresados consultados.\r\n");


            await sii.AbrirFormulario(pagina);

            txtLog.AppendText(
                "✓ Formulario abierto.\r\n");


            Task<IPage> popupTask =
                pagina.WaitForPopupAsync();


            await sii.SeleccionarFormularioCompacto(
                pagina);


            IPage paginaPdf =
                await sii.AbrirPdf(
                    popupTask);


            string urlPdf =
                paginaPdf.Url;


            IBrowserContext contexto =
                sii.ObtenerContexto();


            PdfService pdf =
                new PdfService();


            string nombrePdf =
                ConfigService.ObtenerNombrePdf(
                    contribuyente);


            string carpetaContribuyente =
                Path.Combine(
                    ConfigService.DownloadsPath,
                    contribuyente.Rut);


            string rutaPdf =
                Path.Combine(
                    carpetaContribuyente,
                    nombrePdf);


            bool pdfGuardado =
                await pdf.GuardarPdfDesdeUrl(
                    contexto,
                    urlPdf,
                    rutaPdf);


            if (pdfGuardado)
            {
                txtLog.AppendText(
                    "✓ PDF guardado correctamente.\r\n");

                txtLog.AppendText(
                    $"✓ Archivo: {nombrePdf}\r\n");
            }
            else
            {
                txtLog.AppendText(
                    $"✗ {pdf.UltimoMensaje}\r\n");
            }


            await sii.CerrarPopup(
                paginaPdf);

            txtLog.AppendText(
                "✓ Popup PDF cerrado.\r\n");


            await sii.CerrarSesion(
                pagina);

            txtLog.AppendText(
                "✓ Sesión SII cerrada.\r\n");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
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
            Models.Contribuyente contribuyente =
                new Models.Contribuyente
                {
                    Rut = "76869422-2",
                    Clave = "Patricio2026@",
                    Periodo = "05/2025"
                };


            await ProcesarContribuyente(
                contribuyente);
        }

    }
}
