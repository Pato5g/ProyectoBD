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


namespace ProyectoBD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ProbarConexionSQLServer();
            ProbarConexionMySQL();

        }

        private void ProbarConexionSQLServer()
        {
            string cadenaConexion =
                @"Server = localhost;
                Database = AdquisicionesProveedores;
                Trusted_Connection=True;
                TrustServerCertificate=True;";
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    conexion.Open();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos SQL Server: {ex.Message}");
            }
        }
        
        private void ProbarConexionMySQL()
        {
            string cadenaConexion =
                @"Server=localhost;
                Port=3306;
                Database=proveedores_externos;
                Uid=root;
                Pwd=Lolplayers1;";
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(cadenaConexion))
                {
                    conexion.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar a la base de datos MySQL: {ex.Message}");
            }
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
            string cadenaConexion =
                @"Server = localhost;
                Database = AdquisicionesProveedores;
                Trusted_Connection=True;
                TrustServerCertificate=True;";
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
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
                            // Aquí puedes abrir la ventana principal de tu aplicación
                        }
                        else
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