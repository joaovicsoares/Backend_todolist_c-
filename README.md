# TodoList API - Backend C#

API REST para sistema de lista de tarefas colaborativo desenvolvida em ASP.NET Core com PostgreSQL.

## 🚀 Funcionalidades Atuais

### Autenticação
- **Registro de usuário** - Criação de conta com nome, email e senha
- **Login** - Autenticação via JWT com duração de 8 horas
- **Proteção de rotas** - Endpoints protegidos por JWT Bearer Token

### Gerenciamento de Listas
- **Criar lista** - Usuário pode criar listas de tarefas
- **Listar listas** - Visualizar todas as listas que o usuário tem acesso
- **Editar lista** - Alterar nome da lista (apenas quem tem acesso)
- **Deletar lista** - Remover lista (apenas quem tem acesso)

### Gerenciamento de Tarefas
- **Criar tarefa** - Adicionar tarefas a uma lista específica
- **Listar tarefas** - Ver todas as tarefas de uma lista
- **Marcar como concluída** - Alterar status de conclusão da tarefa
- **Deletar tarefa** - Remover tarefa da lista

### Compartilhamento Colaborativo
- **Compartilhar lista** - Adicionar outros usuários à lista usando email
- **Controle de acesso** - Apenas usuários com acesso podem compartilhar
- **Validações** - Verifica se usuário existe e se já não tem acesso

## 🛠️ Tecnologias

- **Framework**: ASP.NET Core (.NET 10.0)
- **Banco de Dados**: PostgreSQL
- **ORM**: Entity Framework Core
- **Autenticação**: JWT Bearer Token
- **Hash de Senha**: BCrypt
- **Arquitetura**: MVC com DTOs

## 📋 Pré-requisitos

- .NET 10.0 SDK
- PostgreSQL
- Visual Studio / VS Code (opcional)

## ⚙️ Configuração e Execução

### 1. Clone o repositório
```bash
git clone <url-do-repositorio>
cd Backend_todolist_c-
```

### 2. Configure o banco de dados
Edite `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "ConexaoPadrao": "Host=localhost:5432;Database=tarefas;Username=postgres;Password=sua_senha"
  }
}
```

### 3. Crie o banco de dados
```sql
CREATE DATABASE tarefas;
```

### 4. Execute as migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Execute o projeto
```bash
dotnet restore
dotnet run
```

A API estará disponível em: `http://localhost:5038`

## 📚 Endpoints da API

### Autenticação
```http
POST /api/auth/signup    # Criar usuário
POST /api/auth/login     # Fazer login
```

### Listas
```http
GET    /api/list         # Listar listas do usuário
POST   /api/list         # Criar nova lista
PUT    /api/list/{id}    # Editar lista
DELETE /api/list/{id}    # Deletar lista
```

### Tarefas
```http
GET    /api/task/{idLista}  # Listar tarefas da lista
POST   /api/task            # Criar nova tarefa
PUT    /api/task/{id}       # Atualizar tarefa (marcar como concluída)
DELETE /api/task/{id}       # Deletar tarefa
```

### Compartilhamento
```http
POST /api/sharelist      # Compartilhar lista com outro usuário
```

## 📝 Exemplos de Uso

### Registro de Usuário
```http
POST /api/auth/signup
Content-Type: application/json

{
  "nome": "João Silva",
  "email": "joao@email.com",
  "senha": "123456"
}
```

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "joao@email.com",
  "senha": "123456"
}
```

### Criar Lista
```http
POST /api/list
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "Compras do Mercado"
}
```

### Criar Tarefa
```http
POST /api/task
Authorization: Bearer {token}
Content-Type: application/json

{
  "titulo": "Comprar leite",
  "idLista": 1
}
```

### Compartilhar Lista
```http
POST /api/sharelist
Authorization: Bearer {token}
Content-Type: application/json

{
  "idLista": 1,
  "email": "maria@email.com"
}
```

## 🗄️ Estrutura do Banco de Dados

### Tabelas
- **usuarios** - Dados dos usuários (id, nome, email, senha)
- **listas** - Listas de tarefas (id, nome)
- **tarefas** - Tarefas individuais (id, titulo, concluida, idlista)
- **lista_usuario** - Relacionamento many-to-many para compartilhamento

### Relacionamentos
- Um usuário pode ter várias listas (many-to-many via lista_usuario)
- Uma lista pode ter vários usuários (compartilhamento)
- Uma lista pode ter várias tarefas (one-to-many)

## 🔒 Segurança

- Senhas hasheadas com BCrypt
- Autenticação JWT com expiração
- Validação de acesso em todas as operações
- Proteção contra compartilhamento não autorizado
- Validação de entrada nos endpoints

## 📁 Estrutura do Projeto

```
├── Controllers/          # Controllers da API
│   ├── AuthController.cs
│   ├── ListController.cs
│   ├── TaskController.cs
│   └── ShareListController.cs
├── Data/                 # Contexto do banco de dados
│   └── AppDbContext.cs
├── Dtos/                 # Data Transfer Objects
│   ├── Auth/
│   ├── List/
│   └── Share/
├── Models/               # Modelos de dados
│   ├── Usuario.cs
│   ├── Lista.cs
│   ├── Tarefa.cs
│   └── ListaUsuario.cs
└── Program.cs           # Configuração da aplicação
```

## 🚧 TODO - Funcionalidades Futuras

### Segurança e Configuração
- [ ] Mover chave JWT para variáveis de ambiente
- [ ] Habilitar HTTPS em produção
- [ ] Implementar refresh tokens
- [ ] Adicionar rate limiting
- [ ] Configurar CORS para produção

### Validações e DTOs
- [ ] Adicionar Data Annotations nos DTOs
- [ ] Validação de email no registro
- [ ] Validação de força da senha
- [ ] Criar DTOs de resposta para todos os endpoints
- [ ] Implementar validação de entrada global

### Funcionalidades de Usuário
- [ ] Endpoint para alterar senha
- [ ] Endpoint para recuperar senha (reset via email)
- [ ] Perfil do usuário (GET/PUT /api/user/profile)
- [ ] Verificação de email no registro
- [ ] Upload de avatar do usuário

### Melhorias no Compartilhamento
- [ ] Listar usuários com acesso à lista (GET /api/sharelist/{idLista})
- [ ] Remover acesso de usuário (DELETE /api/sharelist/{idLista}/{email})
- [ ] Diferentes níveis de permissão (owner, editor, viewer)
- [ ] Notificações quando lista é compartilhada
- [ ] Buscar usuários por email para compartilhar

### Funcionalidades de Lista e Tarefas
- [ ] Ordenação de tarefas (prioridade, data)
- [ ] Categorias/tags para tarefas
- [ ] Data de vencimento para tarefas
- [ ] Descrição detalhada da tarefa
- [ ] Anexos em tarefas
- [ ] Comentários em tarefas
- [ ] Histórico de alterações

### Performance e Qualidade
- [ ] Implementar async/await em todos os endpoints
- [ ] Adicionar paginação nas listagens
- [ ] Implementar cache (Redis)
- [ ] Logging estruturado (Serilog)
- [ ] Testes unitários e de integração
- [ ] Documentação Swagger completa

### DevOps e Deploy
- [ ] Dockerfile para containerização
- [ ] CI/CD pipeline
- [ ] Configuração para diferentes ambientes
- [ ] Monitoramento e métricas
- [ ] Backup automático do banco

### API Melhorias
- [ ] Versionamento da API
- [ ] Filtros e busca avançada
- [ ] Exportar listas (PDF, Excel)
- [ ] API para estatísticas (tarefas concluídas, etc.)
- [ ] Webhooks para integrações

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.