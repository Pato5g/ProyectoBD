using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;


namespace ProyectoBD.Interfaces
{
    public partial class AdministradorDelSistema : Window
    {
        public AdministradorDelSistema()
        {
            InitializeComponent();
            CargarPantallas();
        }

        private void CargarPantallas()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT NombrePantalla, Modulo FROM Proveedores ORDER BY Modulo, NombrePantalla";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    dtgCargarPantallas.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }
        //Se crea el evento SelectionChanged para el DataGrid llamado dataGridPantalla
        private void dataGridPantalla_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void cmbRubros_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

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
        private void dtgSincronizacion_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void cmbProductos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
