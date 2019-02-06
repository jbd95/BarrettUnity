using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridCorner : MonoBehaviour, IDragHandler
{
    public GridResize.Corner corner;

    public Button Left;
    public Button Right;
    public Button Top;
    public Button Bottom;
    public Button Center;

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);

        switch(corner)
        {
            case GridResize.Corner.TopLeft:
                Left.GetComponent<xGridTemplateMove>().Move(Input.mousePosition.x);
                Top.GetComponent<yGridTemplateMove>().Move(Input.mousePosition.y);
                break;
            case GridResize.Corner.TopRight:
                Right.GetComponent<xGridTemplateMove>().Move(Input.mousePosition.x);
                Top.GetComponent<yGridTemplateMove>().Move(Input.mousePosition.y);
                break;
            case GridResize.Corner.BottomLeft:
                Left.GetComponent<xGridTemplateMove>().Move(Input.mousePosition.x);
                Bottom.GetComponent<yGridTemplateMove>().Move(Input.mousePosition.y);
                break;
            case GridResize.Corner.BottomRight:
                Right.GetComponent<xGridTemplateMove>().Move(Input.mousePosition.x);
                Bottom.GetComponent<yGridTemplateMove>().Move(Input.mousePosition.y);
                break;
        }
    }
}
