using System.Text.Json;
using Payme;
using Payme.Configurations;
using Payme.Data;
using Payme.Exceptions;
using Payme.Types;

var builder = WebApplication.CreateBuilder(args);

builder.AddDbServices();
builder.Services.AddLogging();
builder.Services.AddSingleton<Merchant>();

builder.Services.AddEndpointsApiExplorer()
    .ConfigureHttpJsonOptions(
    options =>
    {
        options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/api", async context =>
{
    try
    {
        var merchant = app.Services.GetService<Merchant>() 
                       ?? throw new Exception("Merchant service not registered");

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        await merchant.Handle(db, context.Request, context.Response);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        await context.Response.WriteAsJsonAsync(new ErrorResult(new ApiInternalException().Error, 0));
    }
});

app.Run();