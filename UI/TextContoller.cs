using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TextContoller : MonoBehaviour {

    static TextContoller instance;

    [SerializeField]
    Text timeValueText;
    [SerializeField]
    Text progressText;
    [SerializeField]
    Text livesLeftText;
    [SerializeField]
    Text levelsDoneText;
    [SerializeField]
    Image opacity;

    float timer = 0;
    float timePassed = 0;
    float timeForLevel = 60;
    bool startTimer = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (!startTimer)
            return;

        timer += Time.deltaTime;

        if(timer >= 1)
        {
            timePassed++;
            ShowTimeLeft(timeForLevel - timePassed);
            timer = 0;
        }

        if (timePassed >= timeForLevel)
            LevelManager.INSTANCE.EndGame();
    }

    public void ShowTimeLeft(float time)
    {
        timeValueText.text = ":" + time;
    }

    public void ShowProgress(float progress)
    {
        progressText.text = (int)progress + "%";
    }

    public void ShowLivesLeft(float lives)
    {
        livesLeftText.text = lives.ToString();
    }

    public void SetLevelsDone(int levelsDone)
    {
        levelsDoneText.text = levelsDone.ToString();
    }

    public void StartTimer()
    {
        startTimer = true;
    }

    public void StopTimer()
    {
        startTimer = false;
    }

    public void ResetTexts()
    {
        ShowProgress(0);
        ShowLivesLeft(3);
        ShowTimeLeft(60);
        timePassed = 0;
    }

    public void MakeDarker()
    {
        opacity.gameObject.SetActive(true);
        opacity.DOFade(0.4f, 1.5f);
    }

    public void MakeLighter()
    {
        opacity.DOFade(0f, 1f).OnComplete(() => opacity.gameObject.SetActive(false));
    }

    public IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }

    static public TextContoller INSTANCE
    {
        get { return instance; }
    }
}
