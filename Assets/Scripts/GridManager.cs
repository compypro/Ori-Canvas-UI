using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public Vector2 cellSize = new Vector2(5f, 4f);
    public GameObject gridLinePrefab;

    public GameObject gridLineHolder;
    private List<LineRenderer> gridLines = new List<LineRenderer>();

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < gridLineHolder.transform.childCount; ++i)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void Start()
    {
        UpdateGrid();
    }

    public void UpdateGrid()
    {
        Vector3 cameraPosition = Camera.main.transform.position;

        float cameraHeight = Camera.main.orthographicSize * 2f;
        float cameraWidth = cameraHeight * Screen.currentResolution.width / Screen.currentResolution.height;

        List<Vector3> points = new List<Vector3>();

        float x = (int)((cameraPosition.x - cameraWidth - cellSize.x) / cellSize.x) * cellSize.x;
        while (x <= cameraPosition.x + cameraWidth + cellSize.x)
        {
            points.Add(new Vector3(x, cameraPosition.y - cameraHeight, 0f));
            points.Add(new Vector3(x, cameraPosition.y + cameraHeight, 0f));

            x += cellSize.x;
        }

        float y = (int)((cameraPosition.y - cameraHeight - cellSize.y) / cellSize.y) * cellSize.y;
        while (y <= cameraPosition.y + cameraHeight + cellSize.y)
        {
            points.Add(new Vector3(cameraPosition.x - cameraWidth, y));
            points.Add(new Vector3(cameraPosition.x + cameraWidth, y));

            y += cellSize.y;
        }

        int numLines = points.Count / 2;

        while (gridLines.Count > numLines)
        {
            Destroy(gridLines[gridLines.Count - 1].gameObject);
            gridLines.RemoveAt(gridLines.Count - 1);
        }

        while (gridLines.Count < numLines)
        {
            gridLines.Add(GameObject.Instantiate<GameObject>(gridLinePrefab, gridLineHolder.transform).GetComponent<LineRenderer>());
        }

        for (int i = 0; i < numLines; ++i)
        {
            LineRenderer gridLine = gridLines[i];
            gridLine.SetPositions(new Vector3[] { points[i * 2], points[i * 2 + 1] });
            gridLine.startWidth = gridLine.endWidth = Camera.main.orthographicSize * 0.005f;
        }
    }
}
