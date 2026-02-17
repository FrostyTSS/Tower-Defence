using UnityEngine;

public class DartAbilitySwordCollision : MonoBehaviour
{

    public BaseTower OwnerTower;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided?");
        if (OwnerTower && other.GetComponent<BaseEnemy>())
        {
            Debug.Log("hit");
            BaseEnemy HitEnemy = other.GetComponent<BaseEnemy>();
            HitEnemy.TakeDamage(OwnerTower.Ability.Damage, OwnerTower);
        }
    }
}
