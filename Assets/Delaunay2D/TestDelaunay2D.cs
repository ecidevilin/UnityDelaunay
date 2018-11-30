using System.Collections.Generic;
using UnityEngine;

public class TestDelaunay2D : MonoBehaviour {
    public Material mat;
    List<Vector2> points = new List<Vector2>();

    void Awake()
    {
        points.Add(new Vector2(0, 0));
        points.Add(new Vector2(1, 0));
        points.Add(new Vector2(0.5f, 1));
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mp = Input.mousePosition;
            points.Add(new Vector2(mp.x / Screen.width, mp.y / Screen.height));
        }
    }

    void OnPostRender()
    {
        if (!mat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }
        Delaunay2D d = new Delaunay2D();
        List<Triangle> triangles = d.Triangulate(points);
        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.TRIANGLES);
        for (int i = 0, imax = triangles.Count; i < imax; i++)
        {
            GL.Vertex(triangles[i].V0);
            GL.Vertex(triangles[i].V1);
            GL.Vertex(triangles[i].V2);
        }
        GL.End();
        GL.PopMatrix();
    }
}
