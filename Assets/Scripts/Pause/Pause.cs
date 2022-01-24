using UnityEngine;

public class Pause : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Config.config.ClosePause();
    }
}
