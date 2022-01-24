using UnityEngine;
using UnityEngine.EventSystems;

public class BtnSound : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        AudioM.inst.BtnSound();
    }
}
