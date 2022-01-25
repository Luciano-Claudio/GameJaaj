using UnityEngine;

public class Credits : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            Menu.menu.CloseCredits();
    }
}
