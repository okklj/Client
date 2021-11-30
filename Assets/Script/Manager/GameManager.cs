using MySingleTon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : SingleTon<GameManager>
{
    public CharacterStats playerStats;
    private CinemachineFreeLook followCamera;

    List<IEndGameOberver> endGameObervers = new List<IEndGameOberver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    public void RigisterPlayer(CharacterStats player)
    {
        playerStats = player;
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera != null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    public void AddObserver(IEndGameOberver observer)
    {
        endGameObervers.Add(observer);
    }
    public void RemoveObserver(IEndGameOberver observer)
    {
        endGameObervers.Remove(observer);
    }

    public void NotifyObserver()
    {
        foreach (var observer in endGameObervers)
        {
            observer.EndNotify();
        }
    }

    public Transform GetEntrance()
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        foreach (var destination in entrances)
        {
            if (destination.destinationTag == TransitionDestination.DestinationTag.ENTER)
            {
                return destination.transform;
            }
        }
        return null;
    }
}
