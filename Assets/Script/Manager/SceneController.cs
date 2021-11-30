using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySingleTon;
using UnityEngine.SceneManagement;

public class SceneController : SingleTon<SceneController>,IEndGameOberver
{
    public GameObject playerPrefab;//加载场景时生成
    public SceneFader sceneFaderPrefab;//淡入淡出
    SceneFader fader;
    GameObject player;
    bool fadeFinish = true;//防止多次加载主菜单而奔溃

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        GameManager.Instance.AddObserver(this);
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitiontype)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }


    //开始第一个场景
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("Level One"));
    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    public void TransitionToMainMenu()
    {
        StartCoroutine(LoadMain());
    }

    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        if (sceneName == SceneManager.GetActiveScene().name)
        {
            player = GameManager.Instance.playerStats.gameObject;
            TransitionDestination destination = GetDestination(destinationTag);
            player.transform.SetPositionAndRotation(destination.transform.position, destination.transform.rotation);
            yield return null;
        }
        else
        {
            if (fader == null)
                fader = Instantiate(sceneFaderPrefab);
            yield return StartCoroutine(fader.FadeOut(1.5f));
            //yield return 基本理解：等待该行命令执行完
            yield return SceneManager.LoadSceneAsync(sceneName);
            //TODO:保存数据
            SaveManager.Instance.SavePlayerData();
            TransitionDestination destination = GetDestination(destinationTag);
            yield return Instantiate(playerPrefab, destination.transform.position, destination.transform.rotation);
            //读取数据
            SaveManager.Instance.LoadPlayerData();
            yield return StartCoroutine(fader.FadeIn(1.5f));
            yield break;
        }
    }

    IEnumerator LoadLevel(string scene)
    {
        if(fader==null)
            fader = Instantiate(sceneFaderPrefab);
        if (scene != "")
        {
            yield return StartCoroutine(fader.FadeOut(1.5f));
            yield return SceneManager.LoadSceneAsync(scene);
            yield return Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);
            //保存数据
            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fader.FadeIn(1.5f));
            yield break;
        }
        yield return null;
    }
    IEnumerator LoadMain()
    {
        if (fader == null)
            fader = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fader.FadeOut(1.5f));
        //保存数据
        SaveManager.Instance.SavePlayerData();
        yield return SceneManager.LoadSceneAsync("MainMenu");
        yield return StartCoroutine(fader.FadeIn(1.5f));
        yield break;
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        foreach(var destination in entrances)
        {
            if (destination.destinationTag == destinationTag)
            {
                return destination;
            }
        }
        return null;
    }

    //角色死亡后回到主界面
    public void EndNotify()
    {
        if (fadeFinish)
        {
            fadeFinish = false;
            StartCoroutine(LoadMain());
        }
    }
}
