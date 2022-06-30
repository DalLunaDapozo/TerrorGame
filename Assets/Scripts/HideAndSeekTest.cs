using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HideAndSeekTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberPopUp;
    [SerializeField] private GameObject cue;

    [SerializeField] private GameObject monster;

    [SerializeField] private PlayerMovement player;

    private bool hasListened = false;
    private bool hasCounted = false;

    private bool startedToSeek = false;

    public int count;

    private void Start()
    {
        ((Ink.Runtime.IntValue)DialogueManager.
        GetInstance().GetVariableState("countTimeBeforePlay")).value = count;
    }

    private void Update()
    {
        bool hasSaidYesToHide = ((Ink.Runtime.BoolValue)DialogueManager.
        GetInstance().GetVariableState("hide")).value;


        if (!hasListened && hasSaidYesToHide)
        {
            StartCoroutine(CountSequence());
            Debug.Log("EMPIEZA EL JUEGO");
            hasListened = true;
        }

        if (hasCounted && player.GetComponent<PlayerLocation>().playerCurrentRoom != CurrentRoom.Closet)
        {
            Debug.Log("MORIDO");
            hasCounted = false;
        }
        else if(hasCounted && player.GetComponent<PlayerLocation>().playerCurrentRoom == CurrentRoom.Closet)
        {
            Debug.Log("VIVIDO");
            hasCounted = false;
        }
           
    }

    private IEnumerator CountSequence()
    {
        cue.SetActive(false);
        numberPopUp.gameObject.SetActive(true);
        InputManager.GetInstance().canInteract = false;
        
        while (count > -1)
        {
            numberPopUp.text = count--.ToString();
            yield return new WaitForSeconds(1f);
        }

        numberPopUp.gameObject.SetActive(false);
        InputManager.GetInstance().canInteract = true;
        monster.SetActive(false);
        hasCounted = true;
    }
}
