# OficinaAPI

API de gestão de oficina mecânica desenvolvida em .NET, construída como projeto de portfólio.

> 🚧 Em desenvolvimento. O módulo de veículos está pronto, e os próximos módulos estão listados abaixo.

## Tecnologias

- .NET 10 / ASP.NET Core (Minimal API)
- Entity Framework Core
- SQL Server
- Swagger

## Arquitetura

O projeto é dividido em camadas:

| Projeto | Responsabilidade |
|---|---|
| **Oficina.Domain** | Entidades e enums. Não depende de nenhuma outra camada. |
| **Oficina.Application** | DTOs, interfaces e regras de negócio (services) |
| **Oficina.Infrastructure** | Acesso a dados com EF Core e implementação dos repositórios |
| **Oficina.API** | Endpoints e configuração da aplicação |

## Regras de negócio implementadas

- A placa é normalizada antes de salvar (`abc-1d23`, `ABC 1D23` e `ABC1D23` são a mesma placa)
- Não é permitido cadastrar duas vezes a mesma placa
- A quilometragem não pode diminuir na atualização
- Todo veículo novo é cadastrado com o status `Disponivel`
- O usuário não define campos controlados pelo sistema, como status inicial e data de cadastro
- Validação de ano, quilometragem, tipo e status

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/veiculos` | Lista todos os veículos |
| GET | `/api/veiculos/{id}` | Busca um veículo por Id |
| POST | `/api/veiculos` | Cadastra um veículo |
| PUT | `/api/veiculos/{id}` | Atualiza um veículo |
| DELETE | `/api/veiculos/{id}` | Remove um veículo |

## Como rodar

1. Clone o repositório
2. Ajuste a connection string em `Oficina.API/appsettings.json`, se necessário
3. Crie o banco:

   ```bash
   dotnet ef database update --project Oficina.Infrastructure --startup-project Oficina.API
   ```

4. Rode a API:

   ```bash
   dotnet run --project Oficina.API
   ```

5. Acesse o Swagger em `https://localhost:{porta}/swagger`

## Próximos passos

- [ ] Autenticação com JWT e perfis de acesso (Admin e Funcionário)
- [ ] Cadastro de clientes (1 cliente → N veículos)
- [ ] Ordens de serviço, peças e serviços
- [ ] Tratamento global de erros
- [ ] Testes unitários
- [ ] Docker
- [ ] Front-end

João Guilherme  
[LinkedIn](https://www.linkedin.com/in/joaoguilhermegj) · [GitHub](https://github.com/joaoguilhermgj)
