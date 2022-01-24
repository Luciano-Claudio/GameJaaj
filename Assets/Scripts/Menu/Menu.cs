using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static Menu menu;

    public GameObject Camera, StartBtn, OptionsBtn, CreditsBtn, FirstOptionsButton;
    public Animator anim;
    public Toggle FullscreenTogle;
    public Slider Volume, Music, LoadingSlider;
    public Text LoadingTxt;

    bool fullscr = true;
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

        EventSystem.current.firstSelectedGameObject = StartBtn;
    }

    void Awake()
    {
        _audioS = Camera.GetComponent<AudioSource>();
        menu = this;
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(lastselect);
        else
            lastselect = EventSystem.current.currentSelectedGameObject;
    }

    public void PlayButton(string scene)
    {
        StartCoroutine(LoadAsynchronously(scene));
    }
    public void OpenOptions()
    {

        anim.SetBool("Options", true);
        anim.SetBool("Menu", false);
        EventSystem.current.SetSelectedGameObject(FirstOptionsButton);
    }
    public void CloseOptions()
    {
        anim.SetBool("Menu", true);
        anim.SetBool("Options", false);
        EventSystem.current.SetSelectedGameObject(OptionsBtn);
    }
    public void OpenCredits()
    {

        anim.SetBool("Credits", true);
        anim.SetBool("Menu", false);
        EventSystem.current.SetSelectedGameObject(FirstOptionsButton);
    }
    public void CloseCredits()
    {
        anim.SetBool("Menu", true);
        anim.SetBool("Credits", false);
        EventSystem.current.SetSelectedGameObject(CreditsBtn);
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

    IEnumerator LoadAsynchronously(string scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        anim.SetBool("Play", true);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            LoadingSlider.value = progress;
            LoadingTxt.text = progress * 100 + "%";
            yield return null;
        }
    }
}
