using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    Animator animator;

    [SerializeField]
    private int _maxHealth = 100;
    public int MaxHealth
    {
        get 
        {
            return _maxHealth;
        }
        set 
        {
            _maxHealth = value;
        }
    }
    [SerializeField]
    private int _health = 100;
   
    public int Health
    {
        get 
        {
            return _health;
        }
        set 
        {
            _health = value;
            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

    [SerializeField]
    private bool _isAlive = true;
    [SerializeField]
    private bool isInvincible = false;

    private float timeSinceHit =0f;
    public float invincibilityTime = 0.25f;

    public bool IsAlive {
    get
    {
        return _isAlive;
    } private set
    {
        _isAlive = value;
        animator.SetBool(AnimationStrings.isAlive, value);
        Debug.Log("IsAlive set " + value);
        
    } }

    public bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            animator.SetTrigger(AnimationStrings.hitTrigger);
            //Notify other components that the damageable was hit to handle knockback
            damageableHit?.Invoke(damage, knockback);

            return true;
            
        }
        return false;
    }

    public void Update()
    {
        if(isInvincible)
        {
            if(timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }
}
