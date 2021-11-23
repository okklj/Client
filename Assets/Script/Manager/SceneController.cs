using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySingleTon;
using UnityEngine.SceneManagement;

public class SceneController : SingleTon<SceneController>
{
    public GameObject playerPrefab;//���س���ʱ����
    GameObject player;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
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

    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
    {
        //TODO:��������
        if (sceneName == SceneManager.GetActiveScene().name)
        {
            player = GameManager.Instance.playerStats.gameObject;
            TransitionDestination destination = GetDestination(destinationTag);
            player.transform.SetPositionAndRotation(destination.transform.position, destination.transform.rotation);
            yield return null;
        }
        else
        {
            //yield return ������⣺�ȴ���������ִ����
            yield return SceneManager.LoadSceneAsync(sceneName);
            TransitionDestination destination = GetDestination(destinationTag);
            yield return Instantiate(playerPrefab, destination.transform.position, destination.transform.rotation);
            yield break;
        }
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
}
