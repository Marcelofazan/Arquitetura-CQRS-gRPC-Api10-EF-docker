## 🌐 Arquitetura-CQRS-gRPC-Api10-EF-docker
Exemplo de projeto com Arquitetura CQRS com comunicação gRPC e MediatR, mensageria RabbitMQ em C# ASP.NET Core 10 com banco de dados Postgres e MongoDB.

#### 📋 O que você vai encontrar neste projeto
| Tecnologia | Descrição |
|-----------|-----------|
| **CQRS** | Padrão arquitetural que separa as operações de escrita (comandos) das operações de leitura (consultas) |
| **gRPC** | O gRPC é um framework para a comunicação de alta performance, utiliza o protocolo HTTP/2 na camada de rede e realiza a serialização de dados em Protocol Buffers. |
| **Mediatr** | Desacoplar classes, permitindo que diferentes componentes de um sistema se comuniquem através de um ponto central (o mediador) |
| **RabbitMQ** | Agente de mensagens (message broker) ele atua como um carteiro altamente inteligente e seguro  |

#### 💬 Requisitos do Projeto
- Necessário **Docker** instalado.
- Realizar Migrations EntityFramework .NET
  
## 📁 Producao
#### 🔄 Executar a aplicação

VSCode Terminal [1]
- Iniciar Docker 
```bash
docker-compose up --build  
```
VSCode Terminal [2]
- Iniciar Servidor 
```bash
cd Producao 
dotnet ef migrations add InitialCreate --project InfraEstrutura.Producao.DataModels --startup-project Sistema.Producao.API
dotnet ef database update --project InfraEstrutura.Producao.DataModels --startup-project Sistema.Producao.API
cd InfraEstrutura.Producao.Server 
dotnet run 
```
VSCode Terminal [3]
- Iniciar API 
```bash
cd Sistema.Producao.API
dotnet run --launch-profile https
```

- Para fechar o Container após execução
```bash 
docker compose down              
```

#### 🧪 Executar Endpoints
| Host | URL | Projeto | 
|-----------|-----------|-----------|
| **Server** | http://localhost:5184 | **Infraestrutura** |
| **API** | https://localhost:7274/swagger/index.html | **API** |
| **gRPC** | https://localhost:7026/ | **API** |
| **RabbitMQ** | http://localhost:15672 | **API** |

RabbitMQ por padrão possui para acesso Login: **guest** **Senha: **guest** 


Registrar Cliente
https://localhost:7274/api/Cliente/create
```bash 
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nome": "Marcelo",
  "tipoPessoa": "fisica",
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
  "tipoPessoa": "fisica",
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

#### 🧪 Executar Testes Unitários
```bash 
dotnet test Sistema.Producao.Testes/Sistema.Producao.Testes.csproj
```

#### ⚙️ Postgres (pgAdmin)
Conexão com o Banco de dados 
- Com o Servidor do **Postgres** parado em Serviços, crie uma conexão Docker Postgres para 127.0.0.1 e informe o Usuário e Senha 

- Caso precisar deletar a Database do Container 
```bash 
dotnet ef database drop --project InfraEstrutura.Producao.Server --startup-project InfraEstrutura.Producao.Server --force
```

## 📁 Reporter
#### 🔄 Executar a aplicação

VSCode Terminal [1]
```bash
docker-compose up --build  
```

- Caso houver falhas na criação do Conteiner na porta do RabbitMQ execute no PowerShell executar . 
```bash 
netstat -ano | findstr 5672
```
O comando vai mostrar um número no final da linha (o PID). Elimine o processo usando:
```bash 
taskkill /PID <NUMERO_DO_PID> /F
```

VSCode Terminal [2]
```bash 
dotnet build

dotnet add Sistema.Reporter.Server package Microsoft.EntityFrameworkCore.Design

dotnet ef migrations add InitialCreate --project InfraEstrutura.Reporter.DataModels --startup-project Sistema.Reporter.Server

dotnet ef database update --project InfraEstrutura.Reporter.DataModels --startup-project Sistema.Reporter.Server

cd Sistema.Reporter.Server

dotnet run 
```

VSCode Terminal [3]
```bash 
cd Sistema.Reporter.API

dotnet run --launch-profile https
```

- Para fechar o Container após execução
```bash 
docker compose down         
```

| Host | URL | Projeto | 
|-----------|-----------|-----------|
| **Server** | http://localhost:5184 | **Infraestrutura** |
| **API** | https://localhost:7080/swagger/index.html | **API** |
| **gRPC** | https://localhost:7237/ | **API** |
| **RabbitMQ** | http://localhost:15672 | **API** |

Exemplo chamada Relatório de consolidação, ele será gravado no banco de dados MongoDB
```bash 
https://localhost:7080/api/Consolidacao/consolidacao?data=2026-06-04
```

#### 🧪 Executar Testes Unitários

- Necessário ter dados de consolidação, para passar todos os testes. 
```bash
dotnet test Sistema.Reporter.Testes/Sistema.Reporter.Testes.csproj
```

#### ⚙️ MongoDB (Compass)
Conexão com o Banco de dados 
- No o Servidor do **Postgres** parado em Serviços, crie uma conexão em porta diferente **mongodb://127.0.0.1:27018/** .

## 📁 Frontend
#### 🔄 Executar a aplicação 

- Recuperar as dependencias do projeto node_modules .
```bash
npm install
```

#### ⚙️ Configuração - Certificados
- Caso for necessário renovar o certificado SSL 
```bash 
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

Executar o Build do Projeto
```bash
npm start
```

O projeto ira rodar em **localhost:3000**
