using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using QdrantConsoleTest.Configuration;
using Microsoft.Extensions.Options;

namespace QdrantConsoleTest;
#pragma warning disable SKEXP0010 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。

public class AiSemanticKernel
{
    public Kernel? _kernel;
    private readonly AiConfigOption aiConfigOption;
    public AiSemanticKernel(IOptionsSnapshot<AiConfigOption> optionsSnapshot)
    {
        aiConfigOption = optionsSnapshot.Value;
        Initialized();
    }

    public void Initialized()
    {
        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddOpenAIChatCompletion(
                aiConfigOption.Models[0],
                aiConfigOption.Key,
                httpClient: new HttpClient
                {
                    BaseAddress = new Uri("https://dashscope.aliyuncs.com/compatible-mode/v1")
                }
            );

        kernelBuilder.AddOpenAIEmbeddingGenerator(
                modelId: aiConfigOption.Embeddings[0],
                apiKey: aiConfigOption.Key,
                orgId: "",
                serviceId: "",
                httpClient: new HttpClient
                {
                    BaseAddress = new Uri("https://dashscope.aliyuncs.com/compatible-mode/v1")
                },
                dimensions: 1536
            );
        _kernel = kernelBuilder.Build();
    }


    public async Task<ReadOnlyMemory<float>> GenerateEmbedVector(string text)
    {
        OpenAITextEmbeddingGenerationService textEmbeddingGenerationService = new(
            modelId: aiConfigOption.Embeddings[0],          // Name of the embedding model, e.g. "text-embedding-ada-002".
            apiKey: aiConfigOption.Key,
            organization: "",  // Optional organization id.
            httpClient: new HttpClient()
            {
                BaseAddress = new Uri("https://dashscope.aliyuncs.com/compatible-mode/v1")
            }, // Optional; if not provided, the HttpClient from the kernel will be used
            dimensions: 1536              // Optional number of dimensions to generate embeddings with.
        );
        var result = await textEmbeddingGenerationService.GenerateEmbeddingAsync(text);
        return result;
    }
}
