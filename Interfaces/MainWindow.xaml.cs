using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;

namespace ProyectoBD.Interfaces
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtNombreUsuario_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string nombreUsuario = txtNombreUsuario.Text;
            string contrasena = passwordBox.Password;

            Login(nombreUsuario, contrasena);
        }

        private void Login(string nombreUsuario, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                MessageBox.Show("Ingrese el nombre de usuario y la contraseña.");
                return;
            }

            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();

                    string consulta = @"
                                        SELECT UsuarioID, RolID, ProveedorID
                                        FROM Usuario
                                        WHERE NombreUsuario = @NombreUsuario
                                        AND PasswordHash = HASHBYTES('SHA2_256', @Contrasena)
                                        AND Activo = 1";

                    SqlCommand comando = new SqlCommand(consulta, conexion);
                    comando.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    comando.Parameters.Add("@Contrasena", System.Data.SqlDbType.VarChar, 100).Value = contrasena;

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            MessageBox.Show("Nombre de usuario o contraseña incorrectos.");
                            return;
                        }

                        int usuarioID = Convert.ToInt32(reader["UsuarioID"]);
                        int rolID = Convert.ToInt32(reader["RolID"]);
                        int? proveedorID = reader["ProveedorID"] == DBNull.Value ? null : Convert.ToInt32(reader["ProveedorID"]);

                        MessageBox.Show("Inicio de sesión exitoso.");

                        if (rolID == 1)
                        {
                            AdministradorDelSistema adminWindow = new AdministradorDelSistema();
                            adminWindow.Show();
                        }
                        else if (rolID == 2)
                        {
                            GestorCompras gestorWindow = new GestorCompras();
                            gestorWindow.Show();
                        }
                        else if (rolID == 3)
                        {
                            if (proveedorID == null)
                            {
                                MessageBox.Show("Este usuario no tiene un proveedor asociado.");
                                return;
                            }

                            AdministradorProveedores adminProvWindow = new AdministradorProveedores(proveedorID.Value);
                            adminProvWindow.Show();
                        }
                        else if (rolID == 4)
                        {
                            Auditor auditorWindow = new Auditor();
                            auditorWindow.Show();
                        }
                        else
                        {
                            MessageBox.Show("El usuario no tiene un rol válido.");
                            return;
                        }

                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar sesión: {ex.Message}");
            }
        }

        private void cbxRol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int RolID = cbxRol.SelectedIndex;
            switch (RolID)
            {
                case 1:
                    MessageBox.Show("Administrador del Sistema");
                    break;
                case 2:
                    MessageBox.Show("Gestor de Compras");
                    break;
                case 3:
                    MessageBox.Show("Administrador de Proveedores");
                    break;
                case 4:
                    MessageBox.Show("Auditor");
                    break;

            }
        }
    }
}