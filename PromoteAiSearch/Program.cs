
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Microsoft.SemanticKernel.PromptTemplates.Liquid;

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(
        modelId: "qwen3-max",
        apiKey: "sk-68e8956297be4d6b903a3394f89032e3",
        httpClient: new HttpClient
        {
            BaseAddress = new Uri("https://dashscope.aliyuncs.com/compatible-mode/v1/")
        }
    )
    .Build();

var templateFactory = new LiquidPromptTemplateFactory()
{
    AllowDangerouslySetContent = true
};


var arguments = new KernelArguments()
{
    {"customer", new {
        firstName = "小明"
    } },
    {
        "history", new []
        {
            new { role = "user", content = "这是一个消息"}
        }
    }
};

var config = new PromptTemplateConfig()
{
    Template = File.ReadAllText("Liquid.yaml"),
    TemplateFormat = "liquid",
    Name = "Liquid",
    AllowDangerouslySetContent = true
};
var promptTemplate = templateFactory.Create(config);
// 模板渲染
var render = await promptTemplate.RenderAsync(kernel, arguments);
Console.WriteLine(render);

// AI调用
var function = kernel.CreateFunctionFromPromptYaml(File.ReadAllText("Liquid.yaml"), templateFactory);
var response = await kernel.InvokeAsync(function, arguments);
Console.WriteLine(response);
