using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;
using static BuffAttributes;

public class PathHolder : MonoBehaviour // was intended to just hold the paths etc but then it turned into a manager lol
{

    public static PathHolder instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<Vector3> Positions;
    public List<BaseEnemy> Enemies;
    public bool WaveInProgress = false;
    public List<WaveInfo> WaveData;
    public Transform WaveSpawnpoint;
     public Vector3 StartPosition;
    public int Round = 0;
    public bool RoundFinishedSpawning = true;
    public bool RoundCleared = false;
    public bool AutoRoundStart = false;
    public GameObject RoundStartButton;
    //   public Image RoundStart;
    //  public Image RoundPause;
    public GameObject AutoStartButton;
    public GameObject HintOverlay;

    public bool DraggingTower = false;

    public BaseTower SelectedTower;
    public GameObject RangeVisual;
    public Material GlowMat;
    Material SelectedTowerMat;
    public List<BaseTower> PlacedTowers;

    public TextMeshProUGUI HPText;
    public TextMeshProUGUI MoneyText;
    public TextMeshProUGUI RoundText;

    public Transform UpgradeUIInitalPos;
    public GameObject UpgradeUITemplate;
    public List<GameObject> UpgradeUIList;


 
    public float ThemeFadeTime = 2.0f;
     bool Fading = false;


    public int Money = 0;
    public float Lives = 100;

  //  public LevelInfo CurrentLevel;
 
    public VictoryScreenScript WinScript;
    public GameObject FailureScreen;
   

    //do before everything else
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
       
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Positions.Add(this.transform.GetChild(i).position);
        }
        StartPosition = WaveSpawnpoint.position;
        ShowUpgradeUI(false);
        RangeVisual.SetActive(false);
    }

    void Start()
    {
        if (LevelManager.instance)
        {
            GetComponent<AudioSource>().clip = LevelManager.instance.CurrentLevel.LobbyTheme;
            GetComponent<AudioSource>().Play();
        }
        if (AutoStartButton)
        {
            if (AutoRoundStart == true)
            {
                AutoStartButton.GetComponent<Image>().color = Color.white;
            }
            else
            {
                AutoStartButton.GetComponent<Image>().color = Color.gray;
            }
        }

        if (LevelManager.instance && LevelManager.instance.CurrentDifficulty)
        {
            Lives = LevelManager.instance.CurrentDifficulty.StartingHealth;
            Money = LevelManager.instance.CurrentDifficulty.StartingCash;
           
        }
        if (WinScript)
        {
            WinScript.gameObject.SetActive(false);
        }
        if (FailureScreen)
        {
            FailureScreen.SetActive(false);
        }
        UpdateLivesCounter();
        UpdateMoneyCounter();
        UpdateRoundCounter();
        //  Round = -1;
        if (RoundText)
        {
            RoundText.text = "Setup Your Defenses!";
        }
       
        //StartNewRound();
    }


    public void OnlyPlayAudioSourceWhenRoundOver(AudioSource SourceRef)
    {
        if (RoundFinishedSpawning == true)
        {
            SourceRef.Play();
        }
    }

    public void ToggleAutoRound()
    {
        
           AutoRoundStart = !AutoRoundStart;
            if (AutoStartButton)
            {
                if (AutoRoundStart == true)
                {
                    AutoStartButton.GetComponent<Image>().color = Color.white;
                }
                else
                {
                    AutoStartButton.GetComponent<Image>().color = Color.gray;
                }
            }
        
    }

    public void ButtonStartNewRound()
    {
        Debug.Log("CLICK!");
        StartCoroutine(FadeBetweenTracks(LevelManager.instance.CurrentLevel.RoundTheme));
        StartNewRound();
    }
     void StartNewRound()
    {

        UpdateRoundCounter();
        Enemies.TrimExcess();
        if (Enemies.Count <= 0 && RoundFinishedSpawning == true && Round < WaveData.Count)
        {
           
            if (RoundStartButton)
            {


                RoundStartButton.GetComponent<Image>().color = Color.gray;

            }
           // if (Round < WaveData.Count)
            //{
              
                RoundFinishedSpawning = false;
                WaveInProgress = true;

           
            if (WaveData[Round].DialogueToLoad != null && WaveData[Round].DialogueToLoad.Length > 0
                && DialogueHolder.instance)
            {
                Debug.Log("MAKE SURE TO SPAWN ROUNDS IN VIA YARN AFTER!");
                DialogueHolder.instance.DialogueRunner.GetComponent<DialogueRunner>().Stop();

      
                DialogueHolder.instance.DialogueRunner.GetComponent<DialogueRunner>().StartDialogue(WaveData[Round].DialogueToLoad);
               
            }
            else
            {
                StartCoroutine("SpawnInEnemies");
            }
            //}
            /*
            else
            {
                RoundStartButton.GetComponent<Button>().enabled = false;
                RoundStartButton.GetComponent<Image>().color = Color.gray;
                
                
            }
            */
        }
    }

  public void ShowUpgradeUI(bool Active)
    {
        if (Active)
        {
            //UpgradeUIInitalPos.parent.gameObject.SetActive(true);
            Debug.Log("ShowUI.." + SelectedTower.CurrentlyBuyableUpgrades.Count);
            for (int i = 0; i < SelectedTower.CurrentlyBuyableUpgrades.Count; i++)
            {
              //  Debug.Log("And it loops through" + i);
                Vector3 UIButtonPos = UpgradeUIInitalPos.position;
                UIButtonPos.z += (-10 * i);
                UpgradeUIList.Add(Instantiate<GameObject>(UpgradeUITemplate, UIButtonPos, UpgradeUIInitalPos.rotation, UpgradeUIInitalPos.parent));
                UpgradeUIList[i].GetComponent<ApplyUpgrade>().UpgradeData = SelectedTower.CurrentlyBuyableUpgrades[i];
                UpgradeUIList[i].GetComponent<Image>().sprite = SelectedTower.CurrentlyBuyableUpgrades[i].UpgradeImage;
            }
        }
        else
        {
            //UpgradeUIInitalPos.parent.gameObject.SetActive(false);
            for (int i = 0; i < UpgradeUIList.Count; i++)
            {
                Destroy(UpgradeUIList[i]);
            }
            UpgradeUIList.Clear();
        }
    }

    public void SelectTower(GameObject SelectedObject)
    {
       
            if (DraggingTower == false && SelectedTower == null && SelectedObject.GetComponent<BaseTower>())
            {
           
            SelectedTower = SelectedObject.GetComponent<BaseTower>();
                SelectedTowerMat = SelectedTower.gameObject.GetComponentInChildren<SpriteRenderer>().material;
                SelectedTower.gameObject.GetComponentInChildren<SpriteRenderer>().material = GlowMat;
            
            InitRangeVisual(SelectedObject);
            ShowUpgradeUI(true);
                Debug.Log("Selected!");
            }
            else if (SelectedTower)
            {
            RangeVisual.SetActive(false);
            SelectedTower.gameObject.GetComponentInChildren<SpriteRenderer>().material = SelectedTowerMat;
                SelectedTowerMat = null;
                SelectedTower = null;
            ShowUpgradeUI(false);
            Debug.Log("Unselected!");
            }
            

            //   {
        
        
    }

    //rescales the range visual so we can call it from upgrading towers
    public void InitRangeVisual(GameObject SelectedObject)
    {
        Debug.Log("Resize?");
        RangeVisual.SetActive(false);
        RangeVisual.transform.position = SelectedObject.transform.position;
        RangeVisual.transform.localScale = Vector3.one;
        RangeVisual.transform.localScale *= SelectedTower.Range * 2;
        RangeVisual.SetActive(true);
    }

    [YarnCommand("SpawnWave")]
    IEnumerator SpawnInEnemies()
    {
        RoundCleared = false;
        yield return new WaitForSeconds(1.25f); // delay just incase lol
        for (int i = 0; i < WaveData[Round].WaveOrder.Count; i++)
        {

            if (WaveData[Round].WaveOrder[i].AmountOfEnemy <= 1)
            {
                Enemies.Add(
                    Instantiate(WaveData[Round].WaveOrder[i].Enemy, StartPosition, Quaternion.identity).GetComponent<BaseEnemy>()
                    );
            }
            else
            {
                for (int j = 0; j < WaveData[Round].WaveOrder[i].AmountOfEnemy; j++)
                {
                    Enemies.Add(
                    Instantiate(WaveData[Round].WaveOrder[i].Enemy, StartPosition, Quaternion.identity).GetComponent<BaseEnemy>()
                    );
                    yield return new WaitForSeconds(WaveData[Round].WaveOrder[i].SwarmDelay);
                }
            }
               // Enemies[i].transform.position = StartPosition;
            
            yield return new WaitForSeconds(WaveData[Round].WaveOrder[i].DelayUntilNextEnemyType);
        }
        yield return new WaitForSeconds(1f);
        RoundFinishedSpawning = true;
        EnemyCheck();
    }


  public  void RemoveEnemies(GameObject enemytodelete)
    {
        

        if (enemytodelete != null)
        {

            enemytodelete.GetComponent<MeshRenderer>().enabled = false;
            enemytodelete.GetComponent<BaseEnemy>().enabled = false;

            if (enemytodelete.GetComponent<BaseEnemy>().DeathSound && enemytodelete.GetComponent<AudioSource>())
            {
                enemytodelete.GetComponent<AudioSource>().PlayOneShot(enemytodelete.GetComponent<BaseEnemy>().DeathSound);
                Destroy(enemytodelete, enemytodelete.GetComponent<BaseEnemy>().DeathSound.length);

            }
            else
            {
                DestroyImmediate(enemytodelete);
            }
            
        }
    
       // Enemies.RemoveAll(BaseEnemy => BaseEnemy == null);

        
        for (int i = 0; i < Enemies.Count; i++)
        {
            if (Enemies[i].gameObject == enemytodelete ||Enemies[i] == null || Enemies[i].Health <= 0) // should hopefully fix issues where the list never gets cleared?
            {
                Debug.Log("REMOVING" + Enemies[i]);
                Enemies.RemoveAt(i);
            }
        }
        // Enemies.TrimExcess();

        EnemyCheck();
        /*
        //update text here idk
        if (Enemies.Count <= 0 && RoundFinishedSpawning == true)
        {
            Debug.Log("ROUND CLEAR");

            


            Round++;
            UpdateRoundCounter();
            CleanupAllTowerProjectiles();
           
            if (RoundStartButton)
            {


                RoundStartButton.GetComponent<Image>().color = Color.white;

            }
            if (AutoRoundStart)
            {
                StartNewRound();
            }
            else
            {
                StartCoroutine(FadeBetweenTracks(LobbyTheme));
            }
        }
        */
    }


    public void LevelSwapPassthrough(string LevelName) // since level manager is made at the start of main menu, use this as a passthrough
    {
        if (LevelManager.instance)
        {
            LevelManager.instance.LoadLevel(LevelName);
        }
    }

    public void ReloadLevel() // since level manager is made at the start of main menu, use this as a passthrough
    {
        if (LevelManager.instance)
        {
            LevelManager.instance.LoadLevel(SceneManager.GetActiveScene().name);
        }
    }


    public void EnemyCheck()
    {
        Enemies.RemoveAll(s => s == null);
        if (Enemies.Count <= 0 && RoundFinishedSpawning == true && RoundCleared == false)
        {
            Debug.Log("ROUND CLEAR");
            RoundCleared = true;



            
            CleanupAllTowerProjectiles();

            if (RoundStartButton)
            {


                RoundStartButton.GetComponent<Image>().color = Color.white;

            }
            



                if (Round + 1  < WaveData.Count)
                {
                Round++;
                UpdateRoundCounter();

                if (AutoRoundStart && Lives > 0)
                         {
                    StartNewRound();
                         }
                             else
                         {
                    StartCoroutine(FadeBetweenTracks(LevelManager.instance.CurrentLevel.LobbyTheme));
                         }
                   }
                else
                {
                if (WinScript)
                {
                    WinScript.gameObject.SetActive(true);
                    WinScript.VictoryShow();
                }
                else
                {
                    Debug.Log("NO WIN SCRIPT");
                }
                // RoundStartButton.GetComponent<Button>().enabled = false;
                // RoundStartButton.GetComponent<Image>().color = Color.gray;
                if (RoundText)
                {
                    RoundText.text = "YOU WIN!";
                }

                if (LevelManager.instance.CurrentDifficulty)
                {
                    StartCoroutine(FadeBetweenTracks(LevelManager.instance.CurrentDifficulty.VictoryTheme));
                }

                    RoundStartButton.GetComponent<Button>().enabled = false;
                    RoundStartButton.GetComponent<Image>().color = Color.red;
                AutoStartButton.GetComponent<Image>().color = Color.red;
                AutoStartButton.GetComponent<Button>().enabled = false;

               
               

                }


         }

           

            
        
    }

    void CleanupAllTowerProjectiles()
    {
        for (int i = 0; i < PlacedTowers.Count; i++)
        {
            PlacedTowers[i].CleanupProjectiles();
        }
    }


    public void UpdateLivesCounter()
    {
        HPText.text = "HP" + Lives.ToString();
    }
    public void UpdateMoneyCounter()
    {
        MoneyText.text = "$" + Money.ToString();
    }

    public void UpdateRoundCounter()
    {
        if (RoundText)
        {
            RoundText.text = "Round " + Round.ToString();
        }
    }
    public void HurtPlayer()
    {
        Lives -= 1;
        UpdateLivesCounter();
        EnemyCheck();
        if (Lives <= 0)
        {
            Debug.Log("DEATH");
            HPText.text = "GAME OVER";

            RoundStartButton.GetComponent<Button>().enabled = false;
            RoundStartButton.GetComponent<Image>().color = Color.red;
            AutoStartButton.GetComponent<Image>().color = Color.red;
            AutoStartButton.GetComponent<Button>().enabled = false;


           

        

        if (FailureScreen)
            {
                FailureScreen.SetActive(true);
            }
        }
    }


    IEnumerator FadeBetweenTracks(AudioClip ClipToSwapTo)
    {
        if (this.GetComponent<AudioSource>() && Fading == false && LevelManager.instance.CurrentLevel.LobbyTheme && LevelManager.instance.CurrentLevel.RoundTheme)
        {
            Fading = true;
            AudioSource ManagerSource = this.GetComponent<AudioSource>();
            float MusicVolumeBase = this.GetComponent<AudioSource>().volume;

            while (ManagerSource.volume > 0)
            {
                ManagerSource.volume -= MusicVolumeBase * Time.deltaTime / ThemeFadeTime;

                yield return null;
            }

            ManagerSource.Stop();
            ManagerSource.clip = ClipToSwapTo;
            ManagerSource.volume = 0;
            ManagerSource.Play();
            while (ManagerSource.volume < 1.0f)
            {
                ManagerSource.volume += MusicVolumeBase * Time.deltaTime / ThemeFadeTime;

                yield return null;
            }
            Fading = false;
        }
        else
        {
            Debug.Log("NO AUDIO SOURCE OR FADING");
        }
        yield return new WaitForSeconds(0.25f);
    }

        // Update is called once per frame
        void Update()
    {
      
    }
}
