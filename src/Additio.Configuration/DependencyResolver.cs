using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Additio.Configuration
{
    public interface IDependencyResolver
    {
        IList<Node> GetSortedFiles(IList<string> configFiles);
    }

    public class DependencyResolver : IDependencyResolver
    {
        protected IDependencyLoader DependencyLoader { get; }

        public DependencyResolver() : this(new DependencyLoader())
        {
        }

        public DependencyResolver(IDependencyLoader dependencyLoader)
        {
            DependencyLoader = dependencyLoader;
        }

        public virtual IList<Node> GetSortedFiles(IList<string> configFiles)
        {
            var graph = configFiles.Select(x => new Node {FilePath = x}).ToList() as IList<Node>;

            MapDependencies(graph);

            return GetSortedGraph(graph);
        }

        protected virtual void MapDependencies(IList<Node> graph)
        {
            foreach (var node in graph)
            {
                var dependencyPatterns = DependencyLoader.GetDependencyPatterns(node.FilePath);
                foreach (var pattern in dependencyPatterns)
                {
                    var matchingNodes = graph.Where(x => IsWildcardMatch(x.RelativePath, pattern));
                    foreach (var match in matchingNodes)
                    {
                        // Skip self
                        if (match == node) continue;

                        node.Dependencies.Add(match);
                    }
                }
            }
        }

        protected virtual IList<Node> GetSortedGraph(IList<Node> graph)
        {
            // Nodes without dependencies should retain their original order
            var list = graph.Where(x => !x.Dependencies.Any()).ToList();

            // Only sort nodes with dependencies
            var stack = new Stack<Node>(graph.Where(x => x.Dependencies.Any()));

            var marked = new HashSet<Node>();
            var visited = new HashSet<Node>();

            while (stack.Any())
            {
                VisitNode(stack.Pop(), marked, visited, list);
            }

            return list;
        }

        protected virtual void VisitNode(Node node, HashSet<Node> markedNodes, HashSet<Node> visitedNodes, IList<Node> list)
        {
            // If currently marked then we have a circular dependency
            if (markedNodes.Contains(node))
                throw CircularDependencyException(node, markedNodes);

            // We don't need to visit dependencies for already visited nodes
            if (visitedNodes.Contains(node))
                return;

            // Mark node to track circular dependencies
            markedNodes.Add(node);

            foreach (var dependency in node.Dependencies)
            {
                VisitNode(dependency, markedNodes, visitedNodes, list);
            }

            markedNodes.Remove(node);
            visitedNodes.Add(node);
            list.Add(node);
        }

        protected virtual bool IsWildcardMatch(string input, string wildcardPattern)
        {
            var regexPattern = Regex.Escape(wildcardPattern).Replace("\\*", ".*").Replace("\\?", ".");
            return Regex.IsMatch(input, $"^{regexPattern}$", RegexOptions.IgnoreCase);
        }

        protected virtual Exception CircularDependencyException(Node node, HashSet<Node> markedNodes)
        {
            var chain = markedNodes.ToList().Concat(new[] { node }).Select(x => x.RelativePath);
            var prefix = Environment.NewLine + "\t→ ";
            return new Exception("Uh oh! There's a circular dependency between the following config files: " + string.Join(prefix, chain));
        }
    }
}
