var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapGet("/add/{value1:int}/{value2:int}", (int value1, int value2) =>
{
    return value1 + value2;
})
.WithName("Add");

app.Run();
