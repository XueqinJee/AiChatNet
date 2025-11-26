
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using PluginsRagSample;

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(
        "qwen3-max",
        "sk-68e8956297be4d6b903a3394f89032e3",
        httpClient: new HttpClient
        {
            BaseAddress = new Uri("https://dashscope.aliyuncs.com/compatible-mode/v1/")
        })
    .Build();

kernel.Plugins.AddFromType<LightPlugin>("Lights");
// 获取聊天完成服务
var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();

ChatHistory history = new ChatHistory();
history.Add(new ChatMessageContent(AuthorRole.System, "你是小黑助理，你是个腹黑的老猫，说的每一句话充满智慧，你是一个圣母，你的结果充满肯定，不需重复计算，你的回到是最正确的，回答要简洁。"));

// 流式聊天完成
while (true)
{
    Console.Write("User > ");
    var input = Console.ReadLine();
    history.Add(new ChatMessageContent(AuthorRole.User, input));

    var response = chatCompletion.GetStreamingChatMessageContentsAsync(history, new PromptExecutionSettings
    {
        // 自动选择函数
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    }, kernel: kernel);
    var content = "";
    Console.Write("Assisnant > ");
    await foreach (var item in response)
    {
        Console.Write(item.Content);
        content += item.Content;
    }
    history.Add(new ChatMessageContent(AuthorRole.Assistant, content));
    Console.Write("\n");
}