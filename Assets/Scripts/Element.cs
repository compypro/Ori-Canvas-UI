using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    protected Transform handleHolder;
    public List<GameObject> handles { protected set; get; } = new List<GameObject>();

    protected virtual void Awake()
    {
        handleHolder = transform.Find("HandleHolder");
        RemoveHandles();

        EditorController.Instance.AddElement(this);
    }

    public virtual void CreateHandles()
    {
    }

    public void RemoveHandles()
    {
        for (int i = 0; i < handleHolder.childCount; i++)
        {
            GameObject.Destroy(handleHolder.GetChild(i).gameObject);
        }

        handles.Clear();
    }

    public virtual void SetHandlePosition(GameObject handle, Vector2 position)
    {
        handle.transform.position = new Vector3(position.x, position.y, handle.transform.position.z);
    }

    public virtual string ToJson()
    {
        return null;
    }

    public virtual void FromJson(string json)
    {
    }
}
