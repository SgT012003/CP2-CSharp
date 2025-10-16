using Microsoft.Data.SqlClient;

namespace SistemaLoja.Lab12_ConexaoSQLServer;

public class ProdutoRepository
{
    // EXERCÍCIO 1: Listar todos os produtos
    public void ListarTodosProdutos()
    {

        string sql = "SELECT * FROM Produtos";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
                
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Produto produto = new Produto() {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nome = reader["Nome"] as string ?? string.Empty,
                        Preco = Convert.ToDecimal(reader["Preco"]),
                        Estoque = Convert.ToInt32(reader["Estoque"])
                    };
                    Console.WriteLine(produto.ToString());
                }
            }
        }
    }

    // EXERCÍCIO 2: Inserir novo produto
    public void InserirProduto(Produto produto)
    {

        string sql = "INSERT INTO Produtos (Nome, Preco, Estoque, CategoriaId) " +
                     "VALUES (@Nome, @Preco, @Estoque, @CategoriaId)";
            
        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
                
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Nome", produto.Nome);
                cmd.Parameters.AddWithValue("@Preco", produto.Preco);
                cmd.Parameters.AddWithValue("@Estoque", produto.Estoque);
                cmd.Parameters.AddWithValue("@CategoriaId", produto.CategoriaId);

                if (cmd.ExecuteNonQuery() > 0) { 
                    Console.WriteLine("Produto inserido com sucesso!");
                } else { 
                    Console.WriteLine("Falha ao inserir o produto.");
                }
            }
        }
    }

    // EXERCÍCIO 3: Atualizar produto
    public void AtualizarProduto(Produto produto)
    {
            
        string sql = "UPDATE Produtos SET " +
                     "Nome = @Nome, " +
                     "Preco = @Preco, " +
                     "Estoque = @Estoque " +
                     "WHERE Id = @Id";
            
        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", produto.Id);
                cmd.Parameters.AddWithValue("@Nome", produto.Nome);
                cmd.Parameters.AddWithValue("@Preco", produto.Preco);
                cmd.Parameters.AddWithValue("@Estoque", produto.Estoque);
                cmd.Parameters.AddWithValue("@CategoriaId", produto.CategoriaId);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    Console.WriteLine("Produto atualizado com sucesso!");
                }
                else
                {
                    Console.WriteLine("Falha ao atualizar o produto.");
                }
            }
        }
    }

    // EXERCÍCIO 4: Deletar produto
    public void DeletarProduto(int id)
    {
        // Logging only -> query it self is safe to dont need this check
        if (VerificarVinculoDeProduto(id))
        {
            Console.WriteLine("Este Produto esta vinculado a um pedido e nao pode ser deletado");
            return;
        }

        // Not very performatic because has a sub-query but safe
        string sql = "DELETE pd FROM Produtos pd WHERE pd.Id = @Id AND NOT EXISTS ( SELECT 1 FROM PedidoItens pdi WHERE pdi.ProdutoId = pd.Id );";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    Console.WriteLine("Produto foi deletado com sucesso!");
                }
                else
                {
                    Console.WriteLine("Falha ao deletar o produto.");
                }
            }
        }
    }

    private bool VerificarVinculoDeProduto(int id)
    {
        string sql = "SELECT COUNT(*) FROM PedidoItens WHERE ProdutoId = @Id";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                if ((int)cmd.ExecuteScalar() > 0)
                {
                    return true;
                }
                return false;
            }
        }
    }

    // EXERCÍCIO 5: Buscar produto por ID
    public Produto? BuscarPorId(int id)
    {
        string sql = @"SELECT * FROM Produtos Where Id = @id";
        Produto? produto = null;

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        produto = new Produto()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nome = reader["Nome"] as string ?? string.Empty,
                            Preco = Convert.ToDecimal(reader["Preco"]),
                            Estoque = Convert.ToInt32(reader["Estoque"]),
                            CategoriaId = Convert.ToInt32(reader["CategoriaId"])
                        };
                    }
                }
            }
        }
        return produto;
    }

    // EXERCÍCIO 6: Listar produtos por categoria
    public void ListarProdutosPorCategoria(int categoriaId)
    {
            
        string sql = @"SELECT p.*, c.Nome as NomeCategoria 
                          FROM Produtos p
                          INNER JOIN Categorias c ON p.CategoriaId = c.Id
                          WHERE p.CategoriaId = @CategoriaId";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CategoriaId", categoriaId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Produto produto = new Produto()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nome = reader["Nome"] as string ?? string.Empty,
                            Preco = Convert.ToDecimal(reader["Preco"]),
                            Estoque = Convert.ToInt32(reader["Estoque"]),
                            CategoriaId = Convert.ToInt32(reader["CategoriaId"])
                        };
                        string nomeCategoria = reader["NomeCategoria"] as string ?? "N/A";
                        Console.WriteLine($"{produto.ToString()}, Categoria: {nomeCategoria}");
                    }
                }
            }
        }
    }

    // DESAFIO 1: Buscar produtos com estoque baixo
    public void ListarProdutosEstoqueBaixo(int quantidadeMinima)
    {
        string sql = @"SELECT * FROM Produtos Where Estoque <= @Quantidade";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Quantidade", quantidadeMinima);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    bool first = true;
                    while (reader.Read())
                    {
                        Produto produto = new Produto()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nome = reader["Nome"] as string ?? string.Empty,
                            Preco = Convert.ToDecimal(reader["Preco"]),
                            Estoque = Convert.ToInt32(reader["Estoque"]),
                            CategoriaId = Convert.ToInt32(reader["CategoriaId"])
                        };
                        Console.ForegroundColor = produto.Color(quantidadeMinima);
                        Console.Write((first ? string.Empty : ",\n") + produto.ToString());
                        Console.ResetColor();
                        first = false;
                    }
                }
            }
        }
    }

    // DESAFIO 2: Buscar produtos por nome (LIKE)
    public void BuscarProdutosPorNome(string termoBusca)
    {
        string sql = @"SELECT * FROM Produtos WHERE Nome LIKE @Busca";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Busca", $"%{termoBusca}%");

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Produto produto = new Produto()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nome = reader["Nome"] as string ?? string.Empty,
                            Preco = Convert.ToDecimal(reader["Preco"]),
                            Estoque = Convert.ToInt32(reader["Estoque"]),
                            CategoriaId = Convert.ToInt32(reader["CategoriaId"])
                        };
                        Console.WriteLine(produto.ToString());
                    }
                }
            }
        }
    }
}