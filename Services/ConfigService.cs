using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
    }
}
