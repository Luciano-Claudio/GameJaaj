using UnityEngine;

public class PauseOptions : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Config.config.CloseOptions();
    }
}
