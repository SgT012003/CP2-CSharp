# CP2-CSharp

## Alunos
|Nome|RM|
|:--:|:--:|
|Victor Didoff|552965|
|Matheus Zottis|94119|

## C#

- Version .NET CORE 9.0
- Dependencies [Microsoft.Data.SqlClient 6.1.2](https://www.nuget.org/packages/Microsoft.Data.SqlClient/6.1.2?_src=template)

## Docker

- `docker-compose.yml`

> o arquivo foi anexado ja com as configuracoes de build

### Commandos

1. Build and Up | Container

```bash
docker compose up -d
```

2. Delete | Container

```bash
docker compose down -v
```


## SQL Server

- Versao: 2022-latest | 16/10/2025
- Para se conectar atravez do SGDB use master no campo de database

```yml
enviroment:
    - SERVER : "Server=localhost,1433;"
    - DATABASE : "Database=LojaDB;"
    - USER: "User Id=sa;"
    - PASSWORD: "Password=SqlServer2024!;"
    - CONNECTION: "TrustServerCertificate=True;"
    - CONNECTIONSTRING: $SERVER $DATABASE $USER $PASSWORD $CONNECTION
```


## SGDB

- DBeaver Community

## Concluidos:

### Básicos (Obrigatórios)
1. [v] Connection String
2. [v] Listar Produtos
3. [v] Inserir Produto
4. [v] Atualizar Produto
5. [v] Deletar Produto
6. [v] Buscar por ID

### Intermediários (Obrigatórios)
7. [v] Listar por Categoria (JOIN)
8. [v] Listar Pedidos de Cliente
9. [v] Detalhes do Pedido

### Avançados (Obrigatórios)
10. [v] Criar Pedido com Transação

### Desafios (Opcionais - Pontos Extra)
- [v] Estoque Baixo
- [v] Busca por Nome (LIKE)
- [v] Total de Vendas por Período
- [v] Métodos Auxiliares Completos

## Directory Tree

```
├── SistemaLoja/
│   ├── SistemaLoja/
│   │   ├── Categoria.cs           #Editado
│   │   ├── CategoriaRepository.cs #Criado
│   │   ├── Cliente.cs
│   │   ├── DatabaseConnection.cs
│   │   ├── Pedido.cs              #Editado
│   │   ├── PedidoItem.cs
│   │   ├── PedidoRepository.cs    #(Exercicios 7 - 10 |  Desafio  3  )
│   │   ├── Produto.cs             #Editado
│   │   ├── ProdutoRepository.cs   #(Exercicios 1 - 6  | Desafios 1,2 )
│   │   ├── Program.cs             #Principal( Exercicios e Desafio 4 )
│   │   └── SistemaLoja.csproj
│   ├── SistemaLoja.sln
│   └── global.json
├── .gitignore
├── LICENSE
├── README.md                      #Esse Arquivo
├── Setup.sql
└── docker-compose.yml             #Criado
```
