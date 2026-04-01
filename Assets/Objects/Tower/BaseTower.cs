using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
//using static UnityEditor.Progress;
//using static UnityEditor.Rendering.CameraUI;







public class BaseTower : MonoBehaviour
{
    public PathHolder LevelPath;
    public BaseEnemy CurrentTarget;

    public UpgradeList ListOfUpgrades;
    public List<UpgradeInfo> CurrentlyBuyableUpgrades;
    public List<UpgradeInfo> UpgradeList;

    public VillageTowerScript CurrentBuffer;

   public enum TargetMode {First, Last, Close, Manual}; // add strong when enemys are ready
    public TargetMode CurrentTargetingMode = TargetMode.Close;
    public int Damage = 1;
    public float Range = 5;
    public int BasePierce = 0; // see comment in projectilebase.cs
    public bool Searching = false;
  //  public bool ManualTarget = false; // replaced with targeting mode, makes more sense
    public Vector3 ManualTargetPos = Vector3.zero;
    public Vector2 ManualTargetPosUI = Vector2.zero;
    public float ManualTargetingYPos = 0;
    public bool CanSeeCamo = false;
    public float TimeBetweenShots = 1;
    public bool AutoProjectileCleanup = true; // turn off for long lasting AOE attacks using projectiles, such as the pin tower.
    public bool RotateToShoot = true;
   
    public float DelayTimer = 1; // only public for EMP
    public GameObject ProjectileType;
    public List<ProjectileBase> ProjectileList;
    public TextMeshPro AbilityTimerText;

    public AudioClip ProjectileSound;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //to do: strong targeting


    //abilities
    public TowerAbility Ability;
    public float TowerAbilityCooldown = 0;

    private void OnMouseDown()
    {
       // Debug.Log("Goddamnit i got mouse down the wrong way around");
        if (!LevelPath)
        {
            LevelPath = PathHolder.instance; // get pathholder if missing
        }
        LevelPath.SelectTower(this.gameObject);
    }

   
    
    
    

    void Start()
    {
       
        if (!LevelPath)
        {
            LevelPath = PathHolder.instance; // get pathholder if missing
        }


        ManualTargetingYPos = LevelPath.StartPosition.y;
        ManualTargetPos = this.transform.position;
        ManualTargetPosUI = Camera.main.WorldToScreenPoint(ManualTargetPos);

        if (ListOfUpgrades)
           // Debug.Log("breh");
        { // use a scriptable object as our source of upgrades
            for (int i = 0; i < ListOfUpgrades.PossibleUpgradeList.Count; i++)
            {
                //Debug.Log("bleh");
                if (ListOfUpgrades.PossibleUpgradeList[i].StartingUpgrade == true)
                {
                    CurrentlyBuyableUpgrades.Add(ListOfUpgrades.PossibleUpgradeList[i]);
                }
            }
        }
        CurrentlyBuyableUpgrades.TrimExcess();
        UpgradeList.Clear();
        UpgradeList.TrimExcess();
        DelayTimer = TimeBetweenShots;

       // ApplyUpgrade("Test");
    }

    public void SetCurrentDelayTimer(float DelayTime)
    {
        DelayTimer = DelayTime;
    }

    public void UseAbility()
    {
        if (TowerAbilityCooldown <= 0)
        {
            TowerAbilityCooldown = Ability.Cooldown;
            AbilityTimerText.gameObject.SetActive(true);
            
            Ability.OnUse(this);
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (AbilityTimerText)
        {
            //AbilityTimerText.transform.localRotation = Quaternion.Euler(90, 0, -90);
        }
        }
    //  public virtual void  FixedUpdate()
    void FixedUpdate()
    {

       
        if (DelayTimer > 0)
        {
            
            DelayTimer -= Time.fixedDeltaTime;
        }
        DelayChecker();
        if (TowerAbilityCooldown > 0)
        {
            if (AbilityTimerText)
            {
               
                AbilityTimerText.text = TowerAbilityCooldown.ToString("#.00") + " ";
            }
            TowerAbilityCooldown -= Time.fixedDeltaTime;
            if (TowerAbilityCooldown < 0)
            {
                TowerAbilityCooldown = 0;
                Destroy(AbilityTimerText.gameObject);
                AbilityTimerText = null;
            }
        }

        if (LevelPath && LevelPath.WaveInProgress)
        {
            //moved here so you don't have that awkward pause everytime it swaps targets
            /*
            if (DelayTimer > 0)
            {
                DelayTimer -= Time.fixedDeltaTime;
            }
            if (TowerAbilityCooldown > 0)
            {
                TowerAbilityCooldown -= Time.fixedDeltaTime;
            }
            */

            if (!CurrentTarget && Searching == false && CurrentTargetingMode != TargetMode.Manual)
            {

                FindEnemy();



            }
            else if (CurrentTarget && Searching == false || CurrentTargetingMode == TargetMode.Manual)
            {
               // DelayTimer -= Time.fixedDeltaTime;
                if (DelayTimer <= 0)
                {
                    DelayTimer = TimeBetweenShots;
                    ShootEnemy();
                }
            }
        }
    }


    public virtual void DelayChecker()
    {

    }
    /*
    public virtual void ApplyUpgrade(string UpgradeID) // add support for replacing upgrades, although given the effect will retrigger it should be fine. Do wish we could have individual effect ones.. well we can but not editable from effectinfo.
    {
        bool AlreadyGotten = false;
        Debug.Log("ApplyUpgradeStart" + UpgradeList.Count);
        for (int i = 0; i < UpgradeList.Count; i++) // better then number ids maybe?
        {
            if (UpgradeList[i].UpgradeName == UpgradeID)
            {
                AlreadyGotten = true;
            }
        }
        if (AlreadyGotten == false)
        {
            for (int i = 0; i < CurrentlyBuyableUpgrades.Count; i++) // better then number ids maybe?
            {
                Debug.Log("AlreadyGottenFalse");
                
                if (CurrentlyBuyableUpgrades[i].UpgradeName == UpgradeID)
                {

                    UpgradeList.Add(CurrentlyBuyableUpgrades[i]);
                    UpgradeList[UpgradeList.Count - 1].Effect.OnUnlocked(this);

                    if (CurrentlyBuyableUpgrades[i].NextUpgrade == null)
                    {
                        CurrentlyBuyableUpgrades.RemoveAt(i);
                        CurrentlyBuyableUpgrades.TrimExcess();

                    }
                    else
                    {


                        CurrentlyBuyableUpgrades[i] = UpgradeList[UpgradeList.Count - 1].NextUpgrade; // this is way better then strings, tf was i smoking

                      
                    }

                    return;

                }
            }
        }

        
    }

    */

    public virtual void ApplyUpgrade(UpgradeInfo UpgradeData, bool DirectUpgrade) // add support for replacing upgrades, although given the effect will retrigger it should be fine. Do wish we could have individual effect ones.. well we can but not editable from effectinfo.
    {
        bool AlreadyGotten = false;
        Debug.Log("ApplyUpgradeStart" + UpgradeList.Count);
        for (int i = 0; i < UpgradeList.Count; i++) // better then number ids maybe?
        {
            Debug.Log(UpgradeData.name);
            if (UpgradeList[i] == UpgradeData)
            {
                AlreadyGotten = true;
            }
        }
        if (AlreadyGotten == false)
        {
            if (!DirectUpgrade)
            { 
                for (int i = 0; i < CurrentlyBuyableUpgrades.Count; i++) // better then number ids maybe?
                {
                    Debug.Log("AlreadyGottenFalse");

                    if (CurrentlyBuyableUpgrades[i] == UpgradeData)
                    {

                        UpgradeList.Add(CurrentlyBuyableUpgrades[i]);
                        for (int j = 0; j < UpgradeList[UpgradeList.Count - 1].Effects.Count; j++)
                        {
                            UpgradeList[UpgradeList.Count - 1].Effects[j].OnUnlocked(this);
                        }

                        if (CurrentlyBuyableUpgrades[i].NextUpgrade == null)
                        {
                            CurrentlyBuyableUpgrades.RemoveAt(i);
                            CurrentlyBuyableUpgrades.TrimExcess();

                        }
                        else
                        {


                            CurrentlyBuyableUpgrades[i] = UpgradeList[UpgradeList.Count - 1].NextUpgrade; // this is way better then strings, tf was i smoking


                        }

                        break;

                    }
                }
            
            }
            else if (DirectUpgrade == true)
            {
                Debug.Log("DirectUpgradeTrue");
                UpgradeList.Add(UpgradeData);


               
                    for (int j = 0; j < UpgradeList[UpgradeList.Count - 1].Effects.Count; j++)
                    {
                        UpgradeList[UpgradeList.Count - 1].Effects[j].OnUnlocked(this);
                    }
                

               // UpgradeList[UpgradeList.Count - 1].Effect.OnUnlocked(this);
            }
        }

        if (PathHolder.instance && PathHolder.instance.RangeVisual)
        {
            PathHolder.instance.InitRangeVisual(this.gameObject);
        }

    }
    public void CleanupProjectiles()
    {
        if (AutoProjectileCleanup)
        {
            for (int i = 0; i < ProjectileList.Count; i++)
            {
                if (ProjectileList[i] != null)
                {
                    Destroy(ProjectileList[i].gameObject);
                    //ProjectileList.RemoveAt(i);
                }
            }
        }
        //ProjectileList.TrimExcess();


       // if (ProjectileList.Count == 1 && ProjectileList[0] == null || ProjectileList.Count == 0)
       // {
            ProjectileList.Clear();
       // }
    }

   public virtual void ShootEnemy()
    {
        //check if the first ones second condition doesn't screw anything up
        if (CurrentTarget && CurrentTargetingMode != TargetMode.Manual && CurrentTarget.isActiveAndEnabled)
        {
            CleanupProjectiles();
            if (Vector3.Distance(this.transform.position, CurrentTarget.transform.position) > Range || CurrentTarget.Health <= 0)
            {
                FindEnemy();
            }
            else
            {
                //CHECK HOW THIS WORKS WITH OTHER TOWERS.
                //if ((CurrentTarget.IncomingDamage - (ProjectileList.Count + 1) * Damage) >= (0 - Damage))
                

                if (CurrentTarget.EnemyFutureDamage < CurrentTarget.Health && CurrentTarget.Health > 0 && CheckEnemyTowerUnique(CurrentTarget) == false) // if damage incoming is not enough to kill, and enemy is not already killed
                //   if ((CurrentTarget.Health - (ProjectileList.Count + 1) * Damage) >= (0 - Damage) ) // make sure you only use the amount of shots necessary to kill
                {
                    if (ProjectileType.GetComponent<ProjectileBase>().Tracking)
                    {
                        CurrentTarget.EnemyFutureDamage += Damage;
                    }
                    //Debug.Log(CurrentTarget.EnemyFutureDamage + "Health check clear" + CurrentTarget.Health);
                    if (RotateToShoot)
                    {
                        transform.LookAt(CurrentTarget.transform);
                        transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
                    }

                    if (ProjectileSound && this.GetComponent<AudioSource>())
                    {
                        this.GetComponent<AudioSource>().PlayOneShot(ProjectileSound);
                    }

                    // Quaternion RotatedForwardQ = Quaternion.LookRotation(TowerRef.CurrentTarget.transform.position - TowerRef.transform.position);
                    ProjectileList.Add(Instantiate(ProjectileType, transform.position, Quaternion.LookRotation(CurrentTarget.transform.position - transform.position)).GetComponent<ProjectileBase>());
                    ProjectileList[ProjectileList.Count - 1].ProjOwner = this;
                    ProjectileList[ProjectileList.Count - 1].ProjTargetObj = CurrentTarget;
                    for (int i = 0; i < UpgradeList.Count; i++) // better then number ids maybe?
                    {
                       // UpgradeList[i].Effect.OnFire(this);
                        for (int j = 0; j < UpgradeList[i].Effects.Count; j++)
                        {
                            UpgradeList[UpgradeList.Count - 1].Effects[j].OnFire(this);
                        }
                    }
                }
                else
                {
                    FindEnemy();
                }
                
                //ProjectileList[ProjectileList.Count - 1].ProjTargetPos = CurrentTarget.transform.position;
               // Debug.Log("Shooting!");
                /*
                Debug.DrawRay(transform.position, (CurrentTarget.transform.position - transform.position), Color.green);
                CurrentTarget.Health -= Damage;
                //if out of range
                if (CurrentTarget.Health <= 0 && Searching == false)
                {
                    // Destroy(CurrentTarget.gameObject);
                    // LevelPath.Enemies.TrimExcess();
                    CurrentTarget.Death();
                    FindEnemy();
                }
                */
            }
        }
        else if (CurrentTargetingMode != TargetMode.Manual)
        {
            FindEnemy();
        }
        else if (CurrentTargetingMode == TargetMode.Manual)
        {
            CleanupProjectiles();
            //if (ProjectileType.GetComponent<ProjectileBase>().Tracking)
            // {
            //    CurrentTarget.EnemyFutureDamage += Damage;
            // }
            // Debug.Log(CurrentTarget.EnemyFutureDamage + "Health check clear" + CurrentTarget.Health);
            if (RotateToShoot)
            {
                transform.LookAt(ManualTargetPos);
                transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
            }
            // Quaternion RotatedForwardQ = Quaternion.LookRotation(TowerRef.CurrentTarget.transform.position - TowerRef.transform.position);

            // Debug.Log(ProjectileAngle);


            if (ProjectileSound && this.GetComponent<AudioSource>())
            {
                this.GetComponent<AudioSource>().PlayOneShot(ProjectileSound);
            }

            Vector3 SpawnPos = new Vector3(transform.position.x, ManualTargetingYPos, transform.position.z);

            ProjectileList.Add(Instantiate(ProjectileType, SpawnPos, Quaternion.LookRotation(ManualTargetPos - transform.position)).GetComponent<ProjectileBase>());
            ProjectileList[ProjectileList.Count - 1].ProjOwner = this;
            ProjectileList[ProjectileList.Count - 1].ProjTargetObj = null;
            //copied from splitproj
            ProjectileList[ProjectileList.Count - 1].Tracking = false;
            ProjectileList[ProjectileList.Count - 1].ExtraProjectileFailsafe();
            for (int i = 0; i < UpgradeList.Count; i++) // better then number ids maybe?
            {
                // UpgradeList[i].Effect.OnFire(this);
                for (int j = 0; j < UpgradeList[i].Effects.Count; j++)
                {
                    UpgradeList[UpgradeList.Count - 1].Effects[j].OnFire(this);
                }
            }
        }

    }


    public bool CheckCamoStatus(BaseEnemy enemytocheck)
    {
        if (enemytocheck.Camo == false || enemytocheck.Camo == true && CanSeeCamo == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public virtual bool CheckEnemyTowerUnique(BaseEnemy enemytocheck) // use this for special tower checks ie if slowed, don't shoot if a flamethrower tower
    {
       
            return false;
        
    }

    public virtual void FindEnemy()
    {
        /*
        for (int i = 0; i < ProjectileList.Count; i++)
        {
            if (ProjectileList[i] != null)
            {
                Destroy(ProjectileList[i].gameObject);
            }
        }
        ProjectileList.Clear();
        */
        CleanupProjectiles();
        if (CurrentTarget)
        {
            CurrentTarget.EnemyFutureDamage -= Damage;
        }
        if (LevelPath && LevelPath.Enemies.Count > 0)
        {
            CurrentTarget = null;
            Searching = true;
            switch (CurrentTargetingMode)
            {

                //IF BROKEN CHECK CAMO !!!!!!!!!!!!!!!!

                case TargetMode.Close:
                    BaseEnemy CurrentClosestEnemy = null; 
                    
                    for (int i = 0; i < LevelPath.Enemies.Count; i++)
                    {
                        if (CurrentClosestEnemy == null && LevelPath.Enemies[i] != null && LevelPath.Enemies[i].Targetable == true && CheckCamoStatus(LevelPath.Enemies[i]) && CheckEnemyTowerUnique(LevelPath.Enemies[i]) == false) // inital enemy
                        {
                            CurrentClosestEnemy = LevelPath.Enemies[i];


                        }

                        if (CurrentClosestEnemy != null && LevelPath.Enemies[i] != null && CheckCamoStatus(LevelPath.Enemies[i]) && CheckEnemyTowerUnique(LevelPath.Enemies[i]) == false)
                        {
                            Vector3 currentoffset = CurrentClosestEnemy.transform.position - transform.position;
                            float currentsqrLen = currentoffset.sqrMagnitude;
                            Vector3 offset = LevelPath.Enemies[i].transform.position - transform.position;
                            float sqrLen = offset.sqrMagnitude;



                            // square the distance we compare with
                            if (sqrLen < currentsqrLen)
                            {
                             //   print(LevelPath.Enemies[i].name + " Is closer then " + CurrentClosestEnemy.name + "!");

                                CurrentClosestEnemy = LevelPath.Enemies[i];
                               // print(Vector3.Distance(this.transform.position, CurrentClosestEnemy.transform.position));
                            }
                        }
                    }
                    if (CurrentClosestEnemy != null && Vector3.Distance(this.transform.position, CurrentClosestEnemy.transform.position) <= Range )
                    {
                        CurrentTarget = CurrentClosestEnemy;
                    }
                    Searching = false;

                    break;


                case TargetMode.First:
                     CurrentClosestEnemy = null; // local is weird with cases it seems

                    for (int i = 0; i < LevelPath.Enemies.Count; i++)
                    {
                        if (CurrentClosestEnemy == null && LevelPath.Enemies[i] != null && Vector3.Distance(this.transform.position, LevelPath.Enemies[i].transform.position) <= Range && LevelPath.Enemies[i].Targetable == true && CheckCamoStatus(LevelPath.Enemies[i]) && CheckEnemyTowerUnique(LevelPath.Enemies[i]) == false)
                        {
                            CurrentClosestEnemy = LevelPath.Enemies[i];
                            break;

                        }

                        
                    }
                    if (CurrentClosestEnemy != null && Vector3.Distance(this.transform.position, CurrentClosestEnemy.transform.position) <= Range)
                    {
                        CurrentTarget = CurrentClosestEnemy;
                    }
                    Searching = false;

                    break;
                case TargetMode.Last:
                    CurrentClosestEnemy = null; // local is weird with cases it seems

                    for (int i = LevelPath.Enemies.Count - 1; i >= 0; i--)
                    {
                        if (CurrentClosestEnemy == null && LevelPath.Enemies[i] != null && Vector3.Distance(this.transform.position, LevelPath.Enemies[i].transform.position) <= Range && LevelPath.Enemies[i].Targetable == true && CheckCamoStatus(LevelPath.Enemies[i]) && CheckEnemyTowerUnique(LevelPath.Enemies[i]) == false)
                        {
                            CurrentClosestEnemy = LevelPath.Enemies[i];
                            break;

                        }


                    }
                    if (CurrentClosestEnemy != null && Vector3.Distance(this.transform.position, CurrentClosestEnemy.transform.position) <= Range)
                    {
                        CurrentTarget = CurrentClosestEnemy;
                    }
                    Searching = false;

                    break;
            }
        }
    }
}
