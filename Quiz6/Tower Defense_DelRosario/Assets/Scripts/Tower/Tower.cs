using System;
using UnityEngine;

using Random = UnityEngine.Random;

public enum Tier
{
    One, Two, Three
}

public enum Range
{
    Short, Medium, Long
}

public class Tower : MonoBehaviour
{
    public Material towerMatInstance;

    [Header("Tower Info")]
    [SerializeField] Material towerMaterial; //access tower material
    [SerializeField] private int towerPrice;
    [SerializeField] private int upgradePrice;

    [SerializeField] private float buildDuration;
    [SerializeField] private float buildTimer;
    [SerializeField] private bool isBuilding = false;
    private bool isBuilt = false;

    [SerializeField] Tier tierNumber;

    [Header("Tower Variables")]
    [SerializeField] private float damage;
    [SerializeField] private float fireRateValue;
    [SerializeField] private float fireCountdown = 0f;

    [SerializeField] private Range fireRange;
    [SerializeField] private float rangeValue;

    [Header("Tower Turret")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform turret; //rotating part of tower
    [SerializeField] private float turnSpeed; //how fast turret turns/rotates
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileStartingPoint;
    private string enemyTag = "Enemy";

    [Header("Debuffs")]
    [SerializeField] private float chilledDebuff = 1f;
    [SerializeField] private float burningDebuff = 5f;

    [Header("Audio")]
    [SerializeField] AudioClip towerShootClip;

    private void Awake()
    {
        towerMatInstance = new Material(towerMaterial);
        SetMaterial(towerMatInstance);
    }

    private void Start()
    {
        buildTimer = buildDuration;
        Setup();
        SetDamage();
        InvokeRepeating("UpdateTarget", 0f, .5f);
    }

    private void Update()
    {
        if (isBuilding == true)
        {
            if (buildTimer <= 0f) //building time end
            {
                isBuilding = false;
                isBuilt = true;
                buildTimer = buildDuration;
            }

            buildTimer -= Time.deltaTime;
        }
        else if (isBuilding == false && isBuilt == true)
        {
            LockOnTarget();
            if (fireCountdown <= 0f) //if it is time to shoot
            {
                if (target == null) return; //stop whole script if there is no target

                Shoot();
                fireCountdown = 1f / this.fireRateValue; //reset countdown 
            }

            fireCountdown -= Time.deltaTime;
        }
    }

    private void Setup()
    {
        turret = this.transform.Find("Base1/Turret1").transform;
        projectileStartingPoint = this.transform.Find("Base1/Turret1/Projectile Starting Point");
    }

    public void UpdateTurretAndProjectileStartingPoint()
    {
        if (tierNumber == Tier.Two)
        {
            turret = this.transform.Find("Base2/Turret2").transform;
            projectileStartingPoint = this.transform.Find("Base2/Turret2/Projectile Starting Point");
        }
        if (tierNumber == Tier.Three)
        {
            turret = this.transform.Find("Base3/Turret3").transform;
            projectileStartingPoint = this.transform.Find("Base3/Turret3/Projectile Starting Point");
        }
    }

    #region Building
    public void Buildable()
    {
        SetColor(Color.green);
    }

    public void NonBuildable()
    {
        SetColor(Color.red);
    }

    public void Build()
    {
        isBuilding = true;
        SetColor(Color.white);
    }

    private void SetMaterial(Material mat)
    {
        Renderer[] rend = GetComponentsInChildren<Renderer>();
        foreach (var renderer in rend)
        {
            renderer.material = mat;
        }
    }

    public void SetColor(Color col)
    {
        Renderer[] rend = GetComponentsInChildren<Renderer>();
        foreach (var renderer in rend)
        {
            renderer.material.color = col;
        }
    }
    #endregion

    #region Targeting
    private void UpdateTarget()
    {
        if (isBuilding == true) return;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDist = Mathf.Infinity;
        GameObject nearestEnemy = null;

        //loops thru enemies array
        foreach (GameObject enemy in enemies)
        {
            float distToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distToEnemy < shortestDist) //found new enemy closer than prev enemy
            {
                shortestDist = distToEnemy;
                nearestEnemy = enemy; //the nearest enemy is this enemy closest to the tower
            }
        }

        if (nearestEnemy != null && shortestDist <= rangeValue) //change enemy target
        {
            target = nearestEnemy.transform;
        }
        else target = null; // removes enemy from targeting when out of range
    }

    private void LockOnTarget()
    {
        if (isBuilding == true) return;
        if (target == null) return;

        //direction of turret / turret rotation based on enemy's position
        Vector3 dir = (target.position - transform.position).normalized;
        var lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(turret.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles; //smoothly rotate the turret based on turn speed and delta time
        turret.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }
    #endregion

    #region Attacking
    private void Shoot()
    {
        if (isBuilding == true) return;
        GameObject projectileObj = (GameObject)Instantiate(projectilePrefab, projectileStartingPoint.position, projectileStartingPoint.rotation);
        Projectile projectile = projectileObj.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Setup(damage, rangeValue, chilledDebuff, burningDebuff);
            projectile.SetTarget(target);
        }

        FindObjectOfType<AudioManager>().PlaySound(this.towerShootClip.name);
    }

    private void SetDamage()
    {
        if (fireRange == Range.Short)
        {
            damage = Random.Range(20, 25);
        }
        else if (fireRange == Range.Medium)
        {
            damage = Random.Range(10, 13);
        }
        else if (fireRange == Range.Long)
        {
            damage = Random.Range(30, 50);
        }
    }
    #endregion

    #region Upgrading
    public void UpgradeStats()
    {
        if (tierNumber == Tier.Two)
        {
            upgradePrice += 150;
            damage += 3f;
            fireRateValue += 1f;
            rangeValue += 1f;
        }
        if (tierNumber == Tier.Three)
        {
            damage += 5f;
            fireRateValue += 2f;
            rangeValue += 2f;
        }

        if (this.name.Contains("Fire")) //increase burning debuff
        {
            if (tierNumber == Tier.Two) burningDebuff += 1.5f;
            if (tierNumber == Tier.Three) burningDebuff += 3f;
        }
        if (this.name.Contains("Crystal")) //increase chilled debuff
        {
            if (tierNumber == Tier.Two) chilledDebuff += 1.5f;
            if (tierNumber == Tier.Three) chilledDebuff += 3f;
        }
    }
    #endregion

    private void OnDrawGizmosSelected() //show range
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeValue);
    }

    #region Getters and Setters
    public bool GetIsBuilding() { return isBuilding; }
    public int GetTowerPrice() { return towerPrice; }
    public int GetUpgradePrice() { return upgradePrice; }
    public Tier GetTierNumber() { return tierNumber; }
    public float GetDamage() {  return damage; }
    public float GetFireRate() {  return fireRateValue; }
    public float GetRange() {  return rangeValue; }
    public float GetBurningDamage() { return burningDebuff; }
    public float GetChilledValue() { return chilledDebuff; }
    public void SetTierNumber(Tier newTierNum) { tierNumber = newTierNum; }
    #endregion
}