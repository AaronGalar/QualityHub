
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;

namespace QualityHub.Tests;



public class TestCasesApiTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TestCasesApiTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    // Debajo de esto conservamos todos tus métodos de prueba.

    [Fact]
    public async Task GetTestCases_ReturnsOkAndThreeCases()
    {
        // Arrange: el cliente HTTP ya está preparado.

        // Act: solicitamos los casos de prueba a la API.
        var response = await _client.GetAsync("/api/testcases");

        // Assert: comprobamos que la API responde correctamente.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Leemos y analizamos el JSON.
        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);
        var cases = document.RootElement;

        // Comprobamos que recibimos una lista con tres casos.
        Assert.Equal(JsonValueKind.Array, cases.ValueKind);
        Assert.Equal(3, cases.GetArrayLength());
        
        // Obtenemos el primer caso de prueba de la respuesta.
        var firstCase = cases[0];

        // Comprobamos su identificador.
        Assert.Equal("TC-001", firstCase.GetProperty("id").GetString());

        // Comprobamos su título.
        Assert.Equal(
            "Inicio de sesión correcto",
            firstCase.GetProperty("title").GetString()
        );

        // Comprobamos su prioridad.
        Assert.Equal("High", firstCase.GetProperty("priority").GetString());


    }
    
    [Fact]
    public async Task GetUnknownRoute_ReturnsNotFound()
    {
        // Act: solicitamos una ruta que no existe.
        var response = await _client.GetAsync("/api/ruta-inventada");

        // Assert: la API debe responder 404.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateTestCase_WithValidData_ReturnsCreated()
    {
        var request = new
        {
            title = "Registro de usuario correcto",
            priority = "High",
            expectedResult = "El usuario se registra correctamente."
        };

        var response = await _client.PostAsJsonAsync(
            "/api/testcases",
            request
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);

        Assert.Equal(
            "Registro de usuario correcto",
            document.RootElement.GetProperty("title").GetString()
        );
    }

    [Fact]
    public async Task CreateTestCase_WithEmptyTitle_ReturnsBadRequest()
    {
        var request = new
        {
            title = " ",
            priority = "High",
            expectedResult = "El registro debería funcionar."
        };

        var response = await _client.PostAsJsonAsync(
            "/api/testcases",
            request
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTestCase_WithInvalidPriority_ReturnsBadRequest()
    {
        var request = new
        {
            title = "Prueba de prioridad inválida",
            priority = "Urgent",
            expectedResult = "Se rechaza la prioridad."
        };

        var response = await _client.PostAsJsonAsync(
            "/api/testcases",
            request
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
public async Task UpdateStatus_WithPassed_ReturnsOk()
{
    // Arrange: preparamos el estado que queremos enviar.
    var request = new
    {
        status = "Passed"
    };

    // Act: enviamos la petición PUT.
    var response = await _client.PutAsJsonAsync(
        "/api/testcases/TC-001/status",
        request
    );

    // Assert: la API debe responder 200 OK.
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    // Comprobamos también el estado devuelto.
    var result = await response.Content.ReadFromJsonAsync<JsonElement>();

    Assert.Equal(
        "Passed",
        result.GetProperty("status").GetString()
    );
}

[Fact]
public async Task UpdateStatus_WithInvalidStatus_ReturnsBadRequest()
{
    var request = new
    {
        status = "Unknown"
    };

    var response = await _client.PutAsJsonAsync(
        "/api/testcases/TC-001/status",
        request
    );

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
}

[Fact]
public async Task UpdateStatus_WithUnknownId_ReturnsNotFound()
{
    var request = new
    {
        status = "Passed"
    };

    var response = await _client.PutAsJsonAsync(
        "/api/testcases/TC-999/status",
        request
    );

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
}

[Fact]
public async Task GetTestCaseById_WithExistingId_ReturnsOk()
{
    // Arrange: identificador de un caso que existe.
    var id = "TC-001";

    // Act: consultamos ese caso.
    var response = await _client.GetAsync($"/api/testcases/{id}");

    // Assert: debe responder 200 OK.
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    // Comprobamos los datos del caso devuelto.
    var result = await response.Content.ReadFromJsonAsync<JsonElement>();

    Assert.Equal("TC-001", result.GetProperty("id").GetString());
    Assert.Equal(
        "Inicio de sesión correcto",
        result.GetProperty("title").GetString()
    );
    Assert.Equal(
        "NotRun",
        result.GetProperty("status").GetString()
    );
}

[Fact]
public async Task GetTestCaseById_WithUnknownId_ReturnsNotFound()
{
    // Act: consultamos un identificador inexistente.
    var response = await _client.GetAsync("/api/testcases/TC-999");

    // Assert: debe responder 404 Not Found.
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
}



}
