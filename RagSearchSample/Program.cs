
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using OpenAI;
using Qdrant.Client;
using RagSearchSample;
using System.ClientModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

var embeddingService = new OpenAIClient(new ApiKeyCredential("sk-68e8956297be4d6b903a3394f89032e3"), new OpenAIClientOptions
{
    Endpoint = new Uri("https://dashscope.aliyuncs.com/compatible-mode/v1")
})
    .GetEmbeddingClient("text-embedding-v4")
    .AsIEmbeddingGenerator();

var qdrantClient = new QdrantClient("115.190.187.71");
var vectorStore = new QdrantVectorStore(qdrantClient, ownsClient: true);
var collection = vectorStore.GetCollection<Guid, Model>("finances");
//await collection.EnsureCollectionDeletedAsync();
await collection.EnsureCollectionExistsAsync();

// 插入向量数据
async Task<ReadOnlyMemory<float>> GenerateTextEmbedding(string content)
{
    var embdding = await embeddingService.GenerateVectorAsync(content, new EmbeddingGenerationOptions
    {
        Dimensions = 768
    });
    return embdding;
}

var model = new Model
{
    Key = Guid.NewGuid(),
    Title = "矢量搜索选项",
    Description = "VectorProperty 选项可用于指定要在搜索期间定位的向量属性。 如果未提供，并且数据模型仅包含一个向量，将使用该向量。 如果数据模型不包含矢量或包含多个矢量，并且未提供 VectorProperty，则搜索方法将抛出。",
};
//model.Text = await GenerateTextEmbedding(model.Description);
// 插入
//await collection.UpsertAsync(model);

var searchVector = await GenerateTextEmbedding("aras plm？");

var vectorSearchOptions = new VectorSearchOptions<Model>
{
    // 指定搜索向量
    VectorProperty = r => r.Text,
    // 过滤条件
    Filter = r => r.Title == "aras plm",
    // 是否包含向量数据
    IncludeVectors = true
};

var hybridSeachOption = new HybridSearchOptions<Model>
{
    VectorProperty = r => r.Text,
    AdditionalProperty = r => r.Description
};
var search = collection.HybridSearchAsync(searchVector, ["aras", "skhotels"], top: 20, hybridSeachOption);
await foreach (var item in search)
{
    Console.WriteLine($"Description: {item.Record.Description}, \nScore: {item.Score}\n\n");
}

