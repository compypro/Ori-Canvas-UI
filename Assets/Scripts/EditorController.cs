using UnityEngine;
using TMPro;

public class EditorController : MonoBehaviour
{
    public TMP_Text positionText;

    private void LateUpdate()
    {
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionText.text = string.Format("({0:0.00},{1:0.00})", cursorPosition.x, cursorPosition.y);
    }
}
