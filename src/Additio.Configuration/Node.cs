using System;
using System.Collections.Generic;

namespace Additio.Configuration
{
    public class Node
    {
        public const string IncludeFolderPath = @"App_Config\Include";

        public string FilePath { get; set; }

        public string RelativePath => FilePath.Remove(0, FilePath.IndexOf(IncludeFolderPath, StringComparison.OrdinalIgnoreCase) + 19);

        public List<Node> Dependencies { get; } = new List<Node>();
    }
}
