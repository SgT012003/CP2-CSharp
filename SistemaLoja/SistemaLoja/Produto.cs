using System.Drawing;
using System.Text.Json;

namespace SistemaLoja.Lab12_ConexaoSQLServer;

public class Produto
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public int CategoriaId { get; set; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
    public ConsoleColor Color(int min)
    {
        if (Estoque == 0)
        {
            return ConsoleColor.Red;
        }
        if (Estoque < min)
        {
            return ConsoleColor.Yellow;
        }
        return ConsoleColor.White;
    }
}