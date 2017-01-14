using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Additio.Configuration
{
    [DebuggerDisplay("{RelativePath}")]
    public class Node
    {
        private string Root { get; }

        public Node(string root)
        {
            if (!root.EndsWith("\\"))
                root += "\\";
            Root = root;
        }

        public string FilePath { get; set; }

        public string RelativePath => FilePath.Replace(Root, "");

        public List<Node> Dependencies { get; } = new List<Node>();
    }
}
