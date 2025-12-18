using UnityEngine;

public class EMPEnemy : BaseEnemy
{
    public float Radius = 2.5f;
    public float EMPLength = 5.0f;
    public ParticleSystem EMPParticleToSpawnOnTower;
    public ParticleSystem BigEMPEffect;
    public bool HaveEMPd = false;
    //public bool CanEMP = true;
    public AudioClip EMPSound;
    public float FlashSpeed = 1.0f;
    public GameObject RadiusVisual;
   // float OGDuration = 1;
   // float OGSize = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnStart()
    {
       // OGDuration = BigEMPEffect.main.duration;
        //OGSize = BigEMPEffect.main.startSizeMultiplier;
        if (PathHolder.instance && PathHolder.instance.RoundFinishedSpawning == false)
        {

            this.GetComponentInChildren<Animator>().speed = 0;
        }
        if (RadiusVisual)
        {
            RadiusVisual.transform.localScale = Vector3.one;
            RadiusVisual.layer = 10; //set it to ortho only so it renders properly no matter where it is
            RadiusVisual.transform.localScale *= Radius * 1.95f;
        }
        //var main = BigEMPEffect.main;
        //main.duration = OGDuration * 0.5f;
        //main.startSizeMultiplier = OGSize * 0.25f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            FlashHandler();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (EMPSound && GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().PlayOneShot(EMPSound, 0.8f);
            }
        }
    }
  

    public void FlashHandler()
    {
        if (this.GetComponentInChildren<Animator>())
        {
             FlashSpeed = Mathf.Lerp(4, 1, Health / MaxHealth); // speed up animation based on damage
            this.GetComponentInChildren<Animator>().speed = FlashSpeed;

            //BigEMPEffect.Stop();
           // ps.Stop(); // Cannot set duration whilst particle system is playing

           

          //  BigEMPEffect.Play();
            if (Health <= MaxHealth * 0.5f && this.GetComponentInChildren<Animator>())
            {
                this.GetComponentInChildren<Animator>().SetBool("Panic", true);
            }

           
        }
    }


    private void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
           // if (Camera.current == Camera.main)
           // {


                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position, Radius);

           // }
        }
    }
    public override void OnTakeDamage(int ProjectileDamage, BaseTower TargetingTower)
    {
        if (Health > 0)
        {
            FlashHandler();
        }
    }

   


    void KillEnemyActual()
    {
        Debug.Log("KILL ENEMY");
        //Destroy(gameObject);
        //this.GetComponentInChildren<Animator>().StopPlayback();
        PathManager.Money += MoneyOnDeath;
        PathManager.UpdateMoneyCounter();
        PathManager.RemoveEnemies(gameObject); // kill here to properly count enemies
    }

    public override void KillEnemy()
    {
        Health = 0;
        if (HaveEMPd == false)
        {
            EMPScan(true);
        }
       // KillEnemyActual();
        

        
    }
    
    //maybe check if its near the end
    /*
    public override void OnNewPoint()
    {
        base.OnNewPoint();
        Debug.Log("CurrentNodeID" + CurrentPath + " and Percent Looking for is " + (int)(PathHolder.instance.Positions.Count * 0.75f));
        if (HaveEMPd == false && CurrentPath >= (PathHolder.instance.Positions.Count * 0.75f))
        {
            EMPScan(false);
        }
    }
    */


    public void EMPScan(bool FromKill)
    {
        //for later:
        //explosion VFX
        //bloons going down to their next layer

        Health = 0;

        //Debug.Log("Scanning emp, kill status is: " + FromKill);
        LayerMask mask = LayerMask.GetMask("Tower");
        //make sure its the right layer
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius, mask);

        if (BigEMPEffect)
        {
            //BigEMPEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            // ps.Stop(); // Cannot set duration whilst particle system is playing

            //var main = BigEMPEffect.main;
            //main.duration = OGDuration;
            ////main.startSizeMultiplier = OGSize;

            // BigEMPEffect.Play();

            ParticleSystem BigEMP;
            if (PathHolder.instance.FullscreenParticleSpawnpoint != Vector3.zero)
            {
                 BigEMP = Instantiate(BigEMPEffect, PathHolder.instance.FullscreenParticleSpawnpoint, BigEMPEffect.transform.rotation);
            }
            else
            {
                 BigEMP = Instantiate(BigEMPEffect, this.transform.position, BigEMPEffect.transform.rotation);
            }
            var main = BigEMP.main;
            //main.duration = OGDuration * 0.5f;
            //main.startSizeMultiplier = OGSize * 0.25f;
            main.duration = EMPLength;
            BigEMP.Play();



            // Destroy(BigEMP, BigEMP.main.duration + BigEMP.main.startLifetime.curveMultiplier);  // jusr destroy in the particle effect editor stop action, easy!
        }
        if (EMPSound && GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().PlayOneShot(EMPSound);
        }
        if (hitColliders.Length > 0)
        {
            
            
            foreach (var hitCollider in hitColliders)
            {
                //hitCollider.SendMessage("AddDamage");
                /*
                if (hitCollider.GetComponent<BaseEnemy>() && hitCollider.GetComponent<BaseEnemy>() != ProjTargetObj)
                {
                    Debug.Log("BOOM, SPLASH! SPLASHED TO" + hitCollider.name);
                    ProjectileCollideOtherTarget(hitCollider.GetComponent<BaseEnemy>(), ActivateHitOnSecondaryTargets, false);
                }
                */
                if (hitCollider.GetComponent<BaseTower>())
                {
                    hitCollider.GetComponent<BaseTower>().SetCurrentDelayTimer(EMPLength);
                    Debug.Log(hitCollider.name);
                    if (EMPParticleToSpawnOnTower)
                    {

                        Vector3 AdjustedPos = hitCollider.transform.position;
                        AdjustedPos.y++;
                        ParticleSystem TowerEMP = Instantiate(EMPParticleToSpawnOnTower, AdjustedPos, hitCollider.transform.rotation);
                        var main = BigEMPEffect.main;
                        //main.duration = OGDuration * 0.5f;
                        //main.startSizeMultiplier = OGSize * 0.25f;
                        main.duration = EMPLength;
                        TowerEMP.Play();


                        //   Destroy(TowerEMP, TowerEMP.main.duration + TowerEMP.main.startLifetime.curveMultiplier);
                    }
                   
                }
            }


           

        }
        HaveEMPd = true;
        KillEnemyActual();

    }
}
