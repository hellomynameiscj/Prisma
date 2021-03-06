﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using GooglePlayGames;

[RequireComponent(typeof(LoadSceneAsync))]
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Enumerated type to represent the state of the game as either Paused or Running
    /// </summary>
    public enum State
    {
        Paused, Running
    }
    public static GameManager instance;
    public int orbsCollected = 0;
    public float specialCharge = 0;
    public const int SPECIAL_CHARGE_TARGET = 1000;
    public bool canSpecialAtk = false;
    public int enemiesOnScreen = 0;
    public int stage;
    public bool isTutorial;
    public bool isBulletTime;

    public int ORB_COUNT_TARGET = 20;

    public Text orbCountDisp;
    public Slider chargeBar;
    public Text healthDisp;
    public Text powerupCountdown;
    public Text levelDisp;
    public string nextScene;
    public LoadSceneAsync lsa;
    public int currentLevel;
    public GameObject focus;
    public GameObject player;
    public SkinnedMeshRenderer skin;
    public SkinnedMeshRenderer pauseSkin;
    public Material[] materials;
    public Material[] pauseMaterials;
    public Texture[] textures;

    private Text stageText;
    private GameObject stageImage;
    private GameObject pausedCharacter;

    private const float POWERUP_TIME = 10f;

    private State state;

    public State getState()
    {
        return state;
    }

    public void SetState(State setState)
    {
        state = setState;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);  // This enforces Singleton pattern
        }
        DontDestroyOnLoad(gameObject);
        InitGame();
    }

    void Start()
    {
        PlayGamesPlatform.Activate();

        Social.localUser.Authenticate((bool success) =>
        {
        });
    }

    void InitGame()
    {
        // Initialises the stage indicator text
        if (!isTutorial)
        {
            stageImage = GameObject.Find("StageImage");
            stageText = GameObject.Find("StageText").GetComponent<Text>();
            if (currentLevel == -1)
            {
                stageText.text = "Boss Stage";
            }
            else { stageText.text = "Stage " + currentLevel; }

            stageImage.SetActive(true);
            Invoke("HideStageImage", 2);
        }

        // initialise values for starting game
        orbsCollected = 0;
        specialCharge = 0;
        enemiesOnScreen = 0;
        orbCountDisp.text = "0 / " + ORB_COUNT_TARGET.ToString();
        powerupCountdown.text = "";
        canSpecialAtk = false;
        chargeBar.maxValue = SPECIAL_CHARGE_TARGET;  // set max value of attack charge slider
        state = State.Running;

        // Grabs the player game object
        var i = GameControl.control != null ? GameControl.control.playerSprite : 1;
        player = GameObject.Find("Player");
        pausedCharacter = GameObject.Find("PauseScreenPlayer");
        skin = player.transform.FindChild("p_sotai").GetComponent<SkinnedMeshRenderer>();
        pauseSkin = pausedCharacter.transform.FindChild("p_sotai").GetComponent<SkinnedMeshRenderer>();
        materials = skin.materials;
        pauseMaterials = pauseSkin.materials;
        // Renders the player with different skins depending on the user's choice
        if (i == 1)
        {
            for (i = 0; i < 4; i++)
            {
                pauseMaterials[i].mainTexture = Resources.Load("ch034", typeof(Texture2D)) as Texture2D;
                materials[i].mainTexture = Resources.Load("ch034", typeof(Texture2D)) as Texture2D;
            }




        }
        else if (i == 2)
        {
            for (i = 0; i < 4; i++)
            {
                pauseMaterials[i].mainTexture = Resources.Load("ch037", typeof(Texture2D)) as Texture2D;
                materials[i].mainTexture = Resources.Load("ch037", typeof(Texture2D)) as Texture2D;
            }
        }
        else if (i == 3)
        {
            for (i = 0; i < 4; i++)
            {
                pauseMaterials[i].mainTexture = Resources.Load("ch029", typeof(Texture2D)) as Texture2D;
                materials[i].mainTexture = Resources.Load("ch029", typeof(Texture2D)) as Texture2D;
            }
        }


    }
    void Update()
    {
        if (GameManager.instance.isPaused())
        {
            // Enables the paused player sprite to be true when paused
            GameObject.Find("PauseScreenPlayer").GetComponent<Animator>().enabled = true;
            return;
        }
        countEnemies();
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        //update health counter
        healthDisp.text = "x " + player.currentHealth;
        orbCountDisp.text = orbsCollected.ToString() + " / " + ORB_COUNT_TARGET.ToString(); //update orb counter text
        chargeBar.value = specialCharge;  // set value of special attack slider
        levelDisp.text = "Lvl: " + GameControl.control.playerLevel; //update player level text
        //health check
        if (player.currentHealth <= 0)
        {
            gameOver();
        }
        //orb check
        if (orbsCollected >= ORB_COUNT_TARGET)
        {
            levelCleared();
        }
        //special attack check
        canSpecialAtk = specialCharge >= SPECIAL_CHARGE_TARGET ? true : false;  // set boolean true if player can special attack
        if (!canSpecialAtk)
        {
            incrementSpecialAtkCounter(player);
        }


    }


    void HideStageImage()
    {
        stageImage.SetActive(false);
    }

    private void countEnemies()
    {
        enemiesOnScreen = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    /// <summary>
    /// Charges special attack bar for player. The attack bar is incremented based on the player's intelligence stat.
    /// </summary>
    /// <param name="player"></param>
    public void incrementSpecialAtkCounter(Player player)
    {
        specialCharge += (float)(player.intelligence) / 5;
    }

    public void resetSpecialAtkCounter()
    {
        specialCharge = 0;
    }

    void levelCleared()
    {
        GameControl.control.SaveToCloud();
        // only moves up the current level if its the current 
        if (currentLevel == GameControl.control.currentGameLevel)
        {
            if (currentLevel == 0)
            {
                if (Social.localUser.authenticated)
                {
                    Social.ReportProgress("CgkIpKjLyoEdEAIQAg", 100.0f, (bool success) =>
                    {
                    });
                }
            }

            if (currentLevel == 1)
            {
                if (Social.localUser.authenticated)
                {
                    Social.ReportProgress("CgkIpKjLyoEdEAIQBA", 100.0f, (bool success) =>
                    {
                    });
                }
            }
            GameControl.control.currentGameLevel = GameControl.control.currentGameLevel + 1;
        }
        lsa.ClickAsync(nextScene);
    }

    void gameOver()
    {
        Application.LoadLevel("game_over_screen");
    }

    /// <summary>
    /// Checks if the game is in a pause state
    /// </summary>
    /// <returns></returns>
    public bool isPaused()
    {
        if (GameManager.instance.getState().Equals(GameManager.State.Paused))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Activates Focus mode
    /// </summary>
    public void activateBulletTime()
    {
        if (isBulletTime)
            return;
        isBulletTime = true;
        StartCoroutine(StartBulletTime());
    }

    /// <summary>
    /// Coroutine that sets Focus as active for 10 seconds
    /// </summary>
    /// <returns></returns>
    IEnumerator StartBulletTime()
    {
        isBulletTime = true;
        // go through countdown timer
        for (int i = (int)POWERUP_TIME; i > 0; i--)
        {
            powerupCountdown.text = "" + i;
            yield return new WaitForSeconds(1f);
        }
        powerupCountdown.text = "";  // rest countdown timer text
        isBulletTime = false;
    }
}