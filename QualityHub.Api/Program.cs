
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
var testCases = new[]
{
    new TestCase(
        "TC-001",
        "Inicio de sesión correcto",
        "High",
        "El usuario accede con credenciales válidas."
    ),
    new TestCase(
        "TC-002",
        "Contraseña incorrecta",
        "High",
        "Se muestra un mensaje de error y se deniega el acceso."
    ),
    new TestCase(
        "TC-003",
        "Campo de usuario vacío",
        "Medium",
        "Se solicita al usuario que introduzca su nombre de usuario."
    )
};

// GET /api/testcases: obtener todos los casos de prueba.
app.MapGet("/api/testcases", () =>
{
    return Results.Ok(testCases);
})
.WithName("GetTestCases")
.WithTags("Test Cases");

app.Run();

// Modelo que representa un caso de prueba.
record TestCase(
    string Id,
    string Title,
    string Priority,
    string ExpectedResult
);

public partial class Program { } 