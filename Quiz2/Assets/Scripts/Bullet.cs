using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f;

    void Start()
    {
        // Destroy bullet after lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move forward based on bullet's current rotation
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(10);
            }
            Destroy(gameObject);
        }
    }
}