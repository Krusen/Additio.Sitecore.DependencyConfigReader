using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Additio.Configuration
{
    public interface IDependencyLoader
    {
        IEnumerable<string> GetDependencyPatterns(string configFile);
    }

    public class DependencyLoader : IDependencyLoader
    {
        protected const string DependencyAttributeName = "dependencies";

        protected static readonly char[] Separators = {',', ';', '|'};

        public virtual IEnumerable<string> GetDependencyPatterns(string configFile)
        {
            var value = GetDependencyAttributeValue(configFile);

            if (string.IsNullOrWhiteSpace(value))
                return Enumerable.Empty<string>();

            value = value.Replace('/', '\\').Replace(".config", "");

            return value.Split(Separators, StringSplitOptions.RemoveEmptyEntries).Select(x => x.TrimStart('\\'));
        }

        protected virtual string GetDependencyAttributeValue(string configFile)
        {
            var attributes = XElement.Load(configFile).Attributes();

            var attribute =
                attributes.FirstOrDefault(
                    x => string.Equals(x.Name.ToString(), DependencyAttributeName, StringComparison.OrdinalIgnoreCase));

            return attribute?.Value;
        }
    }
}
