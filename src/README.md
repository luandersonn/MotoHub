# MotoHub Notifier

## Descrição
MotoHub Notifier é um microserviço responsável por processar notificações assíncronas relacionadas a eventos importantes do sistema MotoHub. Implementado como uma função AWS Lambda que consome mensagens de uma fila SQS, o serviço processa as notificações e encaminha para destinatários externos via webhooks.

## Funcionalidades
- Processamento de eventos assíncronos via AWS Lambda e SQS
- Notificação de cadastro de novas motocicletas no sistema
- Envio de informações detalhadas para webhooks configurados
- Tratamento de erros com logging completo no CloudWatch
- Formato de mensagens padronizado e extensível

## Arquitetura
O microserviço utiliza os seguintes componentes da AWS:

1. **AWS Lambda**: Função serverless que é acionada automaticamente quando uma mensagem é recebida na fila SQS. Isso elimina a necessidade de manter servidores ativos constantemente, reduzindo custos.

2. **Amazon SQS (Simple Queue Service)**: Fila de mensagens que armazena eventos a serem processados, garantindo a entrega das mensagens mesmo em caso de falha temporária do serviço.

3. **CloudWatch**: Serviço para monitoramento e logs de execução, permitindo acompanhar o funcionamento do sistema e investigar problemas.

## Fluxo de Processamento
1. A API MotoHub (aplicação principal) publica um evento na fila SQS quando uma nova motocicleta é cadastrada
2. A função Lambda é acionada automaticamente quando uma mensagem chega à fila
3. O Lambda processa a mensagem, extraindo os dados do evento
4. Os dados são formatados e enviados para o webhook configurado
5. Logs são registrados no CloudWatch para auditoria e monitoramento

## Eventos Suportados
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
## Configuração e Personalização

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

## Integração com o Sistema Principal
O sistema principal MotoHub se comunica com este microserviço através da classe `MotorcycleEventPublisher` na camada de infraestrutura, que publica eventos na fila SQS sempre que ações relevantes ocorrem no sistema.

## Dependências
- Amazon.Lambda.Core v2.5.0
- Amazon.Lambda.Serialization.SystemTextJson v2.4.4
- Amazon.Lambda.SQSEvents v2.2.0
