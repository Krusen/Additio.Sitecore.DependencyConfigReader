using System;
using System.IO;
using System.Linq;
using System.Text;
using Sitecore.Configuration;
using Sitecore.Diagnostics;

namespace Additio.Configuration
{
    public class DependencyConfigReader : ConfigReader
    {
        protected IDependencyResolver DependencyResolver { get; set; }

        public DependencyConfigReader() : this(new DependencyResolver())
        {
        }

        public DependencyConfigReader(IDependencyResolver dependencyResolver)
        {
            DependencyResolver = dependencyResolver;
        }

        protected override void LoadAutoIncludeFiles(ConfigPatcher patcher, string folder)
        {
            Assert.ArgumentNotNull(patcher, nameof(patcher));
            Assert.ArgumentNotNull(folder, nameof(folder));

            try
            {
                if (!Directory.Exists(folder))
                    return;

                var configFiles = Directory.GetFiles(folder, "*.config", SearchOption.AllDirectories).ToList();
                var sortedFiles = DependencyResolver.GetSortedFiles(configFiles);

                foreach (var node in sortedFiles)
                {
                    try
                    {
                        using (var reader = new StreamReader(node.FilePath, Encoding.UTF8))
                        {
                            // Add patch source with relative path, not just file name
                            patcher.ApplyPatch(reader, node.RelativePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Could not load configuration file: " + node.FilePath + ": " + ex, typeof(DependencyConfigReader));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Could not scan configuration folder " + folder + " for files: " + ex, typeof(DependencyConfigReader));
            }
        }
    }
}
