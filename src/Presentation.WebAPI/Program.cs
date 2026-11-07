using System.Text.Json.Serialization;
using Core.Application.UseCases;
using Core.Domain.Interfaces;
using Core.Domain.Services;
using Infrastructure.AI.Adapters;
using Presentation.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSingleton<PrerequisiteValidator>();
builder.Services.AddSingleton<ITranscriptParserGateway, GeminiTranscriptParserAdapter>();
builder.Services.AddSingleton<InMemoryAuditLogger>();
builder.Services.AddSingleton<IAuditLogger>(services => services.GetRequiredService<InMemoryAuditLogger>());
builder.Services.AddSingleton<InMemoryOverrideRequestStore>();
builder.Services.AddSingleton<InMemoryNoticeOutbox>();
builder.Services.AddScoped<ProcessOverrideRequestUseCase>();
builder.Services.AddScoped<SubmitAdvisorDecisionUseCase>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapGet("/", (IWebHostEnvironment environment) =>
{
    var webRootPath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
    var publishedPage = Path.Combine(webRootPath, "index.html");
    var sourcePage = Path.GetFullPath(Path.Combine(environment.ContentRootPath, "../../document-agent.html"));
    var page = File.Exists(publishedPage) ? publishedPage : sourcePage;
    return Results.File(page, "text/html");
});
app.MapControllers();

app.Run();

public partial class Program;