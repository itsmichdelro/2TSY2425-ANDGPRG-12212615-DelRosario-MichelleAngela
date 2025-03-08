using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterType
{
    Ground,
    Flying,
    Boss
}

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] MonsterType monsterType;
    [SerializeField] float health = 100f;
    [SerializeField] int goldValue = 10;
    [SerializeField] float movementSpeed = 3.5f;
    [SerializeField] bool isBoss = false;

    // Added waypoints for flying enemies
    private List<Vector3> waypoints = new List<Vector3>();
    private int currentWaypointIndex = 0;
    private bool waypointsGenerated = false;

    void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();

        // Set up flying monsters differently
        if (monsterType == MonsterType.Flying)
        {
            agent.baseOffset = 2f; // Make it fly above ground
            // Flying enemies will use waypoint system instead of NavMesh
            if (agent != null)
            {
                agent.enabled = false; // Disable NavMeshAgent for flying enemies
            }
        }

        // Make boss monsters larger and stronger
        if (isBoss)
        {
            transform.localScale *= 1.5f;
            health *= 5f;
            goldValue *= 3;
            if (agent != null && agent.enabled)
            {
                agent.speed *= 0.7f; // Make bosses a bit slower
            }
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;

        if (monsterType == MonsterType.Flying)
        {
            // For flying enemies, generate waypoints along the path
            GenerateWaypoints();
        }
        else if (agent != null && agent.enabled)
        {
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(target.position);
            }
            else
            {
                Debug.LogWarning("Agent not on NavMesh: " + gameObject.name);
                // Try to place on NavMesh
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                    if (agent.isOnNavMesh)
                    {
                        agent.SetDestination(target.position);
                    }
                }
            }
        }
    }

    private void GenerateWaypoints()
    {
        waypoints.Clear();
        waypointsGenerated = true;

        // Try to find the ground path using NavMesh
        NavMeshPath path = new NavMeshPath();
        NavMeshHit hit;
        Vector3 startPos = transform.position;

        // Sample a nearby position on NavMesh for path finding
        if (NavMesh.SamplePosition(startPos, out hit, 10f, NavMesh.AllAreas))
        {
            startPos = hit.position;
        }

        // Calculate a path along the NavMesh from start to target
        if (NavMesh.CalculatePath(startPos, target.position, NavMesh.AllAreas, path))
        {
            // Add waypoints for each corner in the path
            foreach (Vector3 corner in path.corners)
            {
                Vector3 flyingWaypoint = corner;
                flyingWaypoint.y += 2f; // Elevate waypoint for flying
                waypoints.Add(flyingWaypoint);
            }

            // Add final target as last waypoint
            Vector3 finalPoint = target.position;
            finalPoint.y += 2f;
            waypoints.Add(finalPoint);
        }
        else
        {
            // If path generation fails, just add direct target as waypoint
            Debug.LogWarning("Flying enemy path generation failed, using direct path");
            waypoints.Add(transform.position);
            Vector3 finalPoint = target.position;
            finalPoint.y += 2f;
            waypoints.Add(finalPoint);
        }

        currentWaypointIndex = 0;
    }

    public MonsterType GetMonsterType()
    {
        return monsterType;
    }

    public void SetupEnemy(float waveHealthMultiplier, float waveGoldMultiplier)
    {
        health *= waveHealthMultiplier;
        goldValue = Mathf.RoundToInt(goldValue * waveGoldMultiplier);
    }

    public bool IsBoss()
    {
        return isBoss;
    }

    public int GetGoldValue()
    {
        return goldValue;
    }

    // Add this to your existing Enemy.cs file if needed
    public void TakeDamage(float damage)
    {
        health -= damage;

        // Visual feedback (optional)
        StartCoroutine(FlashRed());

        if (health <= 0)
        {
            // Add gold to player
            GameManager.Instance.AddGold(goldValue);

            // Remove from enemy list and destroy
            SpawnerController.Instance.RemoveEnemy(this);
            Destroy(this.gameObject);
        }
    }

    private IEnumerator FlashRed()
    {
        // Get all renderers
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        List<Material> originalMaterials = new List<Material>();

        // Store original materials and set to red
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            foreach (Material material in materials)
            {
                originalMaterials.Add(new Material(material));
                material.color = Color.red;
            }
        }

        yield return new WaitForSeconds(0.1f);

        // Restore original materials
        int materialIndex = 0;
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materialIndex < originalMaterials.Count)
                {
                    materials[i].color = originalMaterials[materialIndex].color;
                    materialIndex++;
                }
            }
        }
    }

    private void Update()
    {
        // For flying monsters, use waypoint system
        if (monsterType == MonsterType.Flying)
        {
            if (waypointsGenerated && waypoints.Count > 0)
            {
                // Move towards current waypoint
                Vector3 targetWaypoint = waypoints[currentWaypointIndex];
                Vector3 direction = (targetWaypoint - transform.position).normalized;
                transform.position += direction * movementSpeed * Time.deltaTime;

                // Look at direction of movement
                if (direction != Vector3.zero)
                {
                    transform.forward = direction;
                }

                // Check if reached current waypoint
                float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint);
                if (distanceToWaypoint < 0.5f)
                {
                    // Move to next waypoint
                    currentWaypointIndex++;

                    // If reached last waypoint, move directly to target
                    if (currentWaypointIndex >= waypoints.Count)
                    {
                        currentWaypointIndex = waypoints.Count - 1;
                    }
                }
            }
            else if (target != null)
            {
                // Fallback if waypoints weren't generated
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += direction * movementSpeed * Time.deltaTime;

                // Look at direction of movement
                if (direction != Vector3.zero)
                {
                    transform.forward = direction;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("CrystalCore"))
        {
            // Reduce crystal core health
            if (isBoss)
            {
                GameManager.Instance.DamageCrystalCore(GameManager.Instance.GetMaxCoreHealth());
            }
            else
            {
                GameManager.Instance.DamageCrystalCore(1);
            }

            SpawnerController.Instance.RemoveEnemy(this);
            Destroy(this.gameObject);
        }
    }
}