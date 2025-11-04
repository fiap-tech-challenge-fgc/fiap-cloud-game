<!-- Banner -->
<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=6C63FF&height=180&section=header&text=FIAP%20Cloud%20Games%20(FCG)&fontSize=38&fontColor=ffffff&animation=fadeIn&fontAlignY=35" />
</p>

<h1 align="center">🎮 FIAP Cloud Games (FCG)</h1>

<p align="center">
  <i>API de cadastro e autenticação de usuários com .NET 8 e PostgreSQL</i>
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
| 🐳 Containerização | Docker |
| 🔐 Autenticação | JWT + Identity |
| 🧾 Documentação | Swagger / OpenAPI |
| 🧪 Testes | xUnit / MSTest |
| 📊 Logging | Serilog |
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

/FCG.Host               # Entry point da aplicação Aspire
/FCG.Api                # Endpoints REST
/FCG.Application        # Casos de uso e serviços
/FCG.Domain             # Entidades, ValueObjects, Aggregates
/FCG.Infrastructure     # Persistência, Identity, Migrations
/FCG.Tests              # Testes unitários

---

## 🟢 COMO EXECUTAR LOCALMENTE
🚦 PRÉ-REQUISITOS
° .NET 8 SDK
° Docker Desktop

✅ EXECUTAR O PROJETO
# Clone o repositório
git clone https://github.com/fiap-tech-challenge-fgc/fiap-cloud-game.git
cd fiap-cloud-game

# Rode o projeto via Aspire (Host)
dotnet run --project FCG.Host

O Aspire vai subir:
A API (http://localhost:5000/swagger)
O PostgreSQL (container local)
O dashboard de observabilidade (http://localhost:16000)

---

## 🧪 TESTES
Para executar os testes unitários:


---

# 👨🏽‍💻 EQUIPE
Integrante                    GitHub
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
