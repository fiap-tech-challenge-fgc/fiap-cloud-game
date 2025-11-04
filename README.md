<!-- Banner -->
<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&color=6C63FF&height=180&section=header&text=FIAP%20Cloud%20Games%20(FCG)&fontSize=38&fontColor=ffffff&animation=fadeIn&fontAlignY=35" />
</p>

<p align="center">
  <i>Plataforma de venda e gestão de jogos digitais — MVP do Tech Challenge FIAP (Fase 1)</i>
</p>

<p align="center">
  <a href="https://dotnet.microsoft.com/"><img src="https://img.shields.io/badge/.NET-8.0-blueviolet?logo=dotnet" /></a>
  <img src="https://img.shields.io/badge/Architecture-Clean%20Architecture%20%2B%20DDD-0078D7" />
  <img src="https://img.shields.io/badge/Platform-.NET%20Aspire-512BD4?logo=dotnet" />
  <img src="https://img.shields.io/badge/Database-PostgreSQL-blue?logo=postgresql" />
  <img src="https://img.shields.io/badge/Container-Docker-0db7ed?logo=docker" />
  <img src="https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow" />
</p>

---

## 🧭 Sumário
- [Visão Geral](#-visão-geral)
- [Objetivo da Fase 1](#-objetivo-da-fase-1)
- [O que é o .NET Aspire](#-o-que-é-o-net-aspire)
- [Arquitetura do Projeto](#-arquitetura-do-projeto)
- [Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Identity & Roles](#-identity--roles)
- [Modelagem (DDD)](#-modelagem-ddd)
- [Como Executar Localmente](#️-como-executar-localmente)
- [Testes](#-testes)
- [Entregáveis FIAP - Fase 1](#-entregáveis-fiap---fase-1)
- [Equipe](#-equipe)
- [Próximos Passos](#-próximos-passos)
- [Licença](#-licença)
- [Créditos](#-créditos)

---

## 🚀 Visão Geral

A **FIAP Cloud Games (FCG)** é uma plataforma de **venda de jogos digitais** e **gestão de servidores de partidas online**.

Nesta **Fase 1**, o objetivo é construir um **serviço de cadastro e autenticação de usuários**, junto à **biblioteca de jogos adquiridos**, servindo como base sólida para as próximas fases do projeto (como matchmaking, promoções e gerenciamento de servidores).

> 💡 Este MVP é o ponto de partida para o ecossistema completo da FCG, que futuramente integrará os alunos FIAP, Alura e PM3.

---

## 🎯 Objetivo da Fase 1

Conforme o desafio oficial da FIAP:

> Criar uma **API REST em .NET 8** para gerenciar usuários e seus jogos adquiridos, garantindo:
> - Persistência de dados (PostgreSQL via EF Core)
> - Autenticação via JWT
> - Arquitetura limpa e escalável (Clean Architecture + DDD)
> - Qualidade de software e testes automatizados
> - Documentação via Swagger

### Requisitos da Fase
- ✅ Cadastro de usuários (nome, e-mail e senha forte)
- ✅ Autenticação via JWT Token
- ✅ Perfis de acesso: `Admin` e `User`
- ✅ Persistência com PostgreSQL
- ✅ Documentação Swagger
- ✅ Testes unitários
- ✅ README.md completo com instruções e objetivos

---

## 🧩 O que é o .NET Aspire

O **.NET Aspire** é uma **plataforma de orquestração e observabilidade** nativa do .NET 8.  
Ele facilita o desenvolvimento de aplicações **modulares e distribuídas**, permitindo rodar múltiplos projetos, bancos e serviços **com apenas um comando** — sem precisar escrever `docker-compose`.

Em resumo:
- 🔄 Orquestra automaticamente todos os projetos da solução.  
- 🧩 Conecta APIs, bancos e filas sem configuração manual.  
- 🪄 Cria um **Dashboard Web** com logs, métricas e status dos serviços.  
- 🐳 Usa **Docker** por baixo dos panos, mas com integração direta no Visual Studio ou CLI.  

No contexto da **FIAP Cloud Games**, o Aspire é responsável por:
- Subir o **PostgreSQL** localmente;  
- Rodar simultaneamente os projetos `FCG.Api`, `FCG.Application`, `FCG.Infrastructure`, etc.;  
- Fornecer um painel de observabilidade acessível via browser (`http://localhost:16000`).

> ⚙️ Em outras palavras: o Aspire é o “mini Kubernetes” do .NET — perfeito pra desenvolver e testar apps complexos com infraestrutura real.

---

## 🧱 Arquitetura do Projeto

A arquitetura segue **Clean Architecture** + **Domain-Driven Design (DDD)**, garantindo baixo acoplamento, coesão e separação clara entre camadas.

```bash
/FCG.Host               # Entry point da aplicação Aspire
/FCG.Api                # Camada de apresentação (endpoints REST)
/FCG.Application        # Casos de uso e lógica de aplicação
├── Interfaces
└── Services
/FCG.Domain             # Entidades e regras de negócio (DDD)
/FCG.Infrastructure     # Persistência, migrations e Identity
├── Data
├── Identity
└── Migrations
/FCG.Tests              # Testes unitários e de integração
