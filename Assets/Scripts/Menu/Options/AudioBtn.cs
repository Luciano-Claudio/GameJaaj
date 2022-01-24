using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AudioBtn : MonoBehaviour
{
    Slider slider;
    public Text txt;
    float i;
    void Start()
    {
        slider = GetComponent<Slider>();
        i = slider.value * 10;
    }
    void Update()
    {
        if (i == 0)
            txt.text = "<color=#505050><</color> 0 >";
        else if (i == 10)
            txt.text = "< 10 <color=#505050>></color>";
        else
            txt.text = "< " + i.ToString("F0") + " >";

        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && i > 0)
                i--;
            else if (Input.GetKeyDown(KeyCode.RightArrow) && i < 10)
                i++;
        }
        slider.value = i/10;
    }
}
