
var builder = WebApplication.CreateBuilder(args);

// Registrar servicios de OpenAPI.
builder.Services.AddOpenApi();

var app = builder.Build();

// Publicar el documento OpenAPI en desarrollo.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Datos temporales de ejemplo para QualityHub.
var testCases = new List<TestCase>
{
    new(
        "TC-001",
        "Inicio de sesión correcto",
        "High",
        "El usuario accede con credenciales válidas.",
        "NotRun"
    ),
    new(
        "TC-002",
        "Contraseña incorrecta",
        "High",
        "Se muestra un mensaje de error y se deniega el acceso.",
        "NotRun"
    ),
    new(
        "TC-003",
        "Campo de usuario vacío",
        "Medium",
        "Se solicita al usuario que introduzca su nombre de usuario.",
        "NotRun"
    )
}; 

// GET /api/testcases: obtener todos los casos de prueba.
app.MapGet("/api/testcases", () =>
{
    return Results.Ok(testCases);
})
.WithName("GetTestCases")
.WithTags("Test Cases");

app.MapGet("/api/testcases/{id}", (string id) =>
{
    // Buscar el caso de prueba por su identificador.
    var testCase = testCases.FirstOrDefault(
        testCase => testCase.Id == id
    );

    // Si no existe, devolver un 404.
    if (testCase is null)
    {
        return Results.NotFound(new
        {
            error = "No se ha encontrado el caso de prueba."
        });
    }

    // Si existe, devolver sus datos.
    return Results.Ok(testCase);
})
.WithName("GetTestCaseById")
.WithTags("Test Cases");



app.MapPost("/api/testcases", (CreateTestCaseRequest request) =>
{
    // Validar el título.
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new
        {
            error = "El título es obligatorio."
        });
    }

    // Validar la prioridad permitida.
    var validPriorities = new[] { "Low", "Medium", "High" };

    if (!validPriorities.Contains(request.Priority))
    {
        return Results.BadRequest(new
        {
            error = "La prioridad debe ser Low, Medium o High."
        });
    }

    // Validar el resultado esperado.
    if (string.IsNullOrWhiteSpace(request.ExpectedResult))
    {
        return Results.BadRequest(new
        {
            error = "El resultado esperado es obligatorio."
        });
    }

    // Generar un identificador para el nuevo caso.
    var newId = $"TC-{testCases.Count + 1:000}";

    var newTestCase = new TestCase(
        newId,
        request.Title.Trim(),
        request.Priority,
        request.ExpectedResult.Trim(),
        "NotRun"
    );
    testCases.Add(newTestCase); 

    return Results.Created($"/api/testcases/{newId}", newTestCase);
})
.WithName("CreateTestCase")
.WithTags("Test Cases");

app.MapPut("/api/testcases/{id}/status", (string id, UpdateTestCaseStatusRequest request) =>
{
    // Buscar el caso de prueba por su identificador.
    var testCase = testCases.FirstOrDefault(
        testCase => testCase.Id == id
    );

    // Si no existe, devolver un 404.
    if (testCase is null)
    {
        return Results.NotFound(new
        {
            error = "No se ha encontrado el caso de prueba."
        });
    }

    // Comprobar que el estado sea uno de los permitidos.
    var validStatuses = new[] { "NotRun", "Passed", "Failed" };

    if (!validStatuses.Contains(request.Status))
    {
        return Results.BadRequest(new
        {
            error = "El estado debe ser NotRun, Passed o Failed."
        });
    }

    // Crear el caso actualizado manteniendo sus demás datos.
    var updatedTestCase = testCase with
    {
        Status = request.Status
    };

    // Sustituir el caso antiguo por el actualizado.
    var index = testCases.FindIndex(
        testCase => testCase.Id == id
    );

    testCases[index] = updatedTestCase;

    return Results.Ok(updatedTestCase);
})
.WithName("UpdateTestCaseStatus")
.WithTags("Test Cases");



app.Run();

// Modelo que representa un caso de prueba.
record TestCase(
    string Id,
    string Title,
    string Priority,
    string ExpectedResult,
    string Status 
);

record CreateTestCaseRequest(
    string Title,
    string Priority,
    string ExpectedResult
);
record UpdateTestCaseStatusRequest(string Status);
public partial class Program { } 