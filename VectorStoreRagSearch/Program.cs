using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using VectorStoreRagSearch;
using VectorStoreRagSearch.Options;
#pragma warning disable SKEXP0010
var builder = Host.CreateApplicationBuilder();
var configure = builder.Configuration;
configure.SetBasePath(Directory.GetCurrentDirectory());
configure.AddJsonFile("appsettings.json");
var regConfig = configure.Get<RagConfigOption>()!;

builder.Services.Configure<RagConfigOption>(configure);
var kernel = builder.Services.AddKernel()
    .AddOpenAIChatCompletion(
        regConfig.ModelId!,
        regConfig.Key!,
        httpClient: new HttpClient()
        {
            BaseAddress = new Uri(regConfig.Endpoint!)
        }
    )
    .AddOpenAIEmbeddingGenerator(regConfig.EmbeddingId!, regConfig.Key!, httpClient: new HttpClient()
    {
        BaseAddress = new Uri(regConfig.Endpoint!)
    });

builder.Services.AddHostedService<RagChatAiService>();
builder.Services.AddSingleton<LoadPdfData>();
var host = builder.Build();

await host.RunAsync();