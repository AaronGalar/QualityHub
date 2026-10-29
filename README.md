# QualityHub .NET

### API REST para la gestión de casos de prueba de software

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/Lenguaje-C%23-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/es-es/dotnet/csharp/)
[![API REST](https://img.shields.io/badge/API-REST-005571)](#-endpoints-de-la-api)
[![Pruebas automatizadas](https://img.shields.io/badge/Pruebas-automatizadas-512BD4)](#-pruebas-automatizadas)
[![Integración continua](https://github.com/AaronGalar/QualityHub/actions/workflows/dotnet.yml/badge.svg)](https://github.com/AaronGalar/QualityHub/actions/workflows/dotnet.yml)

**QualityHub .NET** es un proyecto desarrollado con C# y ASP.NET Core cuyo objetivo es gestionar casos de prueba de software mediante una API REST.

Permite consultar y crear casos de prueba, actualizar sus estados de ejecución y validar las respuestas de la API mediante pruebas automatizadas.

El proyecto está orientado a poner en práctica conocimientos de **QA, desarrollo de API, diseño de pruebas, validación de errores e integración continua**.

> Proyecto personal de aprendizaje y portfolio profesional en calidad del software y desarrollo .NET.

##  Funcionalidades

- **Consultar casos de prueba:** obtener todos los casos registrados.
- **Consultar por identificador:** recuperar un caso de prueba concreto.
- **Crear casos de prueba:** registrar nuevos casos con título, prioridad y resultado esperado.
- **Actualizar estados de ejecución:** marcar los casos como `NotRun`, `Passed` o `Failed`.
- **Validar datos de entrada:** rechazar títulos vacíos, prioridades no admitidas y estados incorrectos.
- **Gestionar errores HTTP:** devolver códigos adecuados para operaciones correctas, datos inválidos y recursos inexistentes.
- **Pruebas automatizadas:** comprobar escenarios positivos y negativos mediante xUnit.
- **Integración continua:** compilar el proyecto y ejecutar las pruebas automáticamente con GitHub Actions.

##  Tecnologías utilizadas

| Tecnología | Utilidad |
|---|---|
| C# | Lenguaje principal de programación |
| .NET 10 | Plataforma de desarrollo |
| ASP.NET Core Minimal APIs | Implementación de los endpoints REST |
| xUnit | Framework de pruebas automatizadas |
| `Microsoft.AspNetCore.Mvc.Testing` | Pruebas de integración con un servidor de prueba en memoria |
| `System.Net.Http.Json` | Envío de peticiones HTTP y tratamiento de JSON en las pruebas |
| Git y GitHub | Control de versiones y alojamiento del repositorio |
| GitHub Actions | Automatización de compilación y pruebas |

##  Estructura del proyecto

```text
QualityHub/
├── .github/
│   └── workflows/
│       └── dotnet.yml
├── QualityHub.Api/
│   ├── Program.cs
│   └── QualityHub.Api.csproj
├── QualityHub.Tests/
│   ├── TestCasesApiTests.cs
│   └── QualityHub.Tests.csproj
├── .gitignore
├── QualityHub.slnx
└── README.md
```

La solución puede llamarse `QualityHub.slnx` o `QualityHub.sln`, según el archivo generado en el entorno de desarrollo.

##  Instalación y ejecución

### Requisitos previos

- [SDK de .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0).
- [Git](https://git-scm.com/).
- Un editor de código, como [Visual Studio Code](https://code.visualstudio.com/).

### 1. Clonar el repositorio

```bash
git clone https://github.com/AaronGalar/QualityHub.git
cd QualityHub
```

### 2. Restaurar las dependencias

```bash
dotnet restore
```

### 3. Compilar la solución

```bash
dotnet build
```

### 4. Ejecutar la API

```bash
dotnet run --project QualityHub.Api
```

La terminal mostrará la dirección local en la que se inicia la aplicación. Utiliza esa dirección para realizar las peticiones. Por ejemplo, si se inicia en `http://localhost:5291`, los endpoints estarán disponibles bajo esa dirección.

### 5. Ejecutar las pruebas automatizadas

Desde una segunda terminal, situada en la raíz de la solución, ejecuta:

```bash
dotnet test
```

Las pruebas utilizan un servidor de prueba en memoria, por lo que no es necesario iniciar la API por separado para ejecutarlas.

##  Endpoints de la API

URL base de ejemplo: `http://localhost:5291`.

| Método | Endpoint | Descripción | Respuesta esperada |
|---|---|---|---|
| `GET` | `/api/testcases` | Consultar todos los casos de prueba | `200 OK` |
| `GET` | `/api/testcases/{id}` | Consultar un caso por su identificador | `200 OK` / `404 Not Found` |
| `POST` | `/api/testcases` | Crear un nuevo caso de prueba | `201 Created` / `400 Bad Request` |
| `PUT` | `/api/testcases/{id}/status` | Actualizar el estado de un caso | `200 OK` / `400 Bad Request` / `404 Not Found` |

### Ejemplo 1. Consultar los casos de prueba

Petición:

```http
GET /api/testcases
```

Ejemplo de respuesta JSON:

```json
[
  {
    "id": "TC-001",
    "title": "Inicio de sesión correcto",
    "priority": "High",
    "expectedResult": "El usuario accede con credenciales válidas.",
    "status": "NotRun"
  }
]
```

### Ejemplo 2. Crear un caso de prueba

Petición:

```http
POST /api/testcases
Content-Type: application/json
```

Cuerpo de la petición:

```json
{
  "title": "Recuperación de contraseña",
  "priority": "Medium",
  "expectedResult": "Se envía el procedimiento de recuperación."
}
```

Si los datos son válidos, la API devuelve `201 Created` junto con el nuevo caso de prueba.

Prioridades admitidas: `Low`, `Medium` y `High`.

### Ejemplo 3. Actualizar el estado de un caso

Petición:

```http
PUT /api/testcases/TC-001/status
Content-Type: application/json
```

Cuerpo de la petición:

```json
{
  "status": "Passed"
}
```

Estados admitidos:

- `NotRun`: el caso todavía no se ha ejecutado.
- `Passed`: el caso ha superado la prueba.
- `Failed`: el caso no ha superado la prueba.

Si se envía un estado no admitido, la API devuelve `400 Bad Request`. Si el identificador no existe, devuelve `404 Not Found`.

##  Pruebas automatizadas

El proyecto utiliza **xUnit** y un servidor de prueba en memoria para verificar el comportamiento de la API sin depender de un servidor local en ejecución.

La batería actual contempla diez escenarios:

| Prueba | Qué se comprueba |
|---|---|
| Consulta de todos los casos | Respuesta `200 OK` y contenido esperado |
| Ruta desconocida | Respuesta `404 Not Found` |
| Creación válida | Registro correcto de un nuevo caso |
| Título vacío | Rechazo de datos inválidos |
| Prioridad no admitida | Validación de la prioridad |
| Actualización a `Passed` | Actualización correcta del estado |
| Estado no admitido | Respuesta `400 Bad Request` |
| Identificador desconocido al actualizar | Respuesta `404 Not Found` |
| Consulta por identificador existente | Recuperación correcta del caso |
| Consulta por identificador inexistente | Respuesta `404 Not Found` |

Para ejecutar toda la batería:

```bash
dotnet test
```

Se comprueban tanto **escenarios positivos**, en los que las operaciones deben funcionar, como **escenarios negativos**, en los que la API debe rechazar datos incorrectos o peticiones sobre recursos inexistentes.

##  Integración continua con GitHub Actions

El repositorio dispone de un flujo de integración continua que se ejecuta al publicar cambios en la rama `main` o al abrir una solicitud de incorporación de cambios dirigida a esa rama.

Archivo de configuración: `.github/workflows/dotnet.yml`.

El flujo realiza las siguientes operaciones:

1. Descarga el código del repositorio.
2. Instala el SDK de .NET 10.
3. Restaura las dependencias.
4. Compila la solución en configuración `Release`.
5. Ejecuta las pruebas automatizadas.

[Consultar las ejecuciones de GitHub Actions](https://github.com/AaronGalar/QualityHub/actions).

##  Hoja de ruta

- [x] Crear la API REST con ASP.NET Core.
- [x] Implementar la consulta y creación de casos de prueba.
- [x] Incorporar la consulta de casos por identificador.
- [x] Permitir la actualización de estados de ejecución.
- [x] Validar los datos de entrada y las respuestas de error.
- [x] Desarrollar pruebas automatizadas de la API.
- [x] Configurar la integración continua con GitHub Actions.
- [ ] Incorporar persistencia de datos con Entity Framework Core y SQLite.
- [ ] Añadir filtros por prioridad y estado de ejecución.
- [ ] Ampliar la cobertura con más casos límite.
- [ ] Mejorar la documentación de la API y sus ejemplos.
- [ ] Desarrollar un panel web para gestionar los casos de prueba.

##  Limitaciones actuales

- **Almacenamiento en memoria:** los casos se guardan en una lista y se pierden al reiniciar la API.
- **Sin autenticación:** el proyecto está orientado al desarrollo local y al aprendizaje.
- **Sin base de datos persistente:** la persistencia y las migraciones se incorporarán en futuras versiones.

##  Objetivo del proyecto

QualityHub .NET es un proyecto en evolución que combina desarrollo de software y control de calidad.

Su finalidad es aplicar principios de QA a una aplicación funcional, desde el diseño de endpoints y la validación de datos hasta las pruebas de regresión automatizadas y la integración continua.

**Repositorio:** [github.com/AaronGalar/QualityHub](https://github.com/AaronGalar/QualityHub)
