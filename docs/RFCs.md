# RFCs da Lambda

## RFC-001 — Função dedicada para geração de token de cliente

- **Status:** Aceito
- **Contexto:** separar o fluxo de autenticação de cliente da API principal.
- **Decisão:** implementar uma Lambda específica para emissão de token Cognito e envio por e-mail.
- **Impactos:** desacopla responsabilidade e facilita evolução do fluxo de autenticação de clientes.

## RFC-002 — Projeto único com arquitetura em camadas

- **Status:** Aceito
- **Contexto:** necessidade de equilíbrio entre organização arquitetural e tamanho de pacote.
- **Decisão:** manter Clean Architecture/Ports & Adapters dentro de um único projeto .NET.
- **Impactos:** preserva separação de responsabilidades com menor overhead de build e deploy.

## RFC-003 — Segredo consolidado para Cognito e senha de banco

- **Status:** Aceito
- **Contexto:** reduzir variáveis sensíveis distribuídas e simplificar configuração.
- **Decisão:** armazenar credenciais Cognito + `db_password` em um único secret do Secrets Manager.
- **Impactos:** simplifica bootstrap da função e centraliza gestão de segredos.

## RFC-004 — Deploy automatizado via GitHub Actions

- **Status:** Aceito
- **Contexto:** padronizar deploy e evitar preparação manual de infraestrutura auxiliar.
- **Decisão:** workflow descobre recursos AWS e cria/atualiza secret automaticamente.
- **Impactos:** menor erro operacional e maior reprodutibilidade.
