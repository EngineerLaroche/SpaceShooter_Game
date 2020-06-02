using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Ennemies")]

    public int averageSpawnPos = 8;
    public GameObject[] hazards;
    public Boundary boundary;
    public float spawnWait = 0.75f;
    public float startWait = 1f;
    public float waveWait = 4f;

    [Header("Pause")]

    public Button pauseBtn;
    public GameObject pausePanel;
    public Button continuBtn;
    public Button pauseToMenuBtn;

    [Header("Game Over")]

    public GameObject gameOverPanel;
    public Button restartBtn;
    public Button toMenuBtn;
    private bool isGameOver = false;

    [Header("Joystick")]

    public Joystick joystick;
    public Joybutton joybutton;
    public GameObject fixedJoystick;

    [Header("Energy bar")]

    public GameObject energyProgressBar;
    public bool turboEnabled = false;
    public Text energyText;

    private float energyPercent = 0f;
    private int turboCount = 0;

    [Header("Score")]

    public Text roundText;
    public Text scoreText;
    public Text scoreRoundText;
    public Text totalEnemiesKilledText;
    public Text enemiesUntilNextRound;

    private int round = 1;
    private int score = 0;
    private int enemiesLeft = 0;
    private int scorePerRound = 0;
    public static int scoreMinimum;
    private int totalEnemiesKilled = 0;

    [Header("Hiroshima")] 
    public int enemyToKillBeforeHiroshima = 10;
    public Button hiroshimaBtn;
    public GameObject hiroshimaBtnText;
    public GameObject explosion;

    private int enemyKilledHiroshima = 0;
    private static System.Timers.Timer aTimer;


    void Start()
    {
        //Met les valeur GameOver et Pause par defaut pour la nouvelle partie
        GameOverSettings(false);
        PauseSettings(false);
        InitButtonsListener();

        joystick = FindObjectOfType<Joystick>();
        
        // Button Hiroshima
        hiroshimaBtn.interactable = false;
        hiroshimaBtn.onClick.AddListener(triggerHiroshima);
        hiroshimaBtnText.GetComponent<Text>().text = "0 / " + enemyToKillBeforeHiroshima.ToString();
        hiroshimaBtn.GetComponent<Image>().color = new Color32(247, 193, 54, 80);

        // Energy Bar (second weapon)
        energyProgressBar.transform.position = new Vector3(-25, 0, energyProgressBar.transform.position.z);
        AddScore(score);

        enemiesLeft = OptionsController.hazardCount;
        enemiesUntilNextRound.text = "Enemies left:  " + enemiesLeft;

        //Apparition des ennemis en boucle
        StartCoroutine(SpawnWaves());
    }


    // PAUSE MENU - Met en pause la partie
    public void GamePaused()
    {
        PauseSettings(true);
    }

    // CONTINU GAME - Continuer la partie après mise en pause
    void ContinuGame()
    {
        PauseSettings(false);
    }

    // GAME OVER MENU - Cesse le jeu lorsque le héro est éléminé
    public void GameOver()
    {
        GameOverSettings(true);
    }

    // RESTART GAME - Demarre une nouvelle partie
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // BACK TO MAIN MENU - Retour au menu principal
    void BackToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    // GAME OVER SETTINGS
    void GameOverSettings(bool isActive)
    {
        isGameOver = isActive;
        PauseSettings(false);
        pauseBtn.gameObject.SetActive(false);
        gameOverPanel.SetActive(isActive); 
    }

    // PAUSE SETTINGS
    void PauseSettings(bool isActive)
    {
        pausePanel.SetActive(isActive);
        Time.timeScale = isActive ? 0.0f : 1.0f;
        pauseBtn.gameObject.SetActive(isActive ? false : true);
        hiroshimaBtn.interactable = isActive ? false : true;
    }

    // BUTTON LISTENERS
    void InitButtonsListener()
    {      
        //GameOver btn
        restartBtn.onClick.AddListener(RestartGame);
        toMenuBtn.onClick.AddListener(BackToMenu);
        //Pause btn
        pauseBtn.onClick.AddListener(GamePaused);
        continuBtn.onClick.AddListener(ContinuGame);
        pauseToMenuBtn.onClick.AddListener(BackToMenu);
    }

    // Affiche et augmente le score du joueur a l'ecran durant une partie
    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        scoreText.text = "Total score:  " + score;

        scorePerRound += newScoreValue;
        scoreRoundText.text = "Round score:  " + scorePerRound + " / " + scoreMinimum;
        if (scorePerRound >= scoreMinimum && scorePerRound != 0) 
        { 
            scoreRoundText.color = Color.green; 
        }
    }

    // INCREASE ENERGY - for the second weapon
    public void increaseEnergyBar()
    {
        if (turboEnabled) return;
        energyPercent += 10.0f;
        energyPercent = Mathf.Min(energyPercent, 100);

        if (energyPercent >= 95)
        {
            energyPercent = 100;
            turboCount = 0;
            turboEnabled = true;
            Animation anim = energyProgressBar.GetComponent<Animation>();
            anim.enabled = true;
            anim.Play();
        }

        float x_position = -25 + 25 * (energyPercent / 100);
        energyProgressBar.transform.position = new Vector3(x_position, 0, energyProgressBar.transform.position.z);
    }

    // DECREASE ENERGY BAR - for the second weapon
    public void decreaseEnergyBar()
    {
        turboCount++;
        energyPercent -= Mathf.Pow(turboCount / 100, 1.95f);
        energyPercent = Mathf.Max(energyPercent, 0);
        if (energyPercent < 12)
        {
            energyPercent = 0;
            turboEnabled = false;
            energyProgressBar.GetComponent<Animation>().Stop();
        }
        float x_position = -25 + 25 * (energyPercent / 100);
        energyProgressBar.transform.position = new Vector3(x_position, 0, energyProgressBar.transform.position.z);
    }

    //ENNEMIES KILLED - EACH ROUND
    public void enemyToKill()
    {
        totalEnemiesKilledText.text = "Total kill:  " + totalEnemiesKilled++;
    }

    // PREPARE HIROSHIMA
    public void preparingHiroshima()
    {
        if (enemyKilledHiroshima <= enemyToKillBeforeHiroshima)
        {
            enemyKilledHiroshima++;
            hiroshimaBtnText.GetComponent<Text>().text = enemyKilledHiroshima.ToString() + " / " + enemyToKillBeforeHiroshima.ToString();
        }
        if (enemyKilledHiroshima >= enemyToKillBeforeHiroshima)
        {
            hiroshimaBtn.interactable = true;
            hiroshimaBtn.GetComponent<Image>().color = new Color32(247, 193, 54, 255);
        }
    }

    // TRIGGER HIROSHIMA
    public void triggerHiroshima()
    {
        foreach (GameObject hazard in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            AddScore(10);
            Instantiate(explosion, hazard.transform.position, hazard.transform.rotation);
            Destroy(hazard);
        }

        enemyKilledHiroshima = 0;
        energyPercent = 0;

        turboEnabled = false;
        hiroshimaBtn.interactable = false;

        hiroshimaBtnText.GetComponent<Text>().text = "0 / " + enemyToKillBeforeHiroshima.ToString();
        hiroshimaBtn.GetComponent<Image>().color = new Color32(247, 193, 54, 80);
        energyProgressBar.GetComponent<Animation>().Stop();
    }

    // WAVES - Enemies and obstacles
    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            enemiesLeft = OptionsController.hazardCount;

            for (int i = 0; i < OptionsController.hazardCount; i++)
            {
                enemiesUntilNextRound.text = "Enemies left:  " + (enemiesLeft-- -1);
                GameObject hazard = hazards[Random.Range(0, OptionsController.hazardsSize)];

                //Enemy spawn près du joueur (+/- averageSpawnPos) sans depasser les limites imposé 
                float nearPlayerMin = PlayerController.playerPosition.x - averageSpawnPos;
                float nearPlayerMax = PlayerController.playerPosition.x + averageSpawnPos;
                float spawnNearPlayerX; 

                //Varie le focus d'apparition près du joueur
                if(Random.value > 0.30f) { spawnNearPlayerX = Random.Range(nearPlayerMin, nearPlayerMax); }
                else { spawnNearPlayerX = Random.Range(boundary.xMin, boundary.xMax); }

                //Position limite d'apparition d'ennemies
                Vector3 spawnPosition = new Vector3(Mathf.Clamp(spawnNearPlayerX, boundary.xMin, boundary.xMax), 0.0f, boundary.zMax);             
                Quaternion spawnRotation = Quaternion.identity;

                //Apparition des ennemis 
                Instantiate(hazard, spawnPosition, spawnRotation);
                yield return new WaitForSeconds(Random.Range(spawnWait - 0.2f, spawnWait + 0.2f));
            }
            yield return new WaitForSeconds(waveWait);

            //Si le joueuer ne reussi pas a obtenir le score pour une manche
            if (scorePerRound < scoreMinimum)
            {
                //La partie termine
                GameOverSettings(true);
                foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
                    //Le joueur meurt
                    Instantiate(explosion, player.transform.position, player.transform.rotation);
                    Destroy(player);
                }
            }
            //Reinitialise les valeurs pour la nouvelle manche        
            else
            {
                scorePerRound = 0;
                enemiesLeft = 0;

                scoreRoundText.color = Color.white;
                enemiesUntilNextRound.text = "Enemies left:  0";
                scoreRoundText.text = "Round score:  0 / " + scoreMinimum;
                roundText.text = "Round:  " + round++;
            }

            //Cesse l'arrivee de nouvelles vagues d'ennemies
            if (isGameOver)
            {
                break;
            }
        }
    }

    void Update()
    {
        // Label de la barre d'energie
        energyText.text = "Energy:  " + energyPercent.ToString() + " %";

        // Retire le JoyStick si l'accelerometre est activée
        if (OptionsController.isAccelSelected)
        {
            fixedJoystick.SetActive(false);
        }
    }
}