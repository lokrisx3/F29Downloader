using F29Downloader.Models;
using Microsoft.Playwright;

namespace F29Downloader.Services
{
    public class DownloadService
    {
        private SiiService? siiActual;
        private bool cancelado = false;
        private const int MaximoIntentosLogin = 2;
        private HashSet<string> rutOmitidos =
    new HashSet<string>();
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

            rutOmitidos.Clear();
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
        private bool FueOmitidoPorLogin(
    List<Contribuyente> lista,
    Contribuyente actual)
        {
            return lista.Any(x =>
                x.Rut == actual.Rut &&
                x.Estado == "Omitido" &&
                x.Mensaje.Contains(
                    "login",
                    StringComparison.OrdinalIgnoreCase));
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
    Action actualizarGrilla,
    Func<Contribuyente, bool> consultarReintento)
        {
            DatabaseService db =
                new DatabaseService();

            siiActual =
                new SiiService();

            SiiService sii =
                siiActual;

            IPage? pagina = null;

            if (cancelado)
            {
                escribirLog(
                    "⚠ Proceso cancelado por usuario.\r\n");

                return;
            }

            try
            {
                //====================================================
                // Verificar si el PDF ya existe
                //====================================================

                string rutaPdfExistente;

                if (ExistePdf(
                    contribuyente,
                    out rutaPdfExistente))
                {
                    contribuyente.RutaPdf =
                        rutaPdfExistente;

                    CambiarEstado(
                        db,
                        contribuyente,
                        "Finalizado",
                        "PDF ya existe",
                        actualizarGrilla);

                    escribirLog(
                        "\r\n" +
                        "--------------------------------------------------\r\n" +
                        "Procesando contribuyente\r\n" +
                        $"RUT     : {contribuyente.Rut}\r\n" +
                        $"Periodo : {contribuyente.Periodo}\r\n" +
                        "--------------------------------------------------\r\n");

                    escribirLog(
                        "✓ El PDF ya existe.\r\n");

                    escribirLog(
                        "✓ Se omite la descarga.\r\n");

                    return;
                }

                contribuyente.Intentos++;

                CambiarEstado(
                    db,
                    contribuyente,
                    "Conectando",
                    "",
                    actualizarGrilla);

                escribirLog(
                    "\r\n" +
                    "--------------------------------------------------\r\n" +
                    "Procesando contribuyente\r\n" +
                    $"RUT     : {contribuyente.Rut}\r\n" +
                    $"Periodo : {contribuyente.Periodo}\r\n" +
                    "--------------------------------------------------\r\n");

                pagina =
                    await RealizarLoginConReintentos(
                        sii,
                        contribuyente,
                        db,
                        escribirLog,
                        actualizarGrilla,
                        consultarReintento);

                if (pagina == null)
                {
                    escribirLog(
                        "⚠ Contribuyente omitido.\r\n");

                    return;
                }

                string mes =
                    ObtenerNombreMes(
                        contribuyente.Periodo);

                VerificarCancelacion(escribirLog);

                escribirLog(
                    "⏳ Abriendo consulta F29...\r\n");

                await sii.IrAConsultaF29(
                    pagina);

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

                await sii.SeleccionarFormulario29(
                    pagina);

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

                await sii.BuscarDatosIngresados(
                    pagina);

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

                await sii.AbrirFormulario(
                    pagina);

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
                    if (pagina != null &&
                        !pagina.IsClosed)
                    {
                        escribirLog(
                            "⏳ Cerrando sesión SII...\r\n");

                        await sii.CerrarSesion(
                            pagina);

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
    Action actualizarGrilla,
    Action<string> actualizarEstado,
    Action<int, int> actualizarProgreso,
    Func<Contribuyente, bool> consultarReintento)
        {
            IniciarProceso();

            DatabaseService db =
                new DatabaseService();


            escribirLog(
                "\r\n===== Inicio del proceso =====\r\n");


            int total =
                lista.Count;


            for (int i = 0; i < total; i++)
            {
                int actual =
                    i + 1;


                int porcentaje =
                    (actual * 100) / total;


                Contribuyente contribuyente =
                    lista[i];


                actualizarEstado(
                    $"Procesando {actual} de {total} ({porcentaje}%)");


                actualizarProgreso(
                    actual,
                    total);


                //====================================================
                // Verificar si el RUT ya fue omitido por error de login
                //====================================================

                bool omitidoAnteriormente =
                    lista.Any(x =>
                        x.Rut == contribuyente.Rut &&
                        x != contribuyente &&
                        x.Estado == "Omitido");


                if (omitidoAnteriormente)
                {
                    contribuyente.Estado =
                        "Omitido";


                    contribuyente.Mensaje =
                        "Omitido por fallo de login anterior";


                    db.ActualizarContribuyente(
                        contribuyente);


                    actualizarGrilla();


                    escribirLog(
                        $"⚠ {contribuyente.Rut} omitido. " +
                        "Existe un fallo de login anterior.\r\n");


                    continue;
                }


                await ProcesarContribuyente(
                    contribuyente,
                    escribirLog,
                    actualizarGrilla,
                    consultarReintento);

                if (contribuyente.Estado == "Omitido")
                {
                    rutOmitidos.Add(
                        contribuyente.Rut);
                }


                if (ProcesoCancelado())
                {
                    actualizarEstado(
                        $"Proceso cancelado ({actual}/{total})");


                    actualizarProgreso(
                        actual,
                        total);


                    escribirLog(
                        "\r\n⚠ Proceso cancelado por usuario.\r\n");


                    break;
                }



                if (i < total - 1 &&
                    segundosEspera > 0)
                {
                    actualizarEstado(
                        $"Esperando {segundosEspera} segundos...");


                    escribirLog(
                        $"Esperando {segundosEspera} segundos...\r\n");


                    await Task.Delay(
                        TimeSpan.FromSeconds(
                            segundosEspera));
                }
            }


            actualizarEstado(
                $"Proceso finalizado ({total}/{total})");


            actualizarProgreso(
                total,
                total);


            escribirLog(
                "\r\nProceso finalizado.\r\n");
        }

        private bool ExistePdf(
    Contribuyente contribuyente,
    out string rutaPdf)
        {
            string nombrePdf =
                ConfigService.ObtenerNombrePdf(
                    contribuyente);

            string carpetaContribuyente =
                Path.Combine(
                    ConfigService.DownloadsPath,
                    contribuyente.Rut);

            rutaPdf =
                Path.Combine(
                    carpetaContribuyente,
                    nombrePdf);

            return File.Exists(rutaPdf);
        }

        private async Task<IPage?> RealizarLoginConReintentos(
     SiiService sii,
     Contribuyente contribuyente,
     DatabaseService db,
     Action<string> escribirLog,
     Action actualizarGrilla,
     Func<Contribuyente, bool> consultarReintento)
        {
            while (!cancelado)
            {
                if (cancelado)
                {
                    return null;
                }


                IPage pagina =
                    await sii.AbrirLogin();


                LoginService login =
                    new LoginService();


                bool loginCorrecto = false;


                for (int intento = 1;
                     intento <= MaximoIntentosLogin;
                     intento++)
                {
                    if (cancelado)
                    {
                        if (!pagina.IsClosed)
                        {
                            await pagina.CloseAsync();
                        }

                        return null;
                    }


                    escribirLog(
                        $"⏳ Intento de inicio de sesión ({intento}/{MaximoIntentosLogin})...\r\n");


                    loginCorrecto =
                        await login.IniciarSesion(
                            pagina,
                            contribuyente);



                    if (loginCorrecto)
                    {
                        CambiarEstado(
                            db,
                            contribuyente,
                            "Autenticado",
                            "Inicio de sesión exitoso",
                            actualizarGrilla);


                        escribirLog(
                            "✓ Inicio de sesión exitoso.\r\n");


                        return pagina;
                    }



                    escribirLog(
                        $"⚠ Falló el intento {intento}.\r\n");



                    if (!pagina.IsClosed)
                    {
                        await pagina.CloseAsync();
                    }



                    if (intento < MaximoIntentosLogin)
                    {
                        escribirLog(
                            "⏳ Reintentando automáticamente...\r\n");


                        await Task.Delay(2000);



                        if (cancelado)
                        {
                            return null;
                        }



                        pagina =
                            await sii.AbrirLogin();


                        login =
                            new LoginService();
                    }
                }



                CambiarEstado(
                    db,
                    contribuyente,
                    "Error",
                    "No fue posible iniciar sesión",
                    actualizarGrilla);



                escribirLog(
                    "✗ No fue posible iniciar sesión después de los intentos automáticos.\r\n");



                bool volverIntentar =
                    consultarReintento(
                        contribuyente);



                if (!volverIntentar)
                {
                    CambiarEstado(
                        db,
                        contribuyente,
                        "Omitido",
                        "Omitido por usuario",
                        actualizarGrilla);



                    escribirLog(
                        "⚠ Contribuyente omitido por usuario.\r\n");



                    return null;
                }



                escribirLog(
                    "⏳ Nuevo intento solicitado por el usuario...\r\n");
            }



            return null;
        }
    }
}