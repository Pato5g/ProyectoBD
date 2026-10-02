using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace ProyectoBD.Interfaces
{
    public partial class GestorCompras
    {
        private int pedidoSeleccionadoID = 0;
        private int ordenSeleccionadaID = 0;
        private List<int> pedidosOrdenSeleccionados = new List<int>();
        public GestorCompras()
        {
            InitializeComponent();
            CargarDepartamentos();
            CargarArticulos();
            CargarPedidos();
            CargarOrdenes();
            CargarPedidosOferta();
            CargarProveedoresOferta();
            CargarOfertas();
            CargarOrdenesAdjudicacion();
            CargarAdjudicaciones();
            CargarPedidosPendientes();
            CargarOrdenesActivas();
        }
        private void CargarDepartamentos()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT DepartamentoID, Nombre FROM Departamento ORDER BY Nombre";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    cbxDepartamento.ItemsSource = dt.DefaultView;
                    cbxDepartamento.DisplayMemberPath = "Nombre";
                    cbxDepartamento.SelectedValuePath = "DepartamentoID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los departamentos: {ex.Message}");
            }
        }
        private void CargarArticulos()
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
                    cbxArticulo.ItemsSource = dt.DefaultView;
                    cbxArticulo.DisplayMemberPath = "Nombre";
                    cbxArticulo.SelectedValuePath = "ArticuloID"; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los articulos: {ex.Message}");
            }
        }
        private void CargarPedidos()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT p.PedidoID AS pedidoId, d.Nombre AS departamento, a.Nombre AS articulo, p.Cantidad AS cantidad, p.FechaSolicitud AS fecha_solicitud, p.FechaNecesidad AS fecha_necesidad
                                        FROM PedidoInterno p INNER JOIN Departamento d ON p.DepartamentoID = d.DepartamentoID INNER JOIN Articulo a ON p.ArticuloID = a.ArticuloID ORDER BY p.PedidoID DESC";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    dtgSincronizacion.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los pedidos: {ex.Message}");
            }
        }

        private void Button_AgregarPedido_Click(object sender, RoutedEventArgs e)
        {
            // validacion departamento
            if (cbxDepartamento.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un departamento.");
                return;
            }
            // validacion articulo
            if (cbxArticulo.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un articulo");
                return;
            }
            // validacion cantidad
            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad valida mayor a 0");
                return;
            }
            // validacion fechas
            if(dtpFechaSolicitud.SelectedDate == null || dtpFechaNecesidad.SelectedDate == null)
            {
                MessageBox.Show("Seleccione la fecha de la solicitud y la fecha de necesidad");
                return;
            }
            // necesidad no puede ser anterior a la solicitud
            if (dtpFechaNecesidad.SelectedDate < dtpFechaSolicitud.SelectedDate)
            {
                MessageBox.Show("La fecha de necesidad no puede ser anterior a la fecha de solicitud");
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"INSERT INTO PedidoInterno (DepartamentoID, ArticuloID, Cantidad, FechaSolicitud, FechaNecesidad) VALUES (@DepartamentoID, @ArticuloID, @Cantidad, @FechaSolicitud, @FechaNecesidad)";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@DepartamentoID", cbxDepartamento.SelectedValue);
                    cmd.Parameters.AddWithValue("@ArticuloID", cbxArticulo.SelectedValue);
                    cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@FechaSolicitud", dtpFechaSolicitud.SelectedDate.Value);
                    cmd.Parameters.AddWithValue("@FechaNecesidad", dtpFechaNecesidad.SelectedDate.Value);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Pedido agregado correctamente");
                    CargarPedidos();
                    LimpiarCamposPedido();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar el pedido {ex.Message}");
            }
        }
        private void LimpiarCamposPedido()
        {
            cbxDepartamento.SelectedIndex = -1;
            cbxArticulo.SelectedIndex = -1;
            txtCantidad.Clear();
            dtpFechaSolicitud.SelectedDate = null;
            dtpFechaNecesidad.SelectedDate = null;

            pedidoSeleccionadoID = 0;
            dtgSincronizacion.SelectedItem = null;
        }

        private void Button_LimpiarCampos_Click(object sender, RoutedEventArgs e)
        {
            LimpiarCamposPedido();
        }
        private void dtgPedidos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dtgSincronizacion.SelectedItem is DataRowView fila)
            {
                pedidoSeleccionadoID = Convert.ToInt32(fila["pedidoID"]);
                cbxDepartamento.Text = fila["departamento"]?.ToString() ?? string.Empty;
                cbxArticulo.Text = fila["articulo"]?.ToString() ?? string.Empty;
                txtCantidad.Text = fila["cantidad"]?.ToString() ?? string.Empty;
                if (fila["fecha_solicitud"] != DBNull.Value)
                {
                    dtpFechaSolicitud.SelectedDate = Convert.ToDateTime(fila["fecha_solicitud"]);
                }
                if (fila["fecha_necesidad"] != DBNull.Value)
                {
                    dtpFechaNecesidad.SelectedDate = Convert.ToDateTime(fila["fecha_necesidad"]);
                }
            }
        }

        private void Button_ActualizarPedido_Click(object sender, RoutedEventArgs e)
        {
            if (pedidoSeleccionadoID == 0)
            {
                MessageBox.Show("Seleccione un pedido para actualizar");
                return;
            }
            if (cbxDepartamento.SelectedValue == null || cbxArticulo.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un departamento y un articulo");
                return;
            }
            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad valida mayor a 0");
                return;
            }
            if (dtpFechaSolicitud.SelectedDate == null || dtpFechaNecesidad.SelectedDate == null)
            {
                MessageBox.Show("Seleccione ambas fechas");
                return;
            }
            if (dtpFechaNecesidad.SelectedDate < dtpFechaSolicitud.SelectedDate)
            {
                MessageBox.Show("La fecha de necesidad no puede ser anterior a la fecha de solicitud");
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"UPDATE PedidoInterno SET DepartamentoID = @DepartamentoID, ArticuloID = @ArticuloID, Cantidad = @Cantidad, FechaSolicitud = @FechaSolicitud, FechaNecesidad = @FechaNecesidad
                                        WHERE PedidoID = @PedidoID";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@DepartamentoID", cbxDepartamento.SelectedValue);
                    cmd.Parameters.AddWithValue("@ArticuloID", cbxArticulo.SelectedValue);
                    cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@FechaSolicitud", dtpFechaSolicitud.SelectedDate.Value);
                    cmd.Parameters.AddWithValue("@FechaNecesidad", dtpFechaNecesidad.SelectedDate.Value);
                    cmd.Parameters.AddWithValue("@PedidoID", pedidoSeleccionadoID);
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Pedido actualizado correctamente.");
                        CargarPedidos();
                        LimpiarCamposPedido();
                        pedidoSeleccionadoID = 0;
                    }
                    else
                    {
                        MessageBox.Show("No se encontro el pedido.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar el pedido {ex.Message}");
            }
        }

        private void Button_EliminarPedido_Click(object sender, RoutedEventArgs e)
        {
            if (pedidoSeleccionadoID == 0)
            {
                MessageBox.Show("Seleccione un pedido para eliminar");
                return;
            }
            MessageBoxResult respuesta = MessageBox.Show("Esta seguro de eliminar este pedido?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (respuesta != MessageBoxResult.Yes)
            {
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"DELETE FROM PedidoInterno WHERE PedidoID = @PedidoID";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@PedidoID", pedidoSeleccionadoID);
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Pedido eliminado correctamente");
                        CargarPedidos();
                        LimpiarCamposPedido();
                        pedidoSeleccionadoID = 0;
                    }
                    else
                    {
                        MessageBox.Show("No se encontro el pedido");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el pedido: {ex.Message}");
            }
        }

        private void Button_SeleccionarPedidos_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarPedidos ventana;
            if (ordenSeleccionadaID > 0)
            {
                ventana = new SeleccionarPedidos(ordenSeleccionadaID);
            }
            else
            {
                ventana = new SeleccionarPedidos();
            }
            bool? resultado = ventana.ShowDialog();
            if (resultado == true)
            {
                pedidosOrdenSeleccionados = ventana.PedidosSeleccionados;
                txtPedidosSeleccionados.Text = pedidosOrdenSeleccionados.Count.ToString();
            }
        }
        private void CargarOrdenes()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"
                                        SELECT
                                            OrdenID AS ordenID,
                                            Descripcion AS descripcion,
                                            FechaCreacion AS fecha_creacion,
                                            FechaLimiteOfertas AS fecha_limite_oferta,
                                            TipoOrden AS tipo_orden,
                                            SubTipoOrden AS subtipo_orden
                                        FROM OrdenCompra
                                        ORDER BY OrdenID DESC;";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    dtgOrdenCompra.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las ordenes: {ex.Message}");
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
                ORDER BY p.PedidoID;";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);

                    cmbPedido.ItemsSource = dt.DefaultView;
                    cmbPedido.DisplayMemberPath = "Descripcion";
                    cmbPedido.SelectedValuePath = "PedidoID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los pedidos para ofertas: {ex.Message}");
            }
        }

        private void CargarProveedoresOferta()
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
                MessageBox.Show($"Error al cargar los proveedores para ofertas: {ex.Message}");
            }
        }

        private void CargarOfertas()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                SELECT
                    o.OfertaID AS ofertaID,
                    pr.NombreComercial AS proveedores,
                    CONCAT('Pedido ', p.PedidoID, ' - ', a.Nombre) AS pedido,
                    o.PrecioUnitario AS precio_unitario,
                    o.FechaOferta AS fecha_oferta
                FROM Oferta o
                INNER JOIN Proveedor pr ON o.ProveedorID = pr.ProveedorID
                INNER JOIN PedidoInterno p ON o.PedidoID = p.PedidoID
                INNER JOIN Articulo a ON p.ArticuloID = a.ArticuloID
                ORDER BY o.OfertaID DESC;";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);

                    dtgOfertas.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las ofertas: {ex.Message}");
            }
        }

        private void dtgOrdenCompra_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dtgOrdenCompra.SelectedItem is DataRowView fila)
            {
                ordenSeleccionadaID = Convert.ToInt32(fila["ordenID"]);
                txtDescripcion.Text = fila["descripcion"]?.ToString() ?? string.Empty;
                if (fila["fecha_creacion"] != DBNull.Value)
                {
                    dtpFechaCreacion.SelectedDate = Convert.ToDateTime(fila["fecha_creacion"]);
                }
                if (fila["fecha_limite_oferta"] != DBNull.Value)
                {
                    dtpFechaLimite.SelectedDate = Convert.ToDateTime(fila["fecha_limite_oferta"]);
                }
                SeleccionarItemCombo(cmbTipo, fila["tipo_orden"]?.ToString());
                SeleccionarItemCombo(cmbSubtipo, fila["subtipo_orden"] == DBNull.Value ? null : fila["subtipo_orden"].ToString());
                CargarPedidosDeOrden(ordenSeleccionadaID);
            }
        }
        private void SeleccionarItemCombo(System.Windows.Controls.ComboBox combo, string? valor)
        {
            combo.SelectedIndex = -1;
            if (string.IsNullOrEmpty(valor))
                return;
            foreach (System.Windows.Controls.ComboBoxItem item in combo.Items)
            {
                if (item.Content.ToString() == valor)
                {
                    combo.SelectedItem = item;
                    break;
                }
            }
        }

        private void CargarPedidosDeOrden (int ordenID)
        {
            try
            {
                pedidosOrdenSeleccionados.Clear();
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT PedidoID FROM OrdenPedido WHERE OrdenID = @OrdenID;";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@OrdenID", ordenID);
                    using (SqlDataReader reader= cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pedidosOrdenSeleccionados.Add(Convert.ToInt32(reader["PedidoID"]));
                        }    
                    }    
                }
                txtPedidosSeleccionados.Text = pedidosOrdenSeleccionados.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los pedidos de la orden: {ex.Message}");
            }
        }

        private void CargarOrdenesAdjudicacion()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                SELECT
                    oc.OrdenID,
                    CONCAT('Orden ', oc.OrdenID, ' - ', oc.Descripcion) AS Descripcion
                FROM OrdenCompra oc
                LEFT JOIN Adjudicacion ad ON oc.OrdenID = ad.OrdenID
                WHERE ad.AdjudicacionID IS NULL
                ORDER BY oc.OrdenID;";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);

                    cmbOrden.ItemsSource = dt.DefaultView;
                    cmbOrden.DisplayMemberPath = "Descripcion";
                    cmbOrden.SelectedValuePath = "OrdenID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las ordenes para adjudicacion: {ex.Message}");
            }
        }

        private void Button_AgregarOrden_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("Ingrese una descripccion");
                return;
            }
            if (dtpFechaCreacion.SelectedDate == null || dtpFechaLimite.SelectedDate == null)
            {
                MessageBox.Show("Seleccione las fechas de la orden");
                return;
            }
            if (dtpFechaLimite.SelectedDate < dtpFechaCreacion.SelectedDate)
            {
                MessageBox.Show("La fecha limite no puede ser anterior a la fecha de creacion");
                return;
            }
            if (cmbTipo.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un tipo de orden");
                return;
            }
            string tipo = ((System.Windows.Controls.ComboBoxItem)cmbTipo.SelectedItem).Content.ToString() ?? string.Empty;
            string? subtipo = null;
            if (tipo == "Chica")
            {
                if (cmbSubtipo.SelectedItem == null)
                {
                    MessageBox.Show("Las ordenes chicas deben tener un subtipo");
                    return;
                }
                subtipo = ((System.Windows.Controls.ComboBoxItem)cmbSubtipo.SelectedItem).Content.ToString() ?? string.Empty;
            }
            if (pedidosOrdenSeleccionados.Count == 0)
            {
                MessageBox.Show("Seleccione al menos un pedido para la orden");
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    SqlTransaction transaccion = conexion.BeginTransaction();
                    try
                    {
                        string consultaOrden = @"
                                                INSERT INTO OrdenCompra
                                                (
                                                    Descripcion,
                                                    FechaCreacion,
                                                    FechaLimiteOfertas,
                                                    TipoOrden,
                                                    SubTipoOrden
                                                )
                                                OUTPUT INSERTED.OrdenID
                                                VALUES
                                                (
                                                    @Descripcion,
                                                    @FechaCreacion,
                                                    @FechaLimite,
                                                    @TipoOrden,
                                                    @SubTipoOrden
                                                );";
                        SqlCommand cmdOrden = new SqlCommand(consultaOrden, conexion, transaccion);
                        cmdOrden.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text.Trim());
                        cmdOrden.Parameters.AddWithValue("@FechaCreacion", dtpFechaCreacion.SelectedDate.Value);
                        cmdOrden.Parameters.AddWithValue("@FechaLimite", dtpFechaLimite.SelectedDate.Value);
                        cmdOrden.Parameters.AddWithValue("@TipoOrden", tipo);
                        cmdOrden.Parameters.AddWithValue("@SubTipoOrden", (object?)subtipo ?? DBNull.Value);
                        int nuevaOrdenID = Convert.ToInt32(cmdOrden.ExecuteScalar());
                        // relacionar todos los pedidos seleccionados
                        foreach (int pedidoID in pedidosOrdenSeleccionados)
                        {
                            string consultaPedido = @"INSERT INTO OrdenPedido (OrdenID, PedidoID) VALUES (@OrdenID, @PedidoID);";
                            SqlCommand cmdPedido = new SqlCommand(consultaPedido, conexion, transaccion);
                            cmdPedido.Parameters.AddWithValue("@OrdenID", nuevaOrdenID);
                            cmdPedido.Parameters.AddWithValue("@PedidoID", pedidoID);
                            cmdPedido.ExecuteNonQuery();
                        }
                        transaccion.Commit();
                        MessageBox.Show("Orden de compra agregada correctamente");
                        CargarOrdenes();
                        LimpiarCamposOrden();
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar la orden: {ex.Message}");
            }
        }
        private void LimpiarCamposOrden()
        {
            txtDescripcion.Clear();
            dtpFechaCreacion.SelectedDate = null;
            dtpFechaLimite.SelectedDate = null;
            cmbTipo.SelectedIndex = -1;
            cmbSubtipo.SelectedIndex = -1;
            pedidosOrdenSeleccionados.Clear();
            txtPedidosSeleccionados.Text = "0";
            ordenSeleccionadaID = 0;
            dtgOrdenCompra.SelectedItem = null;
        }

        private void Button_LimpiarOrden_Click(object sender, RoutedEventArgs e)
        {
            LimpiarCamposOrden();
        }

        private void Button_ActualizarOrden_Click(object sender, RoutedEventArgs e)
        {
            if (ordenSeleccionadaID == 0)
            {
                MessageBox.Show("Seleccione una orden para actualizar.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("Ingrese una descripción.");
                return;
            }
            if (dtpFechaCreacion.SelectedDate == null ||dtpFechaLimite.SelectedDate == null)
            {
                MessageBox.Show("Seleccione las fechas de la orden.");
                return;
            }
            if (dtpFechaLimite.SelectedDate < dtpFechaCreacion.SelectedDate)
            {
                MessageBox.Show("La fecha límite no puede ser anterior a la fecha de creación.");
                return;
            }
            if (cmbTipo.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un tipo de orden.");
                return;
            }
            string tipo = ((System.Windows.Controls.ComboBoxItem)cmbTipo.SelectedItem).Content.ToString() ?? string.Empty;
            string? subtipo = null;
            if (tipo == "Chica")
            {
                if (cmbSubtipo.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Las órdenes chicas deben tener un subtipo.");
                    return;
                }
                subtipo =((System.Windows.Controls.ComboBoxItem)cmbSubtipo.SelectedItem).Content.ToString() ?? string.Empty;
            }
            if (pedidosOrdenSeleccionados.Count == 0)
            {
                MessageBox.Show("La orden debe tener al menos un pedido.");
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    SqlTransaction transaccion = conexion.BeginTransaction();
                    try
                    {
                        // Actualizar datos de la orden
                        string consultaOrden = @"
                                                UPDATE OrdenCompra
                                                SET Descripcion = @Descripcion,
                                                    FechaCreacion = @FechaCreacion,
                                                    FechaLimiteOfertas = @FechaLimite,
                                                    TipoOrden = @TipoOrden,
                                                    SubTipoOrden = @SubTipoOrden
                                                WHERE OrdenID = @OrdenID;";
                        SqlCommand cmdOrden = new SqlCommand(consultaOrden, conexion, transaccion);

                        cmdOrden.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text.Trim());
                        cmdOrden.Parameters.AddWithValue("@FechaCreacion", dtpFechaCreacion.SelectedDate.Value);
                        cmdOrden.Parameters.AddWithValue("@FechaLimite",dtpFechaLimite.SelectedDate.Value);
                        cmdOrden.Parameters.AddWithValue("@TipoOrden",tipo);
                        cmdOrden.Parameters.AddWithValue("@SubTipoOrden", (object?)subtipo ?? DBNull.Value);
                        cmdOrden.Parameters.AddWithValue("@OrdenID", ordenSeleccionadaID);
                        cmdOrden.ExecuteNonQuery();
                        // Eliminar las relaciones anteriores
                        string eliminarRelaciones = @"DELETE FROM OrdenPedido WHERE OrdenID = @OrdenID;";
                        SqlCommand cmdEliminar = new SqlCommand(eliminarRelaciones, conexion, transaccion);
                        cmdEliminar.Parameters.AddWithValue("@OrdenID", ordenSeleccionadaID);
                        cmdEliminar.ExecuteNonQuery();
                        // Volver a guardar los pedidos de la orden
                        foreach (int pedidoID in pedidosOrdenSeleccionados)
                        {
                            string insertarRelacion = @"INSERT INTO OrdenPedido (OrdenID, PedidoID) VALUES (@OrdenID, @PedidoID);";
                            SqlCommand cmdPedido = new SqlCommand(insertarRelacion, conexion, transaccion);
                            cmdPedido.Parameters.AddWithValue("@OrdenID", ordenSeleccionadaID);
                            cmdPedido.Parameters.AddWithValue("@PedidoID", pedidoID);
                            cmdPedido.ExecuteNonQuery();
                        }
                        transaccion.Commit();
                        MessageBox.Show("Orden actualizada correctamente.");
                        CargarOrdenes();
                        LimpiarCamposOrden();
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar la orden: {ex.Message}");
            }
        }

        private void Button_EliminarOrden_Click(object sender, RoutedEventArgs e)
        {
            if (ordenSeleccionadaID == 0)
            {
                MessageBox.Show("Seleccione una orden para eliminar");
                return;
            }
            MessageBoxResult respuesta = MessageBox.Show("Esta seguro de eliminar esta orden de compra?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (respuesta != MessageBoxResult.Yes)
            {
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    SqlTransaction transaccion = conexion.BeginTransaction();
                    try
                    {
                        string eliminarRelaciones = @"DELETE FROM OrdenPedido WHERE OrdenID = @OrdenID";
                        SqlCommand cmdRelaciones = new SqlCommand(eliminarRelaciones, conexion, transaccion);
                        cmdRelaciones.Parameters.AddWithValue("@OrdenID", ordenSeleccionadaID);
                        cmdRelaciones.ExecuteNonQuery();
                        string eliminarOrden = @"DELETE FROM OrdenCompra WHERE OrdenID = @OrdenID";
                        SqlCommand cmdOrden = new SqlCommand(eliminarOrden, conexion, transaccion);
                        cmdOrden.Parameters.AddWithValue("@OrdenID", ordenSeleccionadaID);
                        int filasAfectadas = cmdOrden.ExecuteNonQuery();
                        if (filasAfectadas > 0)
                        {
                            transaccion.Commit();
                            MessageBox.Show("Orden de compra eliminada correctamente");
                            CargarOrdenes();
                            LimpiarCamposOrden();
                        }
                        else
                        {
                            transaccion.Rollback();
                            MessageBox.Show("No se encontro la orden de compra");
                        }
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la orden: {ex.Message}");
            }
        }

        private void Button_RegistrarOferta_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPedido.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un pedido");
                return;
            }

            if (cmbProveedor.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un proveedor");
                return;
            }

            if (!decimal.TryParse(txtMonto.Text, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("Ingrese un precio unitario valido mayor a 0");
                return;
            }

            if (dtpFechaOferta.SelectedDate == null)
            {
                MessageBox.Show("Seleccione la fecha de la oferta");
                return;
            }

            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"INSERT INTO Oferta (ProveedorID, PedidoID, PrecioUnitario, FechaOferta) VALUES (@ProveedorID, @PedidoID, @PrecioUnitario, @FechaOferta)";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("@ProveedorID", cmbProveedor.SelectedValue);
                    cmd.Parameters.AddWithValue("@PedidoID", cmbPedido.SelectedValue);
                    cmd.Parameters.AddWithValue("@PrecioUnitario", precio);
                    cmd.Parameters.AddWithValue("@FechaOferta", dtpFechaOferta.SelectedDate.Value);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Oferta registrada correctamente");

                    CargarOfertas();

                    cmbPedido.SelectedIndex = -1;
                    cmbProveedor.SelectedIndex = -1;
                    txtMonto.Clear();
                    dtpFechaOferta.SelectedDate = null;
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Este proveedor ya registro una oferta para este pedido");
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

        private void Button_RegistrarAdjudicacion_Click(object sender, RoutedEventArgs e)
        {
            if (cmbOrden.SelectedValue == null)
            {
                MessageBox.Show("Seleccione una orden de compra");
                return;
            }
            if (dtpFechaResolucion.SelectedDate == null)
            {
                MessageBox.Show("Seleccione la fecha de resolucion");
                return;
            }
            int ordenID = Convert.ToInt32(cmbOrden.SelectedValue);
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    SqlTransaction transaccion = conexion.BeginTransaction();
                    try
                    {
                        string consultaPedidos = @"SELECT PedidoID FROM OrdenPedido WHERE OrdenID = @OrdenID";
                        SqlCommand cmdPedidos = new SqlCommand(consultaPedidos, conexion, transaccion);
                        cmdPedidos.Parameters.AddWithValue("@OrdenID", ordenID);
                        List<int> pedidos = new List<int>();
                        using (SqlDataReader reader = cmdPedidos.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pedidos.Add(Convert.ToInt32(reader["PedidoID"]));
                            }
                        }
                        if (pedidos.Count == 0)
                        {
                            throw new Exception("La orden seleccionada no tiene pedidos asociados");
                        }
                        foreach (int pedidoID in pedidos)
                        {
                            string consultaExisteOferta = @"SELECT COUNT(*) FROM Oferta WHERE PedidoID = @PedidoID";
                            SqlCommand cmdExisteOferta = new SqlCommand(consultaExisteOferta, conexion, transaccion);
                            cmdExisteOferta.Parameters.AddWithValue("@PedidoID", pedidoID);
                            int cantidadOfertas = Convert.ToInt32(cmdExisteOferta.ExecuteScalar());
                            if (cantidadOfertas == 0)
                            {
                                throw new Exception($"El pedido {pedidoID} no tiene ofertas registradas");
                            }
                        }
                        string consultaAdjudicacion = @"INSERT INTO Adjudicacion (OrdenID, FechaResolucion) OUTPUT INSERTED.AdjudicacionID VALUES (@OrdenID, @FechaResolucion)";
                        SqlCommand cmdAdjudicacion = new SqlCommand(consultaAdjudicacion, conexion, transaccion);
                        cmdAdjudicacion.Parameters.AddWithValue("@OrdenID", ordenID);
                        cmdAdjudicacion.Parameters.AddWithValue("@FechaResolucion", dtpFechaResolucion.SelectedDate.Value);
                        int adjudicacionID = Convert.ToInt32(cmdAdjudicacion.ExecuteScalar());
                        foreach (int pedidoID in pedidos)
                        {
                            string consultaMejorOferta = @"
                                                            SELECT TOP 1
                                                                o.ProveedorID,
                                                                o.PrecioUnitario,
                                                                p.Cantidad
                                                            FROM Oferta o
                                                            INNER JOIN PedidoInterno p ON o.PedidoID = p.PedidoID
                                                            WHERE o.PedidoID = @PedidoID
                                                            ORDER BY o.PrecioUnitario ASC, o.OfertaID ASC";

                            SqlCommand cmdMejorOferta = new SqlCommand(consultaMejorOferta, conexion, transaccion);
                            cmdMejorOferta.Parameters.AddWithValue("@PedidoID", pedidoID);
                            int proveedorID;
                            decimal precioAcordado;
                            int cantidadFinal;
                            using (SqlDataReader reader = cmdMejorOferta.ExecuteReader())
                            {
                                reader.Read();
                                proveedorID = Convert.ToInt32(reader["ProveedorID"]);
                                precioAcordado = Convert.ToDecimal(reader["PrecioUnitario"]);
                                cantidadFinal = Convert.ToInt32(reader["Cantidad"]);
                            }
                            string consultaDetalle = @"INSERT INTO AdjudicacionDetalle (AdjudicacionID, PedidoID, ProveedorID, CantidadFinal, PrecioAcordado) VALUES (@AdjudicacionID, @PedidoID, @ProveedorID, @CantidadFinal, @PrecioAcordado)";
                            SqlCommand cmdDetalle = new SqlCommand(consultaDetalle, conexion, transaccion);
                            cmdDetalle.Parameters.AddWithValue("@AdjudicacionID", adjudicacionID);
                            cmdDetalle.Parameters.AddWithValue("@PedidoID", pedidoID);
                            cmdDetalle.Parameters.AddWithValue("@ProveedorID", proveedorID);
                            cmdDetalle.Parameters.AddWithValue("@CantidadFinal", cantidadFinal);
                            cmdDetalle.Parameters.AddWithValue("@PrecioAcordado", precioAcordado);
                            cmdDetalle.ExecuteNonQuery();
                        }
                        transaccion.Commit();
                        MessageBox.Show("Adjudicacion registrada correctamente");
                        cmbOrden.SelectedIndex = -1;
                        dtpFechaResolucion.SelectedDate = null;
                        CargarOrdenesAdjudicacion();
                        CargarAdjudicaciones();
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar la adjudicacion: {ex.Message}");
            }
        }

        private void CargarAdjudicaciones()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                SELECT
                    p.NombreComercial AS proveedor,
                    ad.CantidadFinal AS cantidad,
                    ad.PrecioAcordado AS precio,
                    ad.CantidadFinal * ad.PrecioAcordado AS total
                FROM AdjudicacionDetalle ad
                INNER JOIN Proveedor p ON ad.ProveedorID = p.ProveedorID
                INNER JOIN Adjudicacion a ON ad.AdjudicacionID = a.AdjudicacionID
                ORDER BY a.AdjudicacionID DESC, ad.PedidoID;";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);

                    dtgAdjudicaciones.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las adjudicaciones: {ex.Message}");
            }
        }

        private void CargarPedidosPendientes()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                SELECT
                    p.PedidoID AS pedidoID,
                    d.Nombre AS departamento,
                    a.Nombre AS articulo,
                    p.Cantidad AS cantidad,
                    p.FechaNecesidad AS fechaNecesidad
                FROM PedidoInterno p
                INNER JOIN Departamento d ON p.DepartamentoID = d.DepartamentoID
                INNER JOIN Articulo a ON p.ArticuloID = a.ArticuloID
                LEFT JOIN OrdenPedido op ON p.PedidoID = op.PedidoID
                WHERE op.PedidoID IS NULL
                ORDER BY p.FechaNecesidad;";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);

                    dtgConsultasPedidos.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los pedidos pendientes: {ex.Message}");
            }
        }

        private void CargarOrdenesActivas()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                SELECT
                    oc.OrdenID AS ordenID,
                    oc.Descripcion AS descripcion,
                    oc.FechaLimiteOfertas AS fechaLimite,
                    DATEDIFF(DAY, CAST(GETDATE() AS DATE), oc.FechaLimiteOfertas) AS diasRestantes
                FROM OrdenCompra oc
                LEFT JOIN Adjudicacion ad ON oc.OrdenID = ad.OrdenID
                WHERE oc.FechaLimiteOfertas >= CAST(GETDATE() AS DATE)
                AND ad.AdjudicacionID IS NULL
                ORDER BY oc.FechaLimiteOfertas;";

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);

                    dtgOrdenesActivas.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las ordenes activas: {ex.Message}");
            }
        }

    }
}
