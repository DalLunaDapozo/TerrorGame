using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private MonsterAI monsterIA;
    public GameObject player;
    private Ritual ritual;
    
    public bool isGameOver;

    public bool isBrightDay;

    public float timeBeforeRestart;

    [SerializeField] private GameObject gameOverText;
    

    private void Awake()
    {
        
        player.GetComponent<PlayerController>();
        player.GetComponent<PlayerMovement>();

        try { ritual = GameObject.Find("RitualCircle").GetComponent<Ritual>(); }
        catch { Debug.Log("RitualCircle missing"); }

        try { monsterIA = GameObject.Find("Monster").GetComponent<MonsterAI>(); }
        catch { Debug.Log("hola"); }



        if (!isBrightDay)
            try { monsterIA = GameObject.Find("Monster").GetComponent<MonsterAI>(); }
            catch { Debug.Log("hola"); }
        

        if (monsterIA == null)
            return;
    }

    private void Update()
    {
        
    }

    private void OnEnable()
    {
        if (monsterIA != null)
            monsterIA.OnPlayerCatched += GameOver;

        ritual.OnHeartEnding += FinishGame;

    }

    private void OnDisable()
    {
        if (monsterIA != null)
            monsterIA.OnPlayerCatched -= GameOver;
    }

    private void GameOver(object sender, System.EventArgs e)
    {
        isGameOver = true;
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        gameOverText.SetActive(true);

        yield return new WaitForSeconds(timeBeforeRestart);

        SceneManager.LoadScene("DarkHouse");
    }
    private IEnumerator WinSequence()
    {
      
        yield return new WaitForSeconds(timeBeforeRestart);

        SceneManager.LoadScene("Menu");
    }


    private void FinishGame(object sender, System.EventArgs e)
    {
        isGameOver = true;
        StartCoroutine(WinSequence());
    }
}
