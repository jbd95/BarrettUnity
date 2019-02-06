using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleMenu : MonoBehaviour {

    bool visible = true;
    float visiblePos;
    float invisiblePos;

    float percentVisible = 8;

    Button button;

    void Start()
    {
        button = GetComponent<Button>();

        visiblePos = button.transform.localPosition.x;
        invisiblePos = visiblePos + (button.GetComponent<RectTransform>().rect.width - button.GetComponent<RectTransform>().rect.width * (percentVisible/100));
    }

    public void ToggleVisibility()
    {
        if(visible)
        {
            StartCoroutine(HideAnimation());
            visible = false;
        }
        else
        {
            StartCoroutine(ShowAnimation());
            visible = true;
        }
    }

    IEnumerator ShowAnimation()
    {
        while (button.transform.localPosition.x > visiblePos)
        {
            button.transform.localPosition = new Vector3(button.transform.localPosition.x - 10f, button.transform.localPosition.y, button.transform.localPosition.z);
            yield return new WaitForSeconds(0.009f);
        }
    }

    IEnumerator HideAnimation()
    {
        while (button.transform.localPosition.x < invisiblePos)
        {
            button.transform.localPosition = new Vector3(button.transform.localPosition.x + 10f, button.transform.localPosition.y, button.transform.localPosition.z);
            yield return new WaitForSeconds(0.009f);
        }
    }
}
