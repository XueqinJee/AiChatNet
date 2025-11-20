using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using VectorStoreRAG.Configuration.Options;
using VectorStoreRAG.Models;
#pragma warning disable SKEXP0001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
namespace VectorStoreRAG.Services;

public class RAGChatService(
    IDataLoader dataLoader,
    Kernel kernel,
    VectorStoreTextSearch<TextSnippet<Guid>> vectorStoreTextSearch,
    IOptions<RagConfig> ragConfigOptions) : IHostedService
{
    private Task? _dataLoaded;
    private Task? _chatLoop;


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 加载数据
        // await LoadDataAsync(cancellationToken);
        await this.ChatLoopAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        while(this._dataLoaded != null && !this._dataLoaded.IsCompleted && !cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("等待PDF 文件加载。。。。");
            await Task.Delay(1_000, cancellationToken);
        }

        foreach (var pdfFile in ragConfigOptions.Value.PdfFilePaths ?? [])
        {
            Console.WriteLine($"加载PDF路径：{pdfFile}");
            await dataLoader.LoadPdf(pdfFile, 5, 100, cancellationToken);
        }
    }

    private async Task ChatLoopAsync(CancellationToken cancellationToken)
    {
        // 测试聊天服务
        var pdfFiles = string.Join(",", ragConfigOptions.Value.PdfFilePaths ?? []);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("助理 > 按下回车可提示");

        kernel.Plugins.Add(vectorStoreTextSearch.CreateWithGetTextSearchResults("SearchPlugin"));
        // Start the chat loop.
        while (!cancellationToken.IsCancellationRequested)
        {
            // Prompt the user for a question.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Assistant > What would you like to know from the loaded PDFs: ({pdfFiles})?");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("User > ");
            var question = Console.ReadLine();

            // Exit the application if the user didn't type anything.
            if (string.IsNullOrWhiteSpace(question))
            {
                break;
            }

            var response = kernel.InvokePromptStreamingAsync(
                promptTemplate: """
                    Please use this information to answer the question:
                    {{#with (SearchPlugin-GetTextSearchResults question)}}  
                      {{#each this}}  
                        Name: {{Name}}
                        Value: {{Value}}
                        Link: {{Link}}
                        -----------------
                      {{/each}}
                    {{/with}}

                    Include citations to the relevant information where it is referenced in the response.
                    
                    Question: {{question}}
                    """,
                arguments: new KernelArguments()
                {
                    { "question", question },
                },
                templateFormat: "handlebars",
                promptTemplateFactory: new HandlebarsPromptTemplateFactory(),
                cancellationToken: cancellationToken);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\nAssistant > ");

            try
            {
                await foreach (var message in response.ConfigureAwait(false))
                {
                    Console.Write(message);
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Call to LLM failed with error: {ex}");
            }
        }
    }
}
