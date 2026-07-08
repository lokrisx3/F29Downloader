
using ClosedXML.Excel;
using F29Downloader.Models;

namespace F29Downloader.Services
{
    public class ExcelService
    {
        public List<Contribuyente> LeerExcel(string ruta)
        {
            List<Contribuyente> lista = new List<Contribuyente>();

            using (XLWorkbook libro = new XLWorkbook(ruta))
            {
                IXLWorksheet hoja = libro.Worksheet(1);

                foreach (IXLRow fila in hoja.RowsUsed().Skip(1))
                {
                    Contribuyente contribuyente = new Contribuyente();

                    contribuyente.Rut = fila.Cell(1).GetString();
                    contribuyente.Clave = fila.Cell(2).GetString();
                    contribuyente.Periodo = fila.Cell(3).GetString();

                    lista.Add(contribuyente);
                }
            }

            return lista;
        }
    }
}