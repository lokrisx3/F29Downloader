using Microsoft.Playwright;

namespace F29Downloader.Services
{
    public class BrowserService
    {
        private IPlaywright? playwright;

        private IBrowser? browser;

        private IBrowserContext? context;

        public async Task<IPage> CrearPagina(bool headless)
        {
            playwright =
                await Playwright.CreateAsync();

            browser =
                await playwright.Chromium.LaunchAsync(
                    new BrowserTypeLaunchOptions
                    {
                        Headless = headless
                    });

            // Crear un contexto propio
            context =
                await browser.NewContextAsync();

            IPage pagina =
                await context.NewPageAsync();

            await pagina.RouteAsync(
                "**/*",
                async ruta =>
                {
                    string tipo =
                        ruta.Request.ResourceType;

                    if (
                        tipo == "image" ||
                        tipo == "media" ||
                        tipo == "font")
                    {
                        await ruta.AbortAsync();
                    }
                    else
                    {
                        await ruta.ContinueAsync();
                    }
                });

            return pagina;
        }

        public IBrowserContext ObtenerContexto()
        {
            if (context == null)
            {
                throw new InvalidOperationException(
                    "El contexto del navegador aún no ha sido creado.");
            }

            return context;
        }

        public async Task Cerrar()
        {
            if (context != null)
            {
                await context.CloseAsync();
            }

            if (browser != null)
            {
                await browser.CloseAsync();
            }

            playwright?.Dispose();
        }
    }
}