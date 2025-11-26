using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using VectorStoreRagSearch.Options;

namespace VectorStoreRagSearch;

public class LoadPdfData(
        IOptionsSnapshot<RagConfigOption> _ragConfigOption,
        IChatCompletionService _chatCompletionService,
        Kernel _kernel
    )
{
    public Task LoadPdfDataTask()
    {
        var filePaths = _ragConfigOption.Value.Files;
        return Task.CompletedTask;
    }


    public async Task<string> ChatImageToTest(byte[] image)
    {

        return "";
    }
}
