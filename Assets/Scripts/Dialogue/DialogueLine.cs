using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueLine : DialogueBaseClass
    {
        private TextMeshProUGUI textHolder;
        [SerializeField] private string input;
        [SerializeField] private float delay;

        [Header("Character Image")]
        [SerializeField] Sprite chracterImage;
        [SerializeField] Image imageHolder;

        private void Awake() 
        {
            textHolder = GetComponent<TextMeshProUGUI>();
            textHolder.text = "";
            
            imageHolder.sprite = chracterImage;
            imageHolder.preserveAspect = true;

        }

        private void Start()
        {
            StartCoroutine(WriteText(input, textHolder, delay));
        }
    }
}

