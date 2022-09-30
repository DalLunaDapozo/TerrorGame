using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private MonsterAI monsterIA;
    public PlayerMovement player;
    private PlayerLocation playerLocation;
    private Ritual ritual;
    private MadnessManager madness;

    public bool isGameOver;

    public bool isBrightDay;

    public float timeBeforeRestart;

    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject pressEText;
    [SerializeField] private Animator fade;


    public event System.EventHandler PlayerSpawned;

    public bool firstTime;
    
    private void Awake()
    {
       
        try { player = GameObject.Find("Player").GetComponent<PlayerMovement>(); }
        catch { Debug.Log("Player missing"); }

        playerLocation = player.GetComponent<PlayerLocation>();

        try { ritual = GameObject.Find("RitualCircle").GetComponent<Ritual>(); }
        catch { Debug.Log("RitualCircle missing"); }

        try { monsterIA = GameObject.Find("Monster").GetComponent<MonsterAI>(); }
        catch { Debug.Log("Monster not found"); }

        try { madness = GameObject.Find("MadnessManager").GetComponent<MadnessManager>(); }
        catch { Debug.Log("MadnessManager missing"); }

        if (!isBrightDay)
            try { monsterIA = GameObject.Find("Monster").GetComponent<MonsterAI>(); }
            catch { Debug.Log("Monster not found"); }
        

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
        InputManager.GetInstance().test_button_pressed += FinishGame;
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
        yield return new WaitForSeconds(fade.GetCurrentAnimatorStateInfo(0).length + 2f);
        
        fade.SetBool("in", false);
        fade.SetBool("out", true);
        player.SetActivateSprite(false);
        player.transform.position = ritual.transform.position;

        playerLocation.playerCurrentRoom = CurrentRoom.RitualRoom;

        yield return new WaitForSeconds(fade.GetCurrentAnimatorStateInfo(0).length);
        player.SetActivateSprite(true);
        player.SendMessage("RebornRutine");
        
        player.lighterIsOn = true;
        fade.SetBool("out", false);
        
        isGameOver = false;

        PlayerSpawned?.Invoke(this, System.EventArgs.Empty);
    }
    private IEnumerator WinSequence()
    {   
      
        yield return new WaitForSeconds(.2f);


        SceneController.LoadScene("Menu", 1f, 1f);
    }

    public void SetFade(string inOrOut, bool tf)
    {
        fade.SetBool(inOrOut, tf);
    }

    private void FinishGame(object sender, System.EventArgs e)
    {
        isGameOver = true;
        StartCoroutine(WinSequence());
    }
}
