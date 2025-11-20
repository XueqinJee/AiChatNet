
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Newtonsoft.Json.Linq;
using QdrantConsoleTest;
using QdrantConsoleTest.Configuration;

var services = new ServiceCollection();

#region 配置
IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsetting.json")
    .Build();

services.Configure<AiConfigOption>(config.GetSection("AiConfig:Ali"));

var vectorStore = new QdrantVectorStore(
        new Qdrant.Client.QdrantClient(config.GetValue<string>("Qdrant")!),
        ownsClient: true);
var collection = vectorStore.GetCollection<ulong, Hotel>("skhotels");
await collection.EnsureCollectionExistsAsync();
services.AddSingleton(collection);

services.AddScoped<AiSemanticKernel>();
services.AddScoped<QdrantTestService>();
#endregion

var provider = services.BuildServiceProvider();
var qdrantTest = provider.GetService<QdrantTestService>();
//var hotelContent = File.ReadAllText("info.txt");

//var jAry = JArray.Parse(hotelContent);
//foreach (var item in jAry)
//{
//    Console.WriteLine(item["标题"]);
//    await qdrantTest.AddHotelData(item.Value<string>("标题"), item.Value<string>("内容"));
//}
//Console.WriteLine(hotelContent);

var serch = await qdrantTest.GetHotelsAsync("地处苏州市姑苏区平江路68号，江南水乡风格，木质装修搭配灯笼，“姑苏春”用青梅酿酒，配松鼠鳜鱼");
Console.WriteLine(serch.Count);