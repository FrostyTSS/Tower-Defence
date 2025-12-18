using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System.Collections;

public class BaseEnemy : MonoBehaviour
{
    
    public PathHolder PathManager;
    public int CurrentPath = 0;
    public float MaxHealth = 1;
    public float Health = 1;
    public float MaxSpeed = 1;
    public float CurrentSpeed = 0;
    public float CurrentSpeedModifier = 1;
    public int MoneyOnDeath = 1;
    Vector3 TargetPosition = Vector3.zero;
    public int LayerID = 0;
    public EnemyLayerSwap LayerOrder;
    public GameObject LastProjectile;
    bool ReachedEndOfTrack = false;
    public bool Targetable = false;
    public bool CurrentlySlowed = false;
    public bool Camo = false;
    public GameObject CamoOverlay;

    public bool Lead = false;
    public AudioClip HitSound;
    public AudioClip DeathSound;
    public AudioClip GlanceSound;
    public Color ParticleColour;
    //  public float SoundVolume = 0.75f;


    //do future damage like will, add up
    public int EnemyFutureDamage = 0;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // AudioSource.PlayClipAtPoint(HitSound, this.transform.position, SoundVolume);
        CurrentSpeed = MaxSpeed;
        if (!PathManager)
        {
            PathManager = Component.FindAnyObjectByType<PathHolder>();
        }
        if (GetComponent<ParticleSystem>())
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = ps.main; 

          
            main.startColor = this.GetComponent<MeshRenderer>().material.color; // can't  do this via the lower method. Why? Who knows.
            //  GetComponent<ParticleSystem>().main.startColor = this.GetComponent<MeshRenderer>().material.color;
        }
        TargetPosition = PathManager.Positions[CurrentPath];

        if (LevelManager.instance && LevelManager.instance.CurrentDifficulty)
        {
            MaxSpeed += LevelManager.instance.CurrentDifficulty.BloonSpeedIncrease;
        }

        Health = MaxHealth;
        OnStart();
    }

    public Vector3 GetEnemyDestination()
    {
        return TargetPosition;
    }

    public void SetTarget(Vector3 Target)
    {
        TargetPosition = Target;
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        OnFixedTick();
        if (ReachedEndOfTrack == false)
        { 
        if (CurrentPath == 0)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, TargetPosition, (5 * CurrentSpeed) * Time.deltaTime);
        }
        else
        {
            
            this.transform.position = Vector3.MoveTowards(this.transform.position, TargetPosition, (CurrentSpeed * CurrentSpeedModifier) * Time.deltaTime);
                this.transform.LookAt(TargetPosition);
        }

            if (Vector3.Distance(transform.position, TargetPosition) < 0.002f)
            {
               // Debug.Log("PATHSWAP");
               
                // Swap the position of the cylinder.
                CurrentPath++;
                
                Targetable = true;
                if (CurrentPath >= PathManager.Positions.Count)
                {
                    Targetable = false;
                    Debug.Log("Reached the end of its journey!");
                    if (ReachedEndOfTrack == false)
                    {
                        PathManager.HurtPlayer();
                    }
                    ReachedEndOfTrack = true;
                    KillEnemy();
                }
                if (CurrentPath < PathManager.Positions.Count)
                {
                    TargetPosition = PathManager.Positions[CurrentPath];
                }
                OnNewPoint();
            }
            
        }
    }



    public void TakeDamage(int ProjectileDamage, BaseTower TargetingTower)
    {
        //to stop taking damage every frame




        if (Lead == false || TargetingTower.ProjectileType && TargetingTower.ProjectileType.GetComponent<ProjectileBase>().LeadBreaking == true && Lead == true)
        {
            OnTakeDamage(ProjectileDamage, TargetingTower);
            Health -= ProjectileDamage;
            EnemyFutureDamage -= ProjectileDamage;
          
            //if (EnemyFutureDamage < 0)
            //{
            //    EnemyFutureDamage = 0;
            //}
            if (Health <= 0)
            {
                // Destroy(CurrentTarget.gameObject);
                // LevelPath.Enemies.TrimExcess();

                KillEnemy();
                TargetingTower.FindEnemy();

            }
            else if (Health > 0 && LayerOrder && LayerID < LayerOrder.LayerPopOrder.Count)
            {
                if (HitSound && GetComponent<AudioSource>())
                {

                    GetComponent<AudioSource>().PlayOneShot(HitSound);
                }
                LayerSwap();
            }
        }
        else
        {
            if (GlanceSound && GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().PlayOneShot(GlanceSound);
            }
        }
    }


    public void TakeDamage(int IncomingDamage, BaseTower TargetingTower, GameObject CollidingProjectile, bool IntendedTarget)
    {
        //to stop taking damage every frame
        bool DifferentProjectile = false;
        if (LastProjectile == null)
        {
            DifferentProjectile = true;
            LastProjectile = CollidingProjectile;
        }
        else if (LastProjectile != CollidingProjectile)
        {
            DifferentProjectile = true;
        }

        if (DifferentProjectile == true)
        {

            if (Lead == false || Lead == true && CollidingProjectile.GetComponent<ProjectileBase>().LeadBreaking == true)
            {

                OnTakeDamage(IncomingDamage, TargetingTower);
                Health -= IncomingDamage;
                if (IntendedTarget)
                {
                    EnemyFutureDamage -= IncomingDamage;
                }
                if (Health <= 0)
                {
                    // Destroy(CurrentTarget.gameObject);
                    // LevelPath.Enemies.TrimExcess();
                    KillEnemy();
                    TargetingTower.FindEnemy();

                }
                // else if (Health > 0 && LayerOrder && LayerID < LayerOrder.LayerPopOrder.Count && IncomingDamage > 0) //NEED A SQUELCH SOUND FOR QUARINTINER OR SOMETHING
                else if (Health > 0  && IncomingDamage > 0)
                {
                    if (HitSound && GetComponent<AudioSource>())
                    {
                        GetComponent<AudioSource>().PlayOneShot(HitSound);
                    }
                    if ( LayerOrder && LayerID < LayerOrder.LayerPopOrder.Count)
                    {
                        LayerSwap();
                    }
                    else
                    {
                        if (GetComponent<ParticleSystem>())
                        {
                            /* // commented out 'cause this is for no layers
                            ParticleSystem ps = GetComponent<ParticleSystem>();

                            ParticleSystem.MainModule main = ps.main;

                            Debug.Log("CHANGE COLOUR");
                            if (this.GetComponent<MeshRenderer>())
                            {
                                main.startColor = this.GetComponent<MeshRenderer>().material.color;
                            } */
                            if (GetComponent<ParticleSystem>())
                            {
                                ParticleSystem ps = GetComponent<ParticleSystem>();
                                ParticleSystem.MainModule main = ps.main;

                               // Debug.Log("CHANGE COLOUR");
                                main.startColor = ParticleColour;

                                GetComponent<ParticleSystem>().Play();
                                // can't  do this via the lower method. Why? Who knows.
                                //  GetComponent<ParticleSystem>().main.startColor = this.GetComponent<MeshRenderer>().material.color;
                            }

                            // can't  do this via the lower method. Why? Who knows.
                            //  GetComponent<ParticleSystem>().main.startColor = this.GetComponent<MeshRenderer>().material.color;
                        }
                    }



                }
            }
            else
            {
                if (GlanceSound && GetComponent<AudioSource>())
                {
                    GetComponent<AudioSource>().PlayOneShot(GlanceSound);
                }
            }
        }
        
        //use layer swap to use texture, model, speed and hp
    }

    void LayerSwap()
    {



        if (GetComponent<ParticleSystem>())
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = ps.main;

            Debug.Log("CHANGE COLOUR");
            main.startColor = ParticleColour;
            
            GetComponent<ParticleSystem>().Play();
            // can't  do this via the lower method. Why? Who knows.
            //  GetComponent<ParticleSystem>().main.startColor = this.GetComponent<MeshRenderer>().material.color;
        }


        PathManager.Money += LayerOrder.LayerPopOrder[LayerID].GetComponent<BaseEnemy>().MoneyOnDeath;
        PathManager.UpdateMoneyCounter();
        LayerID -= 1;
        

        if (LayerID >= 0)
        {

            ParticleColour = LayerOrder.LayerPopOrder[LayerID].GetComponent<BaseEnemy>().ParticleColour;

            if (this.GetComponent<MeshRenderer>())
            {
                this.GetComponent<MeshRenderer>().material = LayerOrder.LayerPopOrder[LayerID].GetComponent<MeshRenderer>().sharedMaterial;
            }

            if (this.GetComponentInChildren<SpriteRenderer>() && LayerOrder.LayerPopOrder[LayerID].GetComponentInChildren<SpriteRenderer>())
            {
                this.GetComponentInChildren<SpriteRenderer>().sprite = LayerOrder.LayerPopOrder[LayerID].GetComponentInChildren<SpriteRenderer>().sprite;
            }

            this.GetComponent<BaseEnemy>().MaxSpeed = LayerOrder.LayerPopOrder[LayerID].GetComponent<BaseEnemy>().MaxSpeed;
            if (CurrentSpeed >= MaxSpeed)
            {
                CurrentSpeed = MaxSpeed;
            }

            
            this.GetComponent<BaseEnemy>().Lead = LayerOrder.LayerPopOrder[LayerID].GetComponent<BaseEnemy>().Lead;

        }
    }



   

    public IEnumerator Slowdown(float NewSpeed, float TimeBeforeSpeedUp, float SpeedUpDuration)
    {
        Debug.Log("SLOW!");
        

        if (CurrentlySlowed == false || CurrentlySlowed == true && NewSpeed < CurrentSpeed)
        {
            CurrentSpeed = NewSpeed;
            CurrentlySlowed = true;
            float timeElapsed = 0;
           

          


            yield return new WaitForSeconds(TimeBeforeSpeedUp);
            if (timeElapsed < SpeedUpDuration)
            {
                CurrentSpeed = Mathf.Lerp(NewSpeed, MaxSpeed, timeElapsed / SpeedUpDuration);
                timeElapsed += Time.deltaTime;
            }
            /*
            while (CurrentSpeed < MaxSpeed)
            {

                CurrentSpeed += 1f * Time.fixedDeltaTime;
                yield return new WaitForSeconds(0.1f);
            }
            */
            
            CurrentSpeed = MaxSpeed;
            CurrentlySlowed = false;
        }

    }

    public IEnumerator PercentageSlowdown(float SpeedMod, float TimeBeforeSpeedUp, float SpeedUpDuration, BaseTower OwnerTower) // for anything long term, like glue. i think.
    {
       

        if (CurrentlySlowed == false || CurrentlySlowed == true && SpeedMod < CurrentSpeedModifier)
        {
            CurrentSpeedModifier = SpeedMod;
            CurrentlySlowed = true;
            float timeElapsed = 0;

           
            



            yield return new WaitForSeconds(TimeBeforeSpeedUp);
            if (timeElapsed < SpeedUpDuration)
            {
                CurrentSpeedModifier = Mathf.Lerp(SpeedMod, 1, timeElapsed / SpeedUpDuration);
                timeElapsed += Time.deltaTime;
            }
            /*
            while (CurrentSpeed < MaxSpeed)
            {

                CurrentSpeed += 1f * Time.fixedDeltaTime;
                yield return new WaitForSeconds(0.1f);
            }
            */

            CurrentSpeedModifier = 1;
            CurrentlySlowed = false;
        }

    }

    public IEnumerator SpeedUp(float NewSpeed, float Duration)
    {
        
        CurrentSpeed = NewSpeed;
        
        yield return new WaitForSeconds(Duration);
        CurrentSpeed = MaxSpeed;

    }

    public virtual void KillEnemy()
    {

        if (GetComponent<ParticleSystem>())
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = ps.main;

            Debug.Log("CHANGE COLOUR");
            main.startColor = ParticleColour;

            GetComponent<ParticleSystem>().Play();
            // can't  do this via the lower method. Why? Who knows.
            //  GetComponent<ParticleSystem>().main.startColor = this.GetComponent<MeshRenderer>().material.color;
        }
        Debug.Log("KILL ENEMY");
            //Destroy(gameObject);
            PathManager.Money += MoneyOnDeath;
            PathManager.UpdateMoneyCounter();
            PathManager.RemoveEnemies(gameObject); // kill here to properly count enemies
     }

    public virtual void OnNewPoint()
    {

        
    }

    public virtual void OnTakeDamage(int ProjectileDamage, BaseTower TargetingTower)
    {


    }
    public virtual void OnFixedTick()
    {


    }
    public virtual void OnStart()
    {


    }
}
