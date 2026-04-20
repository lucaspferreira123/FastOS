# AGENTS.md

## Project: FastOS

Sistema de gerenciamento de ordens de serviço, filas e peças.

---

## Stack

- Backend: ASP.NET (C#)
- ORM: Entity Framework
- Database: SQL Server
- Auth: JWT (Bearer Token)
- Frontend: jQuery + DataTables

---

## Architecture

Projeto segue:

- Domain Driven Design (DDD)
- Clean Architecture

Camadas:

- Controller → entrada da API
- Business → regras de negócio
- Repository → acesso a dados

---

## Strict Rules

- Controllers NÃO podem conter regra de negócio
- Controllers NÃO acessam repository diretamente
- Controllers devem chamar apenas Business

- Business contém TODA regra de negócio
- Repository apenas acesso a dados

---

## Entity Framework Rules

- Sempre usar AsNoTracking() para consultas
- Evitar Include desnecessário
- Evitar N+1 queries
- Sempre usar async/await

---

## API Rules

- Sempre usar DTOs
- Nunca retornar entidades do EF diretamente
- Padronizar resposta:

{
  success: boolean,
  data: object,
  message: string
}

---

## Security

- Todas rotas protegidas devem validar JWT
- Nunca confiar no frontend
- Validar todos os inputs no backend

---

## Frontend

- Usar jQuery apenas para manipulação leve
- DataTables para listagem
- Nunca colocar regra de negócio no front

---

## Database

- Evitar queries pesadas
- Sempre filtrar corretamente
- Cuidado com duplicações em JOIN

---

## Code Quality

- Usar interfaces (IBusiness, IRepository)
- Seguir injeção de dependência
- Métodos devem ser pequenos e claros
- Evitar código duplicado

---

## Critical Flows

- Login
- Criação de O.S
- Controle de fila
- Manipulação de peças

Esses fluxos devem ser tratados com prioridade e segurança

The project MUST follow this folder structure:

- src/
  - FastOS.API (Controllers, DTOs, Middleware)
  - FastOS.Application (Business, Interfaces)
  - FastOS.Domain (Entities, Interfaces)
  - FastOS.Infrastructure (EF, Repositories)
  - FastOS.CrossCutting (Security, IoC)
  - FastOS.Frontend (jQuery, DataTables)

Rules:

- Do NOT create files outside this structure
- Do NOT mix responsibilities between layers
- Controllers must stay in API layer
- Business logic must stay in Application layer
- Data access must stay in Infrastructure layer