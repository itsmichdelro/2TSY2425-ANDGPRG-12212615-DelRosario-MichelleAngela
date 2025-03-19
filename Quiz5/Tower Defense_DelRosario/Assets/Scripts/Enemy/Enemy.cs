using UnityEngine;
using UnityEngine.AI;

public enum MonsterType
{ 
    Ground, Flying
}

public enum MonsterClass
{ 
    Normal, Boss
}

public class Enemy : MonoBehaviour 
{
    [Header("NavMesh")]
    [SerializeField] Transform goal;
    [SerializeField] NavMeshAgent agent;

    [Header("Enemy Variables")]
    [SerializeField] MonsterType type;
    [SerializeField] MonsterClass monsterClass;

    [SerializeField] private float currentHp;
    [SerializeField] private float maxHp;
    [SerializeField] private int goldDrop = 10;
    [SerializeField] private float damage;
    [SerializeField] private float startSpeed;

    private bool isDead = false;
    [SerializeField] private bool isSlowed = false;
    [SerializeField] private bool isBurning = false;
    [SerializeField] private float debuffTimer = 3f;
    private float debuffTime = 3f;

    [Header("Audio")]
    [SerializeField] AudioClip deathClip;

    private void Awake()
    {
        this.agent = this.GetComponent<NavMeshAgent>(); 
    }

    void Start()
    {
        startSpeed = agent.speed;

        if (this.monsterClass == MonsterClass.Boss)
        {
            this.maxHp = 1000;
            this.goldDrop = 50;
        }
        else
        {
            this.maxHp = 150;
            this.goldDrop = 10;
        }

        UpdateHealthPoints();
        SetGoldDrop();
        SetDamage();

        currentHp = maxHp;
    }

    private void Update()
    {
        if (debuffTimer > 0 && isBurning == true || debuffTimer > 0 && isSlowed == true)
        {
            debuffTimer -= Time.deltaTime;
        }
        else //reset debuff settings
        {
            isBurning = false;
            isSlowed = false;
            agent.speed = startSpeed;
            debuffTimer = debuffTime;
        }
    }

#region Setup / Enemy Variables
    private void SetGoldDrop()
    {
        // every wave, increase gold drop
        int wave = GameManager.Instance.GetWaveNumber;
        if (wave > 1)
        {
            if (this.monsterClass == MonsterClass.Normal)
            {
                wave += Random.Range(20, 40);
            }
            else if (this.monsterClass == MonsterClass.Boss)
            {
                wave += Random.Range(100, 150);
            }

            // new gold drop
            this.goldDrop += wave;
        }
    }

    private void UpdateHealthPoints()
    {
        int wave = GameManager.Instance.GetWaveNumber;
        if (wave > 1)
        {
            int hpIncrease = Random.Range(20,40);
            if (this.monsterClass == MonsterClass.Normal)
            {
                this.maxHp += hpIncrease;
            }
            else if (this.monsterClass == MonsterClass.Boss)
            {
                this.maxHp += hpIncrease * wave;
            }
        }
    }

    private void SetDamage()
    {
        if (this.monsterClass == MonsterClass.Boss)
        {
            this.damage = PlayerManager.Instance.GetStartHealth;
        }
        else this.damage = 1;
    }

    public void ApplySlowSpeed(float value)
    {
        if (isSlowed) return;
        else
        {
            isSlowed = true;
            agent.speed = startSpeed - value;
        }
        
        if (agent.speed <= 0) agent.speed = 1;
    }

    public void ApplyBurning(float value)
    {
        isBurning = true;
        TakeDamage(value);
    }
#endregion

#region Attacking Enemy / Damaging Core
    public void TakeDamage(float value)
    {
        currentHp -= value;
        if (currentHp <= 0 && !isDead) Death();
    }

    private void Death()
    {
        isDead = true;

        PlayerManager.Instance.AddGold(goldDrop);
        SpawnerController.Instance.RemoveEnemyFromList(this.gameObject);
        Destroy(this.gameObject);
    }

    public void SetTarget(Transform target)
    {
        this.goal = target;
        this.agent.SetDestination(this.goal.position); //set goal position
    }

    private void OnTriggerEnter(Collider other)
    {
        //enters core
        if (other.gameObject.name.Contains("CrystalCore"))
        {
            SpawnerController.Instance.RemoveEnemyFromList(this.gameObject);
            Destroy(this.gameObject);

            //remove core life
            PlayerManager.Instance.TakeDamage(damage);
        }
    }
    #endregion
}