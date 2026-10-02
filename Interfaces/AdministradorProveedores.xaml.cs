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
        private int proveedorID;

        public AdministradorProveedores(int proveedorID)
        {
            InitializeComponent();
            this.proveedorID = proveedorID;
            CargarProveedor();
            CargarArticulosProveedor();
            CargarPedidosOferta();
            CargarOfertasProveedor();
            CargarPortalExterno();
            CargarSincronizacion();
        }

        private void CargarProveedor()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"SELECT Codigo, NombreComercial, Direccion, Telefono, Activo FROM Proveedor WHERE ProveedorID = @ProveedorID";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtNombreProveedor.Text = reader["NombreComercial"]?.ToString() ?? string.Empty;
                            txtProveedorArticulos.Text = reader["NombreComercial"]?.ToString() ?? string.Empty;
                            txtProveedorArticulos.IsReadOnly = true;
                            txtProveedorOfertas.Text = reader["NombreComercial"]?.ToString() ?? string.Empty;
                            txtProveedorOfertas.IsReadOnly = true;
                            txtNIT.Text = reader["Codigo"]?.ToString() ?? string.Empty;
                            txtDireccionProveedor.Text = reader["Direccion"]?.ToString() ?? string.Empty;
                            txtTelefonoProveedor.Text = reader["Telefono"]?.ToString() ?? string.Empty;

                            bool activo = Convert.ToBoolean(reader["Activo"]);
                            cbxEstadoProveedor.SelectedIndex = activo ? 0 : 1;
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el proveedor asociado al usuario.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el proveedor: {ex.Message}");
            }
        }

        private void Button_ActualizarProveedor_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreProveedor.Text))
            {
                MessageBox.Show("Ingrese el nombre del proveedor");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNIT.Text))
            {
                MessageBox.Show("Ingrese el NIT del proveedor");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDireccionProveedor.Text))
            {
                MessageBox.Show("Ingrese la direccion del proveedor");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTelefonoProveedor.Text))
            {
                MessageBox.Show("Ingrese el telefono del proveedor");
                return;
            }

            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"UPDATE Proveedor SET Codigo = @Codigo, NombreComercial = @NombreComercial, Direccion = @Direccion, Telefono = @Telefono WHERE ProveedorID = @ProveedorID";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@Codigo", txtNIT.Text.Trim());
                    cmd.Parameters.AddWithValue("@NombreComercial", txtNombreProveedor.Text.Trim());
                    cmd.Parameters.AddWithValue("@Direccion", txtDireccionProveedor.Text.Trim());
                    cmd.Parameters.AddWithValue("@Telefono", txtTelefonoProveedor.Text.Trim());
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Datos del proveedor actualizados correctamente");
                        CargarProveedor();
                    }
                    else
                    {
                        MessageBox.Show("No se encontro el proveedor");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar el proveedor: {ex.Message}");
            }
        }

        private void Button_DarseDeBaja_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult respuesta = MessageBox.Show("¿Esta seguro de darse de baja como proveedor?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (respuesta != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"UPDATE Proveedor SET Activo = 0 WHERE ProveedorID = @ProveedorID";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Proveedor dado de baja correctamente");
                        CargarProveedor();
                    }
                    else
                    {
                        MessageBox.Show("No se encontro el proveedor");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al dar de baja al proveedor: {ex.Message}");
            }
        }
        private void CargarArticulosProveedor()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                SELECT
                    a.ArticuloID AS codigo,
                    a.Nombre AS articulo,
                    pa.PrecioReferencia AS precio
                FROM ProveedorArticulo pa
                INNER JOIN Articulo a ON pa.ArticuloID = a.ArticuloID
                WHERE pa.ProveedorID = @ProveedorID
                ORDER BY a.Nombre";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    dtgArticulos.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los articulos del proveedor: {ex.Message}");
            }
        }

        private void Button_AgregarArticulo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreArticulo.Text))
            {
                MessageBox.Show("Ingrese el nombre del articulo");
                return;
            }

            if (!decimal.TryParse(txtPrecioReferencia.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("Ingrese un precio de referencia valido");
                return;
            }

            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string buscarArticulo = @"SELECT ArticuloID FROM Articulo WHERE Nombre = @Nombre";

                    SqlCommand cmdBuscar = new SqlCommand(buscarArticulo, conexion);
                    cmdBuscar.Parameters.AddWithValue("@Nombre", txtNombreArticulo.Text.Trim());

                    object resultado = cmdBuscar.ExecuteScalar();

                    if (resultado == null)
                    {
                        MessageBox.Show("El articulo no existe en el catalogo");
                        return;
                    }

                    int articuloID = Convert.ToInt32(resultado);

                    string verificar = @"SELECT COUNT(*) FROM ProveedorArticulo WHERE ProveedorID = @ProveedorID AND ArticuloID = @ArticuloID";

                    SqlCommand cmdVerificar = new SqlCommand(verificar, conexion);
                    cmdVerificar.Parameters.AddWithValue("@ProveedorID", proveedorID);
                    cmdVerificar.Parameters.AddWithValue("@ArticuloID", articuloID);

                    int existe = Convert.ToInt32(cmdVerificar.ExecuteScalar());

                    if (existe > 0)
                    {
                        MessageBox.Show("Este articulo ya esta asociado al proveedor");
                        return;
                    }

                    string insertar = @"INSERT INTO ProveedorArticulo (ProveedorID, ArticuloID, PrecioReferencia) VALUES (@ProveedorID, @ArticuloID, @PrecioReferencia)";

                    SqlCommand cmd = new SqlCommand(insertar, conexion);
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);
                    cmd.Parameters.AddWithValue("@ArticuloID", articuloID);
                    cmd.Parameters.AddWithValue("@PrecioReferencia", precio);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Articulo agregado correctamente");

                    txtNombreArticulo.Clear();
                    txtPrecioReferencia.Clear();

                    CargarArticulosProveedor();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar el articulo: {ex.Message}");
            }
        }
        private void dtgArticulos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dtgArticulos.SelectedItem is DataRowView fila)
            {
                txtNombreArticulo.Text = fila["articulo"]?.ToString() ?? string.Empty;
                txtPrecioReferencia.Text = fila["precio"]?.ToString() ?? string.Empty;
            }
        }

        private void Button_ModificarArticulo_Click(object sender, RoutedEventArgs e)
        {
            if (dtgArticulos.SelectedItem is not DataRowView fila)
            {
                MessageBox.Show("Seleccione un articulo para modificar");
                return;
            }

            if (!decimal.TryParse(txtPrecioReferencia.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("Ingrese un precio de referencia valido");
                return;
            }

            try
            {
                int articuloID = Convert.ToInt32(fila["codigo"]);

                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"UPDATE ProveedorArticulo SET PrecioReferencia = @PrecioReferencia WHERE ProveedorID = @ProveedorID AND ArticuloID = @ArticuloID";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@PrecioReferencia", precio);
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);
                    cmd.Parameters.AddWithValue("@ArticuloID", articuloID);

                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Articulo modificado correctamente");
                        txtNombreArticulo.Clear();
                        txtPrecioReferencia.Clear();
                        CargarArticulosProveedor();
                    }
                    else
                    {
                        MessageBox.Show("No se encontro el articulo asociado al proveedor");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar el articulo: {ex.Message}");
            }
        }

        private void Button_EliminarArticulo_Click(object sender, RoutedEventArgs e)
        {
            if (dtgArticulos.SelectedItem is not DataRowView fila)
            {
                MessageBox.Show("Seleccione un articulo para eliminar");
                return;
            }

            MessageBoxResult respuesta = MessageBox.Show("¿Esta seguro de eliminar este articulo del proveedor?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (respuesta != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                int articuloID = Convert.ToInt32(fila["codigo"]);

                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"DELETE FROM ProveedorArticulo WHERE ProveedorID = @ProveedorID AND ArticuloID = @ArticuloID";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);
                    cmd.Parameters.AddWithValue("@ArticuloID", articuloID);

                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Articulo eliminado correctamente");
                        txtNombreArticulo.Clear();
                        txtPrecioReferencia.Clear();
                        CargarArticulosProveedor();
                    }
                    else
                    {
                        MessageBox.Show("No se encontro el articulo asociado al proveedor");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el articulo: {ex.Message}");
            }
        }
        private void CargarPedidosOferta()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                SELECT DISTINCT
                    p.PedidoID,
                    CONCAT('Pedido ', p.PedidoID, ' - ', a.Nombre) AS Descripcion
                FROM PedidoInterno p
                INNER JOIN Articulo a ON p.ArticuloID = a.ArticuloID
                INNER JOIN OrdenPedido op ON p.PedidoID = op.PedidoID
                INNER JOIN OrdenCompra oc ON op.OrdenID = oc.OrdenID
                INNER JOIN ProveedorArticulo pa ON p.ArticuloID = pa.ArticuloID
                WHERE pa.ProveedorID = @ProveedorID
                AND oc.FechaLimiteOfertas >= CAST(GETDATE() AS DATE)
                ORDER BY p.PedidoID";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    cmbPedidoOferta.ItemsSource = dt.DefaultView;
                    cmbPedidoOferta.DisplayMemberPath = "Descripcion";
                    cmbPedidoOferta.SelectedValuePath = "PedidoID";
                    cmbPedidoOferta.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los pedidos disponibles: {ex.Message}");
            }
        }

        private void CargarOfertasProveedor()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                SELECT
                    o.OfertaID AS ofertaID,
                    p.PedidoID AS pedidoID,
                    CONCAT('Pedido ', p.PedidoID) AS pedido,
                    a.Nombre AS articulo,
                    o.PrecioUnitario AS precioU,
                    o.FechaOferta AS fechaO
                FROM Oferta o
                INNER JOIN PedidoInterno p ON o.PedidoID = p.PedidoID
                INNER JOIN Articulo a ON p.ArticuloID = a.ArticuloID
                WHERE o.ProveedorID = @ProveedorID
                ORDER BY o.OfertaID DESC";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    dtgOfertas.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las ofertas del proveedor: {ex.Message}");
            }
        }

        private void dtgOfertas_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dtgOfertas.SelectedItem is DataRowView fila)
            {
                cmbPedidoOferta.SelectedValue = Convert.ToInt32(fila["pedidoID"]);
                txtPrecioOferta.Text = fila["precioU"]?.ToString() ?? string.Empty;
            }
        }

        private void Button_AgregarOferta_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPedidoOferta.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un pedido");
                return;
            }

            if (!decimal.TryParse(txtPrecioOferta.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("Ingrese un precio unitario valido mayor a 0");
                return;
            }

            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"INSERT INTO Oferta (ProveedorID, PedidoID, PrecioUnitario, FechaOferta) VALUES (@ProveedorID, @PedidoID, @PrecioUnitario, @FechaOferta)";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);
                    cmd.Parameters.AddWithValue("@PedidoID", cmbPedidoOferta.SelectedValue);
                    cmd.Parameters.AddWithValue("@PrecioUnitario", precio);
                    cmd.Parameters.AddWithValue("@FechaOferta", DateTime.Today);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Oferta registrada correctamente");

                    cmbPedidoOferta.SelectedIndex = -1;
                    txtPrecioOferta.Clear();

                    CargarOfertasProveedor();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Ya registro una oferta para este pedido");
                }
                else
                {
                    MessageBox.Show($"Error al registrar la oferta: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar la oferta: {ex.Message}");
            }
        }

        private void Button_ModificarOferta_Click(object sender, RoutedEventArgs e)
        {
            if (dtgOfertas.SelectedItem is not DataRowView fila)
            {
                MessageBox.Show("Seleccione una oferta para modificar");
                return;
            }

            if (!decimal.TryParse(txtPrecioOferta.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("Ingrese un precio unitario valido mayor a 0");
                return;
            }

            try
            {
                int ofertaID = Convert.ToInt32(fila["ofertaID"]);

                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"UPDATE Oferta SET PrecioUnitario = @PrecioUnitario WHERE OfertaID = @OfertaID AND ProveedorID = @ProveedorID";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@PrecioUnitario", precio);
                    cmd.Parameters.AddWithValue("@OfertaID", ofertaID);
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Oferta modificada correctamente");

                        cmbPedidoOferta.SelectedIndex = -1;
                        txtPrecioOferta.Clear();

                        CargarOfertasProveedor();
                    }
                    else
                    {
                        MessageBox.Show("No se encontro la oferta");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar la oferta: {ex.Message}");
            }
        }

        private void Button_EliminarOferta_Click(object sender, RoutedEventArgs e)
        {
            if (dtgOfertas.SelectedItem is not DataRowView fila)
            {
                MessageBox.Show("Seleccione una oferta para eliminar");
                return;
            }

            MessageBoxResult respuesta = MessageBox.Show("¿Esta seguro de eliminar esta oferta?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (respuesta != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                int ofertaID = Convert.ToInt32(fila["ofertaID"]);

                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"DELETE FROM Oferta WHERE OfertaID = @OfertaID AND ProveedorID = @ProveedorID";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@OfertaID", ofertaID);
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Oferta eliminada correctamente");

                        cmbPedidoOferta.SelectedIndex = -1;
                        txtPrecioOferta.Clear();

                        CargarOfertasProveedor();
                    }
                    else
                    {
                        MessageBox.Show("No se encontro la oferta");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la oferta: {ex.Message}");
            }
        }
        private void CargarPortalExterno()
        {
            try
            {
                string nitProveedor = string.Empty;

                using (SqlConnection conexionSQL = Conexion.ObtenerConexionSQLServer())
                {
                    conexionSQL.Open();

                    string consultaSQL = @"SELECT Codigo FROM Proveedor WHERE ProveedorID = @ProveedorID";

                    SqlCommand cmdSQL = new SqlCommand(consultaSQL, conexionSQL);
                    cmdSQL.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    object resultado = cmdSQL.ExecuteScalar();

                    if (resultado == null)
                    {
                        MessageBox.Show("No se encontro el proveedor en la base de datos principal");
                        return;
                    }

                    nitProveedor = resultado.ToString() ?? string.Empty;
                }

                using (MySqlConnection conexionMySQL = Conexion.ObtenerConexionMySQL())
                {
                    conexionMySQL.Open();

                    string consulta = @"
                SELECT
                    pe.nit AS nit,
                    pe.nombre_comercial AS nombre,
                    ae.nombre AS articulo,
                    pae.precio_ofrecido AS precio,
                    pae.fecha_actualizado AS fecha,
                    CASE
                        WHEN pe.sincronizado = 1 THEN 'Sí'
                        ELSE 'No'
                    END AS sincronizado
                FROM proveedor_externo pe
                LEFT JOIN proveedor_articulo_externo pae ON pe.id_proveedor = pae.id_proveedor
                LEFT JOIN articulo_externo ae ON pae.id_articulo = ae.id_articulo
                WHERE pe.nit = @nit
                ORDER BY ae.nombre";

                    MySqlCommand cmd = new MySqlCommand(consulta, conexionMySQL);
                    cmd.Parameters.AddWithValue("@nit", nitProveedor);

                    DataTable dt = new DataTable();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);

                    dtgPortalExterno.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos del portal externo: {ex.Message}");
            }
        }

        private void CargarSincronizacion()
        {
            try
            {
                string nit = string.Empty;
                string nombreSQL = string.Empty;

                using (SqlConnection conexionSQL = Conexion.ObtenerConexionSQLServer())
                {
                    conexionSQL.Open();

                    string consultaSQL = @"SELECT Codigo, NombreComercial FROM Proveedor WHERE ProveedorID = @ProveedorID";

                    SqlCommand cmdSQL = new SqlCommand(consultaSQL, conexionSQL);
                    cmdSQL.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    using (SqlDataReader reader = cmdSQL.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            nit = reader["Codigo"]?.ToString() ?? string.Empty;
                            nombreSQL = reader["NombreComercial"]?.ToString() ?? string.Empty;
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                using (MySqlConnection conexionMySQL = Conexion.ObtenerConexionMySQL())
                {
                    conexionMySQL.Open();

                    string consultaMySQL = @"SELECT nombre_comercial FROM proveedor_externo WHERE nit = @nit";

                    MySqlCommand cmdMySQL = new MySqlCommand(consultaMySQL, conexionMySQL);
                    cmdMySQL.Parameters.AddWithValue("@nit", nit);

                    object resultado = cmdMySQL.ExecuteScalar();
                    string nombreMySQL = resultado?.ToString() ?? "No encontrado";

                    DataTable dt = new DataTable();
                    dt.Columns.Add("NSQL");
                    dt.Columns.Add("NMySQL");

                    dt.Rows.Add(nombreSQL, nombreMySQL);

                    dtgSincronizacionProv.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la comparacion de sincronizacion: {ex.Message}");
            }
        }

        private void Button_Sincronizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nit = string.Empty;

                using (SqlConnection conexionSQL = Conexion.ObtenerConexionSQLServer())
                {
                    conexionSQL.Open();

                    string consultaNIT = @"SELECT Codigo FROM Proveedor WHERE ProveedorID = @ProveedorID";

                    SqlCommand cmdNIT = new SqlCommand(consultaNIT, conexionSQL);
                    cmdNIT.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    object resultadoNIT = cmdNIT.ExecuteScalar();

                    if (resultadoNIT == null)
                    {
                        MessageBox.Show("No se encontro el proveedor en SQL Server");
                        return;
                    }

                    nit = resultadoNIT.ToString() ?? string.Empty;
                }

                using (MySqlConnection conexionMySQL = Conexion.ObtenerConexionMySQL())
                {
                    conexionMySQL.Open();

                    string consultaProveedor = @"SELECT id_proveedor, nombre_comercial, direccion, telefono, activo FROM proveedor_externo WHERE nit = @nit";

                    MySqlCommand cmdProveedor = new MySqlCommand(consultaProveedor, conexionMySQL);
                    cmdProveedor.Parameters.AddWithValue("@nit", nit);

                    DataTable dtProveedor = new DataTable();
                    MySqlDataAdapter daProveedor = new MySqlDataAdapter(cmdProveedor);
                    daProveedor.Fill(dtProveedor);

                    if (dtProveedor.Rows.Count == 0)
                    {
                        MessageBox.Show("No existe un registro externo para este proveedor");
                        return;
                    }

                    DataRow proveedorExterno = dtProveedor.Rows[0];

                    int idProveedorExterno = Convert.ToInt32(proveedorExterno["id_proveedor"]);
                    string nombre = proveedorExterno["nombre_comercial"]?.ToString() ?? string.Empty;
                    string direccion = proveedorExterno["direccion"]?.ToString() ?? string.Empty;
                    string telefono = proveedorExterno["telefono"]?.ToString() ?? string.Empty;
                    bool activo = Convert.ToBoolean(proveedorExterno["activo"]);

                    using (SqlConnection conexionSQL = Conexion.ObtenerConexionSQLServer())
                    {
                        conexionSQL.Open();

                        SqlTransaction transaccion = conexionSQL.BeginTransaction();

                        try
                        {
                            string actualizarProveedor = @"UPDATE Proveedor SET NombreComercial = @NombreComercial, Direccion = @Direccion, Telefono = @Telefono, Activo = @Activo WHERE ProveedorID = @ProveedorID";

                            SqlCommand cmdActualizar = new SqlCommand(actualizarProveedor, conexionSQL, transaccion);
                            cmdActualizar.Parameters.AddWithValue("@NombreComercial", nombre);
                            cmdActualizar.Parameters.AddWithValue("@Direccion", string.IsNullOrWhiteSpace(direccion) ? DBNull.Value : direccion);
                            cmdActualizar.Parameters.AddWithValue("@Telefono", string.IsNullOrWhiteSpace(telefono) ? DBNull.Value : telefono);
                            cmdActualizar.Parameters.AddWithValue("@Activo", activo);
                            cmdActualizar.Parameters.AddWithValue("@ProveedorID", proveedorID);
                            cmdActualizar.ExecuteNonQuery();

                            string consultaArticulos = @"
                        SELECT
                            ae.codigo,
                            ae.nombre,
                            ae.descripcion,
                            pae.precio_ofrecido
                        FROM proveedor_articulo_externo pae
                        INNER JOIN articulo_externo ae ON pae.id_articulo = ae.id_articulo
                        WHERE pae.id_proveedor = @id_proveedor";

                            MySqlCommand cmdArticulos = new MySqlCommand(consultaArticulos, conexionMySQL);
                            cmdArticulos.Parameters.AddWithValue("@id_proveedor", idProveedorExterno);

                            DataTable dtArticulos = new DataTable();
                            MySqlDataAdapter daArticulos = new MySqlDataAdapter(cmdArticulos);
                            daArticulos.Fill(dtArticulos);

                            foreach (DataRow fila in dtArticulos.Rows)
                            {
                                string codigoArticulo = fila["codigo"]?.ToString() ?? string.Empty;
                                string nombreArticulo = fila["nombre"]?.ToString() ?? string.Empty;
                                string descripcion = fila["descripcion"]?.ToString() ?? string.Empty;
                                decimal precio = Convert.ToDecimal(fila["precio_ofrecido"]);

                                string buscarArticulo = @"SELECT ArticuloID FROM Articulo WHERE Codigo = @Codigo";

                                SqlCommand cmdBuscar = new SqlCommand(buscarArticulo, conexionSQL, transaccion);
                                cmdBuscar.Parameters.AddWithValue("@Codigo", codigoArticulo);

                                object resultadoArticulo = cmdBuscar.ExecuteScalar();
                                int articuloID;

                                if (resultadoArticulo == null)
                                {
                                    string insertarArticulo = @"INSERT INTO Articulo (Codigo, Nombre, Descripcion) OUTPUT INSERTED.ArticuloID VALUES (@Codigo, @Nombre, @Descripcion)";

                                    SqlCommand cmdInsertarArticulo = new SqlCommand(insertarArticulo, conexionSQL, transaccion);
                                    cmdInsertarArticulo.Parameters.AddWithValue("@Codigo", codigoArticulo);
                                    cmdInsertarArticulo.Parameters.AddWithValue("@Nombre", nombreArticulo);
                                    cmdInsertarArticulo.Parameters.AddWithValue("@Descripcion", string.IsNullOrWhiteSpace(descripcion) ? DBNull.Value : descripcion);

                                    articuloID = Convert.ToInt32(cmdInsertarArticulo.ExecuteScalar());
                                }
                                else
                                {
                                    articuloID = Convert.ToInt32(resultadoArticulo);
                                }

                                string verificarRelacion = @"SELECT COUNT(*) FROM ProveedorArticulo WHERE ProveedorID = @ProveedorID AND ArticuloID = @ArticuloID";

                                SqlCommand cmdVerificar = new SqlCommand(verificarRelacion, conexionSQL, transaccion);
                                cmdVerificar.Parameters.AddWithValue("@ProveedorID", proveedorID);
                                cmdVerificar.Parameters.AddWithValue("@ArticuloID", articuloID);

                                int existe = Convert.ToInt32(cmdVerificar.ExecuteScalar());

                                if (existe > 0)
                                {
                                    string actualizarPrecio = @"UPDATE ProveedorArticulo SET PrecioReferencia = @Precio WHERE ProveedorID = @ProveedorID AND ArticuloID = @ArticuloID";

                                    SqlCommand cmdPrecio = new SqlCommand(actualizarPrecio, conexionSQL, transaccion);
                                    cmdPrecio.Parameters.AddWithValue("@Precio", precio);
                                    cmdPrecio.Parameters.AddWithValue("@ProveedorID", proveedorID);
                                    cmdPrecio.Parameters.AddWithValue("@ArticuloID", articuloID);
                                    cmdPrecio.ExecuteNonQuery();
                                }
                                else
                                {
                                    string insertarRelacion = @"INSERT INTO ProveedorArticulo (ProveedorID, ArticuloID, PrecioReferencia) VALUES (@ProveedorID, @ArticuloID, @Precio)";

                                    SqlCommand cmdRelacion = new SqlCommand(insertarRelacion, conexionSQL, transaccion);
                                    cmdRelacion.Parameters.AddWithValue("@ProveedorID", proveedorID);
                                    cmdRelacion.Parameters.AddWithValue("@ArticuloID", articuloID);
                                    cmdRelacion.Parameters.AddWithValue("@Precio", precio);
                                    cmdRelacion.ExecuteNonQuery();
                                }
                            }

                            transaccion.Commit();
                        }
                        catch
                        {
                            transaccion.Rollback();
                            throw;
                        }
                    }

                    string marcarSincronizado = @"UPDATE proveedor_externo SET sincronizado = 1, fecha_sincronizacion = NOW() WHERE id_proveedor = @id_proveedor";

                    MySqlCommand cmdSincronizado = new MySqlCommand(marcarSincronizado, conexionMySQL);
                    cmdSincronizado.Parameters.AddWithValue("@id_proveedor", idProveedorExterno);
                    cmdSincronizado.ExecuteNonQuery();

                    string insertarLog = @"INSERT INTO log_sincronizacion (id_proveedor, resultado, mensaje) VALUES (@id_proveedor, 'EXITOSO', @mensaje)";

                    MySqlCommand cmdLog = new MySqlCommand(insertarLog, conexionMySQL);
                    cmdLog.Parameters.AddWithValue("@id_proveedor", idProveedorExterno);
                    cmdLog.Parameters.AddWithValue("@mensaje", "Proveedor y articulos sincronizados correctamente");
                    cmdLog.ExecuteNonQuery();

                    MessageBox.Show("Sincronizacion completada correctamente");

                    CargarProveedor();
                    CargarArticulosProveedor();
                    CargarPortalExterno();
                    CargarSincronizacion();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante la sincronizacion: {ex.Message}");
            }
        }

        private void Button_ReporteArticulos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                SELECT
                    p.Codigo AS NIT,
                    p.NombreComercial AS Proveedor,
                    a.Codigo AS CodigoArticulo,
                    a.Nombre AS Articulo,
                    a.Descripcion AS Descripcion,
                    pa.PrecioReferencia AS PrecioReferencia
                FROM ProveedorArticulo pa
                INNER JOIN Proveedor p ON pa.ProveedorID = p.ProveedorID
                INNER JOIN Articulo a ON pa.ArticuloID = a.ArticuloID
                WHERE pa.ProveedorID = @ProveedorID
                ORDER BY a.Nombre";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    dtgReportes.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte de articulos: {ex.Message}");
            }
        }

        private void Button_ReporteOfertas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                SELECT
                    o.OfertaID AS Oferta,
                    p.PedidoID AS Pedido,
                    a.Nombre AS Articulo,
                    p.Cantidad AS Cantidad,
                    o.PrecioUnitario AS PrecioUnitario,
                    o.FechaOferta AS FechaOferta
                FROM Oferta o
                INNER JOIN PedidoInterno p ON o.PedidoID = p.PedidoID
                INNER JOIN Articulo a ON p.ArticuloID = a.ArticuloID
                WHERE o.ProveedorID = @ProveedorID
                ORDER BY o.FechaOferta DESC, o.OfertaID DESC";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    dtgReportes.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte de ofertas: {ex.Message}");
            }
        }

        private void Button_ReporteSincronizacion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nit = string.Empty;

                using (SqlConnection conexionSQL = Conexion.ObtenerConexionSQLServer())
                {
                    conexionSQL.Open();

                    string consultaNIT = @"SELECT Codigo FROM Proveedor WHERE ProveedorID = @ProveedorID";

                    SqlCommand cmdNIT = new SqlCommand(consultaNIT, conexionSQL);
                    cmdNIT.Parameters.AddWithValue("@ProveedorID", proveedorID);

                    object resultado = cmdNIT.ExecuteScalar();

                    if (resultado == null)
                    {
                        MessageBox.Show("No se encontro el proveedor");
                        return;
                    }

                    nit = resultado.ToString() ?? string.Empty;
                }

                using (MySqlConnection conexionMySQL = Conexion.ObtenerConexionMySQL())
                {
                    conexionMySQL.Open();

                    string consulta = @"
                SELECT
                    l.id_log AS Registro,
                    pe.nit AS NIT,
                    pe.nombre_comercial AS Proveedor,
                    l.fecha_sincronizacion AS FechaSincronizacion,
                    l.resultado AS Resultado,
                    l.mensaje AS Mensaje
                FROM log_sincronizacion l
                INNER JOIN proveedor_externo pe ON l.id_proveedor = pe.id_proveedor
                WHERE pe.nit = @nit
                ORDER BY l.fecha_sincronizacion DESC";

                    MySqlCommand cmd = new MySqlCommand(consulta, conexionMySQL);
                    cmd.Parameters.AddWithValue("@nit", nit);

                    DataTable dt = new DataTable();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);

                    dtgReportes.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte de sincronizacion: {ex.Message}");
            }
        }
    }
}