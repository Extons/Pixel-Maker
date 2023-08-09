using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Springy
{
    public class Point
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public float Mass;
    }

    public struct Edge
    {
        public Point From;
        public Point To;
        public float Distance;
    }

    public class Spring : MonoBehaviour
    {
        #region Serialized members

        [SerializeField, Required]
        private int _edgesCount = default;

        [SerializeField, Required]
        private float _distance = default;

        [SerializeField, Required]
        private float _mass = default;

        [SerializeField, Required]
        private GameObject _graphPrefab = default;

        #endregion Serialized members

        #region Private members

        [ShowInInspector]
        private List<Edge> _edges = null;
        private List<Edge> Edge => _edges = _edges ?? new List<Edge>();

        private Dictionary<Point, GameObject> _graph = null;
        private Dictionary<Point, GameObject> Graph => _graph = _graph ?? new Dictionary<Point, GameObject>();

        #endregion Private members

        #region Lifecycle

        private void FixedUpdate()
        {
            Simulate();

            foreach(var edge in Edge)
            {
                Graph[edge.From].transform.position = edge.From.Position;
            }
        }

        #endregion Lifecycle

        #region Private methods

        private void Simulate()
        {
            for (int i = 0, length = Edge.Count; i < length; i++)
            {
                var edge = Edge[i];

                UpdatePointPosition(edge.From, Time.fixedDeltaTime, out var fromPos);
                UpdatePointPosition(edge.To, Time.fixedDeltaTime, out var toPos);
               // UpdateEdgeDistance(edge, out var additif);

                edge.From.Position = fromPos;
                edge.To.Position = toPos /*+ additif*/;
            }
        }

        private void UpdatePointPosition(Point point, float deltaTime, out Vector3 newPosition)
        {
            newPosition = point.Position + (point.Velocity * point.Mass * deltaTime);
        }

        private bool UpdateEdgeDistance(Edge edge, out Vector3 additif)
        {
            Vector3 diff = edge.To.Position - edge.From.Position;
            float distance = diff.magnitude;

            if (distance == _distance)
            {
                additif = Vector3.zero;
                return false;
            }

            float extra = Mathf.Abs(_distance - distance);
            additif = diff.normalized * extra;
            return true;
        }

        [Button("Generate String")]
        private void Generate()
        {
            Edge.Clear();

            var from = new Point()
            {
                Position = Vector3.zero,
                Velocity = Vector3.zero,
                Mass = _mass
            };

            for (int i = 0, length = _edgesCount - 1; i < length; i++)
            {
                var position = Vector3.down * (i + 1);

                var to = new Point()
                {
                    Position = position,
                    Velocity = Vector3.zero,
                    Mass = _mass
                };

                var edge = new Edge()
                {
                    From = from,
                    To = to,
                    Distance = _distance
                };

                Edge.Add(edge);

                from = to;
            }

            GenerateGraph();
        }

        private void GenerateGraph()
        {
            foreach(var edge in _edges)
            {
                if(Graph.TryGetValue(edge.From, out var graph))
                {
                    DestroyImmediate(graph);
                    Graph[edge.From] = Instantiate(_graphPrefab);
                }
                else
                {
                    Graph.Add(edge.From, Instantiate(_graphPrefab));
                }
            }
        }

        #endregion Private methods
    }
}
