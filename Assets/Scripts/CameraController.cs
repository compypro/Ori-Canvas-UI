using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : Singleton<CameraController>
{
    private InputAction pressAction;
    private InputAction pointAction;
    private InputAction scrollAction;

    public bool dragging { private set; get; } = false;
    private Vector3 dragOrigin;
    private Vector3 dragDiff;

    public float zoomPower = 1f;
    public float minZoom = 2f;
    public float maxZoom = 30f;

    private void Start()
    {
        pressAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");
        scrollAction = InputSystem.actions.FindAction("ScrollWheel");
    }

    private void LateUpdate()
    {
        if(pressAction.WasPressedThisFrame())
        {
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(pointAction.ReadValue<Vector2>());
            dragOrigin = cursorPosition;
        }

        if (pressAction.IsPressed())
        {
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(pointAction.ReadValue<Vector2>());
            dragDiff = cursorPosition - Camera.main.transform.position;

            if (!dragging && !EditorController.Instance.dragging && Mathf.Abs((dragOrigin - Camera.main.transform.position - dragDiff).sqrMagnitude) >= 0.01f)
            {
                dragging = true;
            }
        }
        else
        {
            dragging = false;
        }

        if (dragging)
        {
            Camera.main.transform.position = dragOrigin - dragDiff;
            GridManager.Instance.UpdateGrid();
        }

        float newOrthographicSize = Mathf.Max(minZoom, Mathf.Min(Camera.main.orthographicSize - scrollAction.ReadValue<Vector2>().y * zoomPower, maxZoom));
        if(newOrthographicSize != Camera.main.orthographicSize)
        {
            Camera.main.orthographicSize = newOrthographicSize;
            GridManager.Instance.UpdateGrid();
        }
    }
}
