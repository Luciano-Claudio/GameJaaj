using UnityEngine;

public class Options : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            Menu.menu.CloseOptions();
    }
}
