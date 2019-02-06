using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class yGridTemplateMove : MonoBehaviour, IDragHandler
{
    public GridResize.Side side;
    GameController controller;

    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<GameController>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(transform.position.x, Input.mousePosition.y, transform.position.z);
        gameObject.GetComponentInParent<GridResize>().Resize(side);
        controller.UpdateStartValues();
    }

    public void Move(float _ypos)
    {
        transform.position = new Vector3(transform.position.x, _ypos, 0);
        gameObject.GetComponentInParent<GridResize>().Resize(side);
        controller.UpdateStartValues();
    }

    public void Adjust(float _ypos)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, _ypos, 0);
        gameObject.GetComponentInParent<GridResize>().Resize(side);
    }
}