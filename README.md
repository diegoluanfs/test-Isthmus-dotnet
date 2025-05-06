# Test Isthmus .NET

## Descrição do Projeto
Este projeto é uma API desenvolvida em .NET para gerenciar produtos e suas operações. Ele utiliza o Entity Framework Core para acesso ao banco de dados e segue boas práticas de desenvolvimento, como injeção de dependência e separação de responsabilidades.

## Tecnologias Utilizadas
- **.NET 9.0**: Framework principal para desenvolvimento da API.
- **Entity Framework Core**: ORM para acesso ao banco de dados.
- **SQL Server**: Banco de dados relacional.
- **Docker**: Para containerização da aplicação e do banco de dados.
- **Swagger**: Para documentação e teste dos endpoints da API.
- **Serilog**: Para logging estruturado.
- **xUnit**: Framework para testes unitários.

---

## Como Rodar o Projeto Localmente

### Pré-requisitos
- .NET SDK 9.0 instalado.
- SQL Server instalado e configurado.
- Docker (opcional, caso queira rodar com container).

### Configuração do Banco de Dados
Atualize a string de conexão no arquivo `appsettings.Development.json`:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TestIsthmusDB;User=sa;Password=YourStrong!Password;Encrypt=False;TrustServerCertificate=True"
}
```

### Executar a Aplicação
No terminal, navegue até a pasta do projeto e execute:

```bash
dotnet restore
dotnet build
dotnet run --project src/API
```

### Acessar o Swagger
Abra o navegador e acesse: [http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## Como Rodar os Testes
1. Navegue até a pasta do projeto no terminal.
2. Execute o comando:

```bash
dotnet test --logger "console;verbosity=detailed"
```

---

## Como Rodar o Projeto com Docker

### Pré-requisitos
- Docker e Docker Compose instalados.

### Subir os Containers
No terminal, execute:

```bash
docker-compose up --build
```

### Acessar a API
Acesse o Swagger em: [http://localhost:5000/swagger](http://localhost:5000/swagger)

### Parar os Containers
Para parar os containers, execute:

```bash
docker-compose down
```

---

## Lista de Endpoints da API

### Produtos
- **GET** `/api/products`: Retorna a lista de produtos.
- **GET** `/api/products/{id}`: Retorna um produto específico.
- **POST** `/api/products`: Cria um novo produto.
- **PUT** `/api/products/{id}`: Atualiza um produto existente.
- **DELETE** `/api/products/{id}`: Remove um produto.

---

## Lista de Tarefas Implementadas
- Configuração inicial do projeto.
- Implementação do CRUD de produtos.
- Configuração do Entity Framework Core.
- Configuração do Docker para a aplicação e banco de dados.
- Adição de testes unitários com xUnit.
- Configuração do Swagger para documentação da API.
- Configuração do Serilog para logging.