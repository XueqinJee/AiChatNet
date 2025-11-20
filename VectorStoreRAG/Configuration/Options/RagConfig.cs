using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorStoreRAG.Configuration.Options;

public class RagConfig
{
    public const string ConfigSectionName = "Rag";

    [Required]
    public string AIChatService { get; set; } = string.Empty;

    [Required]
    public string AIEmbeddingService { get; set; } = string.Empty;

    [Required]
    public string CollectionName { get; set; } = string.Empty;

    [Required]
    public int DataLoadingBatchSize { get; set; } = 2;

    [Required]
    public string[]? PdfFilePaths { get; set; }

    [Required]
    public string VectorStoreType { get; set; } = string.Empty;
}
