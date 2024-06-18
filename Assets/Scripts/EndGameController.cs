using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameController : MonoBehaviour
{
    public Text playTimeText;
    public Button retryButton;
    public Button startButton;

    private void Start()
    {
        float playTime = GameManager.instance.GetPlayTime();
        playTimeText.text = "Tiempo de juego: " + FormatTime(playTime);

        retryButton.onClick.AddListener(GameManager.instance.RestartGame);
        startButton.onClick.AddListener(GameManager.instance.GoToStartScene);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}