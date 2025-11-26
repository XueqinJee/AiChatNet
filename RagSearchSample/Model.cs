using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.TextGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagSearchSample;

internal class Model
{
    [VectorStoreKey]
    public Guid Key { get; set; }

    [VectorStoreData]
    public string? Title { get; set; }

    [VectorStoreData(IsFullTextIndexed = true)]
    public string? Description { get; set; }

    [VectorStoreData]
    public string? ReferencesLink { get; set; }

    [VectorStoreVector(Dimensions: 768)]
    public ReadOnlyMemory<float> Text { get; set; }
}
