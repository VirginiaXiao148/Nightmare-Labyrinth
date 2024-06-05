using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameController : MonoBehaviour
{
    public Text timeText;

    private float startTime;

    private void Start()
    {
        startTime = PlayerPrefs.GetFloat("StartTime"); // Obtener el tiempo de inicio guardado
        UpdateTimePlayed();
    }

    private void Update()
    {
        UpdateTimePlayed();
    }

    private void UpdateTimePlayed()
    {
        float timePlayed = Time.time - startTime;
        string minutes = Mathf.Floor(timePlayed / 60).ToString("00");
        string seconds = Mathf.Floor(timePlayed % 60).ToString("00");
        timeText.text = "Time Played: " + minutes + ":" + seconds;
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene("MainScene"); // Cargar la escena principal del juego
    }

    public void QuitGame()
    {
        Application.Quit(); // Salir del juego
    }
}

