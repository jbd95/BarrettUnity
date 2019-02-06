using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class xGridTemplateMove : MonoBehaviour, IDragHandler {

    public GridResize.Side side;
    GameController controller;

    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<GameController>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(Input.mousePosition.x, transform.position.y, transform.position.z);
        gameObject.GetComponentInParent<GridResize>().Resize(side);
        controller.UpdateStartValues();
    }

    public void Move(float _xpos)
    {
        transform.position = new Vector3(_xpos, transform.position.y, 0);
        gameObject.GetComponentInParent<GridResize>().Resize(side);
        controller.UpdateStartValues();
    }

    public void Adjust(float _xpos)
    {
        transform.localPosition = new Vector3(_xpos, transform.localPosition.y, 0);
        gameObject.GetComponentInParent<GridResize>().Resize(side);
    }
}
