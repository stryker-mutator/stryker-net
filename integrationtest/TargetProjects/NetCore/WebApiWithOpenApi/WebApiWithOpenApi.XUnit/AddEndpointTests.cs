using Microsoft.AspNetCore.Mvc.Testing;

namespace TargetProjectWebApiNet10.XUnit;

public class AddEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AddEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Add_WithPositiveNumbers_ReturnsSum()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/add/2/3");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("5", result);
    }

    [Fact]
    public async Task Add_WithNegativeNumbers_ReturnsSum()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/add/-5/-3");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("-8", result);
    }

    [Fact]
    public async Task Add_WithZero_ReturnsSum()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/add/0/0");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("0", result);
    }
}
