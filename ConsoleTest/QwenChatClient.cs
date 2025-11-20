using Microsoft.Extensions.AI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleTest
{
    public class QwenChatClient : IChatClient
    {
        private string url;
        private string _model;
        private string _key;

        private readonly HttpClient httpClient = new HttpClient();

        public QwenChatClient(string url, string model, string key)
        {
            this.url = url;
            this._model = model;
            _key = key;
        }

        public object? GetService(Type serviceType, object? serviceKey = null) => this;

        public TService? GetService<TService>(object? key = null) where TService : class => this as TService;


        public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            var content = new {
                model = _model,
                messages = messages.Select(x => new { role = x.Role.Value, content = x.Text }).ToList()
            };
            var result = await SendPostRequestAsync(url + "/compatible-mode/v1/chat/completions", JsonSerializer.Serialize(content), _key);
            
            var jObject = JObject.Parse(result);
            
            return new ChatResponse(new ChatMessage(ChatRole.Assistant, jObject.Value<JArray>("choices")[0].Value<JObject>("message").Value<string>("content")));
        }

        private async Task<string> SendPostRequestAsync(string url, string jsonContent, string apiKey)
        {
            using (var content = new StringContent(jsonContent, Encoding.UTF8, "application/json"))
            {
                // 设置请求头
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // 发送请求并获取响应
                HttpResponseMessage response = await httpClient.PostAsync(url, content);

                // 处理响应
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return $"请求失败: {response.StatusCode}," + result;
                }
            }
        }


        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}
