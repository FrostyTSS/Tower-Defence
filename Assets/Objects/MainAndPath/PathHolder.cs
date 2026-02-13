using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

using static BuffAttributes;

public class PathHolder : MonoBehaviour // was intended to just hold the paths etc but then it turned into a manager lol
{


    //TO FIX:
    //random offset when manual targeting with the range overlay

    public float TESTVALUE = 0;
    public static PathHolder instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<Vector3> Positions;
    public List<BaseEnemy> Enemies;
    public bool WaveInProgress = false;
    public List<WaveInfo> WaveData;
    public Transform WaveSpawnpoint;
    public Vector3 FullscreenParticleSpawnpoint;
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
    public GameObject AimTargetVisual;
    public Color BaseButtonColour = new Color(0.3113208f, 0.3113208f, 0.3113208f);
    public Color GlowColour;
   // public Material GlowMat;
    
    //Material SelectedTowerMat;
    public List<BaseTower> PlacedTowers;

    public TextMeshProUGUI HPText;
    public TextMeshProUGUI MoneyText;
    public TextMeshProUGUI RoundText;

    public Transform UpgradeUIInitalPos;
    public GameObject UpgradeUITemplate;
    public List<GameObject> UpgradeUIList;
    public List<GameObject> TargetSettings;
    public Image TargetingSettingsUIImage;
    public Canvas UICanvas;
    public GameObject TestObj;

    public Camera OrthoCam;
 
    public float ThemeFadeTime = 1.25f;
     bool Fading = false;


    public TextMeshProUGUI AbilityTimerText;
    public Image AbilityImage;
    public GameObject AbilityTimer3D;
    public AudioClip AbilityCooldownFailNoise;
    public Image AbilityTimerGif;

    public int Money = 0;
    public float Lives = 100;

  //  public LevelInfo CurrentLevel;
 
    public VictoryScreenScript WinScript;
    public GameObject FailureScreen;

    public int CurrentLobbyTheme = 0;
    public int CurrentRoundTheme = 1;
        public int CurrentChatTheme = 0;

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
        ShowAbility(false);
        RangeVisual.SetActive(false);
    }

    void Start()
    {
            
        //to do: make a permanent variable instead of spamming instances..

        if (LevelManager.instance)
        {
            if (LevelManager.instance.CurrentLevel)
            {
                CurrentLobbyTheme = LevelManager.instance.CurrentLevel.StartingLobbyTheme;
                CurrentRoundTheme = LevelManager.instance.CurrentLevel.StartingRoundTheme;
                CurrentChatTheme = LevelManager.instance.CurrentLevel.StartingRoundTheme;
            }

            

            GetComponent<AudioSource>().clip = LevelManager.instance.MusicList[CurrentLobbyTheme];
            GetComponent<AudioSource>().Play();
        }
        

        if (LevelManager.instance && LevelManager.instance.CurrentDifficulty)
        {
            Lives = LevelManager.instance.CurrentDifficulty.StartingHealth;
            Money = LevelManager.instance.CurrentDifficulty.StartingCash;
            if (LevelManager.instance.CurrentDifficulty.StartingHealth <= 1 || LevelManager.instance.CurrentDifficulty.DifficultyOrder >= 3) // lock autostart at difficulty
            {
                AutoRoundStart = true;
                AutoStartButton.GetComponent<Image>().color = Color.crimson;
                AutoStartButton.GetComponent<Button>().enabled = false;
            }
           
        }
        else if (AutoStartButton)
        {
            if (AutoRoundStart == true)
            {
                AutoStartButton.GetComponentInChildren<TextMeshProUGUI>().color = GlowColour;
            }
            else
            {
                AutoStartButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.darkSlateGray;
            }
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
        ToggleTargetOptions(false);


        if (AbilityTimerGif)
        {
            AbilityTimerGif.enabled = false;
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
                AutoStartButton.GetComponentInChildren<TextMeshProUGUI>().color = GlowColour;
            }
                else
                {
                AutoStartButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.darkSlateGray;
            }
            }
        
    }

    public void ButtonStartNewRound()
    {
        Debug.Log("CLICK!");
        
        StartNewRound();
    }


    [YarnCommand("FinishPreroundChat")]

    public void StartEnemySpawningCoroutine()
    {
        DialogueHolder.instance.KillPortraits();
        StartCoroutine("SpawnInEnemies");
    }

    void StartNewRound()
    {

        UpdateRoundCounter();
        Enemies.TrimExcess();
        if (Enemies.Count <= 0 && RoundFinishedSpawning == true && Round < WaveData.Count)
        {
           
            if (RoundStartButton)
            {


                RoundStartButton.GetComponentInChildren<TextMeshProUGUI>().color = GlowColour;

            }
           // if (Round < WaveData.Count)
            //{
              
                RoundFinishedSpawning = false;
                WaveInProgress = true;

           
            if (WaveData[Round].DialogueToLoad != null && WaveData[Round].DialogueToLoad.Length > 0
                && DialogueHolder.instance)
            {
                Debug.Log("MAKE SURE TO SPAWN ROUNDS IN VIA YARN AFTER!");
                DialogueHolder.instance.DialogueRunnerScript.Stop();
                /*
                DialogueHolder.instance.PlayerPortrait.SetActive(true);
                DialogueHolder.instance.NPCPortrait.SetActive(true);
                if (DialogueHolder.instance.BackgroundImage)
                {
                    DialogueHolder.instance.BackgroundImage.SetActive(true);
                }
                */
                if (LevelManager.instance)
                {
                    StartCoroutine(FadeBetweenTracks(LevelManager.instance.MusicList[CurrentChatTheme]));
                }
                DialogueHolder.instance.EnablePortraits();
                DialogueHolder.instance.DialogueRunnerScript.StartDialogue(WaveData[Round].DialogueToLoad);
               
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


    public void ShowAbility(bool Active)
    {
        if (Active)
        {
            if (SelectedTower && SelectedTower.Ability)
            {
                AbilityImage.sprite = SelectedTower.Ability.AbilityImage;
               if (AbilityTimerGif && SelectedTower.TowerAbilityCooldown > 0)
                {
                    AbilityTimerGif.enabled = true;
                }

            }
        }
        else
        {
            //UpgradeUIInitalPos.parent.gameObject.SetActive(false);
            AbilityImage.color = new Color(0f, 0f, 0f, 0f); //dont crash pls :)
            AbilityTimerText.text = "No Tower Selected";
            AbilityTimerGif.enabled = false;
        }
    }

    public void UseAbility()
    {
        Debug.Log("Ability Use");
        if (SelectedTower)
        {
            if (SelectedTower.TowerAbilityCooldown <= 0)
            {
                GameObject AbilityTextRef = Instantiate(AbilityTimer3D, SelectedTower.transform.position + AbilityTimer3D.transform.position, AbilityTimer3D.transform.rotation);

                SelectedTower.GetComponent<BaseTower>().AbilityTimerText = AbilityTextRef.GetComponent<TextMeshPro>();

                if (SelectedTower.Ability.AbilityActivateNoise)
                {
                    
                    GetComponent<AudioSource>().PlayOneShot(SelectedTower.Ability.AbilityActivateNoise);
                }
                SelectedTower.UseAbility();
                AbilityTimerGif.enabled = true;
            }
            else
            {
                GetComponent<AudioSource>().PlayOneShot(AbilityCooldownFailNoise);
            }
        }
    }

    public void ChangeTargetMode(int ID)
    {
        if (SelectedTower)
        {
            switch (ID)
            {
                case 0:
                    SelectedTower.CurrentTargetingMode = BaseTower.TargetMode.First;
                    AimTargetVisual.SetActive(false);
                    break;

                case 1:
                    SelectedTower.CurrentTargetingMode = BaseTower.TargetMode.Last;
                    AimTargetVisual.SetActive(false);
                    break;

                case 2:
                    SelectedTower.CurrentTargetingMode = BaseTower.TargetMode.Close;
                    AimTargetVisual.SetActive(false);
                    break;

                case 3:
                    SelectedTower.CurrentTargetingMode = BaseTower.TargetMode.Manual;
                    AimTargetVisual.SetActive(true);
                    AimTargetVisual.GetComponent<TargetSprite>().SwapTargetIcon(0);
                    CheckTargetRangeOnce();

                    AimTargetVisual.GetComponent<RectTransform>().position = SelectedTower.ManualTargetPosUI;
                    // CheckTargetRangeOnce();
                    break;

                default:
                    break;
            }
            ToggleTargetOptions(true);
        }
    }

     void ToggleTargetOptions(bool On)
    {
        //text colour is 1EFF00 in hex, or 0.1170101 1 0 for RGB
        if (SelectedTower && On)
        {
            for (int i = 0; i < TargetSettings.Count; i++)
            {
                if (TargetSettings[i].GetComponent<Button>())
                {
                    GameObject CurrentButton = TargetSettings[i]; // now how in sam hill do I figure out which is which..?

                    Debug.Log(CurrentButton.name + " < obj name and target name >" + SelectedTower.CurrentTargetingMode.ToSafeString());

                    if (SelectedTower.RotateToShoot == false || SelectedTower.ProjectileType == null) // disable target selection since it doesn't "aim" properly
                    {
                        CurrentButton.GetComponent<Button>().enabled = false;
                        CurrentButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.crimson;
                    }
                    
                    else if (CurrentButton.name == SelectedTower.CurrentTargetingMode.ToSafeString()) //this'll do cause I'm lazy..
                    {
                        //CurrentButton.GetComponent<Image>().color = Color.white;
                        CurrentButton.GetComponentInChildren<TextMeshProUGUI>().color = GlowColour;
                        CurrentButton.GetComponent<Button>().enabled = true;
                    }
                    else
                    {
                        CurrentButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.darkSlateGray;
                        CurrentButton.GetComponent<Button>().enabled = true;
                        //CurrentButton.GetComponent<Image>().color = Color.gray;
                    }

                }
            }
        }
        else
        {
            for (int i = 0; i < TargetSettings.Count; i++)
            {
                if (TargetSettings[i].GetComponent<Button>())
                {
                    GameObject CurrentButton = TargetSettings[i]; // now how in sam hill do I figure out which is which..?


                    CurrentButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.darkSlateGray;
                        CurrentButton.GetComponent<Image>().color = Color.gray;
                    

                }
            }
        }
    }

    public void SelectTower(GameObject SelectedObject)
    {
       
            if (DraggingTower == false && SelectedTower == null && SelectedObject.GetComponent<BaseTower>())
            {
           
            SelectedTower = SelectedObject.GetComponent<BaseTower>();
                //SelectedTowerMat = SelectedTower.gameObject.GetComponentInChildren<SpriteRenderer>().material;
                SelectedTower.gameObject.GetComponentInChildren<SpriteRenderer>().color = GlowColour;
            TargetingSettingsUIImage.sprite = AimTargetVisual.GetComponent<TargetSprite>().GoodTarget;
            TargetingSettingsUIImage.color = Color.white;
            InitRangeVisual(SelectedObject);
            ToggleTargetOptions(true);


           
            if (SelectedTower.CurrentTargetingMode == BaseTower.TargetMode.Manual)
            {
                AimTargetVisual.SetActive(true);
                AimTargetVisual.GetComponent<TargetSprite>().SwapTargetIcon(0);
                CheckTargetRangeOnce();
                
                AimTargetVisual.GetComponent<RectTransform>().position = SelectedTower.ManualTargetPosUI;
                //AimTargetVisual.transform.position = new Vector3(SelectedTower.ManualTargetPos.x, SelectedTower.transform.position.y + 5, SelectedTower.ManualTargetPos.z);

            }
            else
            {
                AimTargetVisual.SetActive(false);
            }
            ShowUpgradeUI(true);
            if (SelectedTower.Ability)
            {
                ShowAbility(true);
            }
                Debug.Log("Selected!");
            }
         //   else if (SelectedTower)
         else if (SelectedTower)
            {
            RangeVisual.SetActive(false);
            TargetingSettingsUIImage.sprite = AimTargetVisual.GetComponent<TargetSprite>().BadTarget;
            TargetingSettingsUIImage.color = Color.gray;
            SelectedTower.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
              //  SelectedTowerMat = null;
                SelectedTower = null;
            ShowUpgradeUI(false);
            ToggleTargetOptions(false);
            Debug.Log("Unselected!");
            AimTargetVisual.SetActive(false);
            
                ShowAbility(false);
            
        }
            

            //   {
        
        
    }

    //rescales the range visual so we can call it from upgrading towers
     public void InitRangeVisual(GameObject SelectedObject)
    {
        Debug.Log("Resize?");
        RangeVisual.SetActive(false);

       // RangeVisual.transform.position = TruePosition(SelectedObject.transform.position);

        
        RangeVisual.transform.position = SelectedObject.transform.position;
        
        RangeVisual.transform.localScale = Vector3.one;
       RangeVisual.layer = 10; //set it to ortho only so it renders properly no matter where it is
        RangeVisual.transform.localScale *= SelectedTower.Range * 1.95f;
        //PathHolder.instance.RangeVisual.transform.localScale *= SelectedObject.GetComponent<BaseTower>().Range * (UICanvas.GetComponent<Canvas>().worldCamera.fieldOfView * 0.8f);

        RangeVisual.SetActive(true);
    }

    [YarnCommand("SpawnWave")]
    IEnumerator SpawnInEnemies()
    {


        if (LevelManager.instance)
        {
            StartCoroutine(FadeBetweenTracks(LevelManager.instance.MusicList[CurrentRoundTheme]));
        }
        RoundCleared = false;
        yield return new WaitForSeconds(1.25f); // delay just incase lol
        for (int i = 0; i < WaveData[Round].WaveOrder.Count; i++)
        {

            if (WaveData[Round].WaveOrder[i].AmountOfEnemy <= 1) // optimisation?
            {
                Enemies.Add(
                    Instantiate(WaveData[Round].WaveOrder[i].Enemy, StartPosition, Quaternion.identity).GetComponent<BaseEnemy>()
                    );
                Enemies[Enemies.Count - 1].PathManager = this;
                if (WaveData[Round].WaveOrder[i].EnemyCamo && Enemies[Enemies.Count - 1].CamoOverlay)
                {
                    Enemies[Enemies.Count - 1].Camo = true;
                    Enemies[Enemies.Count - 1].CamoOverlay.SetActive(true);
                }
            }
            else
            {
                for (int j = 0; j < WaveData[Round].WaveOrder[i].AmountOfEnemy; j++)
                {
                    Enemies.Add(
                    Instantiate(WaveData[Round].WaveOrder[i].Enemy, StartPosition, Quaternion.identity).GetComponent<BaseEnemy>()
                    );
                    Enemies[Enemies.Count - 1].PathManager = this;
                    if (WaveData[Round].WaveOrder[i].EnemyCamo && Enemies[Enemies.Count - 1].CamoOverlay)
                    {
                        Enemies[Enemies.Count - 1].Camo = true;
                        Enemies[Enemies.Count - 1].CamoOverlay.SetActive(true);
                    }
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
            enemytodelete.GetComponentInChildren<SpriteRenderer>().enabled = false;
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


     void EnemyCheck()
    {
        Enemies.RemoveAll(s => s == null);
        if (Enemies.Count <= 0 && RoundFinishedSpawning == true && RoundCleared == false)
        {
            Debug.Log("ROUND CLEAR");
            RoundCleared = true;



            
            CleanupAllTowerProjectiles();

            if (RoundStartButton)
            {


                RoundStartButton.GetComponent<TextMeshProUGUI>().color = Color.darkSlateGray;

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
                    StartCoroutine(FadeBetweenTracks(LevelManager.instance.MusicList[CurrentLobbyTheme]));
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
                    RoundStartButton.GetComponent<TextMeshProUGUI>().color = Color.red;
                AutoStartButton.GetComponent<TextMeshProUGUI>().color = Color.red;
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
        // HPText.text = "HP" + Lives.ToSafeString();
        HPText.text = Lives.ToSafeString();
    }
    public void UpdateMoneyCounter()
    {
        //MoneyText.text = "$" + Money.ToString();
        MoneyText.text =  Money.ToSafeString();
    }

    public void UpdateRoundCounter()
    {
        if (RoundText)
        {
            RoundText.text = (Round + 1).ToSafeString();
            //RoundText.text = "Round " + (Round + 1).ToSafeString();
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


    [YarnCommand("MusicSwap")]
    public void MusicSwap(int TypeOfMus, int MusID) // 0 for lobby, 1 for round, 2 for chat
    {

        if (MusID >= 0 && MusID < LevelManager.instance.MusicList.Count)
        {
            switch (TypeOfMus)
            {
                case 0:
                    CurrentLobbyTheme = MusID;
                    break;
                case 1:
                    CurrentRoundTheme = MusID;
                    break;
                case 2:
                    CurrentChatTheme = MusID;
                    break;


                default:
                    break;
            }


            StartCoroutine(FadeBetweenTracks(LevelManager.instance.MusicList[MusID]));
        }

        
    }


    IEnumerator FadeBetweenTracks(AudioClip ClipToSwapTo)
    {
        if (this.GetComponent<AudioSource>() && Fading == false && LevelManager.instance.MusicList.Count > 0)
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



 //   TargetSettingHolder

   

    public void SetTargetColourAndPos(RaycastHit hit)
    {

        //AimTargetVisual.transform.position = new Vector3(hit.point.x, SelectedTower.transform.position.y + 2, hit.point.z);

        // Vector3 Point = new Vector3(hit.point.x, SelectedTower.transform.position.y, hit.point.z);
        Vector3 Point = new Vector3(hit.point.x, SelectedTower.transform.position.y, hit.point.z);
       
        Debug.Log( Vector3.Distance(SelectedTower.transform.position, Point) + " and the z hit is" + hit.point.z);

        Vector2 mousepos = Input.mousePosition;
        //mousepos.x += 0.5f; mousepos.y += 0.5f;
       // Ray ray = Camera.main.ScreenPointToRay(mousepos);

        AimTargetVisual.GetComponent<RectTransform>().position = mousepos;
        //AimTargetVisual.GetComponent<RectTransform>().position = Input.mousePosition;
        // if (Vector3.Distance(SelectedTower.transform.position, AimTargetVisual.transform.position) > SelectedTower.GetComponent<BaseTower>().Range)
        TestObj.transform.position = Point;
        if (Vector3.Distance(SelectedTower.transform.position, Point) >= SelectedTower.GetComponent<BaseTower>().Range)
        // if (Vector3.Distance(SelectedTower.transform.position, Point) > SelectedTower.GetComponent<BaseTower>().Range)
        {
            Cursor.SetCursor(AimTargetVisual.GetComponent<TargetSprite>().BadTarget.texture, new Vector2(0.5f,0.5f), CursorMode.ForceSoftware);
            AimTargetVisual.GetComponent<TargetSprite>().SwapTargetIcon(1);
           
        }
        else
        {
            Cursor.SetCursor(AimTargetVisual.GetComponent<TargetSprite>().GoodTarget.texture, new Vector2(0.5f, 0.5f), CursorMode.ForceSoftware);
            AimTargetVisual.GetComponent<TargetSprite>().SwapTargetIcon(0);
            Debug.Log("WITHIN RANGE");
            // SelectedTower.ManualTargetPos = new Vector3(hit.point.x, SelectedTower.transform.position.y, hit.point.z);
            SelectedTower.ManualTargetPos = Point;
            SelectedTower.ManualTargetPosUI = Input.mousePosition;

            SelectedTower.transform.LookAt(SelectedTower.ManualTargetPos);
           // SelectedTower.transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);

            //SelectedTower.transform.LookAt(SelectedTower.ManualTargetPos);
           
        }
    }

    public void CheckTargetRangeOnce()
    {
        //AimTargetVisual.transform.position = new Vector3(hit.point.x, SelectedTower.transform.position.y + 2, hit.point.z);
        // AimTargetVisual.transform.position = Input.mousePosition;
        //AimTargetVisual.GetComponent<RectTransform>().position = Input.mousePosition;
        // if (Vector3.Distance(SelectedTower.transform.position, AimTargetVisual.transform.position) > SelectedTower.GetComponent<BaseTower>().Range)
        // Vector3 Point = new Vector3(hit.point.x, SelectedTower.transform.position.y + 2, hit.point.z);
        Vector3 Point = new Vector3(SelectedTower.ManualTargetPos.x, SelectedTower.transform.position.y, SelectedTower.ManualTargetPos.z);
        if (Vector3.Distance(SelectedTower.transform.position, Point) > SelectedTower.Range)
        {
            Cursor.SetCursor(AimTargetVisual.GetComponent<TargetSprite>().BadTarget.texture, Vector2.zero, CursorMode.ForceSoftware);
           
            AimTargetVisual.GetComponent<TargetSprite>().SwapTargetIcon(1);
            Debug.Log("OUTTA RANGE");
        }
        else
        {
            Cursor.SetCursor(AimTargetVisual.GetComponent<TargetSprite>().GoodTarget.texture, Vector2.zero, CursorMode.ForceSoftware);
            AimTargetVisual.GetComponent<TargetSprite>().SwapTargetIcon(0);
            Debug.Log("WITHIN RANGE");
            //SelectedTower.ManualTargetPos = new Vector3(hit.point.x, SelectedTower.transform.position.y, hit.point.z);
        }
    }

    /*
    public Vector3 TrueMousePosition()
    {
        Vector3 output = Vector2.zero;
        
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            UICanvas.GetComponent<RectTransform>(),
            Input.mousePosition,
            //Camera.main,
            UICanvas.worldCamera,
            out output);
       
        return output;
    }

    public Vector3 TruePosition(Vector3 Pos)
    {
        Vector3 output = Vector2.zero;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            UICanvas.GetComponent<RectTransform>(),
            Pos,
            UICanvas.worldCamera,
            out output);
        return output;
    }
    */

    /*
    void OnDrawGizmos()
    {
        
        if (Application.isEditor)
            if (Camera.current == Camera.main || Camera.current == SceneView.lastActiveSceneView.camera)
            {


                Plane groundPlane = new Plane(Vector3.up, new Vector3(0, 1, 0));
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float distance;
                //simply initializing vector3 point, nothing else, this vector zero does nothing
                Vector3 point = Vector3.zero;
                if (groundPlane.Raycast(ray, out distance))
                {
                    point = ray.GetPoint(distance);
                    Debug.DrawRay(ray.origin, ray.direction, Color.blue);
                }

                Gizmos.color = Color.orange;
                Gizmos.DrawSphere(point, 1.2f);
                if (SelectedTower)
                {
                   // Debug.Log(Vector3.Distance(point, SelectedTower.transform.position));
                    // Draw a yellow sphere at the transform's position
                    Gizmos.color = new Color(1, 0.7f, 0.7f, 0.35f);
                    Gizmos.DrawSphere(SelectedTower.transform.position, SelectedTower.Range);
                }




                /*
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                //LayerMask mask = LayerMask.GetMask("Tower", "Default");

                //if (Physics.Raycast(ray, out hit, 10000, mask))
                if (Physics.Raycast(ray, out hit, 10000))
                {
                    //TestObj.transform.position = hit.point;
                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.green);
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(new Vector3(hit.point.x, 1, hit.point.z), 0.0f);
                    TestObj.transform.position = new Vector3(hit.point.x, ray.origin.y, hit.point.z);
                }
                ray = new Ray(TestObj.transform.position, -TestObj.transform.up);
                //if (Physics.Raycast(ray, out hit, 10000, mask))
                if (Physics.Raycast(ray, out hit, 10000))
                {
                    //TestObj.transform.position = hit.point;
                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(new Vector3(hit.point.x, 1, hit.point.z), 0.01f);
                    //TestObj.transform.position = new Vector3(hit.point.x, ray.origin.y, hit.point.z);
                }
                //NOTES:
                //output still has that massive offset..
                /*
                Gizmos.color = Color.blue;
                var pos = Input.mousePosition;

                Vector3 output = Vector2.zero;
                Ray ray;


                 Vector3 v3Pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
                v3Pos = Camera.main.ScreenToWorldPoint(v3Pos);
              //  UICanvas.worldCamera.transform.position = v3Pos;

               // Gizmos.DrawSphere(v3Pos, 0.1f);
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                        UICanvas.GetComponent<RectTransform>(),
                        pos,
                        UICanvas.worldCamera,
                        out output);

                    //Gizmos.DrawSphere(output, 1);
                    Gizmos.color = Color.rebeccaPurple;
                // output.y = UICanvas.worldCamera.transform.position.y;

                ray = new Ray(output, UICanvas.worldCamera.transform.forward);



                Gizmos.color = Color.orange;

                //  pos = new Vector3(Mathf.InverseLerp(0, 1920, pos.x), Mathf.InverseLerp(0, 1080, pos.y), 0);
                // ray = Camera.main.ViewportPointToRay(pos);
                RaycastHit hit;


                 //ray = Camera.main.ScreenPointToRay(Input.mousePosition);


               // Vector3 v3Pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 2);
                //v3Pos = Camera.main.ScreenToWorldPoint(v3Pos);
                //Gizmos.DrawSphere(v3Pos, 0.1f);
                // ray = new Ray(v3Pos, new Vector3(v3Pos.x, -1, v3Pos.z));
                //  ray = new Ray(v3Pos, new Vector3(v3Pos.x, -1, v3Pos.z));
                //TestObj.transform.position = v3Pos;
                //  ray = new Ray(TestObj.transform.position, -TestObj.transform.up);


               // ray = Camera.main.ScreenPointToRay(pos);
                //ray.origin = v3Pos;
                //Debug.Log(ray.origin + " " + ray.direction);

                if (Physics.Raycast(ray, out hit))
                {

                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.green);
                   // Gizmos.DrawSphere(hit.point, 1.5f);

                }

                if (SelectedTower)
                {

                    //v3Pos.y = SelectedTower.transform.position.y;
                    Debug.Log(output);
                    //Debug.Log(Vector3.Distance(output, SelectedTower.transform.position));
                    // Gizmos.DrawSphere(output, 2.5f);
                    TestObj.transform.position = v3Pos;

                    Gizmos.color = Color.red;
                    //v3Pos = v3Pos - SelectedTower.transform.position;
                    //Gizmos.DrawSphere(v3Pos, 1);
                    //Debug.Log(Vector2.Distance(new Vector2(v3Pos.x, v3Pos.z), new Vector2(SelectedTower.transform.position.x, SelectedTower.transform.position.z)));
                }

                /*
                //Gizmos.DrawSphere(TrueMousePosition(), 1);
                Vector3 mouseScreen = Input.mousePosition;
                //mousepos.x -= 100; mousepos.y -= 100;
                Ray ray = Camera.main.ScreenPointToRay(mouseScreen);
                Debug.Log(mouseScreen);

                // you must define a starting plane away from the camera for this to work.
                // this essentially specifies the 'radius' of the mouse's influence.


                //ray.origin = TestRay;

                //Vector3 mouseScreen = Input.mousePosition;
                // you must define a starting plane away from the camera for this to work.
                // this essentially specifies the 'radius' of the mouse's influence.
                mouseScreen.z = 10.5f;
                Vector3 rayOrigin = Camera.main.ScreenToWorldPoint(mouseScreen);

                // use the camera's forward direction
                 Vector3 rayDirection = Camera.main.transform.TransformDirection(Vector3.forward);
               // Vector3 rayDirection = new Vector3(rayOrigin)
                // Vector3 rayDirection = ray.direction;
                ray = new Ray(rayOrigin, rayDirection);


                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {

                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.green);
                    Vector3 NewOrigin = new Vector3(hit.point.x, ray.origin.y, hit.point.z);
                    ray.origin = NewOrigin;
                }
                    //int TowerLayerID = LayerMask.NameToLayer("Tower");
                    //  int WaterLayerID = LayerMask.NameToLayer("Water");

                    if (Physics.Raycast(ray, out hit))
                {

                    if (SelectedTower)
                    {
                        Vector3 newpoint = new Vector3(hit.point.x, SelectedTower.transform.position.y, hit.point.z);
                        Gizmos.DrawSphere(newpoint, 0.1f);
                        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
                        TestObj.transform.position = newpoint;
                        newpoint = new Vector3(TestObj.transform.position.x, SelectedTower.transform.position.y, TestObj.transform.position.z);
                        Debug.Log(Vector3.Distance(newpoint, SelectedTower.transform.position));
                    }
                }
                
            }

    }
    */

    // Update is called once per frame
    void Update()
    {

        if (SelectedTower && SelectedTower.Ability)
        {
            // AbilityImage.sprite = SelectedTower.Ability.AbilityImage;
            if (SelectedTower.TowerAbilityCooldown > 0)
            {
                AbilityTimerText.text = ((short)SelectedTower.TowerAbilityCooldown) + "S left";
            }
            else
            {
                AbilityTimerGif.enabled = false;
                AbilityTimerText.text = "READY!";
            }
            if (SelectedTower.TowerAbilityCooldown <= 0)
            {
                AbilityImage.color = Color.white;
            }
            else
            {
                AbilityImage.color = BaseButtonColour;
            }

        }


        if (Input.GetKeyDown(KeyCode.Mouse0) && SelectedTower) // if clicked off tower reset?
        {
           // Debug.Log("Click");
            //   Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Construct a ray from the current mouse coordinates
            Ray ray = OrthoCam.ScreenPointToRay(Input.mousePosition); // Construct a ray from the current mouse coordinates
            RaycastHit hit;
            int TowerLayerID = LayerMask.NameToLayer("Tower");
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            //  int WaterLayerID = LayerMask.NameToLayer("Water");
            if (EventSystem.current.IsPointerOverGameObject() == false) // block via UI?
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.layer != TowerLayerID)
                    {
                        if (this.gameObject == null) // how am i getting erros for this
                        {
                            SelectTower(null);
                        }
                        SelectTower(this.gameObject);
                    }
                }
            }
        }

        //    if (Input.GetKey(KeyCode.Mouse1) && SelectedTower && AimTargetVisual.activeSelf) // for manual target. LOOK INTO SWAPPING FROM OBJECT TO CURSOR.
        if (Input.GetKey(KeyCode.Mouse1) && SelectedTower && SelectedTower.CurrentTargetingMode == BaseTower.TargetMode.Manual) // for manual target. LOOK INTO SWAPPING FROM OBJECT TO CURSOR.
        {
            Debug.Log("Target");

            //TrueMousePosition
            Vector2 mousepos = Input.mousePosition;
            mousepos.x += 0.5f; mousepos.y += 0.5f;
            Ray ray = OrthoCam.ScreenPointToRay(mousepos);
             

            RaycastHit hit;



            //int TowerLayerID = LayerMask.NameToLayer("Tower");
            //  int WaterLayerID = LayerMask.NameToLayer("Water");
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.DrawRay(ray.origin, ray.direction, Color.red, 1);
                SetTargetColourAndPos(hit);
                /*
                AimTargetVisual.transform.position = new Vector3(hit.point.x, SelectedTower.transform.position.y + 2, hit.point.z);
                if (Vector3.Distance(SelectedTower.transform.position, AimTargetVisual.transform.position) > SelectedTower.GetComponent<BaseTower>().Range)
                {
                    AimTargetVisual.GetComponent<TargetSprite>().SwapTargetIcon(1);
                    Debug.Log("OUTTA RANGE");
                }
                else
                {
                    AimTargetVisual.GetComponent<TargetSprite>().SwapTargetIcon(0);
                    Debug.Log("WITHIN RANGE");
                    SelectedTower.ManualTargetPos = new Vector3(hit.point.x, SelectedTower.transform.position.y, hit.point.z);
                }
                */
                //if (hit.transform.gameObject.layer != TowerLayerID)
                // {
                //     PathHolder.instance.SelectTower(this.gameObject);
                // }
            }
           


        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

           // AimTargetVisual.SetActive(false);
        }
    }
}
