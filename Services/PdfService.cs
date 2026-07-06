using System.IO;
using Microsoft.Playwright;

namespace F29Downloader.Services
{
    internal class PdfService
    {
        public async Task<bool> GuardarPdf(
            IBrowserContext contexto,
            IPage paginaPdf,
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

                // Volver a solicitar el PDF utilizando
                // la misma sesión autenticada
                IResponse? respuesta =
                    await paginaPdf.GotoAsync(
                        paginaPdf.Url,
                        new PageGotoOptions
                        {
                            WaitUntil = WaitUntilState.NetworkIdle
                        });

                if (respuesta == null)
                {
                    MessageBox.Show(
                        "No se obtuvo respuesta del servidor.");

                    return false;
                }

                // Obtener el contenido del PDF
                byte[] contenido =
                    await respuesta.BodyAsync();

                // Guardar el archivo
                await File.WriteAllBytesAsync(
                    rutaDestino,
                    contenido);

                MessageBox.Show(
                    $"PDF guardado correctamente.\n\n{rutaDestino}");

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

                return false;
            }
        }
    }
}