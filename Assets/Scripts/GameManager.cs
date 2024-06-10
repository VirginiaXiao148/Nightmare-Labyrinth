using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private float playTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        playTime += Time.deltaTime;
    }

    public float GetPlayTime()
    {
        return playTime;
    }

    public void ResetPlayTime()
    {
        playTime = 0f;
    }

    public void RestartGame()
    {
        ResetPlayTime();
        SceneManager.LoadScene("Maze"); // Cambia "GameScene" por el nombre de tu escena de juego
    }

    public void GoToStartScene()
    {
        SceneManager.LoadScene("StartGame"); // Cambia "StartScene" por el nombre de tu escena de inicio
    }
}
