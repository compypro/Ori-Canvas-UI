using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool dragging = false;
    private Vector3 dragOrigin;
    private Vector3 dragDiff;

    public float zoomPower = 1f;
    public float minZoom = 2f;
    public float maxZoom = 30f;

    private void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            dragDiff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;

            if (!dragging)
            {
                dragging = true;
                dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            dragging = false;
        }

        if (dragging)
        {
            Camera.main.transform.position = dragOrigin - dragDiff;
        }

        Camera.main.orthographicSize = Mathf.Max(minZoom, Mathf.Min(Camera.main.orthographicSize - Input.mouseScrollDelta.y * zoomPower, maxZoom));
    }
}
