using F29Downloader.Models;
using Microsoft.Playwright;
using System.Xml.Linq;

namespace F29Downloader.Services
{
    public sealed class FlujoXmlNoDisponibleException : Exception
    {
        public FlujoXmlNoDisponibleException(string mensaje, Exception? innerException = null)
            : base(mensaje, innerException) { }
    }

    public class FlujoXmlService
    {
        private readonly BrowserService browser = new();

        public Task ForzarCierreAsync() => browser.ForzarCierre();

        public async Task EjecutarAsync(Contribuyente contribuyente, Action<string> escribirLog)
        {
            XDocument documento = CargarFlujo();
            XElement[] pasos = documento.Root?
                .Element("Pasos")?
                .Elements("Paso")
                .OrderBy(p => (int?)p.Attribute("orden") ?? int.MaxValue)
                .ToArray()
                ?? throw new FlujoXmlNoDisponibleException("El XML no contiene pasos para ejecutar.");

            if (pasos.Length == 0)
                throw new FlujoXmlNoDisponibleException("El XML no contiene pasos para ejecutar.");

            Dictionary<string, string> variables = CrearVariables(contribuyente);
            IPage? pagina = null;
            EstadoEjecucion estado = new();

            try
            {
                escribirLog("⏳ Ejecutando flujo XML...\r\n");

                foreach (XElement paso in pasos)
                {
                    string tipo = Atributo(paso, "tipo");
                    string orden = Atributo(paso, "orden");

                    if (tipo == "navegar")
                    {
                        pagina ??= await browser.CrearPagina();
                        await pagina.GotoAsync(Resolver(Atributo(paso, "url"), variables), new PageGotoOptions
                        {
                            WaitUntil = ObtenerEstadoEspera(AtributoOpcional(paso, "esperarHasta"))
                        });
                    }
                    else
                    {
                        if (pagina == null)
                            throw new InvalidOperationException($"El paso {orden} requiere una página abierta.");

                        await EjecutarPasoAsync(paso, pagina, variables, estado);
                    }

                    escribirLog($"✓ Paso {orden}: {tipo}.\r\n");
                }

                escribirLog("✓ Flujo XML finalizado.\r\n");
            }
            finally
            {
                if (estado.Popup != null && !estado.Popup.IsClosed)
                    await estado.Popup.CloseAsync();

                await browser.Cerrar();
                escribirLog("✓ Navegador cerrado.\r\n");
            }
        }

        private async Task EjecutarPasoAsync(
            XElement paso,
            IPage pagina,
            Dictionary<string, string> variables,
            EstadoEjecucion estado)
        {
            string tipo = Atributo(paso, "tipo");
            string selector = Resolver(AtributoOpcional(paso, "selector"), variables);
            string texto = Resolver(AtributoOpcional(paso, "texto"), variables);

            switch (tipo)
            {
                case "rellenar":
                    await pagina.FillAsync(selector, Resolver(Atributo(paso, "valor"), variables));
                    break;
                case "click":
                    await ObtenerLocator(pagina, selector, texto).ClickAsync();
                    break;
                case "esperar":
                    await pagina.WaitForTimeoutAsync(int.Parse(Atributo(paso, "milisegundos")));
                    break;
                case "esperarVisible":
                    await ObtenerLocator(pagina, selector, texto).WaitForAsync(new() { State = WaitForSelectorState.Visible });
                    break;
                case "seleccionar":
                    await pagina.SelectOptionAsync(selector, Resolver(Atributo(paso, "valor"), variables));
                    break;
                case "script":
                    string codigo = paso.Element("Codigo")?.Value
                        ?? throw new InvalidOperationException("El paso script no contiene Código.");
                    await pagina.EvaluateAsync(Resolver(codigo, variables));
                    break;
                case "validar":
                    Validar(pagina, Resolver(Atributo(paso, "condicion"), variables), AtributoOpcional(paso, "mensajeError"));
                    break;
                case "esperarPopup":
                    estado.EsperaPopup = pagina.WaitForPopupAsync();
                    break;
                case "guardarPdfPopup":
                    if (estado.EsperaPopup == null)
                        throw new InvalidOperationException("Debe usar esperarPopup antes de guardar el PDF.");

                    estado.Popup = await estado.EsperaPopup;
                    await estado.Popup.WaitForLoadStateAsync(LoadState.Load);
                    string destino = ObtenerRutaDestino(Resolver(Atributo(paso, "destino"), variables));
                    PdfService pdf = new();
                    if (!await pdf.GuardarPdfDesdeUrl(browser.ObtenerContexto(), estado.Popup.Url, destino))
                        throw new InvalidOperationException(string.IsNullOrWhiteSpace(pdf.UltimoMensaje)
                            ? "No fue posible guardar el PDF."
                            : pdf.UltimoMensaje);
                    break;
                case "cerrarPopup":
                    if (estado.Popup != null && !estado.Popup.IsClosed)
                        await estado.Popup.CloseAsync();
                    estado.Popup = null;
                    break;
                default:
                    throw new InvalidOperationException($"Tipo de paso no soportado: {tipo}.");
            }
        }

        private static XDocument CargarFlujo()
        {
            string ruta = Path.Combine(AppContext.BaseDirectory, "Data", "Flujo.xml");
            if (!File.Exists(ruta))
                throw new FlujoXmlNoDisponibleException($"No se encontró el flujo XML: {ruta}");

            try { return XDocument.Load(ruta); }
            catch (Exception ex) when (ex is System.Xml.XmlException || ex is IOException)
            {
                throw new FlujoXmlNoDisponibleException("No fue posible leer Data\\Flujo.xml.", ex);
            }
        }

        private static Dictionary<string, string> CrearVariables(Contribuyente c) => new()
        {
            ["{rut}"] = c.Rut,
            ["{clave}"] = c.Clave,
            ["{periodo}"] = c.Periodo,
            ["{periodoArchivo}"] = c.Periodo.Replace("/", "_").Replace("-", "_"),
            ["{anio}"] = c.Periodo.Split('/').Last(),
            ["{mes}"] = ObtenerMes(c.Periodo)
        };

        private static string ObtenerMes(string periodo) => periodo.Split('/').First() switch
        {
            "01" => "Enero", "02" => "Febrero", "03" => "Marzo", "04" => "Abril",
            "05" => "Mayo", "06" => "Junio", "07" => "Julio", "08" => "Agosto",
            "09" => "Septiembre", "10" => "Octubre", "11" => "Noviembre", "12" => "Diciembre",
            _ => throw new InvalidOperationException($"Período inválido: {periodo}")
        };

        private static string Resolver(string valor, Dictionary<string, string> variables)
        {
            foreach ((string llave, string reemplazo) in variables)
                valor = valor.Replace(llave, reemplazo, StringComparison.Ordinal);
            return valor;
        }

        private static ILocator ObtenerLocator(IPage pagina, string selector, string texto) =>
            string.IsNullOrWhiteSpace(texto)
                ? pagina.Locator(selector).First
                : pagina.Locator(selector, new() { HasTextString = texto }).First;

        private static WaitUntilState ObtenerEstadoEspera(string valor) => valor switch
        {
            "NetworkIdle" => WaitUntilState.NetworkIdle,
            _ => WaitUntilState.DOMContentLoaded
        };

        private static void Validar(IPage pagina, string condicion, string mensaje)
        {
            if (condicion.StartsWith("urlNoContiene('") && condicion.EndsWith("')"))
            {
                string texto = condicion[15..^2];
                if (!pagina.Url.Contains(texto, StringComparison.OrdinalIgnoreCase)) return;
            }
            else
            {
                throw new InvalidOperationException($"Condición no soportada: {condicion}.");
            }

            throw new InvalidOperationException(string.IsNullOrWhiteSpace(mensaje) ? "La validación del flujo falló." : mensaje);
        }

        private static string Atributo(XElement paso, string nombre) =>
            AtributoOpcional(paso, nombre) ?? throw new InvalidOperationException($"Falta el atributo '{nombre}' en un paso XML.");

        private static string AtributoOpcional(XElement paso, string nombre) => paso.Attribute(nombre)?.Value ?? "";

        private static string ObtenerRutaDestino(string destino) =>
            Path.IsPathRooted(destino) ? destino : Path.Combine(ConfigService.BasePath, destino);

        private sealed class EstadoEjecucion
        {
            public Task<IPage>? EsperaPopup { get; set; }
            public IPage? Popup { get; set; }
        }
    }
}
