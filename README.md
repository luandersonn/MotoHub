# MotoHub - Sistema de Gerenciamento de Aluguel de Motos

O MotoHub é uma solução completa para gerenciamento de aluguel de Motos, construído com uma arquitetura moderna, escalável e baseada em microsserviços, utilizando .NET 8 e tecnologias cloud-native.

## Configuração

Para executar a API MotoHub, você precisa configurar corretamente os serviços externos no arquivo `appsettings.json`. Abaixo estão as configurações necessárias:

### Banco de Dados MongoDB

A API utiliza MongoDB como banco de dados principal. Configure a seção `MongoDbSettings` no arquivo `appsettings.json`:

```json
"MongoDbSettings": {
  "ConnectionString": "mongodb://username:password@hostname:port",
  "Database": "MotoHubDB"
}
```

- `ConnectionString`: String de conexão completa do MongoDB, incluindo credenciais
- `Database`: Nome do banco de dados a ser utilizado

### AWS SQS para Mensageria

A API utiliza Amazon SQS para publicação de eventos assíncronos. Configure a seção `AwsSQSSettings` no arquivo `appsettings.json`:

```json
"AwsSQSSettings": {
  "QueueUrl": "https://sqs.us-east-1.amazonaws.com/123456789012/motohub-events",
  "Key": "SEU_AWS_ACCESS_KEY_ID",
  "Secret": "SEU_AWS_SECRET_ACCESS_KEY"
}
```

- `QueueUrl`: URL da fila SQS para onde os eventos serão enviados
- `Key`: AWS Access Key ID
- `Secret`: AWS Secret Access Key

### Armazenamento de Imagens

A API suporta dois modos de armazenamento de imagens, configuráveis através da chave `imageStorageService`. Os valores possíveis são:

1. **MongoDB** (`"imageStorageService": "mongodb"`): Utiliza o próprio MongoDB para armazenar as imagens
2. **Amazon S3** (`"imageStorageService": "s3"`): Utiliza o Amazon S3 para armazenar as imagens

#### Configuração do Amazon S3

Se você escolher o Amazon S3 como serviço de armazenamento de imagens, configure a seção `AwsS3Settings`:

```json
"AwsS3Settings": {
  "BucketName": "motohub-images",
  "Key": "SEU_AWS_ACCESS_KEY_ID",
  "Secret": "SEU_AWS_SECRET_ACCESS_KEY"
}
```

- `BucketName`: Nome do bucket S3 onde as imagens serão armazenadas
- `Key`: AWS Access Key ID
- `Secret`: AWS Secret Access Key

## Integração com Microserviços

A API MotoHub está integrada com o microserviço MotoHub.Notifier através do AWS SQS. Quando uma nova motocicleta é registrada, um evento é publicado na fila SQS para processamento assíncrono pelo microserviço.

## Visão Geral da Arquitetura

A solução MotoHub é composta por dois componentes principais:

1. **MotoHub API**: Uma API REST principal que gerencia o core do sistema, responsável por todas as operações de negócio
2. **MotoHub Notifier**: Um microsserviço serverless responsável pelo processamento assíncrono de notificações

### Estrutura do Projeto

```
MotoHub/
├── API/
│   ├── MotoHub.API/              # API REST principal 
│   ├── MotoHub.Application/      # Casos de uso e regras de aplicação
│   ├── MotoHub.Domain/           # Entidades e regras de domínio
│   └── MotoHub.Infrastructure/   # Implementações de persistência e serviços externos
├── Microservices/
│   └── MotoHub.Notifier/         # Microsserviço de notificações (AWS Lambda)
├── Tests/
    └── MotoHub.Tests/            # Testes de unidade e integração
```

## MotoHub API

### Descrição
A API MotoHub é o principal ponto de acesso ao sistema de gerenciamento de aluguel de Motos. Implementada com ASP.NET Core 8, ela fornece endpoints REST para interação com todas as funcionalidades do sistema.

### Endpoints Principais

#### Entregadores (/entregadores)
- **POST /entregadores**: Cadastra um novo entregador no sistema
  - Valida informações pessoais, CNPJ e CNH
  - Verifica idade mínima (18 anos)
  - Armazena imagem da CNH
- **POST /entregadores/{id}/cnh**: Atualiza a foto da CNH de um entregador existente

#### Motos (/motos)
- **POST /motos**: Cadastra uma nova motocicleta no sistema para aluguel
  - Validação de placa, ano e modelo
  - Publica evento para notificação
- **GET /motos**: Consulta Motos disponíveis (filtro por placa)
- **PUT /motos/{id}/placa**: Atualiza a placa de uma motocicleta existente

#### Locação (/locacao)
- **POST /locacao**: Registra o aluguel de uma motocicleta
  - Verifica disponibilidade da moto
  - Valida se entregador possui CNH categoria A ou AB
  - Calcula datas e valores conforme o plano
- **GET /locacao/{id}**: Consulta detalhes de uma locação específica
- **PUT /locacao/{id}/devolucao**: Registra a devolução da motocicleta
  - Calcula valor final com base na data real de devolução
  - Aplica penalidades por devolução antecipada ou taxas por atraso

### Tecnologias e Padrões
- ASP.NET Core 8 (API REST)
- Controladores com injeção de dependência
- AutoMapper para mapeamento de DTOs
- Swagger para documentação interativa
- Tratamento global de exceções via middleware
- Validação de modelos
- Padrão Result para retornos padronizados
- Extension methods para facilitar operações comuns

### Executando a API
1. Configure o `appsettings.json` com as informações necessárias
2. Execute via linha de comando:
   ```
   dotnet run --project API/MotoHub.API/MotoHub.API.csproj
   ```
3. Acesse a documentação Swagger: https://localhost:5001/swagger

## MotoHub Notifier

### Descrição
MotoHub Notifier é um microsserviço responsável por processar notificações assíncronas relacionadas a eventos importantes do sistema MotoHub. Implementado como uma função AWS Lambda que consome mensagens de uma fila SQS, o serviço processa as notificações e encaminha para destinatários externos via webhooks.

### Funcionalidades
- Processamento de eventos assíncronos via AWS Lambda e SQS
- Notificação de cadastro de novas Motos no sistema
- Envio de informações detalhadas para webhooks configurados
- Tratamento de erros com logging completo no CloudWatch
- Formato de mensagens padronizado e extensível

### Arquitetura
O microsserviço utiliza os seguintes componentes da AWS:

1. **AWS Lambda**: Função serverless que é acionada automaticamente quando uma mensagem é recebida na fila SQS. Isso elimina a necessidade de manter servidores ativos constantemente, reduzindo custos.

2. **Amazon SQS (Simple Queue Service)**: Fila de mensagens que armazena eventos a serem processados, garantindo a entrega das mensagens mesmo em caso de falha temporária do serviço.

3. **CloudWatch**: Serviço para monitoramento e logs de execução, permitindo acompanhar o funcionamento do sistema e investigar problemas.

### Fluxo de Processamento
1. A API MotoHub (aplicação principal) publica um evento na fila SQS quando uma nova motocicleta é cadastrada
2. A função Lambda é acionada automaticamente quando uma mensagem chega à fila
3. O Lambda processa a mensagem, extraindo os dados do evento
4. Os dados são formatados e enviados para o webhook configurado
5. Logs são registrados no CloudWatch para auditoria e monitoramento

### Eventos Suportados
Atualmente, o serviço processa os seguintes tipos de eventos:

- **MotorcycleRegisteredEvent**: Enviado quando uma nova motocicleta é cadastrada no sistema

O formato do evento é:
```json
{
  "Identifier": "m12345",
  "Plate": "ABC1234",
  "Year": 2022,
  "Model": "Honda CG 160"
}
```

### Payload do Webhook
O payload enviado ao webhook inclui:
```json
{
  "Timestamp": "2023-08-15T13:45:30Z",
  "LambdaFunctionName": "MotoHubNotifier",
  "RequestId": "c2307dde-2a1f-11e6-a530-3ca82a64ff89",
  "LogGroup": "/aws/lambda/MotoHubNotifier",
  "LogStream": "2023/08/15/[$LATEST]c763be94956c41e49e8c6f461e8a1b1c",
  "Message": {
    "Identifier": "m12345",
    "Plate": "ABC1234",
    "Year": 2022,
    "Model": "Honda CG 160"
  },
  "Error": null
}
```

### Integração com o Sistema Principal
O sistema principal MotoHub se comunica com este microsserviço através da classe `MotorcycleEventPublisher` na camada de infraestrutura, que publica eventos na fila SQS sempre que ações relevantes ocorrem no sistema.

## Dependências e Tecnologias

### MotoHub API
- ASP.NET Core 8.0
- EntityFrameworkCore
- AutoMapper
- Swagger/OpenAPI
- MongoDB.EntityFrameworkCore

### MotoHub Notifier
- Amazon.Lambda.Core
- Amazon.Lambda.Serialization.SystemTextJson
- Amazon.Lambda.SQSEvents

## Como Executar a Solução Completa

Para executar a solução completa, você precisa:

1. Configurar as credenciais da AWS para o MotoHub.Notifier
2. Configurar o banco de dados para a API
3. Iniciar a API MotoHub
4. Configurar o webhook no serviço MotoHub.Notifier

Para obter instruções mais detalhadas, consulte a documentação específica de cada componente.