using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NSwag;
using NSwag.CodeGeneration.CSharp;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.ElementAtOrDefault(0) == "generate")
                GenerateClientStuffAsync(args.ElementAtOrDefault(1));
            else
                CreateWebHostBuilder(args).Build().Run();
        }

        private static void GenerateClientStuffAsync(string swaggerJson)
        {
            OpenApiDocument document =
                OpenApiDocument.FromUrlAsync(swaggerJson ?? "http://localhost:5004/swagger/v1/swagger.json").Result;

            CSharpClientGeneratorSettings settings = new CSharpClientGeneratorSettings
            {
                ClassName = "ApiClient",
                CSharpGeneratorSettings =
                {
                    Namespace = "WebApp.Api"
                }
            };

            CSharpClientGenerator generator = new CSharpClientGenerator(document, settings);
            string code = generator.GenerateFile();

            using (StreamWriter outputFile = new StreamWriter(Path.Combine("Api", "ApiClient.cs")))
            {
                outputFile.Write(code);
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}