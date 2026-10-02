using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace ProyectoBD.Interfaces
{
    public partial class SeleccionarPedidos : Window
    {
        public List<int> PedidosSeleccionados { get; private set; }
            = new List<int>();
        private int ordenIDActual = 0;

        public SeleccionarPedidos()
        {
            InitializeComponent();
            CargarPedidosDisponibles();
        }
        public SeleccionarPedidos(int ordenID)
        {
            InitializeComponent();
            ordenIDActual = ordenID;
            CargarPedidosDisponibles();
            SeleccionarPedidosActuales();
        }

        private void CargarPedidosDisponibles()
        {
            try
            {
                using (SqlConnection conexion =
                    Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                        SELECT
                            p.PedidoID,
                            d.Nombre AS Departamento,
                            a.Nombre AS Articulo,
                            p.Cantidad,
                            p.FechaSolicitud,
                            p.FechaNecesidad
                        FROM PedidoInterno p
                        INNER JOIN Departamento d
                            ON p.DepartamentoID = d.DepartamentoID
                        INNER JOIN Articulo a
                            ON p.ArticuloID = a.ArticuloID
                        LEFT JOIN OrdenPedido op
                            ON p.PedidoID = op.PedidoID
                        WHERE op.PedidoID IS NULL
                        ORDER BY p.PedidoID;";

                    DataTable dt = new DataTable();

                    SqlDataAdapter da =
                        new SqlDataAdapter(consulta, conexion);

                    da.Fill(dt);

                    dtgPedidosDisponibles.ItemsSource =
                        dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar los pedidos: {ex.Message}");
            }
        }
        private void SeleccionarPedidosActuales()
        {
            if (ordenIDActual == 0)
            {
                return;
            }
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = @"SELECT PedidoID FROM OrdenPedido WHERE OrdenID = @OrdenID";
                    SqlCommand cmd = new SqlCommand(consulta, conexion);
                    cmd.Parameters.AddWithValue("OrdenID", ordenIDActual);
                    List<int> pedidosActuales = new List<int>();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                        while (reader.Read())
                        {
                            pedidosActuales.Add(Convert.ToInt32(reader["PedidoID"]));
                        }
                    }
                    foreach (DataRowView fila in dtgPedidosDisponibles.Items)
                    {
                        int pedidoID = Convert.ToInt32(fila["PedidoID"]);
                        if (pedidosActuales.Contains(pedidoID))
                        {
                            dtgPedidosDisponibles.SelectedItems.Add(fila);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar los pedidos actuales: {ex.Message}");
            }
        }

        private void Button_Aceptar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (dtgPedidosDisponibles.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione al menos un pedido.");
                return;
            }

            PedidosSeleccionados.Clear();

            foreach (DataRowView fila
                     in dtgPedidosDisponibles.SelectedItems)
            {
                PedidosSeleccionados.Add(
                    Convert.ToInt32(fila["PedidoID"]));
            }

            DialogResult = true;
            Close();
        }

        private void Button_Cancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}