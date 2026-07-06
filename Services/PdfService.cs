using Microsoft.Playwright;
using System.IO;
using System.Text;

namespace F29Downloader.Services
{
    internal class PdfService
    {
        public async Task VerificarPdf(IPage paginaPdf)
        {
            IResponse? respuesta =
                await paginaPdf.WaitForResponseAsync(r =>
                    r.Url == paginaPdf.Url &&
                    r.Status == 200);

            if (respuesta == null)
            {
                throw new Exception("No se obtuvo la respuesta del PDF.");
            }

            byte[] bytes =
                await respuesta.BodyAsync();

            string encabezado =
                Encoding.ASCII.GetString(
                    bytes.Take(10).ToArray());

            MessageBox.Show(
                $"Primeros bytes:\n\n{encabezado}\n\n" +
                $"Tamaño: {bytes.Length} bytes");
        }
        public async Task<bool> GuardarPdf(
            IBrowserContext contexto,
            IPage paginaPdf,
            string rutaDestino)
        {
            return await GuardarPdfDesdeUrl(
                contexto,
                paginaPdf.Url,
                rutaDestino);
        }

        public async Task<bool> GuardarPdfDesdeUrl(
            IBrowserContext contexto,
            string urlPdf,
            string rutaDestino)
        {
            try
            {
                // Crear la carpeta si no existe
                string? carpeta =
                    Path.GetDirectoryName(rutaDestino);

                if (!string.IsNullOrWhiteSpace(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                IPage paginaPdf =
                    await contexto.NewPageAsync();

                try
                {
                    TaskCompletionSource<byte[]> pdfDetectado =
                        new TaskCompletionSource<byte[]>(
                            TaskCreationOptions.RunContinuationsAsynchronously);

                    paginaPdf.Response += async (_, respuestaRed) =>
                    {
                        await IntentarCapturarPdfDesdeRespuesta(
                            respuestaRed,
                            pdfDetectado);
                    };

                    // Solicitar el PDF con el mismo contexto autenticado
                    // para reutilizar cookies/sesión del login SII.
                    IResponse? respuesta =
                        await paginaPdf.GotoAsync(
                            urlPdf,
                            new PageGotoOptions
                            {
                                WaitUntil = WaitUntilState.DOMContentLoaded,
                                Timeout = 60000
                            });

                    Task tareaPdfDetectado =
                        await Task.WhenAny(
                            pdfDetectado.Task,
                            Task.Delay(15000));

                    if (tareaPdfDetectado == pdfDetectado.Task)
                    {
                        await GuardarContenidoPdf(
                            await pdfDetectado.Task,
                            rutaDestino);

                        return true;
                    }

                    if (respuesta == null)
                    {
                        MessageBox.Show(
                            "No se obtuvo respuesta del servidor.");

                        return false;
                    }

                    byte[] contenido =
                        await respuesta.BodyAsync();

                    string encabezado =
                        ObtenerEncabezado(contenido);

                    if (respuesta.Status != 200 ||
                        !EsPdf(contenido))
                    {
                        await paginaPdf.CloseAsync();

                        return await GuardarPdfDesdeVisor(
                            contexto,
                            urlPdf,
                            rutaDestino,
                            respuesta.Status,
                            encabezado);
                    }

                    await GuardarContenidoPdf(
                        contenido,
                        rutaDestino);

                    return true;
                }
                finally
                {
                    if (!paginaPdf.IsClosed)
                    {
                        await paginaPdf.CloseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

                return false;
            }
        }

        private async Task IntentarCapturarPdfDesdeRespuesta(
            IResponse respuesta,
            TaskCompletionSource<byte[]> pdfDetectado)
        {
            if (pdfDetectado.Task.IsCompleted ||
                respuesta.Status != 200)
            {
                return;
            }

            try
            {
                byte[] contenido =
                    await respuesta.BodyAsync();

                if (EsPdf(contenido))
                {
                    pdfDetectado.TrySetResult(contenido);
                }
            }
            catch
            {
                // Algunas respuestas del visor no exponen body; se ignoran.
            }
        }

        private async Task GuardarContenidoPdf(
            byte[] contenido,
            string rutaDestino)
        {
            await File.WriteAllBytesAsync(
                rutaDestino,
                contenido);

            MessageBox.Show(
                $"PDF guardado correctamente.\n\n{rutaDestino}");
        }

        private bool EsPdf(byte[] contenido)
        {
            return ObtenerEncabezado(contenido) == "%PDF-";
        }

        private string ObtenerEncabezado(byte[] contenido)
        {
            if (contenido.Length < 5)
            {
                return "";
            }

            return Encoding.ASCII.GetString(
                contenido.Take(5).ToArray());
        }

        private async Task<bool> GuardarPdfDesdeVisor(
            IBrowserContext contexto,
            string urlPdf,
            string rutaDestino,
            int statusRespuestaOriginal,
            string encabezadoRespuestaOriginal)
        {
            IPage paginaPdf =
                await contexto.NewPageAsync();

            try
            {
                await paginaPdf.GotoAsync(
                    urlPdf,
                    new PageGotoOptions
                    {
                        WaitUntil = WaitUntilState.DOMContentLoaded,
                        Timeout = 60000
                    });

                await paginaPdf.WaitForTimeoutAsync(2000);

                IDownload descarga =
                    await paginaPdf.RunAndWaitForDownloadAsync(
                        async () =>
                        {
                            bool botonPresionado =
                                await PresionarBotonDescargaVisorPdf(
                                    paginaPdf);

                            if (!botonPresionado)
                            {
                                await paginaPdf.Keyboard.PressAsync(
                                    "Control+S");
                            }
                        },
                        new PageRunAndWaitForDownloadOptions
                        {
                            Timeout = 30000
                        });

                await descarga.SaveAsAsync(rutaDestino);

                byte[] primerosBytes =
                    await File.ReadAllBytesAsync(rutaDestino);

                string encabezadoArchivo =
                    Encoding.ASCII.GetString(
                        primerosBytes.Take(5).ToArray());

                if (encabezadoArchivo != "%PDF-")
                {
                    MessageBox.Show(
                        "La descarga se guardó, pero no parece ser un PDF válido.\n\n" +
                        $"HTTP original: {statusRespuestaOriginal}\n" +
                        $"Primeros bytes respuesta original: {encabezadoRespuestaOriginal}\n" +
                        $"Primeros bytes archivo guardado: {encabezadoArchivo}");

                    return false;
                }

                MessageBox.Show(
                    $"PDF guardado correctamente.\n\n{rutaDestino}");

                return true;
            }
            catch (TimeoutException)
            {
                MessageBox.Show(
                    "No se detectó una descarga desde el visor PDF de Chromium.\n\n" +
                    $"HTTP original: {statusRespuestaOriginal}\n" +
                    $"Primeros bytes respuesta original: {encabezadoRespuestaOriginal}");

                return false;
            }
            finally
            {
                await paginaPdf.CloseAsync();
            }
        }

        private async Task<bool> PresionarBotonDescargaVisorPdf(
            IPage paginaPdf)
        {
            return await paginaPdf.EvaluateAsync<bool>(
                """
                () => {
                    const elementos = [];

                    const recorrer = (raiz) => {
                        if (!raiz) {
                            return;
                        }

                        for (const elemento of raiz.querySelectorAll('*')) {
                            elementos.push(elemento);

                            if (elemento.shadowRoot) {
                                recorrer(elemento.shadowRoot);
                            }
                        }
                    };

                    recorrer(document);

                    const boton = elementos.find((elemento) => {
                        const id = (elemento.id || '').toLowerCase();
                        const aria = (elemento.getAttribute('aria-label') || '').toLowerCase();
                        const title = (elemento.getAttribute('title') || '').toLowerCase();

                        return (
                            elemento instanceof HTMLElement &&
                            typeof elemento.click === 'function' &&
                            (
                                id.includes('download') ||
                                aria.includes('download') ||
                                aria.includes('descarg') ||
                                title.includes('download') ||
                                title.includes('descarg')
                            )
                        );
                    });

                    if (!boton) {
                        return false;
                    }

                    boton.click();
                    return true;
                }
                """);
        }
    }
}
