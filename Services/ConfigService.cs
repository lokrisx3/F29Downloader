

namespace F29Downloader.Services
{
    public static class ConfigService
    {
        public static string BasePath =>
            AppContext.BaseDirectory;


        public static string DataPath =>
            Path.Combine(BasePath, "Data");


        public static string DownloadsPath =>
            Path.Combine(BasePath, "Downloads");


        public static string LogsPath =>
            Path.Combine(BasePath, "Logs");


        public static string DatabasePath =>
            Path.Combine(
                DataPath,
                "F29Downloader.db"
            );


        public static void CrearCarpetas()
        {
            Directory.CreateDirectory(DataPath);

            Directory.CreateDirectory(DownloadsPath);

            Directory.CreateDirectory(LogsPath);
        }

        public static string ObtenerNombrePdf(
                Models.Contribuyente contribuyente)
        {
            string periodo =
                contribuyente.Periodo
                    .Replace("/", "_")
                    .Replace("-", "_");

            return
                $"F29_{contribuyente.Rut}_{periodo}.pdf";
        }
    }
}
