using System;
using System.Collections.Generic;
using UnityEngine;

public class Polygon : Element
{
    public ColliderData data = new ColliderData();

    private Mesh polygonMesh;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        UpdatePolygon();
    }

    public void UpdatePolygon()
    {
        PolygonCollider2D collider = GetComponent<PolygonCollider2D>();
        collider.SetPath(0, data.points);

        polygonMesh = new Mesh();

        Vector3[] vertices = new Vector3[data.points.Count];
        Color[] colors = new Color[data.points.Count];

        for (int i = 0; i < data.points.Count; i++)
        {
            vertices[i] = new Vector3(data.points[i].x, data.points[i].y, -1f);
            colors[i] = data.color;
        }

        Triangulator triangulator = new Triangulator(data.points);
        int[] triangles = triangulator.Triangulate();

        polygonMesh.vertices = vertices;
        polygonMesh.triangles = triangles;
        polygonMesh.colors = colors;

        polygonMesh.RecalculateNormals();
        polygonMesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = polygonMesh;
    }

    public override void CreateHandles()
    {
        base.CreateHandles();

        for(int i = 0; i < data.points.Count; ++i)
        {
            GameObject handle = GameObject.Instantiate<GameObject>(EditorController.Instance.handlePrefab, handleHolder);
            handle.transform.localPosition = new Vector3(data.points[i].x, data.points[i].y, EditorController.Instance.handleZ);
            handles.Add(handle);
        }
    }

    public override void SetHandlePosition(GameObject handle, Vector2 position)
    {
        base.SetHandlePosition(handle, position);

        for(int i = 0; i < data.points.Count; ++i)
        {
            if (handles[i] == handle)
            {
                data.points[i] = handle.transform.localPosition;

                UpdatePolygon();
                break;
            }
        }
    }

    public override string ToJson()
    {
        return JsonUtility.ToJson(data);
    }

    public override void FromJson(string json)
    {
        data = JsonUtility.FromJson<ColliderData>(json);
    }

    public class Triangulator
    {
        private List<Vector2> points;

        public Triangulator(List<Vector2> points)
        {
            this.points = points;
        }

        public int[] Triangulate()
        {
            List<int> indices = new List<int>();

            int n = points.Count;
            if (n < 3)
                return indices.ToArray();

            int[] V = new int[n];
            if (Area() > 0)
            {
                for (int v = 0; v < n; v++) V[v] = v;
            }
            else
            {
                for (int v = 0; v < n; v++) V[v] = (n - 1) - v;
            }

            int nv = n;
            int count = 2 * nv;
            for (int m = 0, v = nv - 1; nv > 2;)
            {
                if ((count--) <= 0)
                    return indices.ToArray();

                int u = v;
                if (nv <= u) u = 0;
                v = u + 1;
                if (nv <= v) v = 0;
                int w = v + 1;
                if (nv <= w) w = 0;

                if (Snip(u, v, w, nv, V))
                {
                    int a, b, c, s, t;
                    a = V[u];
                    b = V[v];
                    c = V[w];
                    indices.Add(a);
                    indices.Add(b);
                    indices.Add(c);
                    m++;
                    for (s = v, t = v + 1; t < nv; s++, t++) V[s] = V[t];
                    nv--;
                    count = 2 * nv;
                }
            }

            indices.Reverse();
            return indices.ToArray();
        }

        private float Area()
        {
            int n = points.Count;
            float A = 0.0f;
            for (int p = n - 1, q = 0; q < n; p = q++)
            {
                Vector2 pval = points[p];
                Vector2 qval = points[q];
                A += pval.x * qval.y - qval.x * pval.y;
            }
            return A * 0.5f;
        }

        private bool Snip(int u, int v, int w, int n, int[] V)
        {
            int p;
            Vector2 A = points[V[u]];
            Vector2 B = points[V[v]];
            Vector2 C = points[V[w]];
            if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
                return false;
            for (p = 0; p < n; p++)
            {
                if ((p == u) || (p == v) || (p == w)) continue;
                Vector2 P = points[V[p]];
                if (InsideTriangle(A, B, C, P)) return false;
            }
            return true;
        }

        private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
        {
            float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
            float cCROSSap, bCROSScp, aCROSSbp;

            ax = C.x - B.x; ay = C.y - B.y;
            bx = A.x - C.x; by = A.y - C.y;
            cx = B.x - A.x; cy = B.y - A.y;
            apx = P.x - A.x; apy = P.y - A.y;
            bpx = P.x - B.x; bpy = P.y - B.y;
            cpx = P.x - C.x; cpy = P.y - C.y;

            aCROSSbp = ax * bpy - ay * bpx;
            cCROSSap = cx * apy - cy * apx;
            bCROSScp = bx * cpy - by * cpx;

            return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
        }
    }
}
