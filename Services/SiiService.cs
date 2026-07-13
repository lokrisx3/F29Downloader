using Microsoft.Playwright;

namespace F29Downloader.Services
{
    public class SiiService
    {
        private readonly BrowserService browserService;

        private const string URL_LOGIN =
            "https://zeusr.sii.cl//AUT2000/InicioAutenticacion/IngresoRutClave.html?https://misiir.sii.cl/cgi_misii/siihome.cgi";

        private const string URL_LOGOUT =
                "https://zeusr.sii.cl/cgi_AUT2000/autTermino.cgi";
        public SiiService()
        {
            browserService = new BrowserService();
        }

        public async Task<IPage> AbrirLogin(bool headless)
        {
            IPage pagina =
                await browserService.CrearPagina(headless);

            await pagina.GotoAsync(
                URL_LOGIN,
                new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle
                });

            return pagina;
        }

        public async Task CerrarSesion(IPage pagina)
        {
            if (pagina.IsClosed)
            {
                return;
            }

            await pagina.GotoAsync(
                URL_LOGOUT,
                new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle,
                    Timeout = 30000
                });

            await pagina.WaitForTimeoutAsync(1000);
        }
        public async Task IrAConsultaF29(IPage pagina)
        {
            await pagina.GotoAsync(
                "https://www4.sii.cl/rfiInternet/consulta/index.html#rfiSelFormularioPeriodo",
                new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded
                });
            await pagina.WaitForTimeoutAsync(5000);
        }
        public async Task SeleccionarFormulario29(IPage pagina)
        {
            await pagina.SelectOptionAsync(
    "select.gwt-ListBox",
    "0");

            await pagina.WaitForTimeoutAsync(5000);
        }


        public async Task SeleccionarMes(
         IPage pagina,
         string mes)
        {
            await pagina.EvaluateAsync(
                @"(mes) => {

            const combo =
                document.querySelectorAll('#item-form select')[2];

            const mapaMeses = {
                'Enero': 0,
                'Febrero': 1,
                'Marzo': 2,
                'Abril': 3,
                'Mayo': 4,
                'Junio': 5,
                'Julio': 6,
                'Agosto': 7,
                'Septiembre': 8,
                'Octubre': 9,
                'Noviembre': 10,
                'Diciembre': 11
            };

            if (mapaMeses[mes] === undefined)
                throw new Error('Mes inválido: ' + mes);

            combo.value = mapaMeses[mes];

            combo.dispatchEvent(
                new Event('change', { bubbles: true }));

        }",
                mes);
            await pagina.WaitForTimeoutAsync(500);
        }
        public async Task BuscarDatosIngresados(IPage pagina)
        {
            var boton = pagina.Locator("button", new()
            {
                HasTextString = "Buscar Datos Ingresados"
            });

            await boton.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await boton.ClickAsync();
            await pagina.WaitForTimeoutAsync(500);
        }

        public async Task AbrirFormulario(IPage pagina)
        {
            var link = pagina.Locator("a[href='#rfiSelFormularioPeriodo']").First;

            await link.WaitForAsync(new()
            {
                State = WaitForSelectorState.Visible
            });

            await link.ClickAsync();
            await pagina.WaitForTimeoutAsync(500);
        }

        public async Task SeleccionarFormularioCompacto(IPage pagina)
        {
            await pagina
                .Locator("button", new()
                {
                    HasTextString = "Formulario Compacto"
                })
                .ClickAsync();

            await pagina.WaitForTimeoutAsync(500);
        }

        public async Task<IPage> AbrirPdf(
    Task<IPage> popupTask)
        {
            IPage paginaPdf =
                await popupTask;

            await paginaPdf.WaitForLoadStateAsync(
                LoadState.Load);

            return paginaPdf;
        }

        public IBrowserContext ObtenerContexto()
        {
            return browserService.ObtenerContexto();
        }

        public async Task CerrarPopup(
            IPage paginaPdf)
        {
            if (!paginaPdf.IsClosed)
            {
                await paginaPdf.CloseAsync();
            }
        }

        public async Task CerrarNavegador()
        {
            await browserService.Cerrar();
        }

        public async Task ForzarCierreNavegador()
        {
            await browserService.ForzarCierre();
        }

    }
}