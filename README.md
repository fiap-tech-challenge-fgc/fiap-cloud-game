
# fiap-cloud-game

## Visão Geral
Projeto desenvolvido com arquitetura limpa (Clean Architecture) para o desafio FGC. Ele utiliza .NET 8, Blazor, ASP.NET Core Identity e PostgreSQL como banco de dados.

## Tecnologia
- **Framework**: .NET 8 (Aspire)
- **Frontend**: Blazor
- **Banco de Dados**: PostgreSQL
- **Autenticação**: ASP.NET Core Identity
- **Arquitetura**: Clean Architecture
- **ORM**: Entity Framework Core
## Requisitos:
	1: [Docker](https://docs.docker.com/desktop/setup/install/windows-install/)

## Estrutura da Solução
- **FCG.Host**: Configuração inicial e ponto de entrada da aplicação.
- **FCG.Api**: API REST com endpoints para autenticação, administração e operações de jogadores.
- **FCG.Blazor**: Interface de usuário desenvolvida com Blazor.
- **FCG.Application**: Contém os serviços, interfaces e DTOs da aplicação.
  - **Interfaces**: Contratos para serviços.
  - **Services**: Implementações de lógica de negócios.
- **FCG.Domain**: Camada de domínio com entidades, enums e agregados.
  - **Entities**: Entidades principais do domínio.
  - **ValueObjects**: Objetos de valor.
  - **Enums**: Enumerações.
  - **Aggregates**: Agregados do domínio.
- **FCG.Infrastructure**: Implementações de infraestrutura, como persistência de dados e configurações de Identity.
  - **Data**: Contextos e migrations.
  - **Identity**: Configurações e extensões para ASP.NET Core Identity.
- **FCG.Tests**: Testes automatizados.

## Configuração do Banco de Dados
A connection string usada é `DbFcg`. Configure-a no arquivo `appsettings.json` do projeto `FCG.Api` ou `FCG.Host`.

Exemplo de configuração no `appsettings.json`:


## ASP.NET Core Identity
### Roles
- **Admin**: Permissões administrativas para gerenciar usuários e jogos.
- **Player**: Usuário comum que pode acessar funcionalidades de jogador.

### Seed Inicial
Recomenda-se criar um seed para o primeiro usuário administrador. Exemplo de código para `Program.cs`:

## Segurança
- Certifique-se de que os tokens JWT incluam:
  - `ClaimTypes.NameIdentifier` com o ID do usuário.
  - `ClaimTypes.Role` com as roles atribuídas.
- Use `[Authorize(Roles = RoleConstants.Admin)]` para proteger endpoints administrativos.

## Contribuição
1. Faça um fork do repositório.
2. Crie uma branch: `feature/nome-da-feature` ou `bugfix/nome-do-bug`.
3. Envie um Pull Request com uma descrição clara das mudanças.

## Estrutura base proposta
├─ FCG.Host/
│  └─ (config, startup, seeds)
│
├─ FCG.Api/
│  ├─ Controllers/
│  ├─ Config/
│  ├─ Properties/
│  └─ (Program.cs, appsettings.json no conteúdo do projeto)
│
├─ FCG.Blazor/
│  ├─ Pages/
│  ├─ Shared/
│  ├─ Services/
│  ├─ wwwroot/
│  └─ (Program.cs, _Imports.razor)
│
├─ FCG.Application/
│  ├─ Dtos/
│  ├─ Interfaces/
│  ├─ Services/
│  └─ Security/
│
├─ FCG.Domain/
│  ├─ Entities/
│  ├─ ValueObjects/
│  ├─ Enums/
│  ├─ Aggregates/
│  └─ Data/
│     ├─ Contexts/
│     ├─ Factories/
│     └─ Migrations/
│
├─ FCG.Infrastructure/
│  ├─ Data/
│  ├─ Identity/
│  ├─ Extensions/
│  └─ (implementations)
│
└─ FCG.Tests/
   ├─ Unit/
   └─ Integration/
