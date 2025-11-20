using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QdrantConsoleTest;

public class Hotel
{
    [VectorStoreKey]
    public ulong HotelId { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public string? HotelName { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public string? Description { get; set; }

    [VectorStoreVector(Dimensions: 1536, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
    public ReadOnlyMemory<float> DescriptionEmbeding { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public string[]? Tags { get; set; }
}
