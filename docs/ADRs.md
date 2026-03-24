# ADRs da Lambda

## ADR-001 — Injeção de dependência no construtor da Function

- **Status:** Aceito
- **Decisão:** inicializar `ServiceProvider` uma vez no construtor da Lambda.
- **Justificativa:** otimiza warm starts e mantém o handler enxuto.

## ADR-002 — Persistência com Npgsql direto

- **Status:** Aceito
- **Decisão:** consultar PostgreSQL com `Npgsql` sem EF Core.
- **Justificativa:** reduz tempo de inicialização e dependências da função.

## ADR-003 — SecretService com cache em memória

- **Status:** Aceito
- **Decisão:** cachear configurações do Cognito após primeira leitura do Secrets Manager.
- **Justificativa:** reduz latência e chamadas repetidas a serviço externo.

## ADR-004 — Resposta padronizada por mensagem

- **Status:** Aceito
- **Decisão:** retornar objeto simples com `message` em sucesso e falha.
- **Justificativa:** simplifica consumo por clientes e API Gateway.

## ADR-005 — CPF mascarado em logs

- **Status:** Aceito
- **Decisão:** registrar CPF com mascaramento parcial (`***1234`).
- **Justificativa:** reduz exposição de dado sensível em logs operacionais.
