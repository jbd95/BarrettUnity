using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridResize : MonoBehaviour
{
    public Button Left;
    public Button Right;
    public Button Top;
    public Button Bottom;
    public Button Center;

    public Button TopLeft;
    public Button TopRight;
    public Button BottomLeft;
    public Button BottomRight;

    public void Resize(float xstart, float ystart, float xend, float yend)
    {
        Left.GetComponent<xGridTemplateMove>().Adjust(xstart);
        Right.GetComponent<xGridTemplateMove>().Adjust(xend);
        Bottom.GetComponent<yGridTemplateMove>().Adjust(ystart);
        Top.GetComponent<yGridTemplateMove>().Adjust(yend);
    }


    public void Resize(Side side)
    {
        Vector3 position = Center.transform.localPosition;
        switch(side)
        {
            case Side.Left:
            case Side.Right:
                Center.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (System.Math.Abs(Right.transform.position.x - Left.transform.position.x)));
                position.x = (Right.transform.localPosition.x + Left.transform.localPosition.x) / 2;
                Top.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (System.Math.Abs(Right.transform.position.x - Left.transform.position.x)));
                Top.transform.localPosition = new Vector3(position.x, Top.transform.localPosition.y, 0);
                Bottom.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (System.Math.Abs(Right.transform.position.x - Left.transform.position.x)));
                Bottom.transform.localPosition = new Vector3(position.x, Bottom.transform.localPosition.y, 0);
                break;
            case Side.Top:
            case Side.Bottom:
                Center.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (System.Math.Abs(Top.transform.position.y - Bottom.transform.position.y)));
                position.y = (Top.transform.localPosition.y + Bottom.transform.localPosition.y) / 2;
                Left.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (System.Math.Abs(Top.transform.position.y - Bottom.transform.position.y)));
                Left.transform.localPosition = new Vector3(Left.transform.localPosition.x, position.y, 0);
                Right.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (System.Math.Abs(Top.transform.position.y - Bottom.transform.position.y)));
                Right.transform.localPosition = new Vector3(Right.transform.localPosition.x, position.y, 0);
                break;

            case Side.Center:

                break;
        }
        Center.transform.localPosition = position;
        UpdateCornerPositions();
    }

    void UpdateCornerPositions()
    {
        TopLeft.transform.localPosition = new Vector3(Left.transform.localPosition.x, Top.transform.localPosition.y, 0);
        TopRight.transform.localPosition = new Vector3(Right.transform.localPosition.x, Top.transform.localPosition.y, 0);
        BottomLeft.transform.localPosition = new Vector3(Left.transform.localPosition.x, Bottom.transform.localPosition.y, 0);
        BottomRight.transform.localPosition = new Vector3(Right.transform.localPosition.x, Bottom.transform.localPosition.y, 0);
    }

    public enum Side
    {
        Left,
        Right,
        Top,
        Bottom,
        Center
    }

    public enum Corner
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

}
