using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Config : MonoBehaviour
{

    public static Config config;

    public GameObject Camera, PauseMenuUI, StartPauseBtn, OptionsPauseBtn, FirstOptionsButton;
    public Slider Volume, Music;
    public Toggle FullscreenTogle;
    public Animator anim;

    bool fullscr = true, pause=false;
    private GameObject lastselect;
    private AudioSource _audioS;
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        if (PlayerPrefs.GetInt("HasFullscreen") == 1)
            fullscr = Convert.ToBoolean(PlayerPrefs.GetInt("_fullscreen"));
        if (PlayerPrefs.GetInt("HasVolume") == 1)
        {
            Volume.value = PlayerPrefs.GetFloat("_volume");
            AudioListener.volume = PlayerPrefs.GetFloat("_volume");
        }
        if (PlayerPrefs.GetInt("HasMusic") == 1)
        {
            _audioS.volume = PlayerPrefs.GetFloat("_music");
            Music.value = PlayerPrefs.GetFloat("_music");
        }
        FullscreenTogle.isOn = fullscr;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        lastselect = new GameObject();

        EventSystem.current.firstSelectedGameObject = StartPauseBtn;

    }
    void Awake()
    {
        _audioS = Camera.GetComponent<AudioSource>();
        config = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pause)
            OpenPause();

        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(lastselect);
        else
            lastselect = EventSystem.current.currentSelectedGameObject;
    }

    public void OpenPause()
    {
        PauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(StartPauseBtn);
        Time.timeScale = 0f;
        pause = true;
    }
    public void ClosePause()
    {
        PauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        pause = false;
    }

    public void OpenOptions()
    {
        anim.SetBool("Options", true);
        EventSystem.current.SetSelectedGameObject(FirstOptionsButton);
    }
    public void CloseOptions()
    {
        anim.SetBool("Options", false);
        EventSystem.current.SetSelectedGameObject(OptionsPauseBtn);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void SetMusicVolume(float volume)
    {
        _audioS.volume = volume;
        PlayerPrefs.SetInt("HasMusic", 1);
        PlayerPrefs.SetFloat("_music", volume);
    }
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetInt("HasVolume", 1);
        PlayerPrefs.SetFloat("_volume", volume);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        fullscr = isFullscreen;
        PlayerPrefs.SetInt("HasFullscreen", 1);
        PlayerPrefs.SetInt("_fullscreen", Convert.ToInt16(isFullscreen));
    }
}
