using UnityEngine;

public class Credits : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Menu.menu.CloseCredits();
    }
}
