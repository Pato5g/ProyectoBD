using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
// using System.Windows.Automation.Text;


namespace ProyectoBD.Interfaces
{
    public partial class AdministradorDelSistema : Window
    {
        private int rolSeleccionadoID = 0;
        public AdministradorDelSistema()
        {
            InitializeComponent();
            CargarSucursales();
            CargarPantallas();
            CargarSucursalesCombo();
            CargarDepartamentos();
            CargarArticulos();
            CargarRubros();
            CargarProductos();
            CargarRoles();
            CargarProveedores();
            CargarProveedoresPendientes();
        }
        private void CargarPantallas()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT PantallaID, NombrePantalla AS pantalla, Modulo AS modulo, CAST(0 AS BIT) AS crear, CAST(0 AS BIT) AS leer, CAST(0 AS BIT) AS actualizar, CAST(0 AS BIT) AS eliminar FROM Pantalla ORDER BY Modulo, NombrePantalla";
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
        private void CargarSucursales()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT SucursalID, Codigo, Direccion, Ciudad, DepartamentoGeo, Activo FROM Sucursal ORDER BY Codigo";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    dtgSucursales.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }
        //Se crea el evento SelectionChanged para el DataGrid llamado dataGridPantalla

        private void CargarProveedoresPendientes()
        {
            try
            {
                using (MySqlConnection conexionMySQL = Conexion.ObtenerConexionMySQL())
                {
                    conexionMySQL.Open();
                    string consulta = @"SELECT id_proveedor, nit, nombre_comercial, fecha_registro, CASE WHEN sincronizado = 0 THEN 'Pendiente' ELSE 'Sincronizado' END AS estado FROM proveedor_externo
                                        WHERE sincronizado = 0 ORDER BY fecha_registro DESC"; // Solo proveedores pendientes de aprobación
                    DataTable dt = new DataTable();
                    MySqlDataAdapter da = new MySqlDataAdapter(consulta, conexionMySQL);
                    da.Fill(dt);
                    dtgSincronizacion.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos MySQL: {ex.Message}");
            }
        }

        private void dtgSucursales_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dtgSucursales.SelectedItem is DataRowView selectedRow)
            {
                txtIdSucursal.Text = selectedRow["SucursalID"]?.ToString() ?? string.Empty;
                txtCodigoSucursal.Text = selectedRow["Codigo"]?.ToString() ?? string.Empty;
                txtDireccionSucursal.Text = selectedRow["Direccion"]?.ToString() ?? string.Empty;
                txtCiudadSucursal.Text = selectedRow["Ciudad"]?.ToString() ?? string.Empty;
                // Use the column name returned by the query ("DepartamentoGeo") and guard against null/DBNull
                txtDepartamentoSucursal.Text = selectedRow["DepartamentoGeo"]?.ToString() ?? string.Empty;
                // Aquí puedes hacer algo con los datos seleccionados, como mostrarlos en otros controles
                chkSucursalFuncionando.IsChecked = selectedRow["Activo"] as bool? ?? false; // Asumiendo que la sucursal está funcionando si está seleccionada
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"INSERT INTO Sucursal (Codigo, Direccion, Ciudad, DepartamentoGeo, Activo) VALUES (@Codigo, @Direccion, @Ciudad, @DepartamentoGeo, @Activo)";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    // Guard against nullable Text values and nullable IsChecked (bool?)
                    cmd.Parameters.AddWithValue("@Codigo", txtCodigoSucursal.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Direccion", txtDireccionSucursal.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Ciudad", txtCiudadSucursal.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@DepartamentoGeo", txtDepartamentoSucursal.Text ?? string.Empty);
                    // Box the nullable bool as a non-nullable value (false when null)
                    cmd.Parameters.AddWithValue("@Activo", (object)(chkSucursalFuncionando.IsChecked ?? false));
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Sucursal agregada correctamente.");
                    CargarSucursales(); // Se recarga la lista de sucursales después de agregar una nueva
                    CargarSucursalesCombo(); // Se recarga el ComboBox de sucursales después de agregar una nueva
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"UPDATE Sucursal SET Direccion = @Direccion, Ciudad = @Ciudad, DepartamentoGeo = @DepartamentoGeo, Activo = @Activo WHERE SucursalID = @SucursalID";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@SucursalID", txtIdSucursal.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Direccion", txtDireccionSucursal.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Ciudad", txtCiudadSucursal.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@DepartamentoGeo", txtDepartamentoSucursal.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Activo", (object)(chkSucursalFuncionando.IsChecked ?? false));
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Sucursal actualizada correctamente.");
                        CargarSucursales(); // Se recarga la lista de sucursales después de actualizar
                        CargarSucursalesCombo(); // Se recarga el ComboBox de sucursales después de actualizar
                    }
                    else
                    {
                        MessageBox.Show("No se encontró la sucursal para actualizar.");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"UPDATE Sucursal SET Activo = @Activo WHERE SucursalID = @SucursalID";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@SucursalID", txtIdSucursal.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Activo", (object)(chkSucursalFuncionando.IsChecked ?? false));
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Sucursal desactivada correctamente.");
                        CargarSucursales(); // Se recarga la lista de sucursales después de eliminar
                        CargarSucursalesCombo(); // Se recarga el ComboBox de sucursales después de eliminar
                    }
                    else
                    {
                        MessageBox.Show("No se encontró la sucursal para eliminar.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT * FROM Sucursal WHERE Codigo = @Codigo";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@Codigo", txtCodigoSucursal.Text ?? string.Empty);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtIdSucursal.Text = reader["SucursalID"].ToString();
                        txtDireccionSucursal.Text = reader["Direccion"].ToString();
                        txtCiudadSucursal.Text = reader["Ciudad"].ToString();
                        txtDepartamentoSucursal.Text = reader["DepartamentoGeo"].ToString();
                        chkSucursalFuncionando.IsChecked = Convert.ToBoolean(reader["Activo"]);
                        MessageBox.Show("Sucursal encontrada.");
                    }
                    else
                    {
                        MessageBox.Show("No se encontró la sucursal para eliminar.");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            txtIdSucursal.Text = string.Empty;
            txtCodigoSucursal.Text = string.Empty;
            txtDireccionSucursal.Text = string.Empty;
            txtCiudadSucursal.Text = string.Empty;
            txtDepartamentoSucursal.Text = string.Empty;
            chkSucursalFuncionando.IsChecked = false;
        }

        private void CargarSucursalesCombo()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT SucursalID, Codigo FROM Sucursal WHERE Activo = 1";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    cmbSucursalDepartamento.ItemsSource = dt.DefaultView;
                    cmbSucursalDepartamento.DisplayMemberPath = "Codigo";
                    cmbSucursalDepartamento.SelectedValuePath = "SucursalID";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las sucursales en el ComboBox: {ex.Message}");
            }
        }

        private void CargarDepartamentos()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT d.DepartamentoID, d.sucursalID, s.Codigo, d.Nombre, d.Descripcion FROM Departamento d INNER JOIN Sucursal s ON d.SucursalID = s.SucursalID";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    dtgDepartamentos.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los departamentos en el ComboBox: {ex.Message}");
            }
        }

        private void dtgDepartamentos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dtgDepartamentos.SelectedItem is DataRowView selectedRow)
            {
                // Aquí puedes hacer algo con los datos seleccionados, como mostrarlos en otros controles
                cmbSucursalDepartamento.SelectedValue = selectedRow["SucursalID"];
                txtDepartamentoId.Text = selectedRow["DepartamentoID"]?.ToString() ?? string.Empty;
                txtDepartamento.Text = selectedRow["Nombre"]?.ToString() ?? string.Empty;
                txtDescripcionDepartamento.Text = selectedRow["Descripcion"]?.ToString() ?? string.Empty;
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"INSERT INTO Departamento (SucursalID, Nombre, Descripcion) VALUES (@SucursalID, @Nombre, @Descripcion)";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@SucursalID", cmbSucursalDepartamento.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Nombre", txtDepartamento.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Descripcion", txtDescripcionDepartamento.Text ?? string.Empty);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Departamento agregado correctamente.");
                    CargarDepartamentos(); // Se recarga la lista de departamentos después de agregar uno nuevo
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"UPDATE Departamento SET SucursalID = @SucursalID, Nombre = @Nombre, Descripcion = @Descripcion WHERE DepartamentoID = @DepartamentoID";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@DepartamentoID", txtDepartamentoId.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@SucursalID", cmbSucursalDepartamento.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Nombre", txtDepartamento.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Descripcion", txtDescripcionDepartamento.Text ?? string.Empty);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Departamento actualizado correctamente.");
                        CargarDepartamentos(); // Se recarga la lista de departamentos después de actualizar
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el departamento para actualizar.");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"DELETE FROM Departamento WHERE DepartamentoID = @DepartamentoID";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@DepartamentoID", txtDepartamentoId.Text ?? string.Empty);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Departamento eliminado correctamente.");
                        CargarDepartamentos(); // Se recarga la lista de departamentos después de eliminar
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el departamento para eliminar.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT * FROM Departamento WHERE DepartamentoID = @DepartamentoID";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@DepartamentoID", txtDepartamentoId.Text ?? string.Empty);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        cmbSucursalDepartamento.SelectedValue = reader["SucursalID"];
                        txtDepartamento.Text = reader["Nombre"].ToString();
                        txtDescripcionDepartamento.Text = reader["Descripcion"].ToString();
                        MessageBox.Show("Departamento encontrado.");
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el departamento.");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            txtDepartamentoId.Clear();
            cmbSucursalDepartamento.SelectedIndex = -1;
            txtDepartamento.Clear();
            txtDescripcionDepartamento.Clear();
        }
        private void CargarArticulos()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT Codigo, Nombre, Descripcion FROM Articulo";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    dtgArticulos.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los artículos: {ex.Message}");
            }
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"INSERT INTO Articulo (Codigo, Nombre, Descripcion) VALUES (@Codigo, @Nombre, @Descripcion)";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@Codigo", txtCodigoArticulo.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Nombre", txtNombreArticulo.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Descripcion", txtDescripcionArticulo.Text ?? string.Empty);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Artículo agregado correctamente.");
                    CargarArticulos(); // Se recarga la lista de artículos después de agregar
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"UPDATE Articulo SET Nombre = @Nombre, Descripcion = @Descripcion WHERE Codigo = @Codigo";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@Codigo", txtCodigoArticulo.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Nombre", txtNombreArticulo.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Descripcion", txtDescripcionArticulo.Text ?? string.Empty);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Artículo actualizado correctamente.");
                        CargarArticulos(); // Se recarga la lista de artículos después de actualizar
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el artículo para actualizar.");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"DELETE FROM Articulo WHERE Codigo = @Codigo";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@Codigo", txtCodigoArticulo.Text ?? string.Empty);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Artículo eliminado correctamente.");
                        CargarArticulos(); // Se recarga la lista de artículos después de eliminar
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el artículo para eliminar.");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT * FROM Articulo WHERE Codigo = @Codigo";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@Codigo", txtCodigoArticulo.Text ?? string.Empty);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtNombreArticulo.Text = reader["Nombre"].ToString();
                        txtDescripcionArticulo.Text = reader["Descripcion"].ToString();
                        MessageBox.Show("Artículo encontrado.");
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el artículo.");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }
        private void dtgArticulos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dtgArticulos.SelectedItem is DataRowView selectedRow)
            {
                txtCodigoArticulo.Text = selectedRow["Codigo"]?.ToString() ?? string.Empty;
                txtNombreArticulo.Text = selectedRow["Nombre"]?.ToString() ?? string.Empty;
                txtDescripcionArticulo.Text = selectedRow["Descripcion"]?.ToString() ?? string.Empty;
            }
        }

        private void Button_Click_14(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    using (SqlTransaction transaction = conexion.BeginTransaction())
                    {
                        try
                        {
                            string consulta = @"INSERT INTO Proveedor (Codigo, NombreComercial, Direccion, Telefono, Activo) OUTPUT INSERTED.ProveedorID VALUES (@Codigo, @NombreComercial, @Direccion, @Telefono, @Activo)";
                            SqlCommand cmd = new SqlCommand(consulta, conexion, transaction);
                            cmd.Parameters.AddWithValue("@Codigo", txtNITProveedor.Text ?? string.Empty);
                            cmd.Parameters.AddWithValue("@NombreComercial", txtNombreComercial.Text ?? string.Empty);
                            cmd.Parameters.AddWithValue("@Direccion", txtDireccionProveedor.Text ?? string.Empty);
                            cmd.Parameters.AddWithValue("@Telefono", txtTelefonoProveedor.Text ?? string.Empty);
                            cmd.Parameters.AddWithValue("@Activo", chkProveedorActivo.IsChecked ?? false);
                            int proveedorId = Convert.ToInt32(cmd.ExecuteScalar()); // Obtener el ProveedorID generado
                            string consultaRubro = @"INSERT INTO ProveedorRubro (ProveedorID, RubroID) VALUES (@ProveedorID, @RubroID)";
                            SqlCommand cmdRubro = new SqlCommand(consultaRubro, conexion, transaction);
                            cmdRubro.Parameters.AddWithValue("@ProveedorID", proveedorId);
                            cmdRubro.Parameters.AddWithValue("@RubroID", cmbRubros.SelectedValue);
                            cmdRubro.ExecuteNonQuery();
                            string consultaArticulo = @"INSERT INTO ProveedorArticulo (ProveedorID, ArticuloID, PrecioReferencia) VALUES (@ProveedorID, @ArticuloID, @PrecioReferencia)";
                            SqlCommand cmdArticulo = new SqlCommand(consultaArticulo, conexion, transaction);
                            cmdArticulo.Parameters.AddWithValue("@ProveedorID", proveedorId);
                            cmdArticulo.Parameters.AddWithValue("@ArticuloID", cmbArticulos.SelectedValue);
                            if (!decimal.TryParse(txtPrecioReferencial.Text, out decimal precioReferencia))
                            {
                                MessageBox.Show("El precio referencial debe ser un número válido.");
                                transaction.Rollback();
                                return;
                            }
                            cmdArticulo.Parameters.AddWithValue("@PrecioReferencia", precioReferencia);
                            cmdArticulo.ExecuteNonQuery();
                            transaction.Commit();
                            MessageBox.Show("Proveedor, rubro y artículo agregados correctamente.");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Error al agregar el proveedor: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click_15(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"UPDATE Proveedor SET NombreComercial = @NombreComercial, Direccion = @Direccion, Telefono = @Telefono, Activo = @Activo WHERE Codigo = @Codigo";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@Codigo", txtNITProveedor.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@NombreComercial", txtNombreComercial.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Direccion", txtDireccionProveedor.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Telefono", txtTelefonoProveedor.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Activo", chkProveedorActivo.IsChecked ?? false);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Proveedor actualizado.");
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el proveedor para actualizar.");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click_16(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"DELETE FROM Proveedor WHERE Codigo = @Codigo";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@Codigo", txtNITProveedor.Text ?? string.Empty);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Proveedor eliminado.");
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el proveedor para eliminar.");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }

        private void Button_Click_17(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT * FROM Proveedor WHERE Codigo = @Codigo";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@Codigo", txtNITProveedor.Text ?? string.Empty);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtNITProveedor.Text = reader["Codigo"].ToString();
                        txtNombreComercial.Text = reader["NombreComercial"].ToString();
                        txtDireccionProveedor.Text = reader["Direccion"].ToString();
                        txtTelefonoProveedor.Text = reader["Telefono"].ToString();
                        chkProveedorActivo.IsChecked = Convert.ToBoolean(reader["Activo"]);
                        MessageBox.Show("Proveedor encontrado.");
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el proveedor.");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }
        private void CargarRubros()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT RubroID, Nombre FROM Rubro ORDER BY Nombre";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    cmbRubros.ItemsSource = dt.DefaultView;
                    cmbRubros.DisplayMemberPath = "Nombre";
                    cmbRubros.SelectedValuePath = "RubroID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los rubros: {ex.Message}");
            }
        }
        private void CargarProductos()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT ArticuloID, Nombre FROM Articulo ORDER BY Nombre";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    cmbArticulos.ItemsSource = dt.DefaultView;
                    cmbArticulos.DisplayMemberPath = "Nombre";
                    cmbArticulos.SelectedValuePath = "ArticuloID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los productos: {ex.Message}");
            }
        }

        private void Button_Click_18(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"INSERT INTO Usuario (NombreUsuario, PasswordHash, RolID, ProveedorID, Activo) VALUES (@NombreUsuario, HASHBYTES('SHA2_256', @PasswordHash), @RolID, @ProveedorID, @Activo)";
                    // se agrega una validación para asegurarse de que se haya seleccionado un rol antes de ejecutar la consulta
                    if (cmbRol.SelectedValue == null)
                    {
                        MessageBox.Show("Por favor, seleccione un rol para el usuario.");
                        return;
                    }
                    // se agrega una validación para asegurarse de que se hayan completado todos los campos requeridos antes de ejecutar la consulta
                    if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text) || string.IsNullOrWhiteSpace(txtContraseñaUsuario.Password))
                    {
                        MessageBox.Show("Por favor, complete todos los campos requeridos.");
                        return;
                    }
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@NombreUsuario", txtNombreUsuario.Text);
                    cmd.Parameters.AddWithValue("@PasswordHash", txtContraseñaUsuario.Password);
                    cmd.Parameters.AddWithValue("@RolID", cmbRol.SelectedValue);
                    cmd.Parameters.AddWithValue("@Activo", chkUsuarioActivo.IsChecked ?? false);
                    cmd.Parameters.AddWithValue("@ProveedorID", cmbProveedor.SelectedValue ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Usuario agregado correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar el usuario: {ex.Message}");
            }
        }
        private void CargarRoles()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT RolID, NombreRol FROM Rol ORDER BY NombreRol";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    cmbRol.ItemsSource = dt.DefaultView;
                    cmbRol.DisplayMemberPath = "NombreRol";
                    cmbRol.SelectedValuePath = "RolID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los roles: {ex.Message}");
            }
        }
        private void CargarProveedores()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT ProveedorID, NombreComercial FROM Proveedor WHERE Activo = 1 ORDER BY NombreComercial";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    cmbProveedor.ItemsSource = dt.DefaultView;
                    cmbProveedor.DisplayMemberPath = "NombreComercial";
                    cmbProveedor.SelectedValuePath = "ProveedorID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los proveedores: {ex.Message}");
            }
        }

        private void Button_Click_19(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos requeridos.");
                return;
            }
            if (cmbRol.SelectedValue == null)
            {
                MessageBox.Show("Por favor, seleccione un rol para el usuario.");
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta;

                    if (!string.IsNullOrWhiteSpace(txtContraseñaUsuario.Password))
                    {
                        consulta = @"UPDATE Usuario SET PasswordHash = HASHBYTES('SHA2_256', @PasswordHash), RolID = @RolID, ProveedorID = @ProveedorID, Activo = @Activo WHERE NombreUsuario = @NombreUsuario";
                    }
                    else
                    {
                        consulta = @"UPDATE Usuario SET RolID = @RolID, ProveedorID = @ProveedorID, Activo = @Activo WHERE NombreUsuario = @NombreUsuario";
                    }

                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@NombreUsuario", txtNombreUsuario.Text);
                    if (!string.IsNullOrWhiteSpace(txtContraseñaUsuario.Password))
                    {
                        cmd.Parameters.AddWithValue("@PasswordHash", txtContraseñaUsuario.Password);
                    }
                    cmd.Parameters.AddWithValue("@RolID", cmbRol.SelectedValue);
                    cmd.Parameters.AddWithValue("@Activo", chkUsuarioActivo.IsChecked ?? false);
                    cmd.Parameters.AddWithValue("@ProveedorID", cmbProveedor.SelectedValue ?? DBNull.Value);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Usuario actualizado correctamente.");
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el usuario para actualizar.");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar el usuario: {ex.Message}");
            }
        }

        private void Button_Click_20(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text))
            {
                MessageBox.Show("Por favor, ingrese el nombre de usuario a eliminar.");
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"UPDATE Usuario SET Activo = 0 WHERE NombreUsuario = @NombreUsuario";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@NombreUsuario", txtNombreUsuario.Text.Trim());
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Usuario desactivado correctamente.");
                        chkUsuarioActivo.IsChecked = false; // Se actualiza el estado del CheckBox a desactivado
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el usuario para desactivar.");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el usuario: {ex.Message}");
            }
        }

        private void Button_Click_21(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text))
            {
                MessageBox.Show("Por favor, ingrese el nombre de usuario a buscar.");
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT RolID, ProveedorID, Activo FROM Usuario WHERE NombreUsuario = @NombreUsuario";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@NombreUsuario", txtNombreUsuario.Text.Trim());
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        cmbRol.SelectedValue = reader["RolID"];
                        if (reader["ProveedorID"] != DBNull.Value)
                        {
                            cmbProveedor.SelectedValue = reader["ProveedorID"];
                        }
                        else
                        {
                            cmbProveedor.SelectedIndex = -1; // Limpiar selección si no hay proveedor asociado
                        }
                        chkUsuarioActivo.IsChecked = Convert.ToBoolean(reader["Activo"]);
                        txtContraseñaUsuario.Clear(); // Limpiar el campo de contraseña por seguridad
                        MessageBox.Show("Usuario encontrado.");
                    }
                    else
                    {
                        MessageBox.Show("Usuario no encontrado.");
                    }
                    reader.Close(); // Cerrar el lector de datos después de usarlo
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el usuario: {ex.Message}");
            }
        }

        private void Button_Click_22(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreRol.Text))
            {
                MessageBox.Show("Por favor, ingrese el nombre del rol a agregar.");
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"INSERT INTO Rol (NombreRol) VALUES (@NombreRol)";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@NombreRol", txtNombreRol.Text.Trim());
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Rol agregado correctamente.");
                    CargarRoles(); // Se recarga la lista de roles después de agregar uno nuevo
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar el rol: {ex.Message}");
            }
        }

        private void Button_Click_23(object sender, RoutedEventArgs e)
        {
            if (rolSeleccionadoID == 0)
            {
                MessageBox.Show("Por favor, seleccione un rol para actualizar.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNombreRol.Text))
            {
                MessageBox.Show("Por favor, ingrese el nuevo nombre del rol.");
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"UPDATE Rol SET NombreRol = @NombreRol WHERE RolID = @RolID";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@NombreRol", txtNombreRol.Text.Trim());
                    cmd.Parameters.AddWithValue("@RolID", rolSeleccionadoID);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Rol actualizado correctamente.");
                        CargarRoles(); // Se recarga la lista de roles después de actualizar
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el rol para actualizar.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar el rol: {ex.Message}");
            }
        }

        private void Button_Click_25(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreRol.Text))
            {
                MessageBox.Show("Por favor, ingrese el nombre del rol a buscar.");
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT RolID, NombreRol FROM Rol WHERE NombreRol = @NombreRol";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@NombreRol", txtNombreRol.Text.Trim());
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        rolSeleccionadoID = Convert.ToInt32(reader["RolID"]);
                        txtNombreRol.Text = reader["NombreRol"].ToString();
                        reader.Close(); // Cerrar el lector de datos antes de mostrar el mensaje
                        CargarPermisosRol(); // Se recarga la lista de roles si se encuentra el rol
                        MessageBox.Show("Rol encontrado.");
                    }
                    else
                    {
                        reader.Close(); // Cerrar el lector de datos antes de mostrar el mensaje
                        rolSeleccionadoID = 0;
                        CargarPantallas(); // Se recarga la lista de pantallas si no se encuentra el rol
                        MessageBox.Show("No se encontró el rol.");
                    }
                    reader.Close(); // Cerrar el lector de datos después de usarlo
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar el rol: {ex.Message}");

            }
        }
        private void CargarPermisosRol()
        {
            if (rolSeleccionadoID == 0)
            {
                MessageBox.Show("Por favor, seleccione un rol para cargar sus permisos.");
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"
                SELECT 
                    p.PantallaID,
                    p.NombrePantalla AS pantalla,
                    p.Modulo AS modulo,
                    CAST(ISNULL(pe.PermiteCrear, 0) AS BIT) AS crear,
                    CAST(ISNULL(pe.PermiteLeer, 0) AS BIT) AS leer,
                    CAST(ISNULL(pe.PermiteActualizar, 0) AS BIT) AS actualizar,
                    CAST(ISNULL(pe.PermiteBorrar, 0) AS BIT) AS eliminar
                FROM Pantalla p
                LEFT JOIN Permiso pe
                    ON p.PantallaID = pe.PantallaID
                    AND pe.RolID = @RolID
                ORDER BY p.Modulo, p.NombrePantalla";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@RolID", rolSeleccionadoID);
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    dtgCargarPantallas.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los permisos del rol: {ex.Message}");
            }
        }

        private void Button_Click_24(object sender, RoutedEventArgs e)
        {
            if (rolSeleccionadoID == 0)
            {
                MessageBox.Show("Por favor, seleccione un rol para eliminar.");
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consultaUsuarios = @"SELECT COUNT(*) FROM Usuario WHERE RolID = @RolID";
                    SqlCommand cmd = new SqlCommand(consultaUsuarios, conexion);
                    cmd.Parameters.AddWithValue("@RolID", rolSeleccionadoID);
                    int cantidadUsuarios = Convert.ToInt32(cmd.ExecuteScalar());
                    if (cantidadUsuarios > 0)
                    {
                        MessageBox.Show("No se puede eliminar el rol porque tiene usuarios asociados.");
                        return;
                    }
                    SqlTransaction transaction = conexion.BeginTransaction();
                    try
                    {
                        string ConsultaPermisos = @"DELETE FROM Permiso WHERE RolID = @RolID";
                        SqlCommand cmdPermisos = new SqlCommand(ConsultaPermisos, conexion, transaction);
                        cmdPermisos.Parameters.AddWithValue("@RolID", rolSeleccionadoID);
                        cmdPermisos.ExecuteNonQuery();
                        // Eliminar el rol después de eliminar los permisos asociados
                        string consultaEliminarRol = @"DELETE FROM Rol WHERE RolID = @RolID";
                        SqlCommand cmdEliminarRol = new SqlCommand(consultaEliminarRol, conexion, transaction);
                        cmdEliminarRol.Parameters.AddWithValue("@RolID", rolSeleccionadoID);
                        int rowsAffected = cmdEliminarRol.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            transaction.Commit();
                            MessageBox.Show("Rol eliminado correctamente.");
                            rolSeleccionadoID = 0; // Reiniciar el ID del rol seleccionado
                            txtNombreRol.Clear(); // Limpiar el campo de texto del nombre del rol
                            CargarRoles(); // Se recarga la lista de roles después de eliminar
                        }
                        else
                        {
                            transaction.Rollback();
                            MessageBox.Show("No se encontró el rol para eliminar.");
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el rol: {ex.Message}");
            }
        }

        private void Button_Click_26(object sender, RoutedEventArgs e)
        {
            if (rolSeleccionadoID == 0)
            {
                MessageBox.Show("Por favor, seleccione un rol para actualizar sus permisos.");
                return;
            }
            if (dtgCargarPantallas.ItemsSource == null)
            {
                MessageBox.Show("No hay permisos para actualizar.");
                return;
            }
            dtgCargarPantallas.CommitEdit(System.Windows.Controls.DataGridEditingUnit.Cell, true); // Asegurarse de que los cambios en la grilla se guarden
            dtgCargarPantallas.CommitEdit(System.Windows.Controls.DataGridEditingUnit.Row, true); // Asegurarse de que los cambios en la grilla se guarden
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    SqlTransaction transaction = conexion.BeginTransaction();
                    try
                    {
                        {
                            string consulta = @"IF EXISTS (SELECT 1 FROM Permiso WHERE RolID = @RolID AND PantallaID = @PantallaID)
                                                    BEGIN
                                                        UPDATE Permiso SET PermiteCrear = @PermiteCrear, PermiteLeer = @PermiteLeer, PermiteActualizar = @PermiteActualizar, PermiteBorrar = @PermiteBorrar WHERE RolID = @RolID AND PantallaID = @PantallaID
                                                    END
                                                ELSE
                                                BEGIN
                                                    INSERT INTO Permiso (RolID, PantallaID, PermiteCrear, PermiteLeer, PermiteActualizar, PermiteBorrar) VALUES (@RolID, @PantallaID, @PermiteCrear, @PermiteLeer, @PermiteActualizar, @PermiteBorrar) END";
                            DataView vista = (DataView)dtgCargarPantallas.ItemsSource;
                            foreach (DataRowView fila in vista)
                            {
                                SqlCommand cmd = new SqlCommand(consulta, conexion, transaction);
                                cmd.Parameters.AddWithValue("@RolID", rolSeleccionadoID);
                                cmd.Parameters.AddWithValue("@PantallaID", fila["PantallaID"]);
                                cmd.Parameters.AddWithValue("@PermiteCrear", fila["crear"]);
                                cmd.Parameters.AddWithValue("@PermiteLeer", fila["leer"]);
                                cmd.Parameters.AddWithValue("@PermiteActualizar", fila["actualizar"]);
                                cmd.Parameters.AddWithValue("@PermiteBorrar", fila["eliminar"]);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                        MessageBox.Show("Permisos actualizados correctamente.");
                        CargarPermisosRol(); // Se recarga la lista de permisos después de actualizar
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar los permisos: {ex.Message}");
            }
        }

        private void Button_Click_27(object sender, RoutedEventArgs e)
        {
            int exitosos = 0;
            int fallidos = 0;
            try
            {
                using (MySqlConnection conexionMySQL = Conexion.ObtenerConexionMySQL())
                using (SqlConnection conexionSQL = Conexion.ObtenerConexionSQLServer())
                {
                    // Lógica de sincronización
                    conexionMySQL.Open();
                    conexionSQL.Open();

                    // 1. Obtener proveedores pendientes de sincronización desde MySQL
                    string consultaPendientes = "SELECT id_proveedor, nit, nombre_comercial, direccion, telefono, correo, activo FROM proveedor_externo WHERE Sincronizado = 0";
                    DataTable proveedores = new DataTable();
                    using (MySqlDataAdapter da = new MySqlDataAdapter(consultaPendientes, conexionMySQL))
                    {
                        da.Fill(proveedores);
                    }
                    if (proveedores.Rows.Count == 0)
                    {
                        MessageBox.Show("No hay proveedores pendientes de sincronización.");
                        return;
                    }
                    // 2. Recorrer cada proveedor pendiente
                    foreach (DataRow fila in proveedores.Rows)
                    {
                        int idProveedorExterno = Convert.ToInt32(fila["id_proveedor"]);
                        string nit = fila["nit"]?.ToString() ?? string.Empty;
                        string nombreComercial = fila["nombre_comercial"]?.ToString() ?? string.Empty;
                        string direccion = fila["direccion"]?.ToString() ?? string.Empty;
                        string telefono = fila["telefono"]?.ToString() ?? string.Empty;
                        bool activo = Convert.ToBoolean(fila["activo"]);
                        SqlTransaction transactionSQL = conexionSQL.BeginTransaction();
                        try
                        {
                            // Revisar si el NIT ya existe en SQL Server
                            string consultaExiste = "SELECT ProveedorID FROM Proveedor WHERE Codigo = @Codigo";
                            int proveedorID = 0;
                            using (SqlCommand cmdExiste = new SqlCommand(consultaExiste, conexionSQL, transactionSQL))
                            {
                                cmdExiste.Parameters.AddWithValue("@Codigo", nit);
                                object resultado = cmdExiste.ExecuteScalar();
                                if (resultado != null)
                                {
                                    proveedorID = Convert.ToInt32(resultado);
                                    string consultaActualizar = @"UPDATE Proveedor SET NombreComercial = @NombreComercial, Direccion = @Direccion, Telefono = @Telefono, Activo = @Activo WHERE ProveedorID = @ProveedorID";
                                    using (SqlCommand cmdActualizar = new SqlCommand(consultaActualizar, conexionSQL, transactionSQL))
                                    {
                                        cmdActualizar.Parameters.AddWithValue("@NombreComercial", nombreComercial);
                                        cmdActualizar.Parameters.AddWithValue("@Direccion", string.IsNullOrWhiteSpace(direccion) ? DBNull.Value : direccion);
                                        cmdActualizar.Parameters.AddWithValue("@Telefono", string.IsNullOrWhiteSpace(telefono) ? DBNull.Value : telefono);
                                        cmdActualizar.Parameters.AddWithValue("@Activo", activo);
                                        cmdActualizar.Parameters.AddWithValue("@ProveedorID", proveedorID);
                                        cmdActualizar.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    string consultaInsertar = @"INSERT INTO Proveedor (Codigo, NombreComercial, Direccion, Telefono, Activo) OUTPUT INSERTED.ProveedorID VALUES (@Codigo, @NombreComercial, @Direccion, @Telefono, @Activo)";
                                    using (SqlCommand cmdInsertar = new SqlCommand(consultaInsertar, conexionSQL, transactionSQL))
                                    {
                                        cmdInsertar.Parameters.AddWithValue("@Codigo", nit);
                                        cmdInsertar.Parameters.AddWithValue("@NombreComercial", nombreComercial);
                                        cmdInsertar.Parameters.AddWithValue("@Direccion", string.IsNullOrWhiteSpace(direccion) ? DBNull.Value : direccion);
                                        cmdInsertar.Parameters.AddWithValue("@Telefono", string.IsNullOrWhiteSpace(telefono) ? DBNull.Value : telefono);
                                        cmdInsertar.Parameters.AddWithValue("@Activo", activo);
                                        proveedorID = Convert.ToInt32(cmdInsertar.ExecuteScalar());
                                    }
                                }
                            }
                            // Configurar cambios en SQL Server
                            transactionSQL.Commit();

                            // Marcar como sincronizado en MySQL
                            string ActualizarMySQL = "UPDATE proveedor_externo SET Sincronizado = 1, fecha_sincronizacion = NOW() WHERE id_proveedor = @id_proveedor";
                            using (MySqlCommand cmdMySQL = new MySqlCommand(ActualizarMySQL, conexionMySQL))
                            {
                                cmdMySQL.Parameters.AddWithValue("@id_proveedor", idProveedorExterno);
                                cmdMySQL.ExecuteNonQuery();
                            }
                            string insertarLogExitoso = @"INSERT INTO log_sincronizacion (id_proveedor, resultado, mensaje) VALUES (@id_proveedor, 'EXITOSO', @mensaje)";
                            using (MySqlCommand cmdLog = new MySqlCommand(insertarLogExitoso, conexionMySQL))
                            {
                                cmdLog.Parameters.AddWithValue("@id_proveedor", idProveedorExterno);
                                cmdLog.Parameters.AddWithValue("@mensaje", "Sincronización exitosa");
                                cmdLog.ExecuteNonQuery();
                            }
                            exitosos++;
                        }
                        catch (Exception exProveedor)
                        {
                            try
                            {
                                transactionSQL.Rollback();

                            }
                            catch
                            {
                                // Ignorar errores de rollback
                            }
                            fallidos++;
                            // Registrar el error en MySQL
                            string insertarLogError = @"INSERT INTO log_sincronizacion (id_proveedor, resultado, mensaje) VALUES (@id_proveedor, 'FALLIDO', @mensaje)";
                            using (MySqlCommand cmdLogError = new MySqlCommand(insertarLogError, conexionMySQL))
                            {
                                cmdLogError.Parameters.AddWithValue("@id_proveedor", idProveedorExterno);
                                string mensajeError = $"Error al sincronizar proveedor: {exProveedor.Message}";
                                if (mensajeError.Length > 255)
                                {
                                    mensajeError = mensajeError.Substring(0, 255); // Truncar a 255 caracteres
                                }
                                cmdLogError.Parameters.AddWithValue("@mensaje", mensajeError);
                                cmdLogError.ExecuteNonQuery();
                            }
                        }
                    }
                }
                //Regrescar Datagrid
                CargarProveedoresPendientes();
                //Actualizar combobox de proveedores
                CargarProveedores();
                MessageBox.Show($"Sincronización completada. Éxitosos: {exitosos}, Fallidos: {fallidos}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante la sincronización: {ex.Message}");
            }
        }
    }
}