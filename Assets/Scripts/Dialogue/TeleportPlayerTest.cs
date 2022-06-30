using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using Cinemachine;

public class TeleportPlayerTest: MonoBehaviour
{
    [SerializeField] private GameObject player;
    
    [SerializeField] private GameObject spawnPointMirrorWorld;
    [SerializeField] private GameObject ExitPoint;
    [SerializeField] private GameObject spawnPointBathroom;

    [SerializeField] private UnityEngine.Experimental.Rendering.Universal.Light2D globalLight;

    [SerializeField] private GameObject monster;

    private GameController gameController;

    private bool hasListened;
    private bool mazeDone;

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
            StartCoroutine(TeleportPlayer(spawnPointMirrorWorld, 14, 0.1f));
            hasListened = true;
        }

        float distanceBetweenPlayerAndExitPoint = Vector2.Distance(player.transform.position, ExitPoint.transform.position);
        if (distanceBetweenPlayerAndExitPoint < 0.5f && !mazeDone)
        {
            StartCoroutine(TeleportPlayer(spawnPointBathroom, 11.5f, 0.01f));
            monster.SetActive(false);
            mazeDone = true;
        }

    }

    private IEnumerator TeleportPlayer(GameObject to, float cameraSize, float light)
    {
        InputManager.GetInstance().canInteract = false;

        gameController.SetFade("in", true);

        yield return new WaitForSeconds(2f);

        globalLight.intensity = light;

        gameController.SetFade("in", false);
        
        gameController.SetFade("out", true);
        player.transform.position = to.transform.position;
        player.GetComponent<PlayerLocation>().playerCurrentRoom = CurrentRoom.MirrorWorld1;
        GameObject.Find("Camera").GetComponent<CinemachineVirtualCamera>().GetComponentInChildren<Camera>().orthographicSize = cameraSize;

        yield return new WaitForSeconds(2f);       
       
        gameController.SetFade("out", false);
        InputManager.GetInstance().canInteract = true;
    }

}
