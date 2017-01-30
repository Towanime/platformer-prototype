using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public float lifeSpan = 2;
    public float speed = 5;
    private int direction; // 1 or -1

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * direction, 0, 0);
    }

    void OnEnable()
    {
        Invoke("Destroy", lifeSpan);
    }

    void Destroy()
    {
        BulletPool.instance.ReleaseObject(gameObject);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    // bullet hit something, do damage and destroy!
    public void OnTriggerEnter(Collider other)
    {
        // maybe do othe checks here later
        // check if the object is damagable
        DamageableEntity entity = other.gameObject.GetComponent<DamageableEntity>();
        if (entity)
        {
            // do bullet damage
            entity.OnDamage(damage);
        }
        Destroy();
    }

    // has to be on the object that has the renderer component!
    public void OnBecameInvisible()
    {
        Destroy();
    }

    public void SetDirection(int direction)
    {
        this.direction = direction;
    }
}
