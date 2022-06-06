using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private MonsterAI monsterIA;
    public GameObject player;
    public bool isGameOver;

    public bool isBrightDay;

    public float timeBeforeRestart;

    [SerializeField] private GameObject gameOverText;

    private void Awake()
    {
        
        player.GetComponent<PlayerController>();
        player.GetComponent<PlayerMovement>();

       
        if(!isBrightDay)
        monsterIA = GameObject.Find("Monster").GetComponent<MonsterAI>();

        if (monsterIA == null)
            return;
    }

    private void Update()
    {
        if (isGameOver)
            StartCoroutine("GameOverSequence");
    }

    private void OnEnable()
    {
        if (monsterIA != null)
            monsterIA.OnPlayerCatched += GameOver;
    }

    private void OnDisable()
    {
        if (monsterIA != null)
            monsterIA.OnPlayerCatched -= GameOver;
    }

    private void GameOver(object sender, System.EventArgs e)
    {
        isGameOver = true;
        StartCoroutine("GameOverSequence");
    }

    private IEnumerator GameOverSequence()
    {
        gameOverText.SetActive(true);

        yield return new WaitForSeconds(timeBeforeRestart);

        SceneManager.LoadScene("DarkHouse");
    }

    
}
