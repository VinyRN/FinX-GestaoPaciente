# FinX — Gestão de Pacientes

API REST em .NET 10 e MongoDB para cadastro de pacientes, histórico médico, hospitais, agendamentos e unificação de cadastros duplicados.

## Arquitetura

A solução segue arquitetura hexagonal:

```text
HTTP
  ↓
FinX.Api        Controllers, contratos HTTP e mapeadores
  ↓
FinX.Core       Domínio, DTOs e portas (interfaces)
  ↓
FinX.Service    Casos de uso e regras de aplicação
  ↓
FinX.Data       Adaptadores de persistência MongoDB

FinX.Adapter    Adaptadores externos, como chaves RSA
FinX.IoC        Composition root e configuração do MongoDB
```

As dependências apontam para as portas definidas em `FinX.Core`. Controllers não acessam o MongoDB diretamente, serviços não conhecem detalhes de persistência e repositórios não tratam HTTP.

## Convenções

- domínio e casos de uso em português;
- um tipo público por arquivo;
- DTOs separados em `Request` e `Response`;
- serviços e repositórios separados por interfaces;
- mapeamentos BSON preservam os campos existentes no MongoDB;
- contratos JSON de paciente preservam `id`, `name`, `cpf`, `dateOfBirth`, `contact` e `dataCadastro`;
- configuração de banco centralizada em `FinX.IoC/MongoConfiguration.cs`;
- código sem comentários que apenas repetem a implementação.

## Endpoint de pacientes

Existe um único controller de pacientes, com rota base `api/Paciente`. As rotas antigas `api/Patients` e `api/v1/pacientes` não são expostas.

| Método | Rota | Finalidade |
| --- | --- | --- |
| POST | `/api/Paciente` | Criar paciente |
| GET | `/api/Paciente/{id}` | Obter paciente |
| GET | `/api/Paciente` | Consultar pacientes com filtros e paginação |
| PUT | `/api/Paciente/{id}` | Atualizar paciente |
| DELETE | `/api/Paciente/{id}` | Excluir paciente |
| POST | `/api/Paciente/{id}/registros-medicos` | Adicionar registro médico |
| GET | `/api/Paciente/{id}/registros-medicos` | Obter histórico médico |
| GET | `/api/Paciente/duplicados` | Listar cadastros duplicados |
| GET | `/api/Paciente/duplicados/{cpf}` | Listar duplicados por CPF |
| POST | `/api/Paciente/unificar-duplicados` | Unificar todos os duplicados |
| POST | `/api/Paciente/unificar-duplicados/{cpf}` | Unificar duplicados de um CPF |

Todos os endpoints, exceto `POST /api/Auth/login`, exigem JWT Bearer.

## Primeiro usuário e autenticação JWT

O login consulta a coleção `usuarios` no MongoDB. Senhas são armazenadas com PBKDF2-SHA256, salt aleatório e comparação em tempo constante; não existe usuário ou senha fixos no código.

Para criar o primeiro administrador no PowerShell:

```powershell
$env:FINX_ADMIN_USERNAME = "admin"
$env:FINX_ADMIN_PASSWORD = "Escolha-Uma-Senha-Forte-123!"
$env:FINX_ADMIN_ROLE = "admin"
dotnet run --project FinX.Script -- create-user
Remove-Item Env:\FINX_ADMIN_PASSWORD
```

Antes da criação, você pode confirmar o banco configurado:

```powershell
dotnet run --project FinX.Script -- check-connection
```

O comando deve informar `Conexão confirmada com o banco finxdb`. O `create-user` recusa conexão local para evitar que o usuário seja criado por engano fora do Atlas.

O script usa a mesma configuração segura do MongoDB da API. A senha precisa ter pelo menos 12 caracteres e não é exibida nem armazenada em texto puro.

Depois, autentique em `POST /api/Auth/login`:

```json
{
  "username": "admin",
  "password": "Escolha-Uma-Senha-Forte-123!"
}
```

Resposta bem-sucedida:

```json
{
  "token": "eyJ...",
  "tipo": "Bearer",
  "expiraEm": "2026-10-15T05:00:00Z",
  "nomeUsuario": "admin",
  "perfil": "admin"
}
```

Envie o valor de `token` no header `Authorization: Bearer {token}`. A validade padrão é de 360 minutos e pode ser alterada em `Jwt:ExpirationMinutes`.

## Consulta paginada

`GET /api/Paciente` aceita:

- `nome`: busca parcial sem diferenciar maiúsculas e minúsculas;
- `cpf`: correspondência exata;
- `nascimentoDe` e `nascimentoAte`: intervalo de nascimento;
- `pagina`: mínimo 1, padrão 1;
- `tamanhoPagina`: de 1 a 100, padrão 20;
- `ordenarPor`: `nome`, `cpf`, `dataNascimento` ou `dataCadastro`;
- `direcao`: `asc` ou `desc`.

Exemplo:

```http
GET /api/Paciente?nome=ana&pagina=2&tamanhoPagina=10&ordenarPor=dataCadastro&direcao=desc
```

Resposta:

```json
{
  "itens": [],
  "pagina": 2,
  "tamanhoPagina": 10,
  "totalRegistros": 0,
  "totalPaginas": 0
}
```

Filtro, contagem, ordenação, `skip` e `limit` são executados no MongoDB. Toda ordenação recebe `Id` como segundo critério para permanecer estável entre páginas.

## Persistência e compatibilidade

As coleções existentes continuam sendo utilizadas: `patients`, `medicalrecords`, `hospitals`, `agendamentos` e `grupospacientehospital`. A entidade `Paciente` mantém os campos BSON `Name`, `CPF`, `DateOfBirth` e `Contact`. `RegistroMedico` usa nomes em português no domínio e mapeia os campos BSON legados `PatientId`, `Date`, `Type` e `Description`.

## Configuração

Defina em `FinX.Api/appsettings.json` ou por variáveis de ambiente:

```text
Mongo__ConnectionString=mongodb://localhost:27017
Mongo__Database=finxdb
Jwt__Issuer=FinXApi
```

Também são aceitas as variáveis padronizadas `MONGODB_URI` e `MONGODB_DATABASE`, que têm prioridade sobre `Mongo__ConnectionString` e `Mongo__Database`. Guarde a URI em variável de ambiente ou secret manager; não a grave no repositório.

Para desenvolvimento local, configure sem salvar a senha no projeto:

```bash
dotnet user-secrets set "MONGODB_URI" "mongodb+srv://usuario:senha@cluster.mongodb.net" --project FinX.Api
dotnet user-secrets set "MONGODB_DATABASE" "finxdb" --project FinX.Api
```

As chaves RSA devem ser fornecidas por segredo ou volume protegido em produção. Não publique chaves privadas reais.

## Execução e validação

```bash
dotnet restore Fin-x.sln
dotnet build Fin-x.sln
dotnet test FinX.Tests/FinX.Tests.csproj
dotnet run --project FinX.Api/FinX.Api.csproj
```

Swagger fica disponível em `/swagger`. A collection `postman_collection.json` contém o login, CRUD, consulta paginada, histórico médico e unificação. O guia visual está em `FinX.Docs/GUIA_ARQUITETURA_FINX.html`.

## Estrutura

| Projeto | Responsabilidade |
| --- | --- |
| `FinX.Core` | Entidades, DTOs, paginação e portas |
| `FinX.Service` | Regras e casos de uso |
| `FinX.Data` | Repositórios MongoDB |
| `FinX.Api` | API, autenticação, controllers e mapeamentos |
| `FinX.Adapter` | Adaptadores externos |
| `FinX.IoC` | Registro de dependências e MongoDB |
| `FinX.Tests` | Testes automatizados |
| `FinX.Script` | Rotinas operacionais de dados |
