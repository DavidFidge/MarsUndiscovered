using NGenerics.DataStructures.General;

namespace MarsUndiscovered.Game.Extensions
{
    public static class NGenericsExtensions
    {
        public static Vertex<T> GetOrAddVertex<T>(this Graph<T> graph, T item)
        {
            var vertex = graph.GetVertex(item);

            if (vertex == null)
            {
                vertex = new Vertex<T>(item);
                graph.AddVertex(vertex);
            }

            return vertex;
        }
        
        public static List<Vertex<T>> NeighboringVertices<T>(this Vertex<T> vertex)
        {
            return vertex.EmanatingEdges
                .Select(e => e.ToVertex).Union(vertex.EmanatingEdges.Select(e => e.FromVertex))
                .Where(e => !e.Equals(vertex))
                .ToList();
        }
    }
}
