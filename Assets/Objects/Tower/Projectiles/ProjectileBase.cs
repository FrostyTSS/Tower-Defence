using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProjectileBase : MonoBehaviour
{
    public BaseTower ProjOwner;
    public BaseEnemy ProjTargetObj;
    //public Vector3 ProjTargetPos;
    public float Speed = 15;
    bool Collided = false;
    public bool Tracking = true;

    public float LifetimeForNonTracking = 1.5f;
    public int Pierce = 0;
    public bool LeadBreaking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Pierce += ProjOwner.BasePierce; // Projectile Pierce + Towers Pierce. Good to seperate out so you can upgrade pierce on a tower without swapping projectiles, but also have a projectle type have its own pierce.
        if (Tracking == false)
        {
            StartCoroutine(DeleteAfterLifetime());
        }
    
       
    }

    public void ExtraProjectileFailsafe()
    {
        StartCoroutine(DeleteAfterLifetime());
    }


    
    

    private IEnumerator DeleteAfterLifetime()
    {
       
            yield return new WaitForSeconds(LifetimeForNonTracking);
        Debug.Log("Destroying after lifetime:" + transform.name);
        if (ProjTargetObj)
        {
            ProjTargetObj.EnemyFutureDamage -= ProjOwner.Damage;
        }
       Destroy(this.gameObject);


    }

    //normal one
    public virtual void ProjectileCollide()
    {
       // Collided = true;
       // Destroy(this.GetComponent<MeshRenderer>());
        Debug.Log("Hit!");
       
     //   Debug.Log("You did: " + ProjOwner.Damage + "To the enemies total health of" + ProjTargetObj.Health + " With the enemy called" + ProjTargetObj.name);


        /*

        for (int i = 0; i < ProjOwner.UpgradeList.Count; i++)
        {
            ProjOwner.UpgradeList[i].Effect.OnHit(ProjOwner, ProjTargetObj);
        }
        */
        for (int j = 0; j < ProjOwner.UpgradeList.Count; j++) // better then number ids maybe?
        {
            for (int l = 0; l < ProjOwner.UpgradeList[j].Effects.Count; l++) // better then number ids maybe?
            {

                ProjOwner.UpgradeList[j].Effects[l].OnHit(ProjOwner, ProjTargetObj);
            }
        }
        OnProjHit();
        if (ProjTargetObj && ProjTargetObj != null && ProjTargetObj.Health > 0)
        {
            ProjTargetObj.TakeDamage(ProjOwner.Damage, ProjOwner, this.gameObject, true); // base damage, checks if enemy is dead


            if (Pierce <= 0)
            {
                Destroy(this.gameObject);
                Collided = true;
            }
            else
            {
                Pierce -= 1;
                Debug.Log("pierce!");
                Tracking = false;
            }
        }
        else
        {
            Debug.Log("fake collision, explode");
            Destroy(this.gameObject);
            return ;
        }
    }

    //used for secondary targets
    public virtual void ProjectileCollideOtherTarget(BaseEnemy Enemy, bool TriggerMainEffect, bool OnHitUpgradeEffect)
    {
        Debug.Log("HIT OTHER TARGET");
       // Collided = true;
        Destroy(this.GetComponent<MeshRenderer>());
        Debug.Log("Hit!");
        if (TriggerMainEffect) // this is specifically so stuff like bombs or other AOE events that trigger on hit don't cascade. IE while going towards the target, bomb explodes, great! It calls Projectile Collide.
            //this applies the projectiles OnProjHit();, which in this case gets any enemies around the projectile and also hurts them.
            //This would then trigger a second explosion, and then a third.. (hypothetically), all from the same projectile.
            //IF YOU DO THIS ON ANYTHING THAT AFFECTS OTHER ENEMIES AFTERWARDS
            //!!! THIS WILL CRASH !!!
        {
            OnProjHit();
            
        }
        else
        {
            OnSecondaryProjHit();
        }
        //this will *presumably* 
        if (OnHitUpgradeEffect)
        {
            for (int j = 0; j < ProjOwner.UpgradeList.Count; j++) // better then number ids maybe?
            {
                for (int l = 0; l < ProjOwner.UpgradeList[j].Effects.Count; l++) // better then number ids maybe?
                {

                    ProjOwner.UpgradeList[j].Effects[l].OnHit(ProjOwner, Enemy);
                }
            }
            /*
            for (int i = 0; i < ProjOwner.UpgradeList.Count; i++)
            {
                ProjOwner.UpgradeList[i].Effect.OnHit(ProjOwner, Enemy);
            }
            */

        }

            //Debug.Log("You did: " + ProjOwner.Damage + "To the enemies total health of" + Enemy.Health + " With the enemy called" + Enemy.name);
        //Enemy.Health -= ProjOwner.Damage;
        Enemy.TakeDamage(ProjOwner.Damage, ProjOwner, this.gameObject, false); // base damage, checks if enemy is dead
        /*
        if (Enemy.Health <= 0 && ProjOwner.Searching == false && Enemy != null)
        {
            // Destroy(CurrentTarget.gameObject);
            // LevelPath.Enemies.TrimExcess();
            Enemy.Death();
            //ProjOwner.FindEnemy();

        }
        */

        //for the love of god don't put pierce on bombs
       // Pierce -= 1;
        if (Pierce <= 0)
        {

            Collided = true;
            if (ProjTargetObj)
            {
                ProjTargetObj.EnemyFutureDamage -= ProjOwner.Damage;
            }
            Destroy(this.gameObject);
        }
        else
        {
            Debug.Log("pierce!");
            Pierce -= 1;
            Collided = false;
            //Tracking = false;
        }
        //Destroy(this.gameObject); // this works for stuff like bombs etc, since it puts it on the delete queue and doesn't delete it instantly

    }

    public virtual void OnProjHit()
    {

    }

    public virtual void OnSecondaryProjHit()
    {

    }
    public virtual void OnProjFire()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (ProjTargetObj && ProjTargetObj.Health <= 0 && Tracking || Tracking && ProjTargetObj == null) //failsafe incase the projectiles STILL double up despite all the other checks. second half of check might be iffy.
        {
            Destroy(this.gameObject);
        }

        //this is so messy..

        //float DistanceBetweenPoints = Vector3.Distance(transform.position, ProjTargetObj.transform.position);

        if (ProjTargetObj && Collided == false && Tracking == true)
        {
            var step = Speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, ProjTargetObj.transform.position, step);
           
            /*
            if (DistanceBetweenPoints < 0.05f) //doing it via distance seems sketchy
            {
                ProjectileCollide();
            }
            */
            
        }
        else if(Collided == false && Tracking == false) // non tracking, who cares about target object
        {
            var step = Speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward * step, step);

            /*
            //since distance won't work since we aren't tracking..gotta do collision stuff
           
            //make sure its the right layer
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.25f, mask);
            foreach (var hitCollider in hitColliders)
            {
                //hitCollider.SendMessage("AddDamage");
                if (hitCollider.GetComponent<BaseEnemy>() && hitCollider.GetComponent<BaseEnemy>() != ProjTargetObj)
                {
                    
                    ProjectileCollideOtherTarget(hitCollider.GetComponent<BaseEnemy>(), true, true);
                }
            }


            if (DistanceBetweenPoints < 0.05f) //doing it via distance seems sketchy
            {
                ProjectileCollide();
            }
            */
        }
        if (Collided == false)
        {
            LayerMask mask = LayerMask.GetMask("Enemy");
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.05f, mask);
            foreach (var hitCollider in hitColliders)
            {
                //hitCollider.SendMessage("AddDamage");
                //if (hitCollider.GetComponent<BaseEnemy>() && hitCollider.GetComponent<BaseEnemy>() != ProjTargetObj)
                if (hitCollider.GetComponent<BaseEnemy>())
                {
                    
                    if (hitCollider.GetComponent<BaseEnemy>().LastProjectile == null || hitCollider.GetComponent<BaseEnemy>().LastProjectile != this.gameObject)
                    {
                        if (ProjTargetObj == null && Tracking == false || hitCollider.GetComponent<BaseEnemy>() != ProjTargetObj && Tracking == false)
                        {
                            ProjectileCollideOtherTarget(hitCollider.GetComponent<BaseEnemy>(), true, true);
                            break; // check if these breaks work
                        }
                        else
                        {
                            ProjectileCollide();
                            break;
                        }
                    }
                }
            }
        }

        /*
        if (DistanceBetweenPoints < 0.05f) //doing it via distance seems sketchy
        {
            ProjectileCollide();
        }
        */
        // else if (Vector3.Distance(transform.position, ProjTargetObj.transform.position) > 3)
        // {
        //     Destroy(this.gameObject);
        // }
    }
}
