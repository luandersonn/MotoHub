# MotoHub API

## Descrição
A API MotoHub é o principal ponto de acesso ao sistema de gerenciamento de aluguel de motocicletas. Implementada com ASP.NET Core 8, ela fornece endpoints REST para interação com todas as funcionalidades do sistema.

## Endpoints Principais

### Entregadores (/entregadores)
- **POST /entregadores**: Cadastra um novo entregador no sistema
  - Valida informações pessoais, CNPJ e CNH
  - Verifica idade mínima (18 anos)
  - Armazena imagem da CNH
- **POST /entregadores/{id}/cnh**: Atualiza a foto da CNH de um entregador existente

### Motos (/motos)
- **POST /motos**: Cadastra uma nova motocicleta no sistema para aluguel
  - Validação de placa, ano e modelo
  - Publica evento para notificação
- **GET /motos**: Consulta motocicletas disponíveis (filtro por placa)
- **PUT /motos/{id}/placa**: Atualiza a placa de uma motocicleta existente

### Locação (/locacao)
- **POST /locacao**: Registra o aluguel de uma motocicleta
  - Verifica disponibilidade da moto
  - Valida se entregador possui CNH categoria A ou AB
  - Calcula datas e valores conforme o plano
- **GET /locacao/{id}**: Consulta detalhes de uma locação específica
- **PUT /locacao/{id}/devolucao**: Registra a devolução da motocicleta
  - Calcula valor final com base na data real de devolução
  - Aplica penalidades por devolução antecipada ou taxas por atraso

## Tecnologias e Padrões
- ASP.NET Core 8 (API REST)
- Controladores com injeção de dependência
- AutoMapper para mapeamento de DTOs
- Swagger para documentação interativa
- Tratamento global de exceções via middleware
- Validação de modelos
- Padrão Result para retornos padronizados
- Extension methods para facilitar operações comuns

## Executando a API
1. Configure o `appsettings.json` com as informações necessárias
2. Execute via linha de comando:
   ```
   dotnet run --project API/MotoHub.API/MotoHub.API.csproj
   ```
3. Acesse a documentação Swagger: https://localhost:5001/swagger