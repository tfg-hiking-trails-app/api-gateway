using HikingTrails.ApiGateway.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtBearer();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddOcelot("Ocelot", builder.Environment, MergeOcelotJson.ToMemory)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.UseOcelot();

app.UseAuthorization();
app.MapControllers();

app.Run();
