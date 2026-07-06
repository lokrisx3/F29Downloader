using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F29Downloader.Models;


namespace F29Downloader.Services
{
    public class DatabaseService
    {
        private readonly string rutaBaseDatos;


        public DatabaseService()
        {
            ConfigService.CrearCarpetas();

            rutaBaseDatos =
                ConfigService.DatabasePath;
        }


        public void CrearBaseDatos()
        {
            using SqliteConnection conexion =
                new SqliteConnection(
                    $"Data Source={rutaBaseDatos}"
                );


            conexion.Open();


            string sql =
            @"
            CREATE TABLE IF NOT EXISTS Contribuyentes
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,

                Rut TEXT NOT NULL,

                Clave TEXT NOT NULL,

                Periodo TEXT NOT NULL,

                Estado TEXT DEFAULT 'Pendiente',

                Mensaje TEXT,

                FechaProceso TEXT,

                RutaPdf TEXT,

                Intentos INTEGER DEFAULT 0
            );
            ";


            using SqliteCommand comando =
                new SqliteCommand(sql, conexion);


            comando.ExecuteNonQuery();
        }


        public void InsertarContribuyentes(
            List<Contribuyente> lista)
        {
            using SqliteConnection conexion =
                new SqliteConnection(
                    $"Data Source={rutaBaseDatos}"
                );


            conexion.Open();


            foreach (Contribuyente contribuyente in lista)
            {
                string sql =
                @"
                INSERT INTO Contribuyentes
                (
                    Rut,
                    Clave,
                    Periodo,
                    Estado,
                    Mensaje,
                    FechaProceso,
                    RutaPdf,
                    Intentos
                )
                VALUES
                (
                    $rut,
                    $clave,
                    $periodo,
                    $estado,
                    $mensaje,
                    $fechaProceso,
                    $rutaPdf,
                    $intentos
                );
                ";


                using SqliteCommand comando =
                    new SqliteCommand(sql, conexion);


                comando.Parameters.AddWithValue(
                    "$rut",
                    contribuyente.Rut
                );

                comando.Parameters.AddWithValue(
                    "$clave",
                    contribuyente.Clave
                );

                comando.Parameters.AddWithValue(
                    "$periodo",
                    contribuyente.Periodo
                );

                comando.Parameters.AddWithValue(
                    "$estado",
                    contribuyente.Estado
                );

                comando.Parameters.AddWithValue(
                    "$mensaje",
                    contribuyente.Mensaje
                );

                comando.Parameters.AddWithValue(
                    "$fechaProceso",
                    contribuyente.FechaProceso
                );

                comando.Parameters.AddWithValue(
                    "$rutaPdf",
                    contribuyente.RutaPdf
                );

                comando.Parameters.AddWithValue(
                    "$intentos",
                    contribuyente.Intentos
                );


                comando.ExecuteNonQuery();
            }
        }


        public void LimpiarContribuyentes()
        {
            using SqliteConnection conexion =
                new SqliteConnection(
                    $"Data Source={rutaBaseDatos}"
                );


            conexion.Open();


            string sql =
                "DELETE FROM Contribuyentes;";


            using SqliteCommand comando =
                new SqliteCommand(sql, conexion);


            comando.ExecuteNonQuery();
        }


        public List<Contribuyente> ObtenerContribuyentes()
        {
            List<Contribuyente> lista =
                new List<Contribuyente>();


            using SqliteConnection conexion =
                new SqliteConnection(
                    $"Data Source={rutaBaseDatos}"
                );


            conexion.Open();


            string sql =
            @"
            SELECT
                Rut,
                Clave,
                Periodo,
                Estado,
                Mensaje,
                FechaProceso,
                RutaPdf,
                Intentos
            FROM Contribuyentes;
            ";


            using SqliteCommand comando =
                new SqliteCommand(sql, conexion);


            using SqliteDataReader reader =
                comando.ExecuteReader();


            while (reader.Read())
            {
                Contribuyente contribuyente =
                    new Contribuyente();


                contribuyente.Rut =
                    reader.GetString(0);


                contribuyente.Clave =
                    reader.GetString(1);


                contribuyente.Periodo =
                    reader.GetString(2);


                contribuyente.Estado =
                    reader.GetString(3);


                contribuyente.Mensaje =
                    reader.IsDBNull(4)
                    ? ""
                    : reader.GetString(4);


                contribuyente.FechaProceso =
                    reader.IsDBNull(5)
                    ? ""
                    : reader.GetString(5);


                contribuyente.RutaPdf =
                    reader.IsDBNull(6)
                    ? ""
                    : reader.GetString(6);


                contribuyente.Intentos =
                    reader.GetInt32(7);


                lista.Add(contribuyente);
            }


            return lista;
        }
    }
}