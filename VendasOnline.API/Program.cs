using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using VendasOnline.API.Data;
using VendasOnline.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<VendasOnlineDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "VendasOnline.Api",
        Version = "v1",
        Description = "API REST de integração de dados para parceiros de negócios"
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<VendasOnlineDbContext>();

        logger.LogInformation("Verificando e aplicando migrations pendentes no banco de dados...");

        // Aplica todas as migrations pendentes. Se o banco não existir, ele é criado automaticamente!
        await dbContext.Database.MigrateAsync();

        logger.LogInformation("Banco de dados atualizado e pronto para uso.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocorreu um erro ao aplicar as migrations no banco de dados.");
    }


    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "VendasOnline API v1");
            c.RoutePrefix = string.Empty;
        });
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
