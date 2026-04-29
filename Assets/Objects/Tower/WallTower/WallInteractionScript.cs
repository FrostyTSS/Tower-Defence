using UnityEngine;
using System.Collections;
public class WallInteractionScript : MonoBehaviour
{
    public WallTowerScript OwningTower;
    int EnemyLayerID;
    public int StartingUses = 10;
    public int RemainingUses = 10;
    public float RechargeTime = 2.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RemainingUses = StartingUses;
        EnemyLayerID = LayerMask.NameToLayer("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.GetComponent<BaseEnemy>())
        {
            BaseEnemy CollidingEnemy = other.GetComponent<BaseEnemy>();
            Debug.Log(other.name);
            if (other.gameObject.layer == EnemyLayerID && CollidingEnemy.CurrentSpeed >= CollidingEnemy.MaxSpeed && RemainingUses > 0)
            //   if (other.gameObject.layer == EnemyLayerID)
            {
                CollidingEnemy.TakeDamage(1, OwningTower);
                //  other.gameObject.GetComponent<BaseEnemy>().Slowdown(-1.0f);
                CollidingEnemy.StartCoroutine(CollidingEnemy.Slowdown(-1.0f, 1.5f, 1.0f));
                RemainingUses -= 1;
                Debug.Log("Hit!");
                if (RemainingUses <= 0)
                {
                    OwningTower.StartCoroutine(OwningTower.RotateAroundOnBreak());
                    StartCoroutine(ShieldRecharge());
                }
            }

        }
    }


    public IEnumerator ShieldRecharge()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(RechargeTime);
        RemainingUses = StartingUses;
        this.GetComponent<MeshRenderer>().enabled = true;
        this.GetComponent<BoxCollider>().enabled = true;
    }

}
