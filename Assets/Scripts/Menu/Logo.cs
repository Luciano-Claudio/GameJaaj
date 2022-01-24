using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Logo : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("Countdown");

        if (PlayerPrefs.GetInt("HasFullscreen") == 1)
            Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("_fullscreen"));
        else
            Screen.fullScreen = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Menu");
    }
}
