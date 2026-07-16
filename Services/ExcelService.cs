
using ClosedXML.Excel;
using F29Downloader.Models;

namespace F29Downloader.Services
{
    public class ExcelService
    {
        public List<string> Errores { get; private set; } =
    new List<string>();
        public List<Contribuyente> LeerExcel(
    string ruta)
        {
            Errores.Clear();

            List<Contribuyente> lista =
                new List<Contribuyente>();

            try
            {
                using XLWorkbook libro =
                    new XLWorkbook(ruta);

                IXLWorksheet hoja =
                    libro.Worksheet(1);

                if (!hoja.RowsUsed().Any())
                {
                    Errores.Add(
                        "El archivo Excel está vacío.");

                    return lista;
                }

                if (hoja.Row(1).CellsUsed().Count() < 3)
                {
                    Errores.Add(
                        "El archivo no contiene las columnas esperadas (Rut, Clave, Periodo).");

                    return lista;
                }

                int filaActual = 2;

                foreach (IXLRow fila in hoja.RowsUsed().Skip(1))
                {
                    string rut =
                        fila.Cell(1).GetString().Trim();

                    string clave =
                        fila.Cell(2).GetString().Trim();

                    string periodo =
                        fila.Cell(3).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(rut))
                    {
                        Errores.Add(
                            $"Fila {filaActual}: RUT vacío.");
                    }
                    else if (!ValidarRut(rut))
                    {
                        Errores.Add(
                            $"Fila {filaActual}: Formato de RUT inválido ({rut}).");
                    }

                    if (string.IsNullOrWhiteSpace(clave))
                    {
                        Errores.Add(
                            $"Fila {filaActual}: Clave vacía.");
                    }

                    if (string.IsNullOrWhiteSpace(periodo))
                    {
                        Errores.Add(
                            $"Fila {filaActual}: Período vacío.");
                    }
                    else if (!ValidarPeriodo(periodo))
                    {
                        Errores.Add(
                            $"Fila {filaActual}: Formato de período inválido ({periodo}).");
                    }

                    lista.Add(
                        new Contribuyente
                        {
                            Rut = rut,
                            Clave = clave,
                            Periodo = periodo
                        });

                    filaActual++;
                }
            }
            catch (IOException)
            {
                Errores.Add(
                    "No fue posible abrir el archivo Excel porque está siendo utilizado por otro programa. Cierre el archivo y vuelva a intentarlo.");
            }
            catch (Exception ex)
            {
                Errores.Add(
                    $"Error al leer el archivo Excel: {ex.Message}");
            }

            return lista;
        }

        private bool ValidarRut(
    string rut)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(
                rut,
                @"^\d{7,8}-[\dkK]$");
        }

        private bool ValidarPeriodo(
    string periodo)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(
                periodo,
                @"^(0[1-9]|1[0-2])\/\d{4}$");
        }
    }
}