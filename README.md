# Igreja Conecta

MVP para encontrar horários de missas e atividades de paróquias.

## Estrutura

- `IgrejaConecta.Domain`: entidades e contratos do domínio.
- `IgrejaConecta.Application`: casos de uso e DTOs.
- `IgrejaConecta.Infrastructure`: persistência (inicialmente em memória).
- `IgrejaConecta.Api`: API ASP.NET Core.
- `frontend`: aplicação Angular.

## Executar a API

```powershell
dotnet run --project .\IgrejaConecta.Api
```

Endpoints: `GET http://localhost:5114/api/parishes?city=Guaxupé` e `GET http://localhost:5114/api/parishes/{id}`.

## Executar o frontend

O frontend usa Angular 16.2, compatível com Node.js 16.14+.

```powershell
cd .\frontend
npm install
npm start
```

O servidor Angular encaminha `/api` para `http://localhost:5114` durante o desenvolvimento.

## Banco de dados e deploy gratuito

Localmente, a API cria um arquivo SQLite (`igreja-conecta.db`) e as tabelas `Parishes`, `MassSchedules`, `Activities` e `Chapels` na primeira execução. Para publicar, envie este projeto a um repositório Git e crie um **Blueprint** no Render a partir de `render.yaml`. Ele sobe um único serviço Docker (API + Angular) e um PostgreSQL gratuito. O banco gratuito do Render expira após 30 dias, portanto é indicado somente para a demonstração.

## Próximos passos

Trocar o repositório em memória por EF Core/PostgreSQL, criar autenticação para administradores paroquiais e adicionar operações de cadastro/edição.
