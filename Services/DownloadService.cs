using F29Downloader.Models;
using Microsoft.Playwright;

namespace F29Downloader.Services
{
    public class DownloadService
    {
        private SiiService? siiActual;
        private bool cancelado = false;
        public async Task ForzarCierre()
        {
            cancelado = true;

            if (siiActual != null)
            {
                await siiActual.ForzarCierreNavegador();
            }
        }
        public void IniciarProceso()
        {
            cancelado = false;
        }

        public bool ProcesoCancelado()
        {
            return cancelado;
        }

        private void CambiarEstado(
            DatabaseService db,
            Contribuyente contribuyente,
            string estado,
            string mensaje,
            Action actualizarGrilla)
        {
            contribuyente.Estado = estado;

            contribuyente.Mensaje = mensaje;

            contribuyente.FechaProceso =
                DateTime.Now.ToString(
                    "dd/MM/yyyy HH:mm:ss");

            db.ActualizarContribuyente(
                contribuyente);

            actualizarGrilla();
        }

        private void VerificarCancelacion(
            Action<string> escribirLog)
        {
            if (cancelado)
            {
                escribirLog(
                    "⚠ Proceso cancelado por usuario.\r\n");

                throw new OperationCanceledException();
            }
        }

        private string ObtenerNombreMes(
            string periodo)
        {
            string numeroMes =
                periodo.Split('/')[0];

            return numeroMes switch
            {
                "01" => "Enero",
                "02" => "Febrero",
                "03" => "Marzo",
                "04" => "Abril",
                "05" => "Mayo",
                "06" => "Junio",
                "07" => "Julio",
                "08" => "Agosto",
                "09" => "Septiembre",
                "10" => "Octubre",
                "11" => "Noviembre",
                "12" => "Diciembre",
                _ => throw new Exception(
                    $"Mes inválido en periodo: {periodo}")
            };
        }
        public async Task ProcesarContribuyente(
    Contribuyente contribuyente,
    Action<string> escribirLog,
    Action actualizarGrilla)
        {
            siiActual =
                new SiiService();

            SiiService sii =
                siiActual;

            DatabaseService db =
                new DatabaseService();

            IPage? pagina = null;

            if (cancelado)
            {
                escribirLog(
                    "⚠ Proceso cancelado por usuario.\r\n");

                return;
            }

            try
            {
                contribuyente.Intentos++;

                CambiarEstado(
                    db,
                    contribuyente,
                    "Conectando",
                    "",
                    actualizarGrilla);

                pagina =
                    await sii.AbrirLogin();

                LoginService login =
                    new LoginService();

                escribirLog(
                    "\r\n" +
                    "--------------------------------------------------\r\n" +
                    "Procesando contribuyente\r\n" +
                    $"RUT     : {contribuyente.Rut}\r\n" +
                    $"Periodo : {contribuyente.Periodo}\r\n" +
                    "--------------------------------------------------\r\n");

                string mes =
                    ObtenerNombreMes(
                        contribuyente.Periodo);

                bool resultado =
                    await login.IniciarSesion(
                        pagina,
                        contribuyente);

                if (!resultado)
                {
                    CambiarEstado(
                        db,
                        contribuyente,
                        "Error",
                        "Error al iniciar sesión",
                        actualizarGrilla);

                    escribirLog(
                        "✗ Error al iniciar sesión.\r\n");

                    return;
                }

                CambiarEstado(
                    db,
                    contribuyente,
                    "Autenticado",
                    "Inicio de sesión exitoso",
                    actualizarGrilla);

                escribirLog(
                    "✓ Inicio de sesión exitoso.\r\n");

                VerificarCancelacion(escribirLog);

                escribirLog(
                    "⏳ Abriendo consulta F29...\r\n");

                await sii.IrAConsultaF29(pagina);

                CambiarEstado(
                    db,
                    contribuyente,
                    "Consulta F29",
                    "Abriendo Consulta F29",
                    actualizarGrilla);

                escribirLog(
                    "✓ Consulta F29 abierta.\r\n");

                VerificarCancelacion(escribirLog);

                escribirLog(
                    "⏳ Seleccionando Formulario 29...\r\n");

                await sii.SeleccionarFormulario29(pagina);

                CambiarEstado(
                    db,
                    contribuyente,
                    "Formulario 29",
                    "Formulario seleccionado",
                    actualizarGrilla);

                escribirLog(
                    "✓ Formulario 29 seleccionado.\r\n");

                VerificarCancelacion(escribirLog);

                escribirLog(
                    $"⏳ Seleccionando mes {mes}...\r\n");

                await sii.SeleccionarMes(
                    pagina,
                    mes);

                CambiarEstado(
                db, 
                contribuyente,
                $"Mes {mes}",
                $"Período {contribuyente.Periodo}",
                actualizarGrilla);

                escribirLog(
                    $"✓ Mes {mes} seleccionado.\r\n");

                VerificarCancelacion(escribirLog);

                escribirLog(
                    "⏳ Buscando datos ingresados...\r\n");

                await sii.BuscarDatosIngresados(pagina);

                CambiarEstado(
                    db,
                    contribuyente,
                    "Buscando datos",
                    "Consultando declaraciones",
                    actualizarGrilla);

                escribirLog(
                    "✓ Datos encontrados.\r\n");

                escribirLog(
                    "⏳ Abriendo formulario...\r\n");

                await sii.AbrirFormulario(pagina);

                CambiarEstado(
                    db,
                    contribuyente,
                    "Formulario abierto",
                    "Esperando generación del PDF",
                    actualizarGrilla);

                escribirLog(
                    "⏳ Abriendo formulario compacto...\r\n");

                Task<IPage> popupTask =
                    pagina.WaitForPopupAsync();

                await sii.SeleccionarFormularioCompacto(
                    pagina);

                escribirLog(
                    "✓ Formulario compacto solicitado.\r\n");

                escribirLog(
                    "⏳ Esperando generación del PDF...\r\n");

                IPage paginaPdf =
                    await sii.AbrirPdf(
                        popupTask);

                escribirLog(
                    "✓ PDF abierto.\r\n");

                

                escribirLog(
                    "⏳ Descargando PDF...\r\n");

                string urlPdf =
                    paginaPdf.Url;

                IBrowserContext contexto =
                    sii.ObtenerContexto();

                PdfService pdf =
                    new PdfService();

                string nombrePdf =
                    ConfigService.ObtenerNombrePdf(
                        contribuyente);

                CambiarEstado(
                    db,
                    contribuyente,
                    "Descargando PDF",
                    nombrePdf,
                    actualizarGrilla);

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
                    contribuyente.RutaPdf =
                        rutaPdf;

                    CambiarEstado(
                        db,
                        contribuyente,
                        "Finalizado",
                        "",
                        actualizarGrilla);

                    escribirLog(
                        "✓ PDF guardado correctamente.\r\n");

                    escribirLog(
                        $"✓ Archivo: {nombrePdf}\r\n");
                }
                else
                {
                    CambiarEstado(
                        db,
                        contribuyente,
                        "Error",
                        pdf.UltimoMensaje,
                        actualizarGrilla);

                    escribirLog(
                        $"✗ {pdf.UltimoMensaje}\r\n");
                }

                escribirLog(
                    "⏳ Cerrando ventana del PDF...\r\n");

                await sii.CerrarPopup(
                    paginaPdf);

                escribirLog(
                    "✓ Popup PDF cerrado.\r\n");
            }
            catch (OperationCanceledException)
            {
                CambiarEstado(
                    db,
                    contribuyente,
                    "Cancelado",
                    "Cancelado",
                    actualizarGrilla);

                escribirLog(
                    "⚠ Proceso cancelado por el usuario.\r\n");
            }
            catch (Exception ex)
            {
                CambiarEstado(
                    db,
                    contribuyente,
                    "Error",
                    ex.Message,
                    actualizarGrilla);

                escribirLog(
                    $"✗ Error inesperado: {ex.Message}\r\n");
            }
            finally
            {
                try
                {
                    if (pagina != null)
                    {
                        escribirLog(
                            "⏳ Cerrando sesión SII...\r\n");

                        await sii.CerrarSesion(pagina);

                        escribirLog(
                            "✓ Sesión SII cerrada.\r\n");
                    }
                }
                catch (Exception ex)
                {
                    escribirLog(
                        $"⚠ Error cerrando sesión: {ex.Message}\r\n");
                }

                try
                {
                    escribirLog(
                        "⏳ Cerrando navegador...\r\n");

                    await sii.CerrarNavegador();

                    escribirLog(
                        "✓ Navegador cerrado.\r\n");
                }
                catch (Exception ex)
                {
                    escribirLog(
                        $"⚠ Error cerrando navegador: {ex.Message}\r\n");
                }
            }
        }

        public async Task IniciarDescarga(
            List<Contribuyente> lista,
            int segundosEspera,
            Action<string> escribirLog,
            Action actualizarGrilla)
        {
            IniciarProceso();

            escribirLog(
                "\r\n===== Inicio del proceso =====\r\n");

            for (int i = 0; i < lista.Count; i++)
            {
                await ProcesarContribuyente(
                    lista[i],
                    escribirLog,
                    actualizarGrilla);

                if (ProcesoCancelado())
                {
                    escribirLog(
                        "\r\n⚠ Proceso cancelado por usuario.\r\n");

                    break;
                }

                if (i < lista.Count - 1 &&
                    segundosEspera > 0)
                {
                    escribirLog(
                        $"Esperando {segundosEspera} segundos...\r\n");

                    await Task.Delay(
                        TimeSpan.FromSeconds(segundosEspera));
                }
            }

            escribirLog(
                "\r\n===== Proceso finalizado =====\r\n");
        }
    }
}