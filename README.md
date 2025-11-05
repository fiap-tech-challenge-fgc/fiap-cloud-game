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
