using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace FuncApp
{
    public static class OnBlobUploadResizeImage
    {
        [FunctionName("OnBlobUploadResizeImage")]
        public static void Run([BlobTrigger("functionsalesrep/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob,
            [Blob("functionsalesrep-sm/{name}", FileAccess.Write)] Stream myBlobOutput,
            string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            using Image<Rgba32> input = Image.Load<Rgba32>(myBlob);
            IImageFormat format = input.Metadata.DecodedImageFormat;
            input.Mutate(x => x.Resize(300, 200));
            input.Save(myBlobOutput, format);
        }
    }
}
