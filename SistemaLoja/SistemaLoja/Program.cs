

using Microsoft.Data.SqlClient;
using SistemaLoja.Lab12_ConexaoSQLServer;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;

namespace SistemaLoja
{
    // ===============================================
    // MODELOS DE DADOS
    // ===============================================

    // ===============================================
    // CLASSE DE CONEXÃO
    // ===============================================

    // ===============================================
    // REPOSITÓRIO DE PRODUTOS
    // ===============================================

    // ===============================================
    // REPOSITÓRIO DE PEDIDOS
    // ===============================================

    // ===============================================
    // CLASSE PRINCIPAL
    // ===============================================

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== LAB 12 - CONEXÃO SQL SERVER ===\n");

            var produtoRepo = new ProdutoRepository();
            var pedidoRepo = new PedidoRepository();
            var categoriaRepo = new CategoriaRepository();

            bool continuar = true;

            while (continuar)
            {
                MostrarMenu();
                string opcao = Console.ReadLine() ?? string.Empty;

                try
                {
                    switch (opcao)
                    {
                        case "1":
                            // Listar produtos (avançado)

                            bool continuarListarProdutos = true;

                            while (continuarListarProdutos)
                            {
                                MostrarMenuListarProdutos();
                                string opcaoListar = Console.ReadLine() ?? string.Empty;
                                switch (opcaoListar)
                                {
                                    case "1":
                                        produtoRepo.ListarTodosProdutos();
                                        break;
                                    case "2":
                                        ListarPorCategoria(produtoRepo, categoriaRepo);
                                        break;
                                    case "3":
                                        ListarPorBaixoEstoque(produtoRepo);
                                        break;
                                    case "4":
                                        BuscaPorIDDoProduto(produtoRepo);
                                        break;
                                    case "5":
                                        BuscaPorNomeDoProduto(produtoRepo);
                                        break;
                                    case "0":
                                        continuarListarProdutos = false;
                                        break;
                                    default:
                                        Console.WriteLine("Opção inválida!");
                                        break;
                                }

                                if (continuarListarProdutos)
                                {
                                    Console.WriteLine("\nPressione qualquer tecla para continuar...");
                                    Console.ReadKey();
                                    Console.Clear();
                                }
                            }
                            break;

                        case "2":
                            InserirNovoProduto(produtoRepo);
                            break;

                        case "3":
                            AtualizarProdutoExistente(produtoRepo);
                            break;

                        case "4":
                            DeletarProdutoExistente(produtoRepo);
                            break;

                        case "5":
                            CriarNovoPedido(pedidoRepo);
                            break;

                        case "6":
                            ListarPedidosDeCliente(pedidoRepo);
                            break;

                        case "7":
                            DetalhesDoPedido(pedidoRepo);
                            break;

                        case "8":
                            VendasNoPeriodo(pedidoRepo);
                            break;

                        case "0":
                            continuar = false;
                            break;

                        default:
                            Console.WriteLine("Opção inválida!");
                            break;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"\n❌ Erro SQL: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ Erro: {ex.Message}");
                }

                if (continuar)
                {
                    Console.WriteLine("\nPressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            Console.WriteLine("\nPrograma finalizado!");
        }

        static void MostrarMenu()
        {
            Console.WriteLine("\n╔════════════════════════════════════╗");
            Console.WriteLine("║       MENU PRINCIPAL               ║");
            Console.WriteLine("╠════════════════════════════════════╣");
            Console.WriteLine("║  PRODUTOS                          ║");
            Console.WriteLine("║  1 - Listar produtos (avancado)    ║");
            Console.WriteLine("║  2 - Inserir novo produto          ║");
            Console.WriteLine("║  3 - Atualizar produto             ║");
            Console.WriteLine("║  4 - Deletar produto               ║");
            Console.WriteLine("║                                    ║");
            Console.WriteLine("║  PEDIDOS                           ║");
            Console.WriteLine("║  5 - Criar novo pedido             ║");
            Console.WriteLine("║  6 - Listar pedidos de cliente     ║");
            Console.WriteLine("║  7 - Detalhes de um pedido         ║");
            Console.WriteLine("║                                    ║");
            Console.WriteLine("║  VENDAS                            ║");
            Console.WriteLine("║  8 - Vendas dentro do periodo      ║");
            Console.WriteLine("║                                    ║");
            Console.WriteLine("║  0 - Sair                          ║");
            Console.WriteLine("╚════════════════════════════════════╝");
            Console.Write("\nEscolha uma opção: ");
        }

        static void MostrarMenuListarProdutos()
        {
            Console.WriteLine("\n╔════════════════════════════════════╗");
            Console.WriteLine("║     LISTAR PRODUTOS (AVANCADO)     ║");
            Console.WriteLine("╠════════════════════════════════════╣");
            Console.WriteLine("║  PRODUTOS                          ║");
            Console.WriteLine("║  1 - Listar todos os produtos      ║");
            Console.WriteLine("║  2 - Listar por categoria          ║");
            Console.WriteLine("║  3 - Listar por baixo estoque      ║");
            Console.WriteLine("║  4 - Busca por id do produto       ║");
            Console.WriteLine("║  5 - Busca por nome do produto     ║");
            Console.WriteLine("║                                    ║");
            Console.WriteLine("║  0 - Voltar                        ║");
            Console.WriteLine("╚════════════════════════════════════╝");
            Console.Write("\nEscolha uma opção: ");
        }

        // TODO: Implemente os métodos auxiliares abaixo

        static void InserirNovoProduto(ProdutoRepository repo)
        {
            Console.WriteLine("\n=== INSERIR NOVO PRODUTO ===");

            Console.Write("Nome: ");
            string nome = Console.ReadLine() ?? string.Empty;
            while (string.IsNullOrWhiteSpace(nome))
            {
                Console.WriteLine("Nome não pode ser vazio. Tente novamente.");
                Console.Write("Nome: ");
                nome = Console.ReadLine() ?? string.Empty;
            }

            Console.Write("Preco: ");
            string precoStr = (Console.ReadLine() ?? string.Empty).Replace(',', '.');
            decimal preco;
            while (!decimal.TryParse(precoStr, NumberStyles.Any, CultureInfo.InvariantCulture, out preco) || preco < 0.0m)
            {
                Console.WriteLine("Preço inválido. Tente novamente.");
                Console.Write("Preco: ");
                precoStr = (Console.ReadLine() ?? string.Empty).Replace(',', '.');
            }

            Console.Write("Estoque: ");
            string estoqueStr = Console.ReadLine() ?? string.Empty;
            int estoque;
            while (!int.TryParse(estoqueStr, out estoque) || estoque < 0)
            {
                Console.WriteLine("Estoque inválido. Tente novamente.");
                Console.Write("Estoque: ");
                estoqueStr = Console.ReadLine() ?? string.Empty;
            }

            Console.Write("CategoriaId: ");
            string categoryidStr = Console.ReadLine() ?? string.Empty;
            int categoryid;
            while (!int.TryParse(categoryidStr, out categoryid) || categoryid <= 0)
            {
                Console.WriteLine("CategoriaId inválido. Tente novamente.");
                Console.Write("CategoriaId: ");
                categoryidStr = Console.ReadLine() ?? string.Empty;
            }

            var produto = new Produto
            {
                Nome = nome,
                Preco = preco,
                Estoque = estoque,
                CategoriaId = categoryid
            };
            
            repo.InserirProduto(produto);
        }

        static void AtualizarProdutoExistente(ProdutoRepository repo)
        {
            Console.WriteLine("\n=== ATUALIZAR PRODUTO ===");
            
            Console.Write("ID: ");
            string idStr = Console.ReadLine() ?? string.Empty;
            int id;
            while (!int.TryParse(idStr, out id) || id < 0)
            {
                Console.WriteLine("ID inválido. Tente novamente.");
                Console.Write("ID: ");
                idStr = Console.ReadLine() ?? string.Empty;
            }

            Console.Write("Nome: ");
            string nome = Console.ReadLine() ?? string.Empty;
            while (string.IsNullOrWhiteSpace(nome))
            {
                Console.WriteLine("Nome não pode ser vazio. Tente novamente.");
                Console.Write("Nome: ");
                nome = Console.ReadLine() ?? string.Empty;
            }

            Console.Write("Preco: ");
            string precoStr = (Console.ReadLine() ?? string.Empty).Replace(',', '.');
            decimal preco;
            while (!decimal.TryParse(precoStr, NumberStyles.Any, CultureInfo.InvariantCulture, out preco) || preco < 0.0m)
            {
                Console.WriteLine("Preço inválido. Tente novamente.");
                Console.Write("Preco: ");
                precoStr = (Console.ReadLine() ?? string.Empty).Replace(',', '.');
            }

            Console.Write("Estoque: ");
            string estoqueStr = Console.ReadLine() ?? string.Empty;
            int estoque;
            while (!int.TryParse(estoqueStr, out estoque) || estoque < 0)
            {
                Console.WriteLine("Estoque inválido. Tente novamente.");
                Console.Write("Estoque: ");
                estoqueStr = Console.ReadLine() ?? string.Empty;
            }

            Console.Write("CategoriaId: ");
            string categoryidStr = Console.ReadLine() ?? string.Empty;
            int categoryid;
            while (!int.TryParse(categoryidStr, out categoryid) || categoryid <= 0)
            {
                Console.WriteLine("CategoriaId inválido. Tente novamente.");
                Console.Write("CategoriaId: ");
                categoryidStr = Console.ReadLine() ?? string.Empty;
            }

            var produto = new Produto
            {
                Id = id,
                Nome = nome,
                Preco = preco,
                Estoque = estoque,
                CategoriaId = categoryid
            };

            repo.AtualizarProduto(produto);
        }

        static void DeletarProdutoExistente(ProdutoRepository repo)
        {
            Console.WriteLine("\n=== DELETAR PRODUTO ===");

            Console.Write("ID: ");
            string idStr = Console.ReadLine() ?? string.Empty;
            int id;
            while (!int.TryParse(idStr, out id) || id <= 0)
            {
                Console.WriteLine("ID inválido. Tente novamente.");
                Console.Write("ID: ");
                idStr = Console.ReadLine() ?? string.Empty;
            }

            repo.DeletarProduto(id);
        }

        static void ListarPorCategoria(ProdutoRepository Prepo, CategoriaRepository Crepo)
        {
            // TODO: Implemente
            Console.WriteLine("\n=== PRODUTOS POR CATEGORIA ===");

            Crepo.ListarTodasCategorias();

            Console.Write("Digite o ID da categoria: ");
            string categoriaIdStr = Console.ReadLine() ?? string.Empty;
            int categoriaId;
            while (!int.TryParse(categoriaIdStr, out categoriaId) || categoriaId <= 0)
            {
                Console.WriteLine("ID inválido. Tente novamente.");
                Console.Write("Digite o ID da categoria: ");
                categoriaIdStr = Console.ReadLine() ?? string.Empty;
            }

            Prepo.ListarProdutosPorCategoria(categoriaId);
        }

        static void ListarPorBaixoEstoque(ProdutoRepository repo)
        {
            Console.WriteLine("\n=== PRODUTOS POR BAIXO ESTOQUE ===");

            Console.Write("Digite o limite de estoque: ");
            string limiteStr = Console.ReadLine() ?? string.Empty;
            int limite;
            while (!int.TryParse(limiteStr, out limite) || limite < 0)
            {
                Console.WriteLine("Limite inválido. Tente novamente.");
                Console.Write("Digite o limite de estoque: ");
                limiteStr = Console.ReadLine() ?? string.Empty;
            }

            repo.ListarProdutosEstoqueBaixo(limite);
        }

        static void BuscaPorIDDoProduto(ProdutoRepository repo)
        {
            // TODO: Implemente
            Console.WriteLine("\n=== PRODUTOS POR NOME ===");

            Console.Write("Digite o ID do produto: ");
            string idStr = Console.ReadLine() ?? string.Empty;
            int id;
            while (!int.TryParse(idStr, out id) || id <= 0)
            {
                Console.WriteLine("ID inválido. Tente novamente.");
                Console.Write("Digite o ID do produto: ");
                idStr = Console.ReadLine() ?? string.Empty;
            }

            Produto? produto = repo.BuscarPorId(id);
            if (produto == null)
            {
                Console.WriteLine("Nenhum produto com esse ID foi encontrado!");
            } else
            {
                Console.WriteLine(produto.ToString());
            }
        }

        static void BuscaPorNomeDoProduto(ProdutoRepository repo)
        {
            Console.WriteLine("\n=== PRODUTOS POR NOME ===");
            
            Console.Write("Digite o nome do produto (ou parte dele): ");
            string nome = Console.ReadLine() ?? string.Empty;
            while (string.IsNullOrWhiteSpace(nome))
            {
                Console.WriteLine("Nome não pode ser vazio. Tente novamente.");
                Console.Write("Digite o nome do produto (ou parte dele): ");
                nome = Console.ReadLine() ?? string.Empty;
            }

            repo.BuscarProdutosPorNome(nome);
        }

        static void CriarNovoPedido(PedidoRepository repo)
        {
            Console.WriteLine("\n=== CRIAR NOVO PEDIDO ===");

            Console.Write("ClienteId: ");
            string clienteIdStr = Console.ReadLine() ?? string.Empty;
            int clienteId;
            while (!int.TryParse(clienteIdStr, out clienteId) || clienteId <= 0)
            {
                Console.WriteLine("ClienteId inválido. Tente novamente.");
                Console.Write("ClienteId: ");
                clienteIdStr = Console.ReadLine() ?? string.Empty;
            }

            // Produtos
            List<PedidoItem> itens = new List<PedidoItem>();

            while (true)
            {
                Console.Write("ProdutoId (ou 0 para finalizar): ");
                string produtoIdStr = Console.ReadLine() ?? string.Empty;
                int produtoId;
                while (!int.TryParse(produtoIdStr, out produtoId) || produtoId < 0)
                {
                    Console.WriteLine("ProdutoId inválido. Tente novamente.");
                    Console.Write("ProdutoId (ou 0 para finalizar): ");
                    produtoIdStr = Console.ReadLine() ?? string.Empty;
                }
                if (produtoId == 0)
                {
                    break;
                }
                Console.Write("Quantidade: ");
                string quantidadeStr = Console.ReadLine() ?? string.Empty;
                int quantidade;
                while (!int.TryParse(quantidadeStr, out quantidade) || quantidade <= 0)
                {
                    Console.WriteLine("Quantidade inválida. Tente novamente.");
                    Console.Write("Quantidade: ");
                    quantidadeStr = Console.ReadLine() ?? string.Empty;
                }

                bool found = false;
                foreach (var item in itens)
                {
                    if (item.ProdutoId == produtoId)
                    {
                        item.Quantidade += quantidade;
                        found = true;
                    }
                }

                if (found) continue;

                itens.Add(new PedidoItem
                {
                    ProdutoId = produtoId,
                    Quantidade = quantidade
                });
            }

            if (itens.Count == 0)
            {
                Console.WriteLine("Nenhum item foi adicionado ao pedido. Operação cancelada.");
                return;
            }

            var pedido = new Pedido
            {
                ClienteId = clienteId,
            };

            repo.CriarPedido(pedido, itens);
        }

        static void ListarPedidosDeCliente(PedidoRepository repo)
        {
            Console.WriteLine("\n=== PEDIDOS DO CLIENTE ===");

            Console.Write("ClienteId: ");
            string clienteIdStr = Console.ReadLine() ?? string.Empty;
            int clienteId;
            while (!int.TryParse(clienteIdStr, out clienteId) || clienteId <= 0)
            {
                Console.WriteLine("ClienteId inválido. Tente novamente.");
                Console.Write("ClienteId: ");
                clienteIdStr = Console.ReadLine() ?? string.Empty;
            }

            repo.ListarPedidosCliente(clienteId);
        }

        static void DetalhesDoPedido(PedidoRepository repo)
        {
            Console.WriteLine("\n=== DETALHES DO PEDIDO ===");

            Console.Write("PedidoId: ");
            string pedidoIdStr = Console.ReadLine() ?? string.Empty;
            int pedidoId;
            while (!int.TryParse(pedidoIdStr, out pedidoId) || pedidoId <= 0)
            {
                Console.WriteLine("PedidoId inválido. Tente novamente.");
                Console.Write("PedidoId: ");
                pedidoIdStr = Console.ReadLine() ?? string.Empty;
            }

            repo.ObterDetalhesPedido(pedidoId);
        }
        
        static void VendasNoPeriodo(PedidoRepository repo)
        {
            Console.WriteLine("\n=== VENDAS DENTRO DO PERIODO ===");

            Console.Write("Data Início (AAAA-MM-DD): ");
            string dataInicioStr = Console.ReadLine() ?? string.Empty;
            DateTime dataInicio;
            while (!DateTime.TryParseExact(dataInicioStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataInicio))
            {
                Console.WriteLine("Data Início inválida. Tente novamente.");
                Console.Write("Data Início (AAAA-MM-DD): ");
                dataInicioStr = Console.ReadLine() ?? string.Empty;
            }

            Console.Write("Data Fim (AAAA-MM-DD): ");
            string dataFimStr = Console.ReadLine() ?? string.Empty;
            DateTime dataFim;
            while (!DateTime.TryParseExact(dataFimStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataFim) || dataFim < dataInicio)
            {
                Console.WriteLine("Data Fim inválida. Tente novamente.");
                Console.Write("Data Fim (AAAA-MM-DD): ");
                dataFimStr = Console.ReadLine() ?? string.Empty;
            }

            repo.TotalVendasPorPeriodo(dataInicio, dataFim);
        }
    }
}