### 🚀 Microservicos-Api10-CQRS

Exemplo de criação de Arquitetura distribuida CQRS utilizando banco de dados MongoDB e Postgres.


### O que você vai encontrar neste projeto
| Tecnologia | Descrição |
|-----------|-----------|
| **RabbitMQ** | Agente de mensagens (message broker) ele atua como um carteiro altamente inteligente e seguro  |
| **CQRS** | É um padrão arquitetural que separa as operações de escrita (comandos) das operações de leitura (consultas) |
| **Mediatr** | Desacoplar classes, permitindo que diferentes componentes de um sistema se comuniquem através de um ponto central (o mediador) |
---


#### Execução da aplicação

VSCode Terminal (1)
```bash
docker-compose up --build  
```
VSCode Terminal (2)
```bash
dotnet build

dotnet add Sistema.Producao.API package Microsoft.EntityFrameworkCore.Design

dotnet ef migrations add InitialCreate --project InfraEstrutura.Producao.DataModels --startup-project Sistema.Producao.API

dotnet ef database update --project InfraEstrutura.Producao.DataModels --startup-project Sistema.Producao.API

cd InfraEstrutura.Producao.Server 

dotnet run 
```
VSCode Terminal (3)
```bash
cd Sistema.Producao.API

dotnet run --launch-profile https
```

- Caso for necessário renovar o certificado SSL 

```bash 
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

```bash 
dotnet ef database drop --project InfraEstrutura.Producao.Server --startup-project InfraEstrutura.Producao.Server --force
```

- Para fechar o Container após execução
```bash 
docker compose down              
```

### O que você vai encontrar neste projeto
| Host | URL |
|-----------|-----------|
| **API** | https://localhost:7274/swagger/index.html |
| **gRPC** | https://localhost:7026/ |
| **RabbitMQ** | http://localhost:15672 |
---
RabbitMQ por padrão possui para acesso Login: **guest** **Senha: **guest** 

#### 🧪 Executar Endpoints

Registrar Cliente
https://localhost:7274/api/Cliente/create

```bash 
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nome": "Marcelo",
  "tipoPessoa": "FISICA",
  "cpfCnpj": "999.999.999",
  "endereco": "R St Sebastian",
  "valor": 1500.50,
  "dataCadastro": "2026-05-26"
}
```

Atualizar Cliente


```bash 

https://localhost:7274/api/Cliente/3fa85f64-5717-4562-b3fc-2c963f66afa6

{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nome": "Marcelo",
  "tipoPessoa": "FISICA",
  "cpfCnpj": "999.999.999",
  "endereco": "R St Sebastian",
  "valor": 1300.50,
  "dataCadastro": "2026-05-26"
}
```


Apagar Cliente

```bash 
https://localhost:7274/api/Cliente/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

