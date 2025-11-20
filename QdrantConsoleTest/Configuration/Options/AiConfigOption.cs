using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QdrantConsoleTest.Configuration;

public class AiConfigOption
{
    public string? Key { get; set; }
    public string? BaseUrl { get; set; }
    public string[]? Models { get; set; }
    public string[]? Embeddings { get; set; }
}
