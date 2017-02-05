using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonListener : MonoBehaviour {
    [SerializeField]
    Image buttonPauseImage;
    [SerializeField]
    GameObject endGamePanel;
    [SerializeField]
    GameObject startGamePanel;

    [SerializeField]
    Sprite pauseImage;
    [SerializeField]
    Sprite unpauseImage;

    bool gameIsPaused = false;

    private void Start()
    {
        TextContoller.INSTANCE.MakeDarker();
    }

    public void PauseUnpause()
    {
        gameIsPaused = !gameIsPaused;

        if (gameIsPaused)
        {
            Time.timeScale = 0;
            buttonPauseImage.overrideSprite = unpauseImage;

            SwipeController.gamePaused = true;
        }
        else
        {
            Time.timeScale = 1;
            buttonPauseImage.overrideSprite = pauseImage;

            SwipeController.gamePaused = false;
        }
    }

    public void RestartButton()
    {
        if (SwipeController.gamePaused)
            PauseUnpause();

        endGamePanel.SetActive(false);
        TextContoller.INSTANCE.MakeLighter();

        LevelManager.INSTANCE.ResetCurrentLevel();
        LevelManager.INSTANCE.ResetAllAndSpawn();
    }

    public void StartGame()
    {
        LevelManager.INSTANCE.SpawnFigures();
        SwipeController.gameStarted = true;

        TextContoller.INSTANCE.MakeLighter();
        startGamePanel.SetActive(false);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

}
