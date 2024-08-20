
using BackEnd;
using BackEnd.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

// Assuming you have a folder for services

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddOData(opt => opt.Select().Filter().Expand().OrderBy().Count().SetMaxTop(100)
    .AddRouteComponents("odata", GetEdmModel(), new DefaultODataBatchHandler()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Mapping
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Register your service


var configuration = builder.Configuration;
builder.Services.AddDbContext<ProjectPRN231Context>(options =>
    options.UseSqlServer(configuration.GetConnectionString("MyDatabase")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

    builder.EntitySet<Subject>("Subjects");


    return builder.GetEdmModel();
}
