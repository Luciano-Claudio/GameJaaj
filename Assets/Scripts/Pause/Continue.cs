using UnityEngine;
using UnityEngine.EventSystems;

public class Continue : MonoBehaviour, IDeselectHandler
{
    public void OnDeselect(BaseEventData eventData)
    {
        gameObject.GetComponent<BtnSound>().enabled = true;
    }
}
