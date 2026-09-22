# 🛒 VendasOnline API — Sistema RESTful MVC em .NET 10 & GCP

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![C# 13](https://img.shields.io/badge/C%23-13.0-239120?logo=csharp)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-4169E1?logo=postgresql)
![Google Cloud](https://img.shields.io/badge/GCP-Cloud_Run_%26_Cloud_SQL-4285F4?logo=googlecloud)
![Docker](https://img.shields.io/badge/Docker-Containerized-2496ED?logo=docker)
![GitHub Actions](https://img.shields.io/badge/CI%2FCD-GitHub_Actions-2088FF?logo=githubactions)
![Code Coverage](https://img.shields.io/badge/Coverage-%E2%89%A580%25-brightgreen?logo=codecov)

> **Projeto Final de Conclusão — Bootcamp de Arquitetura de Software**  
> Solução corporativa de API REST desenvolvida sob o padrão arquitetural **MVC**, com persistência de dados em **PostgreSQL**, arquitetura documentada no modelo **C4**, esteira de **CI/CD no GitHub Actions** com meta de cobertura de código **≥ 80%** e deploy serverless na **Google Cloud Platform (GCP)**.

---

## 🎯 Objetivo do Projeto

Como **Arquiteto(a) de Software** em uma grande empresa de comércio eletrônico, o objetivo deste projeto é projetar, documentar e implantar uma **API RESTful escalável e segura** para disponibilizar publicamente os dados de **Clientes, Produtos e Pedidos** a parceiros de negócios.

A solução atende rigorosamente a todos os requisitos do desafio:
- Operações de **CRUD completo** para todas as entidades de domínio.
- Endpoints especializados para **Contagem de Registros** (`/contar`).
- Consultas otimizadas para **Listagem Geral** (`Find All`), **Busca por ID** (`Find By ID`) e **Busca por Nome/Descrição** (`Find By Name`).
- Regras de negócio avançadas com **gestão atômica de estoque** (baixa na criação e devolução na exclusão do pedido).
- Documentação formal no **Modelo C4** (Níveis 1, 2 e 3) e testes automatizados em todas as camadas:
  - 🌐 **Nível 1 — Diagrama de Contexto:** [`Diagrama C4 - 1 Contexto.jpg`](./docs/Diagrama%20C4%20-%201%20Contexto.jpg) — Visão geral da interação dos parceiros de negócios com o ecossistema VendasOnline e a GCP.
  - 📦 **Nível 2 — Diagrama de Contêineres:** [`Diagrama C4 - 2 Container.jpg`](./docs/Diagrama%20C4%20-%202%20Container.jpg) — Visão da arquitetura de contêineres com Cloud Run, Cloud SQL (PostgreSQL), Artifact Registry e GitHub Actions.
  - ⚙️ **Nível 3 — Diagrama de Componentes:** [`Diagrama C4 - 3 Componentes.jpg`](./docs/Diagrama%20C4%20-%203%20Componentes.jpg) — Detalhamento interno dos componentes da API (Controllers, Services, Repositories, DbContext e Middlewares).

---

## 🛠️ Tecnologias e Ferramentas Utilizadas

### **Backend & Frameworks**
- **.NET 10 / C# 13:** Framework e linguagem principal aproveitando recursos modernos como *Primary Constructors*, *Records* imutáveis, *Pattern Matching* e *Top-Level Statements*.
- **ASP.NET Core Web API:** Padrão arquitetural MVC/REST para construção dos controllers e endpoints HTTP.
- **Entity Framework Core 10:** ORM para mapeamento objeto-relacional, suporte a consultas otimizadas (`AsNoTracking`, `Include`) e migrações automatizadas (`Database.MigrateAsync()`).
- **Npgsql Provider:** Provedor de alta performance para integração entre EF Core e PostgreSQL.

### **Banco de Dados & Infraestrutura Cloud**
- **PostgreSQL 15+:** Banco de dados relacional (instância local via Docker Compose e gerenciado na GCP via Cloud SQL).
- **Google Cloud Platform (GCP):**
  - **GCP Cloud Run:** Hospedagem serverless da API em containers Docker com auto-scaling.
  - **GCP Cloud SQL:** Persistência relacional gerenciada.
  - **Google Artifact Registry (GAR):** Armazenamento de imagens Docker privadas.
  - **Workload Identity Federation (OIDC):** Autenticação segura sem exposição de chaves JSON estáticas.

### **Qualidade, Testes & DevSecOps**
- **xUnit & Moq:** Framework de testes e biblioteca de simulação para testes unitários isolados.
- **EF Core InMemory:** Banco de dados em memória para validação dos repositórios e consultas LINQ.
- **Coverlet:** Ferramenta de análise de cobertura de código com trava de aprovação (**Threshold ≥ 80%**).
- **GitHub Actions:** Esteira de CI/CD para compilação, testes automatizados, scan de vulnerabilidades e deploy.
- **Trivy Scanner:** Análise de vulnerabilidades em imagens de container Docker.
- **Swagger / OpenAPI (Swashbuckle):** Documentação e teste interativo dos endpoints RESTful.

---

## 🏛️ Padrões Arquiteturais e Estrutura de Camadas

A aplicação foi desenvolvida seguindo o padrão **MVC (Model-View-Controller)** adaptado para APIs RESTful e princípios de **Clean Architecture**, dividida em camadas bem definidas e desacopladas:

```text
VendasOnline.API/
│
├── Controllers/                 # Camada Web (Endpoints HTTP, Rotas e Status Codes)
│   ├── ClientesController.cs
│   ├── ProdutosController.cs
│   └── PedidosController.cs
│
├── Services/                    # Camada de Negócio (Regras, Validações e Estoque)
│   ├── Interfaces/              # Contratos das Services (IClienteService, etc.)
│   ├── ClienteService.cs
│   ├── ProdutoService.cs
│   └── PedidoService.cs
│
├── Data/                        # Camada de Acesso a Dados (EF Core & Mapeamentos)
│   ├── Configurations/          # Fluent API (Relacionamentos, Índices e Chaves)
│   ├── Migrations/              # Histórico de Schema do Banco de Dados
│   ├── Repositories/            # Implementação dos Repositórios (Generic & Specific)
│   └── VendasOnlineDbContext.cs # Contexto do Entity Framework Core
│
├── Dtos/                        # Data Transfer Objects (Contratos de Entrada e Saída)
│   ├── Request/                 # DTOs de Entrada com Data Annotations
│   └── Response/                # DTOs de Saída em Formato C# Record
│
├── Models/                      # Entidades de Domínio (Cliente, Produto, Pedido, ItemPedido)
├── Enums/                       # Enumeradores de Domínio (StatusPedido)
└── Middlewares/                 # Middlewares Globais (GlobalExceptionHandler)
```

### **1. Pattern Repository (Generic & Specific)**
- **`IRepository<T>` / `Repository<T>`:** Encapsula as operações básicas de acesso a dados (`ObterTodosAsync`, `ObterPorIdAsync`, `BuscarAsync`, `ContarAsync`, `AdicionarAsync`, `AtualizarAsync`, `RemoverAsync`).
- **Repositórios Específicos:** `ClienteRepository`, `ProdutoRepository` e `PedidoRepository` estendem o repositório genérico para incluir consultas customizadas com carregamento adiantado (`Include`) e filtros otimizados.

### **2. Pattern Service (Domain Services)**
- Isolam a lógica de negócio dos Controllers. Toda a regra de validação de clientes, verificação e baixa de estoque, soma do valor total do pedido e reversão de estoque ao deletar um pedido residem exclusivamente nos serviços.

### **3. DTOs (Data Transfer Objects com C# Records)**
- Imutabilidade e segurança no tráfego de dados. DTOs de requisição possuem validações via Data Annotations (`[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]`).

### **4. Trativa Global de Exceções (`GlobalExceptionHandler`)**
- Middleware customizado que intercepta exceções da aplicação e retorna respostas padronizadas conforme o padrão **RFC 7807 (ProblemDetails)**:
  - `ArgumentException` → **400 Bad Request**
  - `KeyNotFoundException` → **404 Not Found**
  - `InvalidOperationException` → **400 Bad Request**
  - Erros não tratados → **500 Internal Server Error**

---

## ⚙️ Regras de Negócio Implementadas

### **👤 Gestão de Clientes (`ClientesController` & `ClienteService`)**
- Cadastro de clientes com validação de formato de e-mail e obrigatoriedade de campos.
- Busca por nome com suporte a correspondência parcial e busca insensível a maiúsculas/minúsculas (`ToLower().Contains()`).
- Proteção de integridade relacional: impede a exclusão física de clientes que possuem pedidos vinculados no sistema (`DeleteBehavior.Restrict`).

### **📦 Gestão de Produtos (`ProdutosController` & `ProdutoService`)**
- Cadastro de produtos com controle de preço unitário e saldo de estoque.
- Criação de índice de banco de dados (`IX_Produtos_Descricao`) para otimizar as consultas do endpoint `FindByName`.
- Validação para impedir cadastros com preços negativos ou estoque menor que zero.

### **🛒 Gestão de Pedidos e Estoque (`PedidosController` & `PedidoService`)**
- **Criação de Pedido:**
  1. Valida se o cliente existe.
  2. Valida se há pelo menos um item no pedido.
  3. Verifica a disponibilidade em estoque para cada produto solicitado.
  4. Executa a **baixa automática no estoque** do produto (`QuantidadeEstoque -= Quantidade`).
  5. Calcula o valor total do pedido de forma atômica.
- **Atualização de Status:** Permite a transição entre os estados do enum `StatusPedido` (`Pendente`, `Processando`, `Concluido`, `Cancelado`).
- **Exclusão de Pedido e Devolução de Estoque:**
  - Ao excluir um pedido, a aplicação carrega os itens associados e **soma as quantidades compradas de volta ao estoque** de cada produto.
  - Para evitar conflitos no *Change Tracker* do Entity Framework Core ao remover instâncias não rastreadas (`AsNoTracking`), a navegação `item.Produto` é desvinculada em memória antes da deleção, garantindo execução limpa e transacional no PostgreSQL.

---

## 🧪 Qualidade de Código & Esteira CI/CD (GitHub Actions)

### **1. Cobertura de Código Mínima de 80% (Threshold)**
A suíte de testes unitários cobre integralmente as três camadas da aplicação (**Repositories**, **Services** e **Controllers**).

Para garantir a qualidade contínua, o projeto possui o **Coverlet** configurado com um limite mínimo obrigatório de **80% de cobertura de linhas**:
```bash
dotnet test /p:CollectCoverage=true /p:Threshold=80 /p:ThresholdType=line /p:Exclude="[*]*Data.Migrations.*"
```

> **Isolamento de Código Gerado:** As classes auto-geradas do Entity Framework Core (`Data/Migrations/*`) e o ponto de entrada da aplicação (`Program.cs`) utilizam o atributo `[ExcludeFromCodeCoverage]`, garantindo que o relatório reflita apenas a lógica de negócio real do sistema.

### **2. Esteira de CI/CD (`.github/workflows/ci.yml`)**
A automação no GitHub Actions é disparada em cada `push` ou `pull_request` para as branches `main` e `master`:

1. **Build & Test:** Restaura dependências, compila o projeto em modo `Release` e roda a suíte do `xUnit`. Se a cobertura for menor que 80% ou algum teste falhar, o pipeline é interrompido.
2. **NuGet Security Scan:** Executa `dotnet list package --vulnerable` para bloquear dependências com falhas de segurança conhecidas.
3. **Container Scan (Trivy):** Constrói a imagem Docker local e analisa vulnerabilidades críticas no SO/pacotes do container.
4. **Deploy no GCP Cloud Run:** Autentica na GCP via Workload Identity (OIDC), gera a imagem, publica no Artifact Registry e atualiza a revisão no Cloud Run conectando-a ao Cloud SQL PostgreSQL.

---

## 🚀 Como Executar o Projeto Localmente

### **Pré-requisitos**
- SDK do **.NET 10** instalado.
- **Docker & Docker Compose** instalados.

### **Passo a Passo**

1. **Clonar o Repositório:**
   ```bash
   git clone https://github.com/GLEIDOSN/VendasOnline.git
   cd VendasOnline
   ```

2. **Subir o Banco de Dados PostgreSQL via Docker Compose:**
   ```bash
   docker-compose -f VendasOnline.API/Docker/docker-compose.yml up -d
   ```

3. **Executar as Migrations e Iniciar a API:**
   ```bash
   dotnet run --project VendasOnline.API
   ```
   *As migrations do banco serão aplicadas automaticamente na inicialização!*

4. **Acessar a Documentação do Swagger:**
   Abra o navegador em: `http://localhost:5102` (ou `https://localhost:7001`).

5. **Rodar a Suíte Completa de Testes:**
   ```bash
   dotnet test --verbosity normal
   ```

---

## 💼 Destaques para Portfólio / LinkedIn

Esta aplicação demonstra competências avançadas de **Engenharia e Arquitetura de Software**:
- 📐 **Modelagem Arquitetural:** Especificação completa com o **Modelo C4** e padrão **MVC/REST**.
- ☁️ **Cloud Native & Serverless:** Deploy conteinerizado no **GCP Cloud Run** e **Cloud SQL**.
- 🛡️ **DevSecOps & CI/CD:** Pipeline automatizada no **GitHub Actions** com auditoria de pacotes e scan de containers com **Trivy**.
- 🧪 **Test-Driven Design & High Coverage:** Cobertura de testes unitários **≥ 80%** com **xUnit**, **Moq** e **Coverlet**.
- ⚡ **Resiliência & Consistência:** Controle transacional de estoque e tratamento global de erros conforme normas RFC.
