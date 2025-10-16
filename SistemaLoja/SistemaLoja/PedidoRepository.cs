using Microsoft.Data.SqlClient;

namespace SistemaLoja.Lab12_ConexaoSQLServer;

public class PedidoRepository
{
    // EXERCÍCIO 7: Criar pedido com itens (transação)
    public void CriarPedido(Pedido pedido, List<PedidoItem> itens)
    {
        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
                
            // TODO: Inicie a transação
            SqlTransaction transaction = conn.BeginTransaction();
                
            try
            {
                // Get products info
                string sqlProdutos = "SELECT Id, Estoque, Preco FROM Produtos WHERE Id IN ";

                List<Produto> produtos = new List<Produto>();

                if (itens.Count == 0)
                    throw new Exception("Nenhum item para processar no pedido.");


                List<string> param = new List<string>();
                for (int i = 0; i < itens.Count; i++)
                {
                    param.Add($"@Id{i}");
                }

                sqlProdutos += "(" + string.Join(", ", param) + ")";

                using (SqlCommand cmd = new SqlCommand(sqlProdutos, conn, transaction))
                {
                    for (int i = 0; i < itens.Count; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i}", itens[i].ProdutoId);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            produtos.Add(new Produto()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Preco = Convert.ToDecimal(reader["Preco"]),
                                Estoque = Convert.ToInt32(reader["Estoque"])
                            });
                        }
                    }
                }

                // Process PedidoItems

                foreach (PedidoItem item in itens)
                {
                    Produto? produto = produtos.FirstOrDefault(p => p.Id == item.ProdutoId);
                    
                    if (produto == null)
                    {
                        throw new Exception($"Produto com Id {item.ProdutoId} não encontrado.");
                    }

                    if (produto.Estoque < item.Quantidade)
                    {
                        throw new Exception($"Estoque insuficiente para o produto {produto.Nome}. Disponível: {produto.Estoque}, Requerido: {item.Quantidade}");
                    }

                    // Update stock
                    string sqlUpdateEstoque = "UPDATE Produtos SET Estoque = Estoque - @Quantidade WHERE Id = @ProdutoId";
                    using (SqlCommand cmd = new SqlCommand(sqlUpdateEstoque, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Quantidade", item.Quantidade);
                        cmd.Parameters.AddWithValue("@ProdutoId", item.ProdutoId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new Exception($"Falha ao atualizar estoque do produto {produto.Nome}.");
                        }
                    }

                    item.PrecoUnitario = produto.Preco;
                }

                pedido.ValorTotal = itens.Sum(i => i.Quantidade * i.PrecoUnitario);

                // Inseting Pedido
                string sqlPedido = "INSERT INTO Pedidos (ClienteId, ValorTotal) " +
                                "OUTPUT INSERTED.Id " +
                                "VALUES (@ClienteId, @ValorTotal)";
                    
                int pedidoId = 0;
                using (SqlCommand cmd = new SqlCommand(sqlPedido, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ClienteId", pedido.ClienteId);
                    cmd.Parameters.AddWithValue("@ValorTotal", pedido.ValorTotal);

                    pedidoId = (int)cmd.ExecuteScalar()!;
                    if (pedidoId <= 0)
                        throw new Exception("Falha ao inserir pedido.");
                }

                // Inserting PedidoItems

                string sqlPedidoItem = "INSERT INTO PedidoItens (PedidoId, ProdutoId, Quantidade, PrecoUnitario) " +
                                    "VALUES (@PedidoId, @ProdutoId, @Quantidade, @PrecoUnitario)";

                foreach (PedidoItem item in itens)
                {
                    item.PedidoId = pedidoId;
                    using (SqlCommand cmd = new SqlCommand(sqlPedidoItem, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@PedidoId", item.PedidoId);
                        cmd.Parameters.AddWithValue("@ProdutoId", item.ProdutoId);
                        cmd.Parameters.AddWithValue("@Quantidade", item.Quantidade);
                        cmd.Parameters.AddWithValue("@PrecoUnitario", item.PrecoUnitario);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new Exception($"Falha ao inserir item do pedido para o produto Id {item.ProdutoId}.");
                        }
                    }
                }
                transaction.Commit();
                Console.WriteLine("Pedido criado com sucesso!");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Erro ao criar pedido: {ex.Message}");
                throw;
            }
        }
    }

    // EXERCÍCIO 8: Listar pedidos de um cliente
    public void ListarPedidosCliente(int clienteId)
    {
        string sql = "SELECT Id, ClienteId, DataPedido, ValorTotal FROM Pedidos " +
                     "WHERE ClienteId = @ClienteId " +
                     "ORDER BY DataPedido DESC";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ClienteId", clienteId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Pedido pedido = new Pedido()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            ClienteId = Convert.ToInt32(reader["ClienteId"]),
                            DataPedido = Convert.ToDateTime(reader["DataPedido"]),
                            ValorTotal = Convert.ToDecimal(reader["ValorTotal"])
                        };

                        Console.WriteLine(pedido.ToString());
                    }
                }
            }
        }
    }

    // EXERCÍCIO 9: Obter detalhes completos de um pedido
    public void ObterDetalhesPedido(int pedidoId)
    {
        string sql = @"
        SELECT 
            pi.Id AS PedidoItemId,
            pi.PedidoId,
            pi.ProdutoId,
            pi.Quantidade,
            pi.PrecoUnitario,
            p.Nome AS NomeProduto,
            (pi.Quantidade * pi.PrecoUnitario) AS Subtotal
        FROM PedidoItens pi
        INNER JOIN Produtos p ON pi.ProdutoId = p.Id
        WHERE pi.PedidoId = @PedidoId";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@PedidoId", pedidoId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine($"Itens do Pedido ID {pedidoId}:");
                    Console.WriteLine("-------------------------------------------------");

                    decimal totalPedido = 0;

                    while (reader.Read())
                    {
                        int produtoId = Convert.ToInt32(reader["ProdutoId"]);
                        string nomeProduto = reader["NomeProduto"] as string ?? "N/A";
                        int quantidade = Convert.ToInt32(reader["Quantidade"]);
                        decimal precoUnitario = Convert.ToDecimal(reader["PrecoUnitario"]);
                        decimal subtotal = Convert.ToDecimal(reader["Subtotal"]);

                        totalPedido += subtotal;

                        Console.WriteLine($"Produto: {nomeProduto} (ID: {produtoId})");
                        Console.WriteLine($"Quantidade: {quantidade}, Preço Unitário: {precoUnitario:C}, Subtotal: {subtotal:C}");
                        Console.WriteLine("-------------------------------------------------");
                    }

                    Console.WriteLine($"Valor Total do Pedido: {totalPedido:C}");
                }
            }
        }
    }

    // DESAFIO 3: Calcular total de vendas por período
    public void TotalVendasPorPeriodo(DateTime dataInicio, DateTime dataFim)
    {
        string sql = @"
        SELECT ISNULL(SUM(ValorTotal), 0) 
        FROM Pedidos 
        WHERE DataPedido >= @DataInicio AND DataPedido <= @DataFim";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                cmd.Parameters.AddWithValue("@DataFim", dataFim);

                decimal totalVendas = (decimal)cmd.ExecuteScalar()!;

                Console.WriteLine($"Total de vendas de {dataInicio:dd/MM/yyyy} até {dataFim:dd/MM/yyyy}: {totalVendas:C}");
            }
        }
    }
}