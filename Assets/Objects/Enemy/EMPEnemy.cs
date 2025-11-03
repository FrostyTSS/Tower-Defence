using UnityEngine;

public class EMPEnemy : BaseEnemy
{
    public float Radius = 2.5f;
    public float EMPLength = 5.0f;
    public ParticleSystem EMPParticleToSpawnOnTower;
    public ParticleSystem BigEMPEffect;
    public bool HaveEMPd = false;
    public bool CanEMP = true;
    public AudioClip EMPSound;
    public float FlashSpeed = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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

    public override void KillEnemy()
    {
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
        Debug.Log("KILL ENEMY");
        //Destroy(gameObject);
        PathManager.Money += MoneyOnDeath;
        PathManager.UpdateMoneyCounter();
        PathManager.RemoveEnemies(gameObject); // kill here to properly count enemies

        
    }
    
    //maybe check if its near the end
    
    public override void OnNewPoint()
    {
        base.OnNewPoint();
        Debug.Log("CurrentNodeID" + CurrentPath + " and Percent Looking for is " + (int)(PathHolder.instance.Positions.Count * 0.75f));
        if (HaveEMPd == false && CurrentPath >= (PathHolder.instance.Positions.Count * 0.75f))
        {
            EMPScan(false);
        }
    }
    


    public void EMPScan(bool FromKill)
    {
        //for later:
        //explosion VFX
        //bloons going down to their next layer

        LayerMask mask = LayerMask.GetMask("Tower");
        //make sure its the right layer
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius, mask);
        if (hitColliders.Length > 0)
        {
            if (GlanceSound && GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().PlayOneShot(EMPSound);
            }
            if (BigEMPEffect)
            {
                ParticleSystem BigEMP = Instantiate(BigEMPEffect, this.transform.position, this.transform.rotation);
               // Destroy(BigEMP, BigEMP.main.duration + BigEMP.main.startLifetime.curveMultiplier);  // jusr destroy in the particle effect editor stop action, easy!
            }
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
                hitCollider.GetComponent<BaseTower>().SetCurrentDelayTimer(EMPLength);
                if (EMPParticleToSpawnOnTower)
                {
                    ParticleSystem TowerEMP = Instantiate(EMPParticleToSpawnOnTower, hitCollider.transform.position, hitCollider.transform.rotation, hitCollider.transform);
                 //   Destroy(TowerEMP, TowerEMP.main.duration + TowerEMP.main.startLifetime.curveMultiplier);
                }
                HaveEMPd = true;
                if (FromKill == false)
                {
                    KillEnemy();
                }
            }
        }
       
    }
}
