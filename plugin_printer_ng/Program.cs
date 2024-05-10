
using Microsoft.Extensions.Options;
using plugin_printer_ng.Controllers;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
//Configure Services
// Add services to the container.

var key = "ArrendamientoGCCApiv1Tibs!*";
CultureInfo.CurrentCulture = new CultureInfo("en-US");
//builder.Configuration.GetSection(nameof(GCCDatabaseSetting));

builder.Services.AddSingleton<TicketsController>();
builder.Services.AddCors();





builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllers();
// Register the Swagger generator, defening 1 or more Swgager documents
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Ng Solutions",
        Description = "api carga",
        
    });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



//Configure 
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ng Solutions");
    c.RoutePrefix = "swagger";
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseDeveloperExceptionPage();


}
//app.UseHttpsRedirection();


app.UseRouting();
app.MapControllers();

app.UseCors(x => x
.AllowAnyMethod()
.AllowAnyHeader()
.SetIsOriginAllowed(origin => true)
.AllowCredentials());

//app.MapControllers();

app.Run();
