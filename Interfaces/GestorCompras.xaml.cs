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
        public GestorCompras()
        {
            InitializeComponent();
            CargarDepartamentos();
            CargarArticulos();
            CargarPedidos();
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
                    string consulta = @"SELECT ArticuloID FROM Articulo ORDER BY Nombre";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(consulta, conexion);
                    da.Fill(dt);
                    cbxArticulo.ItemsSource = dt.DefaultView;
                    cbxArticulo.DisplayMemberPath = "Nombre";
                    cbxArticulo.SelectedValuePath = "Articulo"; 
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
                    string consulta = @"SELECT p.PedidoID AS pedidoId, d.Nombre AS departamento, a.Nombre AS articulo, p.Cantidad AS cantidad, p.FechaSolicitud AS fecha_solicitud p.FechaNecesidad AS fecha_necesidad
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
                    string consulta = @"INSERT INTO PedidoInterno (DepartamentoID, ArticuloID, Cantidad, FechaSolicitud, FechaNecesidad) VALUES (@DepartamentoID, @ArticuloID @Cantidad, @FechaSolicitud, @FechaNecesidad)";
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
        }

        private void Button_LimpiarCampos_Click(object sender, RoutedEventArgs e)
        {
            LimpiarCamposPedido();
        }
    }
}
