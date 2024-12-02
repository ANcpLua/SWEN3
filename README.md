<h2 align="center">Alexander Nachtann, Jasmin Mondre and Stephanie Rauscher
<p>2024/2025 SWKOM</h2>
</p>
<h3 align="center">A document management system with features including archiving, OCR processing, tagging, and full-text search.
<p></p>

![image](https://github.com/user-attachments/assets/854a3561-eada-4c83-b0d9-d20d22ea36da)

## Sprint Overview and Technologies
  
### Sprint 1: REST Service API
- ASP.NET Core 8
- Swagger/OpenAPI (Swashbuckle.AspNetCore)
- API Controller(minimal api)

### Sprint 2: Web-UI Integration
- ~~Option A: React maybe even with Fluent UI https://react.fluentui.dev/?path=/docs/concepts-introduction--docs~~
- ~~Option B: Blazor WebAssembly (Microsoft.AspNetCore.Components.WebAssembly)~~
- Option C: good old html and js

### Sprint 3: DAL (Persistence)
- Entity Framework Core (Microsoft.EntityFrameworkCore)
- PostgreSQL (Npgsql.EntityFrameworkCore.PostgreSQL) to persist 
- BL(domainmodel) entities
- Unit Tests with a happy and unhappy path
- Mapping using Automapper
- Minimal API
- Docker working

### Sprint 4: Message Broker
- RabbitMQ using EasyNetQ version 7.8(7) https://github.com/EasyNetQ/EasyNetQ/wiki/Quick-Start

### Sprint 5: Worker Services
- Hangfire (Hangfire.AspNetCore)
- TesseractOCR (Tesseract)

### Sprint 6: ElasticSearch
- NEST (Elasticsearch.Net, NEST)

### Sprint 7: Integration-Test
The best way to avoid failure is to fail constantly

## Cross-Cutting Concerns 
- Logging: Serilog (Serilog.AspNetCore)
- Exception Handling: Global exception middleware
- Validation: FluentValidation 
- Mapping: AutoMapper 
- Dependency Injection: Microsoft

## Key Use Cases
1. Upload document
2. Search for a document
