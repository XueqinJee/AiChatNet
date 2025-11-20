using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorStoreRAG.Models;

public sealed class TextSnippet<TKey>
{
    [VectorStoreKey]
    public required TKey Key { get; set; }

    [TextSearchResultValue]
    [VectorStoreData]
    public string? Text { get; set; }

    [TextSearchResultName]
    [VectorStoreData]
    public string? ReferenceRescription { get; set; }

    [TextSearchResultLink]
    [VectorStoreData]
    public string? ReferenceLink { get; set; }

    [VectorStoreVector(1536)]
    public string? TextEmbedding => this.Text;
}
