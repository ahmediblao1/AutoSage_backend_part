using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Server.Data;
using Server.Startup;

var builder = WebApplication.CreateBuilder(args);

// Using startup file
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services); 

// Removing request size limit
builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuring sql server Connection string
builder.Services.AddDbContext<MyDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionString")));

var app = builder.Build();

// configuring app using startup
startup.Configure(app, builder.Environment); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // using swagger
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage(); // the default exception page in the dev environment 
}

// configuring static files in the project
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
    RequestPath = new PathString("/wwwroot")
});

// Mapping HTTP requests to controller actions
app.MapControllers();

app.Run();
