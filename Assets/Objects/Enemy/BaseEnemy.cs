using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BaseEnemy : MonoBehaviour
{
    
    public PathHolder PathManager;
    public int CurrentPath = 0;
    public float Health = 1;
    public float Speed = 1;
    public int MoneyOnDeath = 1;
    Vector3 TargetPosition = Vector3.zero;
    public int LayerID = 0;
    public EnemyLayerSwap LayerOrder;
    public GameObject LastProjectile;
    bool ReachedEndOfTrack = false;
    public bool Targetable = false;

    public bool Lead = false;
    public AudioClip HitSound;
    public AudioClip DeathSound;
    public AudioClip GlanceSound;
    //  public float SoundVolume = 0.75f;


    //do future damage like will, add up
    public int EnemyFutureDamage = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // AudioSource.PlayClipAtPoint(HitSound, this.transform.position, SoundVolume);
       
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
            Speed += LevelManager.instance.CurrentDifficulty.BloonSpeedIncrease;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ReachedEndOfTrack == false)
        { 
        if (CurrentPath == 0)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, TargetPosition, (5 * Speed) * Time.deltaTime);
        }
        else
        {
            
            this.transform.position = Vector3.MoveTowards(this.transform.position, TargetPosition, Speed * Time.deltaTime);
        }

            if (Vector3.Distance(transform.position, TargetPosition) < 0.001f)
            {
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
            }
            
        }
    }



    public void TakeDamage(int ProjectileDamage, BaseTower TargetingTower)
    {
        //to stop taking damage every frame




        if (Lead == false || TargetingTower.ProjectileType && TargetingTower.ProjectileType.GetComponent<ProjectileBase>().LeadBreaking == true && Lead == true)
        {
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
        
        //use layer swap to use texture, model, speed and hp
    }

    void LayerSwap()
    {



        if (GetComponent<ParticleSystem>())
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main = ps.main;

            Debug.Log("CHANGE COLOUR");
            main.startColor = this.GetComponent<MeshRenderer>().material.color;
            GetComponent<ParticleSystem>().Play();
            // can't  do this via the lower method. Why? Who knows.
            //  GetComponent<ParticleSystem>().main.startColor = this.GetComponent<MeshRenderer>().material.color;
        }

        LayerID -= 1;
        

        if (LayerID >= 0)
        {

            
            this.GetComponent<MeshRenderer>().material = LayerOrder.LayerPopOrder[LayerID].GetComponent<MeshRenderer>().sharedMaterial;
            this.GetComponent<BaseEnemy>().Speed = LayerOrder.LayerPopOrder[LayerID].GetComponent<BaseEnemy>().Speed;
            this.GetComponent<BaseEnemy>().Lead = LayerOrder.LayerPopOrder[LayerID].GetComponent<BaseEnemy>().Lead;

        }
    }

    public void KillEnemy()
    {
        
            if (GetComponent<ParticleSystem>())
            {
                ParticleSystem ps = GetComponent<ParticleSystem>();
                ParticleSystem.MainModule main = ps.main;

                Debug.Log("CHANGE COLOUR");
                main.startColor = this.GetComponent<MeshRenderer>().material.color;
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
    
}
