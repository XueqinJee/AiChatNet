using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorStoreRagSearch;

internal class Model
{
    [VectorStoreKey]
    public Guid Key { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public string? Title { get; set; }

    [VectorStoreData]
    public string? ReferencesLink { get; set; }

    [VectorStoreData]
    public string? PageNumber { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public string? Description { get; set; }

    [VectorStoreVector(Dimensions: 768)]
    public ReadOnlyMemory<float> TextEmbedding { get; set; }
}
