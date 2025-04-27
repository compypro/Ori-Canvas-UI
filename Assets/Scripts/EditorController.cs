using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class EditorController : Singleton<EditorController>
{
    private InputAction pressAction;
    private InputAction pointAction;

    public bool dragging { private set; get; } = false;
    private Vector2 dragOrigin;

    public float selectDistance = 0.2f;
    private Element selectedElement = null;
    private List<Element> elements = new List<Element>();

    public GameObject handlePrefab;
    public float handleZ = -10f;
    private GameObject selectedHandle = null;
    private Vector2 handleOriPos;

    public float gridSnap = 1f;

    public TMP_Text positionText;

    private void Start()
    {
        pressAction = InputSystem.actions.FindAction("Click");
        pointAction = InputSystem.actions.FindAction("Point");
    }

    private void Update()
    {
        Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(pointAction.ReadValue<Vector2>());

        if (pressAction.WasPressedThisFrame())
        {
            dragOrigin = cursorPosition;

            if (selectedElement != null)
            {
                selectedHandle = null;
                float currDistanceSq = selectDistance * selectDistance;

                foreach (GameObject handle in selectedElement.handles)
                {
                    Collider2D collider2D = handle.GetComponent<Collider2D>();
                    if (collider2D != null)
                    {
                        float distanceSq = (collider2D.ClosestPoint(cursorPosition) - cursorPosition).sqrMagnitude;

                        if (distanceSq < currDistanceSq)
                        {
                            currDistanceSq = distanceSq;
                            selectedHandle = handle;
                        }
                    }
                }

                if(selectedHandle != null)
                {
                    Debug.Log("Selected handle " + selectedHandle.name);
                    handleOriPos = selectedHandle.transform.position;
                }
            }
        }

        if (pressAction.WasReleasedThisFrame())
        {
            if(!CameraController.Instance.dragging && !dragging)
            {
                Element prevSelectedElement = selectedElement;
                selectedElement = null;
                float currDistanceSq = selectDistance * selectDistance;

                foreach (Element element in elements)
                {
                    Collider2D collider2D = element.GetComponent<Collider2D>();
                    if (collider2D != null)
                    {
                        float distanceSq = (collider2D.ClosestPoint(cursorPosition) - cursorPosition).sqrMagnitude;

                        if (distanceSq < currDistanceSq)
                        {
                            currDistanceSq = distanceSq;
                            selectedElement = element;
                        }
                    }
                }

                if (selectedElement != null)
                {
                    if (prevSelectedElement != selectedElement)
                    {
                        selectedElement.CreateHandles();
                        Debug.Log("Selected " + selectedElement.name + " at distance " + currDistanceSq);
                    }
                }
                else if (prevSelectedElement != null)
                {
                    prevSelectedElement.RemoveHandles();
                }
            }
        }

        if (pressAction.IsPressed())
        {
            if (selectedHandle != null)
            {
                if (!dragging)
                {
                    dragging = true;
                }
            }
        }
        else
        {
            selectedHandle = null;
            dragging = false;
        }

        if (selectedElement && selectedHandle != null && dragging)
        {
            Vector2 newPos = handleOriPos + cursorPosition - dragOrigin;

            if (gridSnap > 0f)
                newPos = new Vector2(Mathf.Round(newPos.x / gridSnap) * gridSnap, Mathf.Round(newPos.y / gridSnap) * gridSnap);

            selectedElement.SetHandlePosition(selectedHandle, newPos);
        }
    }

    private void LateUpdate()
    {
        Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(pointAction.ReadValue<Vector2>());

        positionText.text = string.Format("({0:0.00},{1:0.00})", cursorPosition.x, cursorPosition.y);
    }

    public void AddElement(Element element)
    {
        if(!elements.Contains(element))
        {
            elements.Add(element);
        }
    }

    public void SelectElement(Element element)
    {
        this.selectedElement = element;
    }
}
