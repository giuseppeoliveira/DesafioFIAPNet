# SchoolAPI

Projeto de exemplo para gerenciamento escolar (backend em .NET 9 e frontend em React). Este repositório contém uma API REST (`SchoolAPI`) e uma aplicação frontend (`frontend`) que consome essa API.

Este README descreve como configurar, executar e testar o projeto.

## Visão geral

- Backend: ASP.NET Core Web API (pasta `SchoolAPI`) com endpoints em `/api/v1/*`.
- Frontend: React + Vite (pasta `frontend`). A aplicação cliente faz requisições para a base `/api/v1`.

## Pré-requisitos

- .NET SDK 9.x
- Node.js 18+ e npm
- Um servidor PostgreSQL (ou container) e a string de conexão configurada
- (Opcional) Docker/Docker Compose

## Variáveis de ambiente importantes

- `SchoolAPI_DB_ConnectionString` — string de conexão com o banco PostgreSQL usada pelo backend.
- As configurações de JWT estão em `appsettings.json` (`JwtSettings:Key`). Para rodar localmente sem alterações, verifique esse arquivo.

## Banco de dados

- Este projeto foi desenvolvido com abordagem "database-first" usando Entity Framework Core Scaffold. Ou seja, o modelo de dados e o DbContext foram gerados a partir do banco existente (EF Core Scaffold). Os arquivos relacionados ao contexto e entidades estão em `SchoolAPI/` (procure por `SchoolAPIContext` e entidades em `Domain/Entities`).

## Executando com Docker / Docker Compose (RECOMENDADO)

A forma preferencial de executar este projeto é via Docker Compose — ele sobe os serviços necessários (API, frontend e banco) já configurados.

> Observação: quando executado via Docker Compose, o frontend ficará disponível em

```
http://localhost:5000/index.html
```

Execute (na raiz do repositório):

```powershell
docker compose up --build
```

O comando acima sobe os containers e expõe a API e o frontend conforme definido em `docker-compose.yml`.

## Executando localmente sem Docker

### Backend

1. Configure a variável de ambiente `SchoolAPI_DB_ConnectionString`. Exemplo (PowerShell):

```powershell
$env:SchoolAPI_DB_ConnectionString = "Host=localhost;Database=schoolapi;Username=postgres;Password=postgres"
```

2. Vá para a pasta do projeto e execute:

```powershell
cd SchoolAPI
dotnet restore
dotnet run
```

O backend será iniciado e os controllers ficam disponíveis em `/api/v1` (por exemplo: `https://localhost:5001/api/v1/alunos`).

### Frontend

1. Abra um terminal e entre na pasta `frontend`:

```powershell
cd frontend
npm install
npm run dev
```

2. O Vite indicará a URL onde o frontend está disponível (ex.: `http://localhost:5173`).

Se preferir abrir o frontend sem rodar o servidor do Vite (por exemplo para testes rápidos), você pode abrir o arquivo `index.html` presente na pasta `frontend` ou `frontend/dist` (após build) diretamente no navegador. Porém, quando for usar o Docker Compose, a URL correta para acessar o frontend é `http://localhost:5000/index.html`.

## Build do frontend para produção

```powershell
cd frontend
npm run build

# arquivos prontos em frontend/dist
```

## Integração frontend ↔ backend

- O frontend espera que a API esteja disponível no mesmo host sob o caminho `/api/v1` (veja `frontend/src/services/api.js` que define `baseURL: '/api/v1'`). Se estiver servindo o frontend e backend em origens diferentes, ajuste `baseURL` em `frontend/src/services/api.js` ou configure um proxy.

## Testes

- Backend: na raiz do projeto execute:

```powershell
cd SchoolAPI
dotnet test
```

## Endpoints principais

- GET /api/v1/alunos — listagem paginada de alunos
- POST /api/v1/alunos — cria aluno
- PUT /api/v1/alunos/{id} — atualiza aluno
- DELETE /api/v1/alunos/{id} — remove aluno
- POST /api/v1/alunos/{id}/matriculas?turmaId={turmaId} — matricula um aluno em uma turma
- GET /api/v1/turmas — listagem paginada de turmas
- GET /api/v1/turmas/{id} — detalhes da turma (inclui alunos matriculados)

Consulte os controllers em `SchoolAPI/Controllers` para ver o contrato completo.

Obrigado!
