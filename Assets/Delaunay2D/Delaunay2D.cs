using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Triangle
{
    public Vector2 V0;
    public Vector2 V1;
    public Vector2 V2;
    public bool IsBad;
    public Edge E0;
    public Edge E1;
    public Edge E2;

    public Triangle(Vector2 v0, Vector2 v1, Vector2 v2 )
    {
        V0 = v0;
        V1 = v1;
        V2 = v2;
        E0 = new Edge(v0, v1);
        E1 = new Edge(v1, v2);
        E2 = new Edge(v2, v0);
        IsBad = false;
    }

    public bool circumCircleContains(Vector2 p)
    {
        float ab = V0.sqrMagnitude;
        float cd = V1.sqrMagnitude;
        float ef = V2.sqrMagnitude;
        float circumX = (ab*(V2.y - V1.y) + cd*(V0.y - V2.y) + ef*(V1.y - V0.y))/
                          (V0.x*(V2.y - V1.y) + V1.x*(V0.y - V2.y) + V2.x*(V1.y - V0.y));
        float circumY = (ab*(V2.x - V1.x) + cd*(V0.x - V2.x) + ef*(V1.x - V0.x))/
                          (V0.y*(V2.x - V1.x) + V1.y*(V0.x - V2.x) + V2.y*(V1.x - V0.x));
        Vector2 circum = new Vector2(circumX / 2, circumY / 2);
        float circumRadius = (V0 - circum).sqrMagnitude;
        float dist = (p - circum).sqrMagnitude;
        return dist <= circumRadius;
    }

    public bool ContainsVertex(Vector2 p)
    {
        return Mathf.Approximately(0, Vector2.Distance(p, V0)) ||
               Mathf.Approximately(0, Vector2.Distance(p, V1)) ||
               Mathf.Approximately(0, Vector2.Distance(p, V2));
    }
}

public struct Edge
{
    public Vector2 V0;
    public Vector2 V1;
    public bool IsBad;

    public Edge(Vector2 v0, Vector2 v1)
    {
        V0 = v0;
        V1 = v1;
        IsBad = false;
    }

    public bool AlmostEqual(Edge e)
    {
        return (Mathf.Approximately(0, Vector2.Distance(V0, e.V0)) && Mathf.Approximately(0, Vector2.Distance(V1, e.V1))) ||
               (Mathf.Approximately(0, Vector2.Distance(V0, e.V1)) && Mathf.Approximately(0, Vector2.Distance(V1, e.V0)));
    }
}

public class Delaunay2D {
    public List<Triangle> Triangulate(List<Vector2> vertices)
    {
        List<Triangle> triangles = new List<Triangle>();
        if (0 == vertices.Count)
        {
            return triangles;
        }
        float minX = vertices[0].x;
        float minY = vertices[0].y;
        float maxX = minX;
        float maxY = minY;
        for (int i = 0, imax = vertices.Count; i < imax; i++)
        {
            float x = vertices[i].x;
            float y = vertices[i].y;
            if (x < minX)
            {
                minX = x;
            }
            else if (x > maxX)
            {
                maxX = x;
            }
            if (y < minY)
            {
                minY = y;
            }
            else if (y > maxY)
            {
                maxY = y;
            }
        }
        float dx = maxX - minX;
        float dy = maxY - minY;
        float deltaMax = Mathf.Max(dx, dy);
        float midX = (minX + maxX)/2;
        float midY = (minY + maxY)/2;
        Vector2 p1 = new Vector2(midX - 20 * deltaMax, midY - deltaMax);
        Vector2 p2 = new Vector2(midX, midY + 20 * deltaMax);
        Vector2 p3 = new Vector2(midX + 20 * deltaMax, midY - deltaMax);

        triangles.Add(new Triangle(p1, p2, p3));
        for (int i = 0, imax = vertices.Count; i < imax; i++)
        {
            Vector2 p = vertices[i];
            List<Edge> polygon = new List<Edge>();
            for (int j = 0, jmax = triangles.Count; j < jmax; j++)
            {
                Triangle t = triangles[j];
                if (t.circumCircleContains(p))
                {
                    t.IsBad = true;
                    polygon.Add(t.E0);
                    polygon.Add(t.E1);
                    polygon.Add(t.E2);
                    triangles[j] = t;
                }
            }
            triangles.RemoveAll((t) => t.IsBad);
            for (int j = 0, jmax = polygon.Count; j < jmax; j++)
            {
                Edge e = polygon[j];
                for (int k = j + 1; k < jmax; k++)
                {
                    Edge e2 = polygon[k];
                    if (e.AlmostEqual(e2))
                    {
                        e.IsBad = true;
                        e2.IsBad = true;
                        polygon[j] = e;
                        polygon[k] = e2;
                    }
                }
            }

            polygon.RemoveAll((e) => e.IsBad);
            foreach (Edge e in polygon)
            {
                triangles.Add(new Triangle(e.V0, e.V1, p));
            }
        }
        triangles.RemoveAll((t) => t.ContainsVertex(p1) || t.ContainsVertex(p2) || t.ContainsVertex(p3));
        return triangles;
    } 
}
