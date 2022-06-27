using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private Story currentStory;

    public bool dialogueIsPlaying { get; private set; }

    private InputManager input;

    private static DialogueManager instance;
    public static DialogueManager GetInstance()
    {
        return instance;
    }
    
    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Found more than one");
        instance = this;

        input = new InputManager();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
            return;

        if (input.Keyboard.Action.triggered)
            ContinueStory();
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }
   
    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.05f);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
            dialogueText.text = currentStory.Continue();
        else
            StartCoroutine(ExitDialogueMode());
    }
}
