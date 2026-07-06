using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            // Esperar unos segundos a que responda el SII
            await pagina.WaitForLoadStateAsync(
                LoadState.NetworkIdle);

            // Si sigue existiendo el botón ingresar,
            // significa que el login falló.
            bool sigueEnLogin =
                await pagina.Locator("#bt_ingresar").IsVisibleAsync();

            if (sigueEnLogin)
            {
                return false;
            }

            return true;
        }
    }
}