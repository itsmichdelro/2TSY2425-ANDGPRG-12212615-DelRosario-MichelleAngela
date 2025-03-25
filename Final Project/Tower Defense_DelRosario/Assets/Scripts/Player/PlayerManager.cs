using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
   
    [SerializeField] private int goldAmount;
    private float startHealth = 200;
    private float currentHealth;
    public Image healthBar;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        goldAmount = 500;
        currentHealth = startHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthBar.fillAmount = currentHealth/startHealth;

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death() //end game
    {
        Debug.Log("core destroyed");
        UIHandler.Instance.ShowEndGamePanel("losePanel");
    }

    public void AddGold(int goldDrop)
    {
        this.goldAmount += goldDrop;
    }

    public void DecreaseGold(int amount)
    {
        this.goldAmount -= amount;
    }

    public float GetStartHealth { get { return startHealth; } }
    public float GetCurrentHealth { get { return currentHealth; } }
    public int GetGoldAmount { get { return goldAmount; } }
}
