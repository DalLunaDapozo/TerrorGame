using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using Cinemachine;

public class TeleportPlayerTest: MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject spawnPoint;

    private GameController gameController;

    private bool hasListened;

    private void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        hasListened = false;
    }

    private void Update()
    {
        bool hasSaidYesToMirror = ((Ink.Runtime.BoolValue) DialogueManager.
            GetInstance().GetVariableState("mirror")).value;

        if (hasSaidYesToMirror && !hasListened)
        {
            StartCoroutine(TeleportPlayer());
            hasListened = true;
        }
    }

    private IEnumerator TeleportPlayer()
    {
        InputManager.GetInstance().canInteract = false;

        gameController.SetFade("in", true);

        yield return new WaitForSeconds(2f);
        
        gameController.SetFade("in", false);
        
        gameController.SetFade("out", true);
        player.transform.position = spawnPoint.transform.position;
        player.GetComponent<PlayerLocation>().playerCurrentRoom = CurrentRoom.MirrorWorld1;
        GameObject.Find("Camera").GetComponent<CinemachineVirtualCamera>().GetComponentInChildren<Camera>().orthographicSize = 14;

        yield return new WaitForSeconds(2f);       
       
        gameController.SetFade("out", false);
        InputManager.GetInstance().canInteract = true;
    }

}
