
using ConsoleTest;
using ConsoleTest.Plugins;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var builder = Kernel.CreateBuilder();
builder.Services.AddLogging(service => service.AddConsole().SetMinimumLevel(LogLevel.Trace));

// 模型密钥信息
builder.AddOpenAIChatCompletion(
    "qwen3-max",
    "sk-68e8956297be4d6b903a3394f89032e3",
    httpClient: new HttpClient
    {
        BaseAddress = new Uri("https://dashscope.aliyuncs.com/compatible-mode/v1")
    });



var kernel = builder.Build();
// 获取聊天服务
var chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();
// 添加插件
kernel.Plugins.AddFromType<LightsPlugin>("Lights");
kernel.Plugins.AddFromType<DateTimePlugin>("DateTime");
kernel.Plugins.AddFromType<WeatherPlugin>("Weather");

// 规划
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var chatReducer = new ChatHistorySummarizationReducer(chatCompletionService, 2);

// 历史记录
var history = new ChatHistory();

string? userInput;
do
{
    Console.Write("User >：");
    userInput = Console.ReadLine();
    history.AddUserMessage(userInput!);

    var reduceMessage = await chatReducer.ReduceAsync(history);
    if(reduceMessage is not null)
    {
        history = new ChatHistory(reduceMessage);
    }

    // 流式响应
    var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
        chatHistory: history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    Console.WriteLine("Assistant > ");
    var responseContent = string.Empty;
    await foreach (var chatMessage in result)
    {
        responseContent += chatMessage.ToString();
        Console.Write(chatMessage.ToString());
    }
    history.Add(new ChatMessageContent { Role = AuthorRole.Assistant, Content = responseContent});
    Console.Write("\n");
    
} while (userInput is not null);


