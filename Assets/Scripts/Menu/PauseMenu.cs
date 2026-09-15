using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
public class PauseMenu : MonoBehaviour
{
    public GameObject container;
    public GameObject optionsContainer;
    public Slider masterVolume, musicVolume, sfxVolume;
    public AudioMixer mainAudioMixer;


    public void ChangeMasterVolume()
    {
        mainAudioMixer.SetFloat("MasterVolumeParam", masterVolume.value);
    }

    public void ChangeMusicVolume()
    {
        mainAudioMixer.SetFloat("MusicVolumeParam", musicVolume.value);
    }

    public void ChangeSFXVolume()
    {
        mainAudioMixer.SetFloat("SFXVolumeParam", sfxVolume.value);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (optionsContainer.activeSelf && !container.activeSelf)
            {
                BackButton();
            }
            if (container.activeSelf)
            {
                ResumeButton();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        container.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OptionsButton()
    {
        container.SetActive(false);
        optionsContainer.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void BackButton()
    {
        optionsContainer.SetActive(false);
        container.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}