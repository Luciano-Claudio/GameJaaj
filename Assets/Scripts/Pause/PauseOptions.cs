using UnityEngine;

public class PauseOptions : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Z))
            Config.config.CloseOptions();
    }
}
