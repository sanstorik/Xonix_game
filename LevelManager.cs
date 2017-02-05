using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class LevelManager : MonoBehaviour {
    static LevelManager instance;
    const int MAX_LEVEL_TRIES = 3;
    const float PERCENT_TO_NEXT_LEVEL = 60;
    int currentLevel;
    int currentLevelTries;

    int countOfAquaEnemyToCreate = 1;
    int countOfLandEnemyToCreate = 1;

    float progressPercentage;
    float percentOfGameForOneCell;

    [SerializeField]
    GameObject enemyAquaPrefab;
    [SerializeField]
    GameObject enemyLandPrefab;
    [SerializeField]
    GameObject mainHeroPrefab;
    [SerializeField]
    GameObject endGamePanel;


    List<EnemyFigure> enemyFiguresOnScene;

    private void Awake()
    {
        instance = this;
        enemyFiguresOnScene = new List<EnemyFigure>();
        currentLevel = 1;
        currentLevelTries = MAX_LEVEL_TRIES;
    }

    private void Start()
    {
        int cellsToDiscover = (Board.BoardCellWidth * Board.INSTANCE.BoardCellHeight) -
             (Board.BoardCellWidth * 2 + Board.INSTANCE.BoardCellHeight * 6);

        percentOfGameForOneCell = 100f / cellsToDiscover;
    }

    public void GoToNextLevel()
    {
        if (currentLevel - 1 % 2 == 0)
            countOfAquaEnemyToCreate++;
        else if (currentLevel % 3 == 0)
            countOfLandEnemyToCreate++;

        DestroyAllFigures();

        currentLevel++;

        ResetAllAndSpawn();
    }

    public void ResetAllAndSpawn()
    {
        TextContoller.INSTANCE.ResetTexts();
        Board.INSTANCE.ResetBoard();
        SpawnFigures();

        currentLevelTries = 3;
        progressPercentage = 0;
    }

    public void ResetCurrentLevel()
    {
        currentLevel = 1;
        countOfAquaEnemyToCreate = 1;
        countOfLandEnemyToCreate = 1;
    }

    public void OnHeroDie()
    {
        if (currentLevelTries > 1)
        {
            DestroyAllFigures();
            SpawnFigures();
            currentLevelTries--;
            TextContoller.INSTANCE.ShowLivesLeft(currentLevelTries);
        }
        else
            EndGame();
    }

    public void EndGame()
    {
        DestroyAllFigures();
        TextContoller.INSTANCE.MakeDarker();
        TextContoller.INSTANCE.SetLevelsDone(currentLevel - 1);
        TextContoller.INSTANCE.StopTimer();
        endGamePanel.SetActive(true);
    }

    public void SpawnFigures()
    {
        if (SwipeController.gamePaused)
            Time.timeScale = 0;

        TextContoller.INSTANCE.StartTimer();

        if (enemyFiguresOnScene.Count > 0)
            enemyFiguresOnScene.Clear();

        for(int i=0; i < countOfAquaEnemyToCreate; i++)
          SpawnFigure(enemyAquaPrefab);
        for(int i=0; i < countOfLandEnemyToCreate; i++)
          SpawnFigure(enemyLandPrefab);

        SpawnFigure(mainHeroPrefab);
    }

    void SpawnFigure(GameObject figurePrefab)
    {
        GameObject enemyGO = Instantiate(figurePrefab);
        Figure figure = enemyGO.GetComponent<Figure>();

        if (enemyGO.GetComponent<EnemyFigure>())
            enemyFiguresOnScene.Add((EnemyFigure)figure);
        else if (enemyGO.GetComponent<MainHero>())
            SwipeController.mainHero = (MainHero)figure;
    }

    void DestroyAllFigures()
    {
        foreach (var figure in enemyFiguresOnScene)
            Destroy(figure.gameObject);

        Destroy(SwipeController.mainHero.gameObject);
    }

    public List<EnemyFigure> GetEnemiesOnScene()
    {
        return enemyFiguresOnScene;
    }

    public void DestroyEnemy(EnemyFigure figure)
    {
        enemyFiguresOnScene.Remove(figure);
        Destroy(figure.gameObject);
    }

    public void IncreaseScore(int cellDestroyed)
    {
        progressPercentage += cellDestroyed * percentOfGameForOneCell;
        TextContoller.INSTANCE.ShowProgress(progressPercentage);
    }
    
    public void CheckIfEnemiesDied()
    {
        if (AquaEnemiesCount <= 0 || progressPercentage >= PERCENT_TO_NEXT_LEVEL)
            GoToNextLevel();
    }

    public int CurrentLevel
    {
        get { return currentLevel; }
    }

    public int AquaEnemiesCount
    {
        get
        {
            return enemyFiguresOnScene.Select(cell => cell)
                .Where(cell => cell.CellMoveType == BoardCellType.AQUA).Count();
        }
    }

    static public LevelManager INSTANCE
    {
        get { return instance; }
    }
}
