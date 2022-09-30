using System.Collections;
using System;
using UnityEngine;
using TMPro;

public class HideAndSeekTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberPopUp;
    [SerializeField] private GameObject cue;

    [SerializeField] private GameObject hide_and_seek_monster;

    [SerializeField] private PlayerMovement player;

    public event EventHandler player_is_well_hidden;
    public event EventHandler player_is_not_well_hidden;

    private MonsterAI monster;

    private bool hasListened = false;
    private bool hasCounted = false;

    private bool you_didnt_hide;

    private bool startedToSeek = false;

    public int count;

    private void Awake()
    {
        try { monster = GameObject.Find("Monster").GetComponent<MonsterAI>(); }
        catch { Debug.Log("Monster Not Found"); }
    }

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
            monster.Disable_Monster();
        }

        if (hasCounted)
        {
            if (player.isHidden)
            {
                if (!player.well_hidden) player_is_not_well_hidden?.Invoke(this, EventArgs.Empty);
                else player_is_well_hidden?.Invoke(this, EventArgs.Empty);
            }
            else you_didnt_hide = true;
            
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
        hide_and_seek_monster.SetActive(false);
        hasCounted = true;
    }
}
