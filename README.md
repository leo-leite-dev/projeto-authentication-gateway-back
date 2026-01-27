# AuthService + Gateway — Security Architecture

## Visão Geral

Este projeto implementa uma arquitetura moderna de autenticação e segurança baseada em JWT, cookies seguros, gateway com boundary HTTP, revogação global de tokens e rotação de chaves RSA, seguindo boas práticas utilizadas em ambientes corporativos e sistemas distribuídos.

A solução é composta por dois serviços principais:

- **AuthService** → Responsável por autenticação, emissão de tokens e controle de sessões  
- **Gateway.API** → Responsável por validação de tokens, segurança perimetral e enforcement de políticas  

O objetivo principal é garantir:

- Defesa em profundidade (defense in depth)
- Revogação centralizada de sessões
- Redução de impacto em caso de vazamento de token
- Isolamento de serviços internos
- Perímetro HTTP fortemente controlado

---

## Arquitetura de Segurança (High Level)

```text
Client (Browser / App)
        |
        v
   Gateway.API
   - JWT Validation
   - CSRF Protection
   - CORS + Origin Validation
   - Security Headers
   - Token Revocation Enforcement
        |
        v
   AuthService
   - Login
   - Token Issuance
   - TokenVersion Management
   - Key Management (RSA)
   - Internal Token Validation
```

---

## Princípios Implementados

### JWT com validação no Gateway

O Gateway é responsável por:

- Validar assinatura JWT
- Validar Issuer
- Validar Audience
- Validar expiração

Isso garante que nenhuma API interna confia diretamente no cliente, apenas no Gateway.

**Benefícios:**

- Centralização da validação
- Redução da superfície de ataque
- Padronização de políticas

---

### Cookies Seguros

Os tokens são armazenados em cookies configurados com:

- `HttpOnly`
- `Secure` (em produção)
- `SameSite=Strict`

**Benefícios:**

- Proteção contra XSS
- Mitigação de CSRF
- Redução de exfiltração via JavaScript

---

### Boundary HTTP no Gateway

O Gateway atua como fronteira de segurança HTTP, implementando:

#### CORS
- Whitelist de origens permitidas

#### Origin Validation
- Middleware dedicado para validação de `Origin` e `Referer`

#### Security Headers
- Content-Security-Policy (CSP)
- X-Frame-Options
- X-Content-Type-Options
- Referrer-Policy

**Benefícios:**

- Mitigação de clickjacking
- Mitigação de XSS
- Hardening do navegador

---

### Proteção CSRF no Gateway

Implementado via middleware dedicado que:

- Valida Origin
- Valida Referer
- Aplica regras apenas para requests com cookie

**Benefícios:**

- Proteção contra ataques Cross-Site Request Forgery
- Integração natural com autenticação via cookies

---

### Revogação Global de Tokens (TokenVersion)

Cada usuário possui um campo `TokenVersion`.

O JWT contém:

- `sub` (UserId)
- `tokenVersion`

No Gateway:

- O tokenVersion é extraído do JWT
- O Gateway consulta o AuthService via endpoint interno
- Se houver mismatch, o token é invalidado

**Benefícios:**

- Logout global
- Invalidação imediata de tokens roubados
- Controle centralizado de sessões

---

### Key Rotation RSA com múltiplas chaves

O AuthService:

- Assina tokens com RSA private key
- Define `kid` no header do JWT

O Gateway:

- Carrega múltiplas chaves públicas (PEM)
- Resolve a chave correta via `kid`

**Benefícios:**

- Rotação sem downtime
- Redução de impacto em vazamento de chave
- Compatibilidade com ambientes corporativos

---

### Comunicação Interna Segura (InternalApiKey)

Chamadas internas Gateway → AuthService utilizam:

- Header `X-Internal-Api-Key`

O AuthService valida essa chave antes de processar requests internos.

**Benefícios:**

- Isolamento de endpoints internos
- Proteção contra acesso direto externo
- Defesa adicional além de network-level security

---

## Endpoints Internos

### POST `/internal/token/validate`

Usado pelo Gateway para:

- Validar se o usuário ainda é válido
- Validar TokenVersion
- Enforçar revogação global

Este endpoint é protegido via `InternalApiKey`.

---

## Fluxo de Autenticação (Resumo)

1. Usuário faz login no AuthService  
2. AuthService:
   - Emite JWT
   - Define cookie seguro  
3. Cliente envia requests com cookie  
4. Gateway:
   - Valida JWT
   - Aplica CSRF
   - Valida TokenVersion  
5. Request é encaminhado para APIs internas
