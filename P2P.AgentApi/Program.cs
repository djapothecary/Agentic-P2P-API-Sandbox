using Microsoft.EntityFrameworkCore;
using P2P.AgentApi.Data;
using P2P.AgentApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

//  Database configuration
builder.Services.AddDbContext<AgentApiDbContext>(options =>
    options.UseInMemoryDatabase("EciP2P_Database")
);

var app = builder.Build();

//  Seed Database
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope
        .ServiceProvider
        .GetRequiredService<AgentApiDbContext>();

    await DataSeeder.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

//  Custom Endpoints
app.MapPurchaseOrderEndpoints();
app.MapInvoiceEndpoints();
app.MapGeneralLedgerEndpoints();

app.Run();
