
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using OpenAI;
using System.ClientModel;
using VectorStoreRAG.Configuration;
using VectorStoreRAG.Configuration.Options;
using VectorStoreRAG.Models;
using VectorStoreRAG.Services;

var builder = Host.CreateApplicationBuilder();
builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<RagConfig>(builder.Configuration.GetSection(RagConfig.ConfigSectionName));
var appConfig = new ApplicationConfig(builder.Configuration);


var kernelBuilder = builder.Services.AddKernel();

// 聊天完成服务
switch (appConfig.RagConfig.AIChatService)
{
	case "OpenAI":
		kernelBuilder.AddOpenAIChatCompletion(
				appConfig.OpenAiConfig.ModelId,
				appConfig.OpenAiConfig.ApiKey,
				httpClient: new HttpClient
				{
					BaseAddress = new Uri(appConfig.OpenAiConfig.Endpoint)
				}
			);
		break;
}

// 文本向量模型
switch (appConfig.RagConfig.AIEmbeddingService)
{
	case "OpenAiEmbeddings":
		builder.Services.AddSingleton<IEmbeddingGenerator>(options =>
			new OpenAIClient(new ApiKeyCredential(appConfig.OpenAiEmbeddingsConfig.ApiKey), new OpenAIClientOptions
			{
				Endpoint = new Uri(appConfig.OpenAiEmbeddingsConfig.Endpoint)
			})
			.GetEmbeddingClient(appConfig.OpenAiEmbeddingsConfig.ModelId)
			.AsIEmbeddingGenerator(1536)
		);
		break;
}

// 向量数据库存储
switch (appConfig.RagConfig.VectorStoreType)
{
	case "Qdrant":
		kernelBuilder.Services.AddQdrantCollection<Guid, TextSnippet<Guid>>(
				appConfig.RagConfig.CollectionName,
				appConfig.QdrantConfig.Host,
				appConfig.QdrantConfig.Port,
				appConfig.QdrantConfig.Https,
				appConfig.QdrantConfig.ApiKey
			);
		break;
}

// 相关服务注册
kernelBuilder.AddVectorStoreTextSearch<TextSnippet<Guid>>();
builder.Services.AddSingleton<IDataLoader, DataLoader>();
builder.Services.AddHostedService<RAGChatService>();

var host = builder.Build();
await host.RunAsync().ConfigureAwait(false);

