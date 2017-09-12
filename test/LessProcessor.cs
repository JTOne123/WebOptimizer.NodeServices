using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebOptimizer.NodeServices.Test
{
    public class LessProcessor : NodeProcessor
    {
        public override string Name => "LESS_test";

        public override async Task ExecuteAsync(IAssetContext context)
        {
            var env = (IHostingEnvironment)context.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment));
            var fileProvider = context.Asset.GetFileProvider(env);
            var content = new Dictionary<string, byte[]>();

            if (!EnsureNodeFiles("WebOptimizer.NodeServices.Test.less_files.zip"))
                return;

            string module = Path.Combine(InstallDirectory, "less");

            foreach (string route in context.Content.Keys)
            {
                var file = fileProvider.GetFileInfo(route);
                var input = context.Content[route].AsString();
                var result = await NodeServices.InvokeAsync<string>(module, input, file.PhysicalPath);

                content[route] = result.AsByteArray();
            }

            context.Content = content;
        }
    }
}
