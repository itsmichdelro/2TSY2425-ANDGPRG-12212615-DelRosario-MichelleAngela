using UnityEngine;

public class PlayerAwarenessController : MonoBehaviour
{
    public bool AwareOfPlayer { get; private set; }
    public Vector2 DirectionToPlayer { get; private set; }

    [SerializeField] private float playerAwarenessDistance = 10f;
    private Transform player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player != null)
        {
            Vector2 enemyToPlayerVector = player.position - transform.position;
            DirectionToPlayer = enemyToPlayerVector.normalized;
            AwareOfPlayer = enemyToPlayerVector.magnitude <= playerAwarenessDistance;
        }
    }
}

