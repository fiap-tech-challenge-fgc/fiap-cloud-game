
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
/
├─/FCG.Host/
│  └─ (config, startup, seeds)
│
├─/FCG.Api/
│  ├─ Controllers/
│  ├─ Config/
│  ├─ Properties/
│  └─ (Program.cs, appsettings.json no conteúdo do projeto)
│
├─/FCG.Blazor/
│  ├─ Pages/
│  ├─ Shared/
│  ├─ Services/
│  ├─ wwwroot/
│  └─ (Program.cs, _Imports.razor)
│
├─/FCG.Application/
│  ├─ Dtos/
│  ├─ Interfaces/
│  ├─ Services/
│  └─ Security/
│
├─/FCG.Domain/
│  ├─ Entities/
│  ├─ ValueObjects/
│  ├─ Enums/
│  ├─ Aggregates/
│  └─ Data/
│     ├─ Contexts/
│     ├─ Factories/
│     └─ Migrations/
│
├─/FCG.Infrastructure/
│  ├─ Data/
│  ├─ Identity/
│  ├─ Extensions/
│  └─ (implementations)
│
└─/FCG.Tests/
   ├─ Unit/
   └─ Integration/
<!-- Banner -->
<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=6C63FF&height=180&section=header&text=FIAP%20Cloud%20Games%20(FCG)&fontSize=38&fontColor=ffffff&animation=fadeIn&fontAlignY=35" />
</p>

<h1 align="center">🎮 FIAP Cloud Games (FCG)</h1>

<p align="center">
  <i>Uma plataforma de venda de jogos digitais e gestão de servidores para partidas online.</i>
</p>

---

## 🧭 Sobre o Projeto

A **FIAP Cloud Games (FCG)** é uma aplicação desenvolvida como parte do **Tech Challenge da FIAP (Fase 1)**.  
O projeto tem como objetivo implementar uma **API REST** para gerenciar **usuários**, **autenticação** e **biblioteca de jogos adquiridos**.

A proposta é aplicar boas práticas de desenvolvimento com **.NET 8**, **Entity Framework Core**, **Identity**, **JWT**, e **Clean Architecture**, utilizando o **.NET Aspire** para orquestração e execução local.

---

## 🧰 Tecnologias Utilizadas

| Categoria | Tecnologia |
|------------|-------------|
| 🧠 Linguagem | C# (.NET 8) |
| 🧩 Framework | ASP.NET Core Minimal API |
| 🧭 Orquestração | .NET Aspire |
| 🗄️ Banco de Dados | PostgreSQL |
| 🧱 ORM | Entity Framework Core |
| 🔐 Autenticação | JWT + Identity |
| 🧾 Documentação | Swagger |
| 🧪 Testes | xUnit / MSTest |
| 🧩 Arquitetura | Clean Architecture + DDD |

---

## 🔐 Identity & Roles

A autenticação utiliza o **ASP.NET Identity** com controle de permissões baseado em *roles*.

| Role | Descrição |
|------|------------|
| 🧑‍💼 **Admin** | Pode gerenciar usuários e cadastrar jogos |
| 🎮 **User** | Pode acessar e visualizar sua biblioteca de jogos |

---

## 🧠 Modelagem (DDD)

A modelagem segue os princípios de **Domain-Driven Design (DDD)**, apoiada por **Event Storming** e **Domain Storytelling** para entender os fluxos principais do sistema.

| História | Objetivo | Ferramenta |
|-----------|-----------|------------|
| Criação de Usuário | Cadastro e login | Miro / Egon.io |
| Administração | Gestão de usuários e jogos | Miro |
| Biblioteca | Aquisição e listagem de jogos | Miro |

📎 **Documentação visual completa:**  
🔗 [Miro - FIAP Cloud Games Modelagem](https://miro.com/app/board/uXjVJLyabu4=/)

---

## 🧱 Estrutura do Projeto

```bash
/FCG.Host               # Entry point da aplicação Aspire
/FCG.Api                # Endpoints REST
/FCG.Application        # Casos de uso e serviços
/FCG.Domain             # Entidades, ValueObjects, Aggregates
/FCG.Infrastructure     # Persistência, Identity, Migrations
/FCG.Tests              # Testes unitários
````

## 🟢 Como executar localmente

### 🚦 Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker Desktop](https://docs.docker.com/desktop/setup/install/windows-install/)

---

### ▶️ Executando o projeto

#### 1️⃣ Clone o repositório
```bash
git clone https://github.com/fiap-tech-challenge-fgc/fiap-cloud-game.git
cd fiap-cloud-game
````
#### 2️⃣ Rode o projeto via Aspire (Host)
```bash
dotnet run --project FCG.Host
````
---
## 🧪 TESTES
Para executar os testes unitários:

1. Abra o terminal na raiz do projeto.
2. Execute o comando abaixo:

```bash
dotnet test
```

Esse comando irá buscar e executar todos os testes unitários presentes no projeto, especialmente na pasta `FCG.Tests`.

Caso queira rodar apenas os testes de um projeto específico, utilize:

```bash
dotnet test FCG.Tests/FCG.Tests.csproj
```

Os resultados dos testes serão exibidos diretamente no terminal.

---

# 👨🏽‍💻 EQUIPE
Integrante - GitHub
---
Jhonatan B - https://github.com/Jhonbrayaan
---
Miguel O - https://github.com/Miguel084
---
João C - https://github.com/jsoft-ti
---
Marcelo O - https://github.com/marcel0liveira
---
Matias N - https://github.com/MatiasNeto
---

<p align="center"> <img src="https://capsule-render.vercel.app/api?type=waving&color=6C63FF&height=120&section=footer" /> </p> <p align="center"> <b>FIAP Cloud Games (FCG)</b> • Desenvolvido  pela equipe <b>Grupo 4</b><br> <i>"Build fast. Learn faster. Deliver value."</i> </p>
