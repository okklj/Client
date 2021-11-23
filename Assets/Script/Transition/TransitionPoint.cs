using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType
    {
        SameScene,DifferentScene
    }
    [Header("Transition Info")]
    public string sceneName;
    public TransitionType transitiontype;//传送类型
    public TransitionDestination.DestinationTag destinationTag;//传送目的点
    private bool canTrans;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTrans)
        {
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = false;
        }
    }
}
