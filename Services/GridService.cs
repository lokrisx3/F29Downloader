using F29Downloader.Models;
using System.Windows.Forms;
using System.Diagnostics;

namespace F29Downloader.Services
{
    public class GridService
    {
        private readonly DatabaseService database;

        public GridService()
        {
            database = new DatabaseService();
        }

        public void ActualizarGrilla(
                DataGridView dgv)
        {
            DatabaseService db =
                new DatabaseService();

            dgv.DataSource = null;

            dgv.DataSource =
                db.ObtenerContribuyentes();

            ConfigurarColumnas(dgv);
        }

        private void ConfigurarColumnas(
    DataGridView dgv)
        {
            if (dgv.Columns.Count == 0)
                return;

            dgv.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.None;

            if (dgv.Columns["Id"] != null)
            {
                dgv.Columns["Id"].Visible = false;
            }
            if (dgv.Columns["FechaProceso"] != null)
            {
                dgv.Columns["FechaProceso"].Visible = false;
            }
            dgv.RowHeadersVisible = false;

            dgv.AllowUserToAddRows = false;

            dgv.AllowUserToDeleteRows = false;

            dgv.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgv.MultiSelect = false;

            dgv.Columns["Clave"].Visible = false;

            dgv.Columns["Rut"].HeaderText =
                "RUT";

            dgv.Columns["Periodo"].HeaderText =
                "Período";

            dgv.Columns["Estado"].HeaderText =
                "Estado";

            dgv.Columns["Mensaje"].HeaderText =
                "Mensaje";

            dgv.Columns["RutaPdf"].HeaderText =
                "PDF";

            dgv.Columns["Intentos"].HeaderText =
                "Intentos";

            dgv.Columns["Rut"].Width = 80;

            dgv.Columns["Periodo"].Width = 70;

            dgv.Columns["Estado"].Width = 120;

            dgv.Columns["Intentos"].Width = 70;

            dgv.Columns["RutaPdf"].Width = 100;

            dgv.Columns["Mensaje"].Width = 100;

            dgv.Columns["Periodo"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgv.Columns["Estado"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgv.Columns["Intentos"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgv.Columns["Mensaje"].DefaultCellStyle.WrapMode =
                DataGridViewTriState.True;

            ConfigurarColumnaRutaPdf(dgv);

            AplicarEstilo(dgv);
        }

        private void ConfigurarColumnaRutaPdf(
    DataGridView dgv)
        {
            if (dgv.Columns["RutaPdf"] == null)
                return;

            if (dgv.Columns["RutaPdf"] is DataGridViewLinkColumn)
            {
                DataGridViewLinkColumn linkExistente =
                    (DataGridViewLinkColumn)dgv.Columns["RutaPdf"];

                linkExistente.LinkBehavior =
                    LinkBehavior.AlwaysUnderline;

                linkExistente.TrackVisitedState = false;

                linkExistente.UseColumnTextForLinkValue = false;

                linkExistente.LinkColor =
                    Color.Blue;

                linkExistente.ActiveLinkColor =
                    Color.Red;

                linkExistente.VisitedLinkColor =
                    Color.Blue;

                linkExistente.DefaultCellStyle.Font =
                    new Font(
                        dgv.Font,
                        FontStyle.Underline);

                linkExistente.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleLeft;

                return;
            }

            int indice =
                dgv.Columns["RutaPdf"].Index;

            DataGridViewLinkColumn columna =
                new DataGridViewLinkColumn();

            columna.Name = "RutaPdf";

            columna.DataPropertyName = "RutaPdf";

            columna.HeaderText = "PDF";

            columna.Width = 320;

            columna.TrackVisitedState = false;

            columna.UseColumnTextForLinkValue = false;

            columna.LinkBehavior =
                LinkBehavior.AlwaysUnderline;

            columna.LinkColor =
                Color.Blue;

            columna.ActiveLinkColor =
                Color.Red;

            columna.VisitedLinkColor =
                Color.Blue;

            columna.DefaultCellStyle.Font =
                new Font(
                    dgv.Font,
                    FontStyle.Underline);

            columna.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            dgv.Columns.Remove("RutaPdf");

            dgv.Columns.Insert(
                indice,
                columna);
        }

        public void AbrirPdf(
    DataGridView dgv,
    DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgv.Columns[e.ColumnIndex].Name != "RutaPdf")
                return;

            string rutaPdf =
                dgv.Rows[e.RowIndex]
                   .Cells["RutaPdf"]
                   .Value?
                   .ToString() ?? "";

            if (string.IsNullOrWhiteSpace(rutaPdf))
                return;

            if (File.Exists(rutaPdf))
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = rutaPdf,
                        UseShellExecute = true
                    });

                return;
            }

            string carpeta =
                Path.GetDirectoryName(rutaPdf) ?? "";

            if (Directory.Exists(carpeta))
            {
                MessageBox.Show(
                    "El PDF ya no existe.\n\nSe abrirá la carpeta donde debería encontrarse.");

                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = carpeta,
                        UseShellExecute = true
                    });

                return;
            }

            MessageBox.Show(
                "No existe el archivo ni la carpeta de descarga.");
        }

        private void AplicarEstilo(
    DataGridView dgv)
        {
            dgv.BorderStyle =
                BorderStyle.None;

            dgv.BackgroundColor =
                Color.White;

            dgv.EnableHeadersVisualStyles =
                false;

            dgv.ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.Single;

            dgv.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(240, 240, 240);

            dgv.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.Black;

            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    dgv.Font,
                    FontStyle.Bold);

            dgv.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgv.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(220, 235, 252);

            dgv.DefaultCellStyle.SelectionForeColor =
                Color.Black;

            dgv.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 248, 248);

            dgv.RowTemplate.Height = 28;

            dgv.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.AllCells;
        }
    }
}