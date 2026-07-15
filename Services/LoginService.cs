
using F29Downloader.Models;
using Microsoft.Playwright;

namespace F29Downloader.Services
{
    public class LoginService
    {
        public async Task<bool> IniciarSesion(
    IPage pagina,
    Contribuyente contribuyente)
        {
            await pagina.FillAsync(
                "#rutcntr",
                contribuyente.Rut);

            await pagina.FillAsync(
                "#clave",
                contribuyente.Clave);

            await pagina.ClickAsync(
                "#bt_ingresar");


            try
            {
                await pagina.WaitForTimeoutAsync(3000);


                // Si el botón ingresar sigue visible,
                // el login no fue realizado.
                bool sigueEnLogin =
                    await pagina
                    .Locator("#bt_ingresar")
                    .IsVisibleAsync();


                if (sigueEnLogin)
                {
                    return false;
                }


                // Validación adicional:
                // verificar que realmente salió del login.

                string url =
                    pagina.Url;


                if (url.Contains("AUT2000"))
                {
                    return false;
                }


                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}