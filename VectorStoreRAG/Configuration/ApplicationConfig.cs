using Microsoft.Extensions.Configuration;
using VectorStoreRAG.Configuration.Options;

namespace VectorStoreRAG.Configuration;

public class ApplicationConfig
{
    private readonly OpenAiConfig _openAiConfig = new OpenAiConfig();
    private readonly OpenAiEmbeddingsConfig _openAiEmbeddingsConfig = new OpenAiEmbeddingsConfig();
    private readonly RagConfig _ragConfig = new RagConfig();
    private readonly QdrantConfig _qdrantConfig = new QdrantConfig();

    public ApplicationConfig(ConfigurationManager configurationManager)
    {
        configurationManager
            .GetRequiredSection($"AIService:{OpenAiConfig.ConfigSectionName}")
            .Bind(this._openAiConfig);
        configurationManager
            .GetRequiredSection($"AIService:{OpenAiEmbeddingsConfig.ConfigSectionName}")
            .Bind(this._openAiEmbeddingsConfig);
        configurationManager
            .GetRequiredSection(RagConfig.ConfigSectionName)
            .Bind(this._ragConfig);
        configurationManager
            .GetRequiredSection($"VectorStores:{QdrantConfig.ConfigSectionName}")
            .Bind(this._qdrantConfig);
    }

    public OpenAiConfig OpenAiConfig => this._openAiConfig;
    public OpenAiEmbeddingsConfig OpenAiEmbeddingsConfig => this._openAiEmbeddingsConfig;
    public RagConfig RagConfig => this._ragConfig;
    public QdrantConfig QdrantConfig => this._qdrantConfig;
}
