# MotoHub

## Descrição
MotoHub é um sistema de gerenciamento de aluguel de motocicletas para entregadores de aplicativos. O sistema permite o cadastro de motocicletas, entregadores, gerenciamento de locações e cálculo de valores de aluguel com base em diferentes planos e condições.

## Estrutura do Projeto
O projeto segue uma arquitetura limpa (Clean Architecture) e é organizado em vários projetos .NET 8:

- **MotoHub.API**: Interface REST que expõe os endpoints para interação com o sistema
- **MotoHub.Application**: Contém a lógica de negócios e casos de uso da aplicação
- **MotoHub.Domain**: Define as entidades de domínio, objetos de valor e regras de negócio
- **MotoHub.Infrastructure**: Implementa o acesso a dados e serviços externos
- **MotoHub.Notifier**: Microserviço responsável pelas notificações (AWS Lambda + SQS)
- **MotoHub.Tests**: Testes unitários e de integração

## Principais Funcionalidades

### Entregadores
- Cadastro de novos entregadores no sistema
- Atualização de dados pessoais e CNH
- Validação de requisitos mínimos (idade, tipo de CNH)

### Motocicletas
- Cadastro de novas motocicletas disponíveis para aluguel
- Consulta de motocicletas por placa
- Atualização de dados das motocicletas

### Locação
- Aluguel de motocicletas para entregadores
- Cálculo de valores de aluguel baseado em planos
- Registro de devolução com cálculo automático de valores
- Gerenciamento de penalidades por devolução antecipada
- Cálculo de taxas adicionais por atraso na devolução

### Notificações
- Sistema de notificações baseado em eventos
- Processamento assíncrono de mensagens
- Integração com webhooks para notificação de eventos importantes

## Tecnologias
- .NET 8
- Clean Architecture
- REST API
- AWS Lambda
- AWS SQS
- Entity Framework Core
- AutoMapper
- Testes unitários com NUnit

## Requisitos para Execução
- SDK .NET 8
- Visual Studio 2022 ou VS Code
- Acesso à AWS (para o microserviço Notifier)

## Como Executar
1. Clone este repositório
2. Abra a solução no Visual Studio 2022
3. Restaure os pacotes NuGet
4. Configure as strings de conexão no arquivo appsettings.json
5. Execute o projeto MotoHub.API