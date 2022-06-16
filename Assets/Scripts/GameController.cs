using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private MonsterAI monsterIA;
    public GameObject player;
    private Ritual ritual;
    private MadnessManager madness;

    public bool isGameOver;

    public bool isBrightDay;

    public float timeBeforeRestart;

    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject pressEText;
    [SerializeField] private Animator fade;


    public bool firstTime;
    
    private void Awake()
    {
        
        player.GetComponent<PlayerController>();
        player.GetComponent<PlayerMovement>();

        try { ritual = GameObject.Find("RitualCircle").GetComponent<Ritual>(); }
        catch { Debug.Log("RitualCircle missing"); }

        try { monsterIA = GameObject.Find("Monster").GetComponent<MonsterAI>(); }
        catch { Debug.Log("hola"); }

        try { madness = GameObject.Find("MadnessManager").GetComponent<MadnessManager>(); }
        catch { Debug.Log("MadnessManager missing"); }

        if (!isBrightDay)
            try { monsterIA = GameObject.Find("Monster").GetComponent<MonsterAI>(); }
            catch { Debug.Log("hola"); }
        

        if (monsterIA == null)
            return;
    }

    private void Start()
    {
        firstTime = true;
    }

    private void Update()
    {
        if (firstTime == false)
            pressEText.SetActive(false);
    }

    private void OnEnable()
    {
        if (monsterIA != null)
            monsterIA.OnPlayerCatched += GameOver;

        ritual.OnHeartEnding += FinishGame;
        madness.HeartAttackEvent += GameOver;
    }

    private void OnDisable()
    {
        if (monsterIA != null)
            monsterIA.OnPlayerCatched -= GameOver;
    }

    private void GameOver(object sender, System.EventArgs e)
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        fade.SetBool("in", true);
        yield return new WaitForSeconds(fade.GetCurrentAnimatorStateInfo(0).length);
        fade.SetBool("in", false);
        player.transform.position = ritual.transform.position;
        fade.SetBool("out", true);
        yield return new WaitForSeconds(fade.GetCurrentAnimatorStateInfo(0).length);
        fade.SetBool("out", false);
        
        player.SendMessage("RebornRutine");
        isGameOver = false;
    }
    private IEnumerator WinSequence()
    {   
      
        yield return new WaitForSeconds(timeBeforeRestart);


        SceneController.LoadScene("Menu", 1f, 1f);
    }


    private void FinishGame(object sender, System.EventArgs e)
    {
        isGameOver = true;
        StartCoroutine(WinSequence());
    }
}
