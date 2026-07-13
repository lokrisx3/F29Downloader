using F29Downloader.Models;
using Microsoft.Playwright;

namespace F29Downloader.Services
{
    public class DownloadService
    {
        public async Task ProcesarContribuyente(
            Contribuyente contribuyente,
            bool headless,
            Action<string> escribirLog)
        {
            SiiService sii =
                new SiiService();

            IPage pagina =
                await sii.AbrirLogin(headless);

            LoginService login =
                new LoginService();

            escribirLog(
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
                escribirLog(
                    "✗ Error al iniciar sesión.\r\n");

                return;
            }


            escribirLog(
                "✓ Inicio de sesión exitoso.\r\n");


            await sii.IrAConsultaF29(pagina);

            escribirLog(
                "✓ Consulta F29 abierta.\r\n");


            await sii.SeleccionarFormulario29(pagina);

            escribirLog(
                "✓ Formulario 29 seleccionado.\r\n");


            await sii.SeleccionarMes(
                pagina,
                "Mayo");

            escribirLog(
                "✓ Mes seleccionado.\r\n");


            await sii.BuscarDatosIngresados(pagina);

            escribirLog(
                "✓ Datos ingresados consultados.\r\n");


            await sii.AbrirFormulario(pagina);

            escribirLog(
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
                escribirLog(
                    "✓ PDF guardado correctamente.\r\n");

                escribirLog(
                    $"✓ Archivo: {nombrePdf}\r\n");
            }
            else
            {
                escribirLog(
                    $"✗ {pdf.UltimoMensaje}\r\n");
            }


            await sii.CerrarPopup(
                paginaPdf);

            escribirLog(
                "✓ Popup PDF cerrado.\r\n");


            await sii.CerrarSesion(
                pagina);

            escribirLog(
                "✓ Sesión SII cerrada.\r\n");
        }
    }
}