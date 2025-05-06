
### **1. Configuração do Projeto**
**Título do Card:** Configurar a Solução e os Projetos Base  
**Descrição:**  
- Criar a solução `.NET` com os projetos:
  - `Domain` (Camada de Domínio)
  - `Application` (Camada de Aplicação)
  - `Infrastructure` (Camada de Infraestrutura)
  - `API` (Camada de Apresentação)
  - Tests (Projeto de Testes)
- Configurar o .gitignore para ignorar arquivos desnecessários.
- Criar um arquivo de solução (`.sln`) e adicionar todos os projetos.

**Critérios de Aceitação:**  
- A solução deve conter todos os projetos organizados.
- O .gitignore deve estar configurado corretamente.
- O projeto deve compilar sem erros.

---

### **2. Criar a Entidade `Product`**
**Título do Card:** Criar a Entidade `Product`  
**Descrição:**  
- Criar a classe `Product` no projeto `Domain` com os seguintes campos:
  - `Id` (gerado pelo banco)
  - `Codigo` (string, único)
  - `Nome` (string)
  - `Descricao` (string)
  - `Preco` (decimal)
  - `Ativo` (bool)
- Implementar validações básicas na entidade, como obrigatoriedade de campos.

**Critérios de Aceitação:**  
- A classe `Product` deve estar no namespace correto.
- Todos os campos devem estar implementados.
- Deve haver testes unitários para validar a criação da entidade.

---

### **3. Configurar o Banco de Dados**
**Título do Card:** Configurar o Banco de Dados com Entity Framework Core  
**Descrição:**  
- Configurar o `ApplicationDbContext` no projeto `Infrastructure`.
- Mapear a entidade `Product` no banco de dados.
- Criar as migrações iniciais e aplicar ao banco de dados SQL Server.

**Critérios de Aceitação:**  
- O banco de dados deve ser criado com a tabela `Products`.
- A tabela deve conter as colunas correspondentes à entidade `Product`.
- Deve haver testes para validar a conexão com o banco de dados.

---

### **4. Criar o Endpoint `GET /api/Products`**
**Título do Card:** Implementar o Endpoint `GET /api/Products`  
**Descrição:**  
- Criar o método no `ProductsController` para retornar todos os produtos.
- Configurar o endpoint para responder com `200 OK` e a lista de produtos.

**Critérios de Aceitação:**  
- O endpoint deve retornar todos os produtos cadastrados.
- Deve haver testes de integração para validar o comportamento do endpoint.

---

### **5. Criar o Endpoint `POST /api/Products`**
**Título do Card:** Implementar o Endpoint `POST /api/Products`  
**Descrição:**  
- Criar o método no `ProductsController` para criar um novo produto.
- Validar os campos obrigatórios.
- Impedir duplicação de `Codigo` (atualizar se já existir).

**Critérios de Aceitação:**  
- O endpoint deve criar um produto e retornar `201 Created`.
- Deve haver testes de integração para validar o comportamento do endpoint.

---

### **6. Criar o Endpoint `PUT /api/Products/{id}`**
**Título do Card:** Implementar o Endpoint `PUT /api/Products/{id}`  
**Descrição:**  
- Criar o método no `ProductsController` para atualizar um produto existente.
- Validar os campos obrigatórios.
- Retornar `404 Not Found` se o produto não existir.

**Critérios de Aceitação:**  
- O endpoint deve atualizar o produto e retornar `204 No Content`.
- Deve haver testes de integração para validar o comportamento do endpoint.

---

### **7. Criar o Endpoint `DELETE /api/Products/{id}`**
**Título do Card:** Implementar o Endpoint `DELETE /api/Products/{id}`  
**Descrição:**  
- Criar o método no `ProductsController` para realizar a exclusão lógica de um produto (`Ativo = false`).
- Retornar `404 Not Found` se o produto não existir.

**Critérios de Aceitação:**  
- O endpoint deve marcar o produto como inativo e retornar `204 No Content`.
- Deve haver testes de integração para validar o comportamento do endpoint.

---

### **8. Configurar o Swagger**
**Título do Card:** Configurar o Swagger para Documentação da API  
**Descrição:**  
- Configurar o Swagger no projeto `API`.
- Garantir que todos os endpoints estejam documentados automaticamente.

**Critérios de Aceitação:**  
- O Swagger deve estar acessível em `/swagger`.
- Todos os endpoints devem estar documentados.

---

### **9. Configurar o Serilog**
**Título do Card:** Configurar o Serilog para Logging  
**Descrição:**  
- Configurar o Serilog no projeto `API`.
- Garantir que logs sejam gerados para cada requisição.

**Critérios de Aceitação:**  
- Logs devem ser gerados para cada requisição.
- Deve haver testes para validar a configuração do Serilog.

---

### **10. Configurar o Banco de Dados em Memória para Testes**
**Título do Card:** Configurar o Banco de Dados em Memória para Testes  
**Descrição:**  
- Configurar o `ApplicationDbContext` para usar o banco de dados em memória no ambiente de teste.
- Garantir que os testes não dependam do banco de dados real.

**Critérios de Aceitação:**  
- O banco de dados em memória deve ser usado nos testes.
- Todos os testes devem passar sem dependência do banco real.

---

### **11. Criar Testes de Integração**
**Título do Card:** Implementar Testes de Integração para os Endpoints  
**Descrição:**  
- Criar testes de integração para os endpoints:
  - `GET /api/Products`
  - `POST /api/Products`
  - `PUT /api/Products/{id}`
  - `DELETE /api/Products/{id}`

**Critérios de Aceitação:**  
- Todos os testes devem passar.
- Os testes devem cobrir os cenários principais e de erro.

**Observação**
- Para o futuro: Tirar os testes do Dockerfile e colocar no CI/CD

---

### **12. Criar o Dockerfile**
**Título do Card:** Criar o Dockerfile para a Aplicação  
**Descrição:**  
- Criar um `Dockerfile` para a aplicação.
- Configurar o build e a execução da aplicação no container.

**Critérios de Aceitação:**  
- A aplicação deve rodar corretamente em um container Docker.

---

### **13. Criar o `docker-compose.yml`**
**Título do Card:** Criar o `docker-compose.yml` para Subir a Aplicação e o Banco  
**Descrição:**  
- Criar um arquivo `docker-compose.yml` para subir a aplicação e o banco de dados SQL Server.

**Critérios de Aceitação:**  
- O comando `docker-compose up` deve subir a aplicação e o banco de dados.

---

### **14. Criar o README.md**
**Título do Card:** Criar a Documentação do Projeto  
**Descrição:**  
- Criar o arquivo README.md com:
  - Descrição do projeto.
  - Tecnologias utilizadas.
  - Como rodar o projeto localmente.
  - Como rodar os testes.
  - Como rodar o projeto com Docker.
  - Lista de endpoints da API.
  - Lista de tarefas implementadas.

**Critérios de Aceitação:**  
- O arquivo README.md deve conter todas as informações necessárias para rodar o projeto.

---

**ERROS/QUESTIONAMENTOS IDENTIFICADOS**
- (Resolvido) Ao atualizar o código do produto, ele permite que tenham o mesmo código.
- O usuário deveria ter a possibilidade de alterar a propriedade Ativo de um produto?