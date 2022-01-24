using UnityEngine;

public class Options : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Menu.menu.CloseOptions();
    }
}
