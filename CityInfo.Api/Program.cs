using CityInfo.Api.Services;
using Microsoft.AspNetCore.StaticFiles;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers(options => { options.ReturnHttpNotAcceptable = true; })
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();
builder.Services.AddProblemDetails();
/*
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.Add("additionalInfo", "Additional info example");
        context.ProblemDetails.Extensions.Add("server", Environment.MachineName);
    };
});
*/
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddTransient<LocalMailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoins => { endpoins.MapControllers(); });

app.Run();