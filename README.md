<div align="center">

# FinX — Gestão de Pacientes

**API REST para gestão de pacientes, histórico médico, hospitais, agendamentos e unificação segura de cadastros duplicados.**

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![MongoDB](https://img.shields.io/badge/MongoDB-7%2B-47A248?style=for-the-badge&logo=mongodb&logoColor=white)
![JWT](https://img.shields.io/badge/Auth-JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![Architecture](https://img.shields.io/badge/Architecture-Hexagonal-0A66C2?style=for-the-badge)
![Tests](https://img.shields.io/badge/Tests-xUnit-25A162?style=for-the-badge)

[Visão geral](#visão-geral) • [Arquitetura](#arquitetura) • [Como executar](#como-executar) • [API](#api) • [Decisões técnicas](#decisões-técnicas) • [Testes](#testes)

</div>

---

## Visão geral

O FinX é uma solução backend desenvolvida em **ASP.NET Core** com persistência em **MongoDB**. O projeto organiza regras de negócio, contratos HTTP e acesso a dados em camadas independentes, seguindo os princípios da arquitetura hexagonal.

O fluxo principal permite:

- cadastrar, consultar, atualizar e excluir pacientes;
- pesquisar pacientes com filtros, paginação e ordenação diretamente no MongoDB;
- manter o histórico médico de cada paciente;
- autenticar usuários e proteger endpoints com JWT assinado por RSA;
- gerenciar hospitais, vínculos hospital–paciente e agendamentos;
- consultar exames externos por meio de um adaptador simulado;
- detectar e unificar pacientes duplicados pelo CPF, atualizando suas referências;
- executar rotinas operacionais por uma aplicação de linha de comando;
- explorar e validar a API pelo Swagger ou pela collection do Postman.

## Principais características

| Área | Implementação |
| --- | --- |
| Arquitetura | Hexagonal, com portas no domínio e adaptadores nas bordas |
| Plataforma | .NET 10 / ASP.NET Core |
| Banco de dados | MongoDB, com compatibilidade para os documentos existentes |
| Segurança | JWT RS256, usuários persistidos e senhas protegidas com PBKDF2-SHA256 |
| Consulta | Filtros, contagem, ordenação, `skip` e `limit` executados no MongoDB |
| Contratos | DTOs de entrada e saída separados, sem exposição direta do transporte ao domínio |
| Qualidade | Nullable habilitado, um tipo público por arquivo, formatação padronizada e testes xUnit |
| Documentação | README, guia HTML, Swagger, diagramas e collection Postman |

## Arquitetura

### Fluxo da aplicação

```mermaid
flowchart LR
    Client[Cliente / Postman / Swagger]
    Api[FinX.Api<br/>Controllers e DTOs]
    Ports[FinX.Core<br/>Domínio e portas]
    Services[FinX.Service<br/>Casos de uso]
    Data[FinX.Data<br/>Repositórios]
    Mongo[(MongoDB)]
    Adapter[FinX.Adapter<br/>Serviços externos e RSA]
    IoC[FinX.IoC<br/>Composição e configuração]

    Client -->|HTTP + JWT| Api
    Api --> Services
    Api -. contratos .-> Ports
    Services -->|depende das portas| Ports
    Data -->|implementa as portas| Ports
    Services -. chamada em tempo de execução .-> Data
    Data --> Mongo
    Adapter -->|implementa as portas| Ports
    Services -. chamada em tempo de execução .-> Adapter
    IoC -. registra dependências .-> Api
    IoC -. configura .-> Data
```

Uma requisição chega ao controller, é convertida do DTO para o domínio e encaminhada ao caso de uso. O serviço depende de interfaces definidas no núcleo; a implementação concreta do repositório é injetada pelo projeto de IoC. Assim, regras de negócio não dependem de HTTP ou MongoDB.

### Responsabilidade de cada projeto

| Projeto | Responsabilidade | Exemplos |
| --- | --- | --- |
| `FinX.Core` | Núcleo da solução. Contém domínio, contratos e portas | Entidades, DTOs Request/Response, filtros, paginação e interfaces |
| `FinX.Service` | Implementa casos de uso e regras de aplicação | Pacientes, autenticação, hospitais, agendamentos e unificação |
| `FinX.Data` | Implementa as portas de persistência | Repositórios MongoDB, filtros e atualizações de referências |
| `FinX.Api` | Expõe a aplicação via HTTP | Controllers, mapeadores, Swagger e configuração JWT |
| `FinX.Adapter` | Integra recursos externos ou técnicos | Geração e leitura das chaves RSA |
| `FinX.IoC` | É o *composition root* | Registro de services, repositories, adapters e `MongoConfiguration` |
| `FinX.Script` | Executa operações administrativas e de dados | Criar usuário, validar conexão, gerar massa e unificar duplicados |
| `FinX.Tests` | Valida comportamentos críticos | Autenticação, paginação, filtros e rota canônica de pacientes |
| `FinX.Docs` | Reúne material visual e documentação complementar | Guia HTML, diagramas C4, sequência e material do case |

### Regras de dependência

- `FinX.Core` não conhece infraestrutura, banco ou HTTP.
- `FinX.Service` trabalha com interfaces do núcleo.
- `FinX.Data` implementa as interfaces de persistência.
- `FinX.Api` recebe e responde HTTP, sem consultar o MongoDB diretamente.
- `FinX.IoC` escolhe e registra as implementações concretas.
- DTOs não substituem entidades de domínio; os mapeadores fazem essa conversão.

### Diagramas do fluxo de integração

O repositório também contém a visão C4 e o diagrama de sequência do fluxo de integração de anexos.

<p align="center">
  <img src="./FinX.Docs/diagram/fin-x_c4.png" alt="Diagrama C4 da integração FinX" width="820" />
</p>

<details>
<summary><strong>Ver diagrama de sequência da integração</strong></summary>

<br />

![Diagrama de sequência da integração](./FinX.Docs/diagram/fin-x.png)

</details>

O arquivo editável está em [`FinX.Docs/diagram/fin-x.excalidraw`](./FinX.Docs/diagram/fin-x.excalidraw), e o guia completo pode ser aberto em [`FinX.Docs/GUIA_ARQUITETURA_FINX.html`](./FinX.Docs/GUIA_ARQUITETURA_FINX.html).

## Como executar

### Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download);
- MongoDB local, Docker ou cluster MongoDB Atlas;
- Git;
- Postman opcional para executar a collection.

### 1. Clonar e restaurar

```bash
git clone https://github.com/VinyRN/FinX-GestaoPaciente.git
cd FinX-GestaoPaciente
dotnet restore Fin-x.sln
```

### 2. Configurar o MongoDB com segurança

Use User Secrets no ambiente de desenvolvimento. Não grave usuário, senha ou URI real no repositório.

```bash
dotnet user-secrets set "MONGODB_URI" "mongodb+srv://usuario:senha@cluster.mongodb.net" --project FinX.Api
dotnet user-secrets set "MONGODB_DATABASE" "finxdb" --project FinX.Api
```

`FinX.Api` e `FinX.Script` compartilham o mesmo identificador de User Secrets. Também são aceitas as configurações `Mongo:ConnectionString` e `Mongo:Database`; `MONGODB_URI` e `MONGODB_DATABASE` têm prioridade.

Para validar o acesso antes de iniciar a aplicação:

```bash
dotnet run --project FinX.Script -- check-connection
```

Resultado esperado:

```text
Conexão confirmada com o banco finxdb.
```

### 3. Criar o primeiro usuário

No PowerShell:

```powershell
$env:FINX_ADMIN_USERNAME = "admin"
$env:FINX_ADMIN_PASSWORD = "Escolha-Uma-Senha-Forte-123!"
$env:FINX_ADMIN_ROLE = "admin"

dotnet run --project FinX.Script -- create-user

Remove-Item Env:\FINX_ADMIN_PASSWORD
```

O usuário é gravado na collection `usuarios`. A senha precisa ter pelo menos 12 caracteres e é persistida somente como hash PBKDF2-SHA256 com salt aleatório.

### 4. Compilar, testar e iniciar

```bash
dotnet build Fin-x.sln
dotnet test FinX.Tests/FinX.Tests.csproj
dotnet run --project FinX.Api/FinX.Api.csproj
```

Com a configuração padrão:

- API: `http://localhost:5006`
- Swagger: `http://localhost:5006/swagger`

## Autenticação

Somente `POST /api/Auth/login` é público. Os demais controllers utilizam `[Authorize]` e exigem o header `Authorization`.

### Login

```http
POST /api/Auth/login
Content-Type: application/json
```

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

Utilize o token nas próximas chamadas:

```http
Authorization: Bearer eyJ...
```

O token contém identificador, nome e perfil do usuário, possui validade configurável e é assinado com RSA SHA-256. As chaves são geradas em tempo de execução e a pasta de chaves está excluída do Git.

## API

### Módulos disponíveis

| Módulo | Rota base | Responsabilidade |
| --- | --- | --- |
| Autenticação | `/api/Auth` | Validação de credenciais e emissão do JWT |
| Pacientes | `/api/Paciente` | CRUD, consulta paginada, histórico e unificação |
| Hospitais | `/api/Hospitals` | CRUD e relacionamento com pacientes |
| Agendamentos | `/api/Agendamentos` | CRUD, consultas por paciente/hospital e alteração de status |
| Exames externos | `/api/ExternalExams` | Consulta simulada de exames por CPF |

### Endpoints de pacientes

Existe um único controller para pacientes, com rota canônica `/api/Paciente`. As rotas legadas `/api/Patients` e `/api/v1/pacientes` não são expostas.

| Método | Rota | Finalidade | Resposta principal |
| --- | --- | --- | --- |
| `POST` | `/api/Paciente` | Cadastrar paciente | `201 Created` |
| `GET` | `/api/Paciente/{id}` | Consultar por identificador | `200 OK` ou `404 Not Found` |
| `GET` | `/api/Paciente` | Filtrar e paginar pacientes | `200 OK` |
| `PUT` | `/api/Paciente/{id}` | Atualizar paciente | `204 No Content` |
| `DELETE` | `/api/Paciente/{id}` | Excluir paciente e seus registros médicos | `204 No Content` |
| `POST` | `/api/Paciente/{id}/registros-medicos` | Adicionar registro médico | `201 Created` |
| `GET` | `/api/Paciente/{id}/registros-medicos` | Consultar histórico médico | `200 OK` |
| `GET` | `/api/Paciente/duplicados` | Listar pacientes com CPF duplicado | `200 OK` |
| `GET` | `/api/Paciente/duplicados/{cpf}` | Listar duplicados de um CPF | `200 OK` |
| `POST` | `/api/Paciente/unificar-duplicados` | Unificar todos os CPFs duplicados | `200 OK` |
| `POST` | `/api/Paciente/unificar-duplicados/{cpf}` | Unificar um CPF específico | `200 OK` |

### Exemplo de cadastro

```http
POST /api/Paciente
Authorization: Bearer {token}
Content-Type: application/json
```

```json
{
  "name": "Ana Souza",
  "cpf": "12345678901",
  "dateOfBirth": "1992-04-18",
  "contact": "ana.souza@email.com"
}
```

### Consulta paginada

```http
GET /api/Paciente?nome=ana&pagina=1&tamanhoPagina=10&ordenarPor=dataCadastro&direcao=desc
Authorization: Bearer {token}
```

Parâmetros disponíveis:

| Parâmetro | Regra |
| --- | --- |
| `nome` | Busca parcial, sem diferenciar maiúsculas e minúsculas |
| `cpf` | Correspondência exata |
| `nascimentoDe` | Limite inicial da data de nascimento |
| `nascimentoAte` | Limite final inclusivo da data de nascimento |
| `pagina` | Mínimo 1; padrão 1 |
| `tamanhoPagina` | Entre 1 e 100; padrão 20 |
| `ordenarPor` | `nome`, `cpf`, `dataNascimento` ou `dataCadastro` |
| `direcao` | `asc` ou `desc` |

Exemplo de resposta:

```json
{
  "itens": [
    {
      "id": "21d57653-a3bc-4db1-a808-024310f2187d",
      "name": "Ana Souza",
      "cpf": "12345678901",
      "dateOfBirth": "1992-04-18T00:00:00Z",
      "contact": "ana.souza@email.com",
      "dataCadastro": "2026-10-15T02:00:00Z"
    }
  ],
  "pagina": 1,
  "tamanhoPagina": 10,
  "totalRegistros": 1,
  "totalPaginas": 1
}
```

Filtro, contagem, ordenação, `skip` e `limit` são processados pelo MongoDB. O `Id` é acrescentado como segundo critério de ordenação, evitando repetição ou troca de posição entre páginas quando o campo principal possui valores iguais.

## Unificação de pacientes duplicados

Pacientes com o mesmo CPF são considerados duplicados. A rotina:

1. agrupa cadastros pelo CPF;
2. mantém o registro com `DataCadastro` mais recente;
3. atualiza referências em grupos, agendamentos e registros médicos;
4. remove os cadastros substituídos somente após atualizar as referências;
5. retorna um resumo do processamento.

A execução pode ser feita pela API ou pelo projeto `FinX.Script`:

```bash
dotnet run --project FinX.Script -- create-test-data
dotnet run --project FinX.Script -- unify-duplicates
dotnet run --project FinX.Script -- full-demo
```

## Persistência e compatibilidade

| Collection | Conteúdo |
| --- | --- |
| `patients` | Pacientes |
| `medicalrecords` | Histórico médico |
| `usuarios` | Credenciais, perfil e estado do usuário |
| `hospitals` | Hospitais |
| `agendamentos` | Agendamentos |
| `grupospacientehospital` | Relacionamento entre pacientes e hospitais |

Os nomes BSON existentes foram preservados para manter compatibilidade com dados anteriores. Por exemplo, o domínio utiliza `Nome`, `Cpf`, `DataNascimento` e `Contato`, enquanto os documentos continuam usando `Name`, `CPF`, `DateOfBirth` e `Contact`. O contrato JSON também mantém os campos pertinentes já consumidos pelos clientes.

## Decisões técnicas

| Decisão | Motivação | Resultado |
| --- | --- | --- |
| Arquitetura hexagonal | Isolar regras de negócio de frameworks e infraestrutura | Serviços podem ser testados com repositórios falsos e a persistência pode evoluir sem alterar os casos de uso |
| Domínio em português | Alinhar o código ao vocabulário do negócio | Entidades e casos de uso ficam mais claros para o time responsável pelo domínio |
| DTOs Request/Response separados | Evitar acoplamento entre transporte e entidade | Contratos HTTP evoluem sem expor detalhes internos |
| Um tipo público por arquivo | Melhorar navegação, revisão e manutenção | Estrutura previsível e aderente às convenções adotadas |
| Paginação no MongoDB | Evitar carregar toda a collection em memória | Menor uso de memória e tráfego, com resposta preparada para grandes volumes |
| Ordenação estável por `Id` | Garantir consistência entre páginas | Registros com o mesmo valor de ordenação não mudam de página arbitrariamente |
| `MongoConfiguration.cs` dedicado | Centralizar criação do cliente e seleção do banco | Configuração única, validada e reutilizada pela injeção de dependência |
| JWT com RS256 | Separar assinatura e validação usando par de chaves | Autenticação stateless e assinatura assimétrica |
| PBKDF2-SHA256 com 210 mil iterações | Não armazenar senhas reversíveis | Hash individual com salt aleatório e comparação em tempo constante |
| User Secrets e variáveis de ambiente | Impedir credenciais no código-fonte | Configuração local segura e compatível com pipelines/containers |
| Preservação dos mapeamentos BSON/JSON | Manter compatibilidade retroativa | A refatoração não exige migração imediata dos documentos e consumidores existentes |
| Rota única `/api/Paciente` | Eliminar ambiguidade e rotas legadas | Contrato claro e verificável por teste automatizado |

## Testes

Execute:

```bash
dotnet test FinX.Tests/FinX.Tests.csproj
```

Os testes automatizados cobrem os comportamentos centrais:

- emissão de JWT para credenciais válidas;
- rejeição de senha inválida;
- paginação com total de registros e páginas;
- filtro de paciente por CPF;
- exposição exclusiva da rota canônica `/api/Paciente`.

Última validação local: **5 testes aprovados, 0 falhas e 0 ignorados** em .NET 10.

## Postman e documentação

A collection [`postman_collection.json`](./postman_collection.json) inclui:

- login com armazenamento automático do token;
- CRUD de pacientes;
- filtros e paginação;
- histórico médico;
- consulta e unificação de duplicados.

Arquivos complementares:

- [`FinX.Docs/GUIA_ARQUITETURA_FINX.html`](./FinX.Docs/GUIA_ARQUITETURA_FINX.html) — guia visual da arquitetura;
- [`FinX.Docs/diagram`](./FinX.Docs/diagram) — imagens e fonte editável dos diagramas;
- [`terraform`](./terraform) — definição de infraestrutura;
- [`FinX.Script/README.md`](./FinX.Script/README.md) — detalhes das rotinas administrativas.

## Estrutura resumida

```text
FinX-GestaoPaciente/
├── FinX.Api/          # Entrada HTTP, controllers, mapeamentos e Swagger
├── FinX.Core/         # Domínio, DTOs, modelos e interfaces
├── FinX.Service/      # Casos de uso e regras de aplicação
├── FinX.Data/         # Persistência MongoDB
├── FinX.Adapter/      # Adaptadores externos e chaves RSA
├── FinX.IoC/          # Injeção de dependência e MongoConfiguration
├── FinX.Script/       # Ferramentas administrativas
├── FinX.Tests/        # Testes automatizados
├── FinX.Docs/         # Guia e diagramas
├── terraform/         # Infraestrutura como código
├── docker-compose.yml
├── postman_collection.json
└── Fin-x.sln
```

## Segurança

- Não publique a URI real do MongoDB, senhas, tokens ou arquivos `.env`.
- As chaves privadas JWT e os arquivos de User Secrets não são versionados.
- Em produção, armazene chaves e credenciais em um secret manager ou volume protegido.
- Utilize HTTPS e restrinja o acesso de rede ao MongoDB.
- Troque imediatamente qualquer credencial que tenha sido compartilhada ou exposta.

---

<div align="center">

Desenvolvido como uma API modular, testável e preparada para evolução.

</div>
