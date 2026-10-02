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
            CargarProveedores();
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
        private void CargarProveedores()
        {
            try
            {
                using (MySqlConnection conexion = Conexion.ObtenerConexionMySQL())
                {
                    conexion.Open();
                    string consulta = @"SELECT nombre_comercial, nit, direccion, telefono, correo, activo, sincronizado FROM proveedor_externo WHERE nit = @nit";
                    MySqlCommand comando = new MySqlCommand(consulta, conexion);
                    comando.Parameters.AddWithValue("@nit", txtNIT.Text);
                    MySqlDataReader reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        string nombreComercial = reader.GetString("nombre_comercial");
                        string nit = reader.GetString("nit");
                        string direccion = reader.GetString("direccion");
                        string telefono = reader.GetString("telefono");
                        string correo = reader.GetString("correo");
                        bool activo = reader.GetBoolean("activo");
                        bool sincronizado = reader.GetBoolean("sincronizado");
                        // Aquí puedes hacer algo con los datos obtenidos, como mostrarlos en la interfaz de usuario
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los proveedores: {ex.Message}");
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
