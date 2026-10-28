
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace QualityHub.Tests;

public class TestCasesApiTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TestCasesApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

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

}
