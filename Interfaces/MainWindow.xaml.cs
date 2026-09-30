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
            try
            {
                using (SqlConnection conexion = Conexion.ObtenerConexionSQLServer())
                {
                    conexion.Open();
                    string consulta = "SELECT UsuarioID, RolID FROM Usuario WHERE NombreUsuario = @NombreUsuario AND Activo = 1";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                        comando.Parameters.AddWithValue("@Activo", 1);
                        int resultado = (int)comando.ExecuteScalar();
                        if (resultado > 0)
                        {
                            MessageBox.Show("Inicio de sesión exitoso.");
                            // Se abre la ventana correspondiente según el RolID del usuario
                            if (resultado == 1)
                            {
                                // Se abre la ventana de Administrador del Sistema
                                AdministradorDelSistema adminWindow = new AdministradorDelSistema();
                                adminWindow.Show();
                            }
                            else if (resultado == 2)
                            {
                                // Se abre la ventana de Gestor de Compras
                                GestorCompras gestorWindow = new GestorCompras();
                                gestorWindow.Show();
                            }
                            else if (resultado == 3)
                            {
                                // Se abre la ventana de Administrador de Proveedores
                                AdministradorProveedores adminProvWindow = new AdministradorProveedores();
                                adminProvWindow.Show();
                            }
                            else if (resultado == 4)
                            {
                                // Se abre la ventana de Auditor
                                Auditor auditorWindow = new Auditor();
                                auditorWindow.Show();
                            }
                        }
                        else if (resultado == 0)
                            Close();
                        else if (resultado != 0 && resultado >= 5)
                        {
                            MessageBox.Show("Nombre de usuario o contraseña incorrectos.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
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