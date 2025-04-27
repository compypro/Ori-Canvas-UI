using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColliderData
{
    public string type = "Polygon";
    public Vector2 position = Vector2.zero;
    public List<Vector2> points = new List<Vector2>();
    public Color color = Color.black;
    public bool goThrough = false;
}
