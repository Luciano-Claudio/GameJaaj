using UnityEngine;
using UnityEngine.EventSystems;

public class StartBtn : MonoBehaviour, IDeselectHandler
{
    void Start()
    {
        gameObject.GetComponent<BtnSound>().enabled = false;
    }
    public void OnDeselect(BaseEventData eventData)
    {
        gameObject.GetComponent<BtnSound>().enabled = true;
    }
}
