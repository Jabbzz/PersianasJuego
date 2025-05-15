using UnityEngine;

public class Attack : MonoBehaviour
{
    Collider2D attackCollider;
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;

    private void Awake()
    {
        attackCollider = GetComponent<Collider2D>();   
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            float direction = Mathf.Sign(collision.transform.position.x - transform.position.x);
            Vector2 deliveredKnockback = new Vector2(knockback.x * direction, knockback.y);
            bool GotHit = damageable.Hit(attackDamage, deliveredKnockback);
            if(GotHit)
                Debug.Log(collision.name + "hit for " + attackDamage);
        }
    }
}
