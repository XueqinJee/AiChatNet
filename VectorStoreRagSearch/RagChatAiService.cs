using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorStoreRagSearch;

internal class RagChatAiService(
        Kernel _kernel,
        IChatCompletionService _chatCompletionService
    ) : IHostedService
{
    Task? _ragTask = null;
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ragTask = RagChatMessage(cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _ragTask?.Dispose();
        return Task.CompletedTask;
    }

    public async Task RagChatMessage(CancellationToken cancellationToken)
    {
        Console.WriteLine("Assistant：我是一个可爱的小助理，我可以帮助你解答问题，有问必回。");
        while (true) {
            Console.Write("User：");
            var input = Console.ReadLine();
            
            var response = _chatCompletionService.GetStreamingChatMessageContentsAsync(input!);
            Console.Write("Assistant：");
            await foreach (var item in response)
            {
                Console.Write(item.Content);
            }
            Console.Write('\n');
        }
    }
}
