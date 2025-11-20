using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using VectorStoreRAG.Models;

namespace VectorStoreRAG.Services;

public class DataLoader(
       IChatCompletionService chatCompletionService,
       VectorStoreCollection<Guid, TextSnippet<Guid>> vectorStoreCollection
    ): IDataLoader
{
    public async Task LoadPdf(string pdfPath, int batchSize, int betweenBatchDelayInMs, CancellationToken cancellationToken)
    {
        await vectorStoreCollection.EnsureCollectionExistsAsync(cancellationToken);

        var sections = LoadTextAndImages(pdfPath, cancellationToken);
        var batches = sections.Chunk(batchSize);

        foreach (var batch in batches)
        {
            var textContentTasks = batch.Select(async content => {
                if (content.Text != null) return content;

                var textFromImage = await ConvertImageToTextWithRetryAsync(content.Image!.Value, cancellationToken).ConfigureAwait(false);
                return new RawContent { Text = textFromImage, PageNumber = content.PageNumber };
            });
            var textContent = await Task.WhenAll(textContentTasks).ConfigureAwait(false);

            var records = textContent.Select(content =>
            {
                return new TextSnippet<Guid>
                {
                    Key = Guid.NewGuid(),
                    Text = content.Text,
                    ReferenceRescription = $"{new FileInfo(pdfPath).Name}#page={content.PageNumber}",
                    ReferenceLink = $"{new Uri(new FileInfo(pdfPath).FullName).AbsoluteUri}#page={content.PageNumber}"
                };
            });

            await vectorStoreCollection.UpsertAsync(records, cancellationToken);
            await Task.Delay(betweenBatchDelayInMs, cancellationToken);
        }
    }

    private IEnumerable<RawContent> LoadTextAndImages(string pdfPath, CancellationToken cancellationToken)
    {
        using var document = PdfDocument.Open(pdfPath);
        foreach (var page in document.GetPages())
        {
            if (cancellationToken.IsCancellationRequested) break;
            foreach (var image in page.GetImages())
            {
                if(image.TryGetPng(out var png))
                {
                    yield return new RawContent { Image = png, PageNumber = page.Number };
                }
                else
                {
                    Console.WriteLine($"不支持的图片格式，页码：{page.Number}");
                }
            }

            var blocks = DefaultPageSegmenter.Instance.GetBlocks(page.GetWords());
            foreach (var item in blocks)
            {
                if (cancellationToken.IsCancellationRequested) break;
                yield return new RawContent { Text = item.Text, PageNumber = page.Number };
            }
        }
    }

    public async Task<string> ConvertImageToTextWithRetryAsync(ReadOnlyMemory<byte> imageBytes, CancellationToken cancellationToken)
    {
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage([
                    new TextContent("介绍这张图片"),
                    new ImageContent(imageBytes, "image/png"),
            ]);
        var result = await chatCompletionService.GetChatMessageContentsAsync(chatHistory);
        return string.Join("\n", result.Select(x => x.Content));
    }

    private sealed class RawContent
    {
        public string? Text { get; init; }
        public ReadOnlyMemory<byte>? Image { get; init; }
        public int PageNumber { get; init; }
    }
}
