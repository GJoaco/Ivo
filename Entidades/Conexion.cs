using System.Data;
using System.Data.SqlClient;


namespace Entidades
{
    public class Conexion
    {
        private static SqlConnection Conectarse()
        {
            SqlConnectionStringBuilder cadenaConexion = new SqlConnectionStringBuilder();
            cadenaConexion.DataSource = "DESKTOP-EGF4AFE\\DB";
            cadenaConexion.InitialCatalog = "Productos";
            cadenaConexion.UserID = "sa";
            cadenaConexion.Password = "123";

            return new SqlConnection(cadenaConexion.ConnectionString);
        }

        public DataTable ObtenerDataTable(string sql)
        {
            DataTable dt = new DataTable();
            SqlCommand comando;
            SqlDataAdapter AdaptarComandoDataTable;

            using (SqlConnection conexion = Conectarse())
            {
                conexion.Open();
                comando = new SqlCommand(sql, conexion);
                AdaptarComandoDataTable = new SqlDataAdapter(comando);
                AdaptarComandoDataTable.Fill(dt);
                conexion.Close();
            }

            return dt;
        }

        public void EjecutarQuery(string sql)
        {
            SqlCommand comando;

            using (SqlConnection conexion = Conectarse())
            {
                conexion.Open();
                comando = new SqlCommand(sql, conexion);
                SqlDataReader resultado = comando.ExecuteReader();
                conexion.Close();
            }
        }
    }
}
