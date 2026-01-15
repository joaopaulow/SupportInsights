# Support Insights API

API para gerenciamento e visualização de dados de tickets de suporte técnico com autenticação JWT.

## Versão
- **.NET**: 8.0
- **C#**: 12.0
- **Entity Framework Core**: InMemory
- **Autenticação**: JWT (JSON Web Token)

## Regras de Negócio

1. **Agrupamentos são feitos em memória**: Os dados são recuperados do banco e os agrupamentos por cliente e módulo são processados na aplicação, não via SQL.

2. **Agrupamentos consideram período completo**: Mesmo com paginação, os agrupamentos por cliente e módulo refletem **todos** os tickets do mês/ano filtrado, não apenas os da página atual.

3. **Código do ticket é sequencial**: Gerado automaticamente como `MAX(codigo) + 1`.

4. **Data de abertura é automática**: Definida como `DateTime.Now` no momento da criação.

5. **Paginação**:
   - Página padrão: 1
   - Tamanho de página padrão: 10 itens
   - Tamanho máximo: 100 itens por página
   - Tamanho mínimo: 1 item por página

6. **Validações**:
   - Título é obrigatório
   - Cliente deve existir na base
   - Módulo deve existir na base
   - Número da página deve ser >= 1
   - Tamanho da página deve estar entre 1 e 100

7. **Banco de dados**: Utiliza Entity Framework Core InMemory, os dados são perdidos ao reiniciar a aplicação.

---

## Autenticação

A API utiliza **JWT (JSON Web Token)** para autenticação. Para acessar os endpoints protegidos, você deve:

1. Fazer login no endpoint `/auth/login`
2. Usar o token retornado no header `Authorization` das requisições seguintes

### Header de Autenticação 

---

## Roles (Perfis de Acesso)

| Role | Descrição | Permissões |
|------|-----------|------------|
| **Admin** | Administrador do sistema | Acesso total (dashboard + criar, atualizar e deletar tickets) |
| **Suporte** | Equipe de suporte | Acesso ao dashboard + criar, atualizar e deletar tickets |
| **Cliente** | Cliente final | Apenas criar, atualizar e deletar tickets |

---

## Usuários de Teste

| Email | Senha | Role |
|-------|-------|------|
| admin@supportinsights.com | Admin@123 | Admin |
| suporte@supportinsights.com | Suporte@123 | Suporte |
| cliente@email.com | Cliente@123 | Cliente |