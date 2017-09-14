using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace WebOptimizer.NodeServices
{
    /// <summary>
    /// Compiles TypeScript/ES6 files
    /// </summary>
    /// <seealso cref="IProcessor" />
    public abstract class NodeProcessor : Processor
    {
        private static INodeServices _nodeServices;
        private static object _syncRoot = new object();
        private const string _folderName = "WebOptimizer.Node";

        /// <summary>
        /// Gets the <see cref="INodeServices"/> instance.
        /// </summary>
        public INodeServices NodeServices
        {
            get
            {
                if (_nodeServices == null)
                {
                    lock (_syncRoot)
                    {
                        if (_nodeServices == null)
                        {
                            CreateNodeServicesInstance();
                        }
                    }
                }

                return _nodeServices;
            }
        }

        /// <summary>
        /// Gets the directory where the node modules are installed.
        /// </summary>
        public string InstallDirectory
        {
            get { return Path.Combine(WorkingDirectory, Name); }
        }

        /// <summary>
        /// The name of the processor.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the directory of the node modules.
        /// </summary>
        public string WorkingDirectory { get; } = Path.Combine(Path.GetTempPath(), _folderName);

        /// <summary>
        /// Executes the processor on the specified configuration.
        /// </summary>
        public override abstract Task ExecuteAsync(IAssetContext context);

        /// <summary>
        /// Extracts the zip file into the right location on disk
        /// </summary>
        protected bool EnsureNodeFiles(string zipfilePath)
        {
            string node_modules = Path.Combine(InstallDirectory, "node_modules");

            if (Directory.Exists(node_modules))
            {
                return true;
            }

            lock (_syncRoot)
            {
                if (!Directory.Exists(node_modules))
                {
                    try
                    {
                        if (Directory.Exists(InstallDirectory))
                        {
                            Directory.Delete(InstallDirectory, true);
                        }

                        var assembly = GetType().Assembly;

                        using (var resourceStream = assembly.GetManifestResourceStream(zipfilePath))
                        using (var zip = new ZipArchive(resourceStream))
                        {
                            zip.ExtractToDirectory(InstallDirectory);
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void CreateNodeServicesInstance()
        {
            var services = new ServiceCollection();

            services.AddNodeServices(options =>
            {
                options.ProjectPath = WorkingDirectory;
                options.WatchFileExtensions = new string[0];
            });

            var serviceProvider = services.BuildServiceProvider();
            _nodeServices = serviceProvider.GetRequiredService<INodeServices>();
        }
    }
}
