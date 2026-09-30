using MySql.Data.MySqlClient;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ProyectoBD
{
    public class Conexion
    {
        private static string cadenaSQLServer = @"Server = localhost; Database = AdquisicionesProveedores; Trusted_Connection=True; TrustServerCertificate=True;";
        private static string cadenaMySQL = @"Server=localhost; Port=3306; Database=proveedores_externos; Uid=root; Pwd=Lolplayers1;";
        public static SqlConnection ObtenerConexionSQLServer()
        {
            return new SqlConnection(cadenaSQLServer);
        }
        public static MySqlConnection ObtenerConexionMySQL()
        {
            return new MySqlConnection(cadenaMySQL);
        }
        public static bool ProbarConexionSQLServer()
        {
            try
            {
                using (SqlConnection cn = ObtenerConexionSQLServer())
                {
                    cn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
