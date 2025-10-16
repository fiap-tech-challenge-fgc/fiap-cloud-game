# fiap-cloud-game

## Tecnologia: Aspire
## DB: Postgres
## Arquitetura: Clean Architecture
## Requisitos:
	1: [Docker](https://docs.docker.com/desktop/setup/install/windows-install/)

## Estrutura base proposta
/FCG.Host  
/FCG.Api  
/FCG.Application  
├── Interfaces  
└── Services  
/FCG.Domain  
├── Entities  
├── ValueObjects  
├── Enums  
└── Aggregates  
/FCG.Infrastructure  
├── Data  
├── Identity  
└── Migrations  
/FCG.Tests

## Identity
### Roles
	- Admin
	- User