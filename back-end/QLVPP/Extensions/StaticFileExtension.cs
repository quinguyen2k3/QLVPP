using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace QLVPP.Extensions
{
    public static class StaticFileExtensions
    {
        public static IApplicationBuilder UseConfiguredStaticFiles(
            this IApplicationBuilder app,
            IConfiguration configuration,
            string sectionName = "StaticFiles"
        )
        {
            var section = configuration.GetSection(sectionName);
            var physicalPath = section.GetValue<string>("BasePath");
            var requestPath = section.GetValue<string>("RequestPath");

            if (string.IsNullOrEmpty(physicalPath) || string.IsNullOrEmpty(requestPath))
            {
                throw new System.Exception(
                    $"Section '{sectionName}' not configured fully with PhysicalPath or RequestPath."
                );
            }

            if (!Directory.Exists(physicalPath))
            {
                Directory.CreateDirectory(physicalPath);
            }

            app.UseStaticFiles(
                new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(physicalPath),
                    RequestPath = requestPath,
                }
            );

            return app;
        }
    }
}
