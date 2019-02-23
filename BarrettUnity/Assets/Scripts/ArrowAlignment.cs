using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowAlignment : MonoBehaviour
{

    /*public Button ArrowTemplate;
    public Button ArrowEnd1;
    public Button ArrowEnd2;
    public Vector3 StartPosition;
    Manager manager;
    public GameObject User;
    float ArrowTemplateLength;
    ColorBlock ArrowVisibleColor;
    ColorBlock ArrowInvisibleColor;
    ColorBlock ArrowEndColor;
    public bool IsVisible = false;
    public Vector3 HomePosition;

    // Use this for initialization
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
        ArrowTemplateLength = ArrowTemplate.GetComponent<RectTransform>().rect.width;
        //ArrowVisibleColor = ArrowTemplate.colors;
        //ArrowInvisibleColor = ArrowTemplate.colors;
        //ArrowEndColor = ArrowEnd1.colors;
        //ArrowInvisibleColor.disabledColor = new Color(0, 0, 0, 0);
        //ArrowTemplate.gameObject.SetActive(true);
        Hide();
    }

    public void SetHome()
    {
        HomePosition = Vector3.zero;
    }
    public void Align()
    {
        //Vector3 UserPosition = manager.UIContainer.transform.InverseTransformPoint(manager.MainCamera.WorldToScreenPoint(User.transform.position));
        Vector3 UserPosition = User.transform.position;
        Vector3 CurrentPosition = StartPosition;
        float xdistance = UserPosition.x - StartPosition.x;
        float ydistance = UserPosition.y - StartPosition.y;
        float angle = Mathf.Atan2(ydistance, xdistance);

        float distance = Mathf.Sqrt(Mathf.Pow(xdistance, 2) + Mathf.Pow(ydistance, 2));

        ArrowTemplate.transform.SetParent(manager.UIContainer.transform);
        ArrowTemplate.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, distance);
        ArrowTemplate.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);

        CurrentPosition.x += (distance / 2) * Mathf.Cos(angle);
        CurrentPosition.y += (distance / 2) * Mathf.Sin(angle);

        ArrowTemplate.transform.localPosition = CurrentPosition;


    }
    public void Show()
    {
        ArrowTemplate.colors = ArrowVisibleColor;
        ArrowEnd1.colors = ArrowEndColor;
        ArrowEnd2.colors = ArrowEndColor;
        IsVisible = true;
        ArrowTemplate.gameObject.SetActive(true);
    }
    public void Hide()
    {
        ArrowTemplate.colors = ArrowInvisibleColor;
        ArrowEnd1.colors = ArrowInvisibleColor;
        ArrowEnd2.colors = ArrowInvisibleColor;
        IsVisible = false;
        ArrowTemplate.gameObject.SetActive(false);
    }*/

    [SerializeField] private GameObject Marker;
    public bool IsVisible;
    public Vector3 HomePosition;

    private void Start()
    {
        Marker.gameObject.SetActive(false);
    }

    public void Show()
    {
        Marker.gameObject.SetActive(true);
        IsVisible = true;
    }

    public void Hide()
    {
        Marker.gameObject.SetActive(false);
        IsVisible = false;
    }

    public void Align()
    {

    }

    public void SetHome(Vector3 home)
    {
        HomePosition = Vector3.zero;
        //Marker.transform.position = Vector3.zero;
    }
}
