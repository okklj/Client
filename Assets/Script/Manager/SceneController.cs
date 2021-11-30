using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySingleTon;
using UnityEngine.SceneManagement;

public class SceneController : SingleTon<SceneController>,IEndGameOberver
{
    public GameObject playerPrefab;//���س���ʱ����
    public SceneFader sceneFaderPrefab;//���뵭��
    SceneFader fader;
    GameObject player;
    bool fadeFinish = true;//��ֹ��μ������˵�������

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


    //��ʼ��һ������
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
            //yield return ������⣺�ȴ���������ִ����
            yield return SceneManager.LoadSceneAsync(sceneName);
            //TODO:��������
            SaveManager.Instance.SavePlayerData();
            TransitionDestination destination = GetDestination(destinationTag);
            yield return Instantiate(playerPrefab, destination.transform.position, destination.transform.rotation);
            //��ȡ����
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
            //��������
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
        //��������
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

    //��ɫ������ص�������
    public void EndNotify()
    {
        if (fadeFinish)
        {
            fadeFinish = false;
            StartCoroutine(LoadMain());
        }
    }
}
