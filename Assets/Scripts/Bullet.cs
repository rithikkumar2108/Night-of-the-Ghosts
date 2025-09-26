using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Collider collder;
    public float Damage;
    public LayerMask TargetMask;
    public ParticleSystem Impact;

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
        Debug.Log(gameObject.transform.name);
        Destroy(gameObject,2f);
        ParticleSystem go = Instantiate(Impact, transform.position, transform.rotation);
        Destroy(go,2f);
        
        if(((1<<collision.gameObject.layer) & TargetMask) != 0)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            damageable.TakeDamage(Damage);
        }
    }

}
