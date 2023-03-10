using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IHealth
{
    public UnityEvent Died = new();
    public UnityEvent TreeHealthChanged = new();
    public UnityEvent TreeHealed = new();
    public Animator Animator;

    [SerializeField] bool ebnableDebugLog;
    [SerializeField] private HealthScriptableObject healthScriptableObject;
    private int maxHealth;
    private int currentHealth;
    private int startingHealth;
    private float tickDamageRate;
    private int tickDamageAmount;
    private int tickStableHealth;

    private float lastTickTime;

    void Start()
    {
        ApplyConfig(healthScriptableObject);
        tickDamageRate = tickDamageRate * 0.01f;
        TreeHealthChanged.Invoke();
    }

    public void Die()
    {
        Died.Invoke();
        if (Animator)
        {
            Animator.SetTrigger("Death");
            StartCoroutine(DelayedDestroy());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    public void Heal(int hpToheal)
    {
        currentHealth += hpToheal;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        TreeHealthChanged.Invoke();
        TreeHealed.Invoke();
    }

    void Update()
    {
        if (tickDamageRate > 0 && Time.time > lastTickTime + tickDamageRate)
        {
            lastTickTime = Time.time;
            TickDamage();
        }
    }

    public void TickDamage()
    {
        int tickFloor = tickStableHealth;
        if (GetComponent<Tree>())
        {
            tickFloor /= 2;
            if (transform.parent.GetComponent<Crossroad>())
            {
                tickFloor *= transform.parent.GetComponent<Crossroad>().ConnectedTreesAmount();
            } else
            {
                tickFloor *= transform.parent.GetComponent<CrossroadGrowth>().ConnectedTreesAmount();
            }
        }

        if (currentHealth >= tickStableHealth + tickFloor)
        {
            TakeDamage(tickDamageAmount);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (ebnableDebugLog)
        {
            Debug.Log($"Took damge, new healt: {currentHealth}");
        }
        if (currentHealth < 0)
        {
            Die();
        }

        TreeHealthChanged.Invoke();
    }

    public void ApplyConfig(HealthScriptableObject healthConfig)
    {
        this.maxHealth = healthConfig.MaxHealth;
        this.currentHealth = healthConfig.StartingHealth;
        this.startingHealth = healthConfig.StartingHealth;
        this.tickDamageRate = healthConfig.TickDamageRate;
        this.tickDamageAmount = healthConfig.TickDamageAmount;
        this.tickStableHealth = healthConfig.TickStableHealth;
    }

    public void SetHealthByRatio(float ratio)
    {
        int calulatedHealth = (int)(ratio * this.startingHealth);
        this.currentHealth = calulatedHealth;

        TreeHealthChanged.Invoke();
    }

    public int getCurrentHealth() { return currentHealth; }
    public int getStartingHealth() { return startingHealth; }
}
