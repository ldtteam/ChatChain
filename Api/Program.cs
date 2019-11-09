using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NSwag;
using NSwag.CodeGeneration.CSharp;

namespace Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.ElementAtOrDefault(0) == "generate")
            {
                GenerateClientStuffAsync(args.ElementAtOrDefault(1));
            }
            else
            {
                CreateWebHostBuilder(args).Build().Run();
            }
        }
        
        private static void GenerateClientStuffAsync(string swaggerJson)
        {
            OpenApiDocument document =
                OpenApiDocument.FromUrlAsync(swaggerJson ?? "http://localhost:5001/swagger/v1/swagger.json").Result;

            CSharpClientGeneratorSettings settings = new CSharpClientGeneratorSettings
            {
                ClassName = "IdentityServerClient",
                CSharpGeneratorSettings =
                {
                    Namespace = "Api.Api"
                }
            };

            CSharpClientGenerator generator = new CSharpClientGenerator(document, settings);
            string code = generator.GenerateFile();

            using (StreamWriter outputFile = new StreamWriter(Path.Combine("Api", "IdentityServerClient.cs")))
            {
                outputFile.Write(code);
            }
        }    

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}