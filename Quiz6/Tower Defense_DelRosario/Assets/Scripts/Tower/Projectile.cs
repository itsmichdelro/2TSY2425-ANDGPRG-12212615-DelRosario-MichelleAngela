using UnityEngine;

public enum ProjectileType
{
    Arrow, Cannon, Ice, Fire
}

public class Projectile : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Enemy targetEnemy;

    [SerializeField] ProjectileType type;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage;
    [SerializeField] private float splashRadius;
    [SerializeField] private float debuffValue;

    void Start()
    {
        targetEnemy = target.GetComponent<Enemy>();
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        //find direction the projectile needs to point in
        Vector3 dir = target.position - transform.position;
        float dist = speed * Time.deltaTime;

        if (dir.magnitude <= dist) //if length of direction vector is <= distance this frame / whenever a target is hit
        {
            HitTarget();
            return;
        }

        // if target isnt hit, move at a constant speed relative to world space
        transform.Translate(dir.normalized * dist, Space.World);
        transform.LookAt(target);
    }

    public void Setup(float damageVal, float rangeVal, float chilledVal, float burningVal)
    { 
        damage = damageVal;
        splashRadius = rangeVal;

        if (type == ProjectileType.Ice)
        {
            debuffValue = chilledVal;
        }
        else if (type == ProjectileType.Fire)
        {
            debuffValue = burningVal;
        }
        else debuffValue = 0;
    }

    private void HitTarget()
    {
        //Debug.Log("enemy hit");
        if (splashRadius > 0 && type != ProjectileType.Arrow) //for splash attack towers
        {
            ApplySplashEffect();
        }
        else //make single target, for arrow tower only
        {
            DamageEnemy(target);
        }

        Destroy(gameObject);
    }

    private void ApplySplashEffect()
    {
        //shoot out a sphere and check all colliders that overlap the sphere
        Collider[] colliders = Physics.OverlapSphere(transform.position, splashRadius);

        foreach (Collider collider in colliders)
        {
            if (collider == null) return;

            if (type == ProjectileType.Cannon && collider.gameObject.layer == 7) // 7: ground enemy layer          
            {
                DamageEnemy(collider.transform);
            }
            if (type == ProjectileType.Ice || type == ProjectileType.Fire)
            {
                if (collider.CompareTag("Enemy"))
                {
                    DamageEnemy(collider.transform);
                    if (type == ProjectileType.Ice)
                    {
                        targetEnemy.ApplySlowSpeed(debuffValue); //slow enemy
                    }
                    else
                    {
                        targetEnemy.ApplyBurning(debuffValue * Time.deltaTime); // fire DoT
                    }
                }
            }
        }
    }

    public void DamageEnemy(Transform target)
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null) enemy.TakeDamage(damage);
    }

    public void SetTarget(Transform currentTarget) //gets the current target of tower and sets it as the projectile's target
    {
        this.target = currentTarget;
    }

    private void OnDrawGizmosSelected() //show splash radius
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}
