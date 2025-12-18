using UnityEngine;

public class EMPMovingEnemy : BaseEnemy
{
    public float Radius = 2.5f;
    public float EMPLength = 5.0f;
    public ParticleSystem EMPParticleToSpawnOnTower;
    public ParticleSystem FollowingEMPParticle;
    public GameObject EMPRadiusSphere;
    public bool HaveEMPd = false;
    public bool CanEMP = true;
   // public AudioClip EMPSound;
    //public float FlashSpeed = 1.0f;
    public float RegenSpeedPerTick = 0.05f;
    //LayerMask EMPCheckMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnStart()
    {
        if (FollowingEMPParticle)
        {
            FollowingEMPParticle.gameObject.SetActive(false);
        }
        if (EMPRadiusSphere)
        {
            EMPRadiusSphere.transform.localScale = Vector3.one;
            EMPRadiusSphere.layer = 10; //set it to ortho only so it renders properly no matter where it is
            EMPRadiusSphere.transform.localScale *= Radius * 1.95f;
        }
        //TowerLayerID = LayerMask.NameToLayer("Tower");
    }

    // Update is called once per frame
    /*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            FlashHandler();
        }
    }

    
    public void FlashHandler()
    {
        if (this.GetComponent<Animator>())
        {
            float NewFlashSpeed = Mathf.Lerp(1, 8, Health / MaxHealth); // speed up animation based on damage
            this.GetComponent<Animator>().speed = NewFlashSpeed;
        }
    }

    public override void OnTakeDamage(int ProjectileDamage, BaseTower TargetingTower)
    {
       
        FlashHandler();
    }
    */

    public override void OnFixedTick()
    {

        if (Health < MaxHealth)
        {
            if (Health <= MaxHealth * 0.5f && CanEMP == false)
            {
                CanEMP = true;
                if (FollowingEMPParticle)
                {
                    FollowingEMPParticle.gameObject.SetActive(true);
                }
            }
            Health += RegenSpeedPerTick;
            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
        }

        if (CanEMP)
        {
            EMPScan();
        }
        
    }

    private void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            if (Camera.current == Camera.main)
            {


                if (CanEMP)
                {
                    Gizmos.color = Color.darkCyan;
                }
                else
                {
                    Gizmos.color = Color.orange;
                }
                Gizmos.DrawSphere(transform.position, Radius);
               
            }
        }
     }

    public override void KillEnemy()
    {
        /*
        if (HaveEMPd == false && CanEMP)
        {
            EMPScan(true);
        }
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
        */
        Debug.Log("KILL ENEMY");
        //Destroy(gameObject);
        PathManager.Money += MoneyOnDeath;
        PathManager.UpdateMoneyCounter();
        PathManager.RemoveEnemies(gameObject); // kill here to properly count enemies

        
    }
    
    //maybe check if its near the end
    
   
    


    public void EMPScan()
    {
        //for later:
        //explosion VFX
        //bloons going down to their next layer

        LayerMask mask = LayerMask.GetMask("Tower");
        //make sure its the right layer


        if (GetComponent<AudioSource>() && GetComponent<AudioSource>().isPlaying == false)
        {
            GetComponent<AudioSource>().Play();
        }

        /*
        if (EMPSound && GetComponent<AudioSource>() && GetComponent<AudioSource>().isPlaying == false)
        {
            GetComponent<AudioSource>().PlayOneShot(EMPSound);
        }
        */
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius, mask);
        if (hitColliders.Length > 0)
        {
            
           // if (FollowingEMPParticle)
           // {
           //     FollowingEMPParticle.transform.position = transform.position;
               // Destroy(BigEMP, BigEMP.main.duration + BigEMP.main.startLifetime.curveMultiplier);  // jusr destroy in the particle effect editor stop action, easy!
           // }
            foreach (var hitCollider in hitColliders)
            {
                Debug.Log(hitCollider.name);
                //hitCollider.SendMessage("AddDamage");
                /*
                if (hitCollider.GetComponent<BaseEnemy>() && hitCollider.GetComponent<BaseEnemy>() != ProjTargetObj)
                {
                    Debug.Log("BOOM, SPLASH! SPLASHED TO" + hitCollider.name);
                    ProjectileCollideOtherTarget(hitCollider.GetComponent<BaseEnemy>(), ActivateHitOnSecondaryTargets, false);
                }
                */
                hitCollider.GetComponent<BaseTower>().SetCurrentDelayTimer(EMPLength);
                if (EMPParticleToSpawnOnTower)
                {
                    ParticleSystem TowerEMP = Instantiate(EMPParticleToSpawnOnTower, hitCollider.transform.position, hitCollider.transform.rotation, hitCollider.transform);
                 //   Destroy(TowerEMP, TowerEMP.main.duration + TowerEMP.main.startLifetime.curveMultiplier);
                }
                //HaveEMPd = true;
                
            }
        }
       
    }
}
