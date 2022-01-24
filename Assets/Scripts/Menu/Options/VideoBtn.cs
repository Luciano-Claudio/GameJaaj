using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VideoBtn : MonoBehaviour, IDeselectHandler
{
    Toggle toggle;
    public Text txt;
    void Start()
    {
        toggle = GetComponent<Toggle>();
        gameObject.GetComponent<BtnSound>().enabled = false;
    }
    void Update()
    {
        txt.text = toggle.isOn? "< Sim <color=#505050>></color>" : "<color=#505050><</color> Não >";

        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                toggle.isOn = false;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                toggle.isOn = true;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        gameObject.GetComponent<BtnSound>().enabled = true;
    }
}
