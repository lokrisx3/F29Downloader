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

                await sii.IrAConsultaF29(pagina);

                await sii.SeleccionarFormulario29(pagina);

                await sii.SeleccionarMes(
                    pagina,
                    "Mayo");

                await sii.BuscarDatosIngresados(pagina);

                await sii.AbrirFormulario(pagina);

                // Esperar el popup antes del clic
                Task<IPage> popupTask =
                    pagina.WaitForPopupAsync();

                // Abrir Formulario Compacto
                await sii.SeleccionarFormularioCompacto(
                    pagina);

                // Obtener la pestaña del PDF
                IPage paginaPdf =
                    await sii.AbrirPdf(
                        popupTask);
                IBrowserContext contexto =
                        sii.ObtenerContexto();


                // Servicio encargado del PDF
                PdfService pdf =
                    new PdfService();

                bool guardado =
                await pdf.GuardarPdf(
                    contexto,
                    paginaPdf,
                    @"C:\Temp\F29.pdf");

                if (guardado)
                {
                    txtLog.AppendText(
                        "PDF guardado correctamente.\r\n");
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
    }
}