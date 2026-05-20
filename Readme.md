# LojaDoImovel API

API RESTful para gerenciamento de imóveis com suporte a múltiplos empreendimentos (multi-tenant), autenticação JWT, controle de acesso por perfis e notificações por e-mail.

## Tecnologias

- **ASP.NET Core** (.NET 8+)
- **Entity Framework Core** com PostgreSQL (Npgsql)
- **ASP.NET Core Identity** — autenticação e gerenciamento de usuários
- **JWT Bearer** — autenticação stateless com access token e refresh token
- **Mapster** — mapeamento de objetos (DTOs)
- **Swagger / OpenAPI** — documentação interativa da API
- **Rate Limiting** — proteção contra abuso (100 req/min global)
- **SMTP Gmail** — envio de e-mails transacionais

## Arquitetura

O projeto segue a **Clean Architecture** dividida nos seguintes projetos:

```
LojaDoImovel.Api            → Controllers, Filters, configuração HTTP
LojaDoImovel.Application    → Services, interfaces, settings
LojaDoImovel.Domain         → Entidades de domínio
LojaDoImovel.Infrastructure → DbContext, Repositories, Identity
LojaDoImovel.Contracts      → DTOs de entrada/saída
LojaDoImovel.Transform      → Perfis de mapeamento (Mapster)
LojaDoImovel.IoC            → Registro de dependências
LojaDoImovel.Tests          → Testes unitários
```

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/) rodando localmente
- Conta Gmail para envio de e-mails (opcional)

## Configuração

### 1. Banco de dados

Crie um banco no PostgreSQL:

```sql
CREATE DATABASE lojadoimovel_db;
```

### 2. appsettings.json

Edite `LojaDoImovel.Api/appsettings.json` com suas credenciais:

```json
{
  "ConnectionStrings": {
    "PgSql": "Host=localhost;Database=lojadoimovel_db;Username=postgres;Password=SUA_SENHA"
  },
  "EmailConfig": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "EnableSsl": "true",
    "Username": "seu-email@gmail.com",
    "Password": "sua-senha-de-app"
  },
  "Jwt": {
    "ValidAudience": "https://localhost:7294",
    "ValidIssuer": "https://localhost:7294",
    "SecretKey": "TROQUE_POR_UMA_CHAVE_SECRETA_FORTE",
    "TokenValidityInMinutes": 30,
    "RefreshTokenValidityInMinutes": 60
  }
}
```

> **Importante:** Nunca suba a `SecretKey` real para o controle de versão. Use User Secrets ou variáveis de ambiente em produção.

### 3. Migrations

Execute as migrations para criar as tabelas:

```bash
dotnet ef database update --project LojaDoImovel.Infrastructure --startup-project LojaDoImovel.Api
```

### 4. Executar a API

```bash
dotnet run --project LojaDoImovel.Api
```

A API estará disponível em `https://localhost:7294`. O Swagger estará acessível em `https://localhost:7294/swagger` (apenas em ambiente de desenvolvimento).

## Autenticação e Perfis

| Role              | Descrição                                      |
|-------------------|------------------------------------------------|
| `admin`           | Gerencia usuários, aprova cadastros            |
| `userapproved`    | Usuário ativo, pode gerenciar imóveis          |
| `userunapproved`  | Usuário recém-cadastrado, aguardando aprovação |

### Fluxo de aprovação

1. Usuário se cadastra via `POST /api/auth/register` → recebe e-mail de boas-vindas
2. Admin lista pendentes via `GET /api/auth/pending-users`
3. Admin aprova via `PUT /api/auth/approve-user?username=...` → usuário recebe e-mail de aprovação

## Endpoints

### Auth — `/api/auth`

| Método | Rota              | Autorização    | Descrição                        |
|--------|-------------------|----------------|----------------------------------|
| POST   | `/login`          | Público        | Autentica e retorna JWT          |
| POST   | `/register`       | Público        | Cadastra novo usuário            |
| POST   | `/refresh-token`  | Público        | Renova access + refresh token    |
| PUT    | `/approve-user`   | `admin`        | Aprova cadastro de usuário       |
| GET    | `/pending-users`  | `admin`        | Lista usuários pendentes         |

### Empreendimentos — `/api/enterprise`

| Método | Rota           | Autorização | Descrição                         |
|--------|----------------|-------------|-----------------------------------|
| POST   | `/`            | Público     | Cria novo empreendimento          |
| GET    | `/`            | Público     | Lista todos os empreendimentos    |
| GET    | `/id`          | Público     | Busca por ID                      |
| GET    | `/name`        | Público     | Busca por nome                    |
| PUT    | `/`            | Público     | Atualiza empreendimento           |
| PUT    | `/unactivate`  | Público     | Desativa empreendimento           |

### Imóveis — `/api/property`

| Método | Rota      | Autorização     | Descrição                          |
|--------|-----------|-----------------|------------------------------------|
| POST   | `/`       | `userapproved`  | Cadastra novo imóvel               |
| GET    | `/`       | Público         | Lista imóveis de um empreendimento |
| GET    | `/id`     | Público         | Busca imóvel por ID                |
| GET    | `/code`   | Público         | Busca imóvel por código            |
| PUT    | `/`       | `userapproved`  | Atualiza imóvel                    |
| DELETE | `/`       | `userapproved`  | Remove imóvel                      |

### Upload — `/api/upload`

Gerenciamento de imagens dos imóveis.

## Modelo de Dados

### Property (Imóvel)
- Identificação: `Code`, `Title`, `Description`
- Preços: `SalePrice`, `RentalPrice`, `CondoFee`, `PropertyTax`
- Localização: `Street`, `Number`, `Complement`, `Neighborhood`, `City`, `State`, `ZipCode`
- Detalhes: `Bedrooms`, `Suites`, `Bathrooms`, `LivingRooms`, `ParkingSpaces`, `PrivateArea`, `TotalArea`
- Classificação: `PropertyType`, `Purpose`, `IsPublished`, `IsFeatured`

### Enterprise (Empreendimento)
- Identificador único com `Slug`
- Relacionamento 1:N com `Property`

## CORS

Por padrão, a API aceita requisições de `http://localhost:3000`. Para alterar, edite a política de CORS em `Program.cs`.

## Testes

```bash
dotnet test LojaDoImovel.Tests
```