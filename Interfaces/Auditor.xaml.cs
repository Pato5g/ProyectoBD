using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows;

namespace ProyectoBD.Interfaces
{
    public partial class Auditor
    {
        public Auditor()
        {
            InitializeComponent();
            CargarHistorialCompras();
            CargarRanking();
            CargarOfertas();
            CargarPendientes();
            CargarOrdenesActivas();
            CargarGastos();
            CargarEficiencia();
            CargarVariacionPrecios();
        }

        private void CargarHistorialCompras()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                        SELECT
                            pr.NombreComercial AS proveedor,
                            ar.Nombre AS articulo,
                            ad.PrecioAcordado AS precio,
                            ad.CantidadFinal AS cantidad,
                            a.FechaResolucion AS fecha
                        FROM AdjudicacionDetalle ad
                        INNER JOIN Adjudicacion a ON ad.AdjudicacionID = a.AdjudicacionID
                        INNER JOIN Proveedor pr ON ad.ProveedorID = pr.ProveedorID
                        INNER JOIN PedidoInterno p ON ad.PedidoID = p.PedidoID
                        INNER JOIN Articulo ar ON p.ArticuloID = ar.ArticuloID
                        ORDER BY a.FechaResolucion DESC";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    dtgHistorialCompras.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el historial de compras: {ex.Message}");
            }
        }

        private void CargarRanking()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                        SELECT
                            pr.NombreComercial AS proveedor,
                            SUM(ad.CantidadFinal * ad.PrecioAcordado) AS monto,
                            COUNT(*) AS adjudicaciones
                        FROM AdjudicacionDetalle ad
                        INNER JOIN Proveedor pr ON ad.ProveedorID = pr.ProveedorID
                        GROUP BY pr.ProveedorID, pr.NombreComercial
                        ORDER BY monto DESC";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    dtgRanking.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el ranking de proveedores: {ex.Message}");
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
                            o.OfertaID AS oferta,
                            pr.NombreComercial AS proveedor,
                            ar.Nombre AS articulo,
                            o.PrecioUnitario AS precio,
                            o.FechaOferta AS fecha
                        FROM Oferta o
                        INNER JOIN Proveedor pr ON o.ProveedorID = pr.ProveedorID
                        INNER JOIN PedidoInterno p ON o.PedidoID = p.PedidoID
                        INNER JOIN Articulo ar ON p.ArticuloID = ar.ArticuloID
                        ORDER BY o.FechaOferta DESC, o.OfertaID DESC";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    dtgOfertasAuditoria.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las ofertas: {ex.Message}");
            }
        }

        private void CargarPendientes()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                        SELECT
                            p.PedidoID AS pedido,
                            d.Nombre AS departamento,
                            a.Nombre AS articulo,
                            p.Cantidad AS cantidad,
                            p.FechaNecesidad AS fecha
                        FROM PedidoInterno p
                        INNER JOIN Departamento d ON p.DepartamentoID = d.DepartamentoID
                        INNER JOIN Articulo a ON p.ArticuloID = a.ArticuloID
                        WHERE NOT EXISTS (
                            SELECT 1
                            FROM AdjudicacionDetalle ad
                            WHERE ad.PedidoID = p.PedidoID
                        )
                        ORDER BY p.FechaNecesidad";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    dtgPendientes.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los pendientes: {ex.Message}");
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
                            oc.OrdenID AS orden,
                            oc.Descripcion AS descripcion,
                            oc.FechaCreacion AS creacion,
                            oc.FechaLimiteOfertas AS fecha_limite,
                            DATEDIFF(DAY, CAST(GETDATE() AS DATE), oc.FechaLimiteOfertas) AS dias_restantes
                        FROM OrdenCompra oc
                        WHERE oc.FechaLimiteOfertas >= CAST(GETDATE() AS DATE)
                        ORDER BY oc.FechaLimiteOfertas";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    dtgOrdenesActivas.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar las ordenes activas: {ex.Message}");
            }
        }

        private void CargarGastos()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                        SELECT
                            pr.NombreComercial AS proveedor,
                            COUNT(*) AS compras,
                            SUM(ad.CantidadFinal * ad.PrecioAcordado) AS gasto
                        FROM AdjudicacionDetalle ad
                        INNER JOIN Proveedor pr ON ad.ProveedorID = pr.ProveedorID
                        GROUP BY pr.ProveedorID, pr.NombreComercial
                        ORDER BY gasto DESC";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    dtgGastos.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los gastos: {ex.Message}");
            }
        }

        private void CargarEficiencia()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                        SELECT
                            pr.NombreComercial AS proveedor,
                            COUNT(DISTINCT o.OfertaID) AS ofertas,
                            COUNT(DISTINCT ad.PedidoID) AS adjudicaciones,
                            CAST(
                                CASE
                                    WHEN COUNT(DISTINCT o.OfertaID) = 0 THEN 0
                                    ELSE COUNT(DISTINCT ad.PedidoID) * 100.0 / COUNT(DISTINCT o.OfertaID)
                                END
                                AS DECIMAL(10,2)
                            ) AS eficiencia
                        FROM Proveedor pr
                        LEFT JOIN Oferta o ON pr.ProveedorID = o.ProveedorID
                        LEFT JOIN AdjudicacionDetalle ad ON pr.ProveedorID = ad.ProveedorID
                        GROUP BY pr.ProveedorID, pr.NombreComercial
                        ORDER BY eficiencia DESC";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    dtgEficiencia.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la eficiencia de proveedores: {ex.Message}");
            }
        }

        private void CargarVariacionPrecios()
        {
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                        SELECT
                            a.Nombre AS articulo,
                            MIN(o.PrecioUnitario) AS minimo,
                            MAX(o.PrecioUnitario) AS maximo,
                            CAST(AVG(o.PrecioUnitario) AS DECIMAL(12,2)) AS promedio,
                            MAX(o.PrecioUnitario) - MIN(o.PrecioUnitario) AS variacion
                        FROM Oferta o
                        INNER JOIN PedidoInterno p ON o.PedidoID = p.PedidoID
                        INNER JOIN Articulo a ON p.ArticuloID = a.ArticuloID
                        GROUP BY a.ArticuloID, a.Nombre
                        ORDER BY variacion DESC";

                    SqlCommand cmd = new SqlCommand(consulta, conexion);

                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    dtgVariacion.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la variacion de precios: {ex.Message}");
            }
        }
    }
}