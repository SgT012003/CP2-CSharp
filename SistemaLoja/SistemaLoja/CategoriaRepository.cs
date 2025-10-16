using Microsoft.Data.SqlClient;
using SistemaLoja.Lab12_ConexaoSQLServer;

namespace SistemaLoja
{
    public class CategoriaRepository
    {
        public void ListarTodasCategorias()
        {

            string sql = "SELECT * FROM Categorias";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Categoria categoria = new Categoria()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nome = reader["Nome"] as string ?? string.Empty,
                            Descricao = reader["Descricao"] as string ?? string.Empty
                        };
                        Console.WriteLine(categoria.ToString());
                    }
                }
            }
        }
    }
}
