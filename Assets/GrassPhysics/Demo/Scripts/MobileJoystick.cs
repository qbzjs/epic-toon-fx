using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[System.Serializable]
public class UnityEvent_Vector2 : UnityEvent<Vector2> { }

public class MobileJoystick : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public float radius = 1f;

    public UnityEvent_Vector2 onDragEvent;

    private Vector3 startPos;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = rectTransform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {

        rectTransform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
        if(Vector3.Distance(rectTransform.position, startPos) > radius)
        {
            rectTransform.position = Vector3.Normalize(rectTransform.position - startPos) * radius + startPos;
        }
        Vector2 result = (rectTransform.position - startPos) / radius;
        onDragEvent.Invoke(result);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onDragEvent.Invoke(Vector2.zero);
        rectTransform.position = startPos;
    }
}
