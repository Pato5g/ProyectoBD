using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace ProyectoBD.Interfaces
{
    public partial class AdministradorProveedores
    {
        public AdministradorProveedores()
        {
            InitializeComponent();
            CargarPantallas();
        }
        private void CargarPantallas()
        {
            try
            {
                using (MySqlConnection conexion = Conexion.ObtenerConexionMySQL())
                {
                    conexion.Open();
                    string consulta = @"SELECT NombrePantalla, Modulo FROM Proveedores ORDER BY Modulo, NombrePantalla";
                    DataTable dt = new DataTable();
                    MySqlDataAdapter da = new MySqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    dtgSincronizacionProv.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos MySQL: {ex.Message}");
            }
        }
        private void CargarBDs()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                using (MySqlConnection conexionMySQL = Conexion.ObtenerConexionMySQL())
                {
                    conexion.Open();
                    string consulta = @"SELECT NombreBD, Modulo FROM BasesDeDatos ORDER BY Modulo, NombreBD";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    dtgSincronizacion.ItemsSource = dt.DefaultView;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
