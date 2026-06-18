# URL Shortener

Sistema web para encurtar URLs e gerenciar redirecionamentos com geração automática de alias em Base62.

## Stack

- **Backend:** .NET 9, ASP.NET Core
- **Persistência:** SQLite com Dapper (SQL raw)
- **Frontend:** HTML5, JavaScript vanilla, Bootstrap 5
- **Containerização:** Docker + Docker Compose
- **Testes:** xUnit

## Arquitetura

Estrutura em camadas desacopladas:

- **Domain/** → Entidades, interfaces
- **Application/** → Casos de uso, DTOs, serviços
- **Infrastructure/** → Repositórios, Dapper
- **API/** → Controllers, endpoints

### Fluxo de Encurtamento

1. POST `/api/url/shorten` com `{ originalUrl, customAlias? }`
2. **ShortenUrlUseCase** processa:
   - Se `customAlias` → valida e persiste
   - Se não → incrementa sequência (começa 916132199) e converte para Base62
3. **UrlRepository** salva em SQLite
4. Response retorna `shortenedUrl` (ex: `/jyzab`)

### Redirecionamento

- Rota: `GET /{alias}`
- Regex constraint `^[a-zA-Z0-9]+$` previne captura de arquivos estáticos
- Busca alias, incrementa AccessCount, redireciona

## Como Rodar

### Local

```bash
dotnet restore
dotnet run --project src/UrlShortener.API/UrlShortener.API.csproj
```

Acessa: `http://localhost:5210`

### Docker

```bash
docker-compose up
```

## Decisões Técnicas

Optei por **Base62 em vez de UUID** para manter aliases curtos (5-8 caracteres vs 36). Além disso, usei **sequência numérica monotônica** em vez de aleatório puro: garante zero collisions, evita retry logic e oferece performance previsível. O custo é previsibilidade dos aliases, aceitável para este caso de uso.

Escolhi **Dapper em vez de EF Core puro** porque o projeto é simples e SQL direto é mais legível e performático. **Minimal API para o endpoint de redirect** reduz boilerplate e o regex constraint resolve roteamento limpo sem conflitar com arquivos estáticos. O **frontend servido pela API** permite deploy único e simplicidade, aceitando o trade-off de acoplamento.

## Testes

6 testes unitários para `Base62Service`:

```bash
dotnet test src/UrlShortener.API.Tests/UrlShortener.API.Tests.csproj
```

Cobrem casos limites (0, transição de dígitos) e propriedade fundamental (inversa).

## Schema

```sql
CREATE TABLE ShortenedUrls (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OriginalUrl TEXT NOT NULL,
    Alias TEXT NOT NULL UNIQUE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    AccessCount INTEGER NOT NULL DEFAULT 0
);

CREATE INDEX idx_alias ON ShortenedUrls(Alias);
```

## Endpoints

| Método | Rota               | Descrição    |
| ------ | ------------------ | ------------ |
| POST   | `/api/url/shorten` | Encurtar URL |
| GET    | `/{alias}`         | Redirecionar |
| GET    | `/swagger`         | Documentação |

## No IIS

```bash
dotnet publish -c Release -o C:\inetpub\wwwroot\url-shortener
```

Criar Application Pool (.NET CLR: "No Managed Code"), dar permissão ao arquivo `.db`.
