using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDrag : MonoBehaviour, IDragHandler {

    public Vector2 MoveDifference;
    public void OnDrag(PointerEventData eventData)
    {
        MoveDifference = ((eventData.pressPosition - eventData.position) / 10);
    }
}
