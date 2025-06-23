using UnityEngine;

public class Bomb : ProjectileBase
{
    public float Radius = 1.5f;
     bool ActivateHitOnSecondaryTargets = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnProjHit()
    {
        //for later:
        //explosion VFX
        //bloons going down to their next layer

        LayerMask mask = LayerMask.GetMask("Enemy");
        //make sure its the right layer
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius, mask);
        foreach (var hitCollider in hitColliders)
        {
            //hitCollider.SendMessage("AddDamage");
            if (hitCollider.GetComponent<BaseEnemy>() && hitCollider.GetComponent<BaseEnemy>() != ProjTargetObj)
            {
                Debug.Log("BOOM, SPLASH! SPLASHED TO" + hitCollider.name);
                ProjectileCollideOtherTarget(hitCollider.GetComponent<BaseEnemy>(), ActivateHitOnSecondaryTargets, false);
            }
        }
    }
}
