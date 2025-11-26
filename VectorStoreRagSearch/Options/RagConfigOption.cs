namespace VectorStoreRagSearch.Options;

public class RagConfigOption
{
    public string? Key { get; set; }
    public string? ModelId { get; set; }
    public string? Endpoint { get; set; }
    public string? EmbeddingId { get; set; }
    public string[]? Files { get; set; }
}