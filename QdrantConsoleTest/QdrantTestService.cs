using Microsoft.SemanticKernel.Connectors.Qdrant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QdrantConsoleTest;

internal class QdrantTestService
{
    private readonly AiSemanticKernel _kernel;
    private readonly QdrantCollection<ulong, Hotel> qdrantCollection;
    public QdrantTestService(AiSemanticKernel kernel, QdrantCollection<ulong, Hotel> qdrantCollection)
    {
        _kernel = kernel;
        this.qdrantCollection = qdrantCollection;
    }

    public async Task AddHotelData(string title ,string content, string[]? tags = null)
    {
        var id = DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Microsecond;
        await qdrantCollection.UpsertAsync(new Hotel
        {
            HotelId = ulong.Parse(id),
            Description = content,
            DescriptionEmbeding = await _kernel.GenerateEmbedVector(content),
            HotelName = title,
            Tags = tags
        });
    }

    public async Task<List<Hotel>> GetHotelsAsync(string content)
    {
        var vector = await _kernel.GenerateEmbedVector(content);
        var result = qdrantCollection.SearchAsync(vector, top: 15);


        List<Hotel> hotels = new List<Hotel>();
        await foreach (var hotel in result) {
            Console.WriteLine(hotel.Score + "-" + hotel.Record.Description);
            hotels.Add(hotel.Record);
        }
        return hotels;
    }
}
