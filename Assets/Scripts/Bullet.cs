using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Hazard
{
    public float lifeSpan = 2;
    public float speed = 5;
    private Vector2 direction;
    public LayerMask ignoreCollisionMask;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * direction.x, speed * Time.deltaTime * direction.y, 0, Space.World);
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
        if (Utils.IsObjectInLayerMask(ignoreCollisionMask, other.gameObject)) return;
        DoDamage(other.gameObject);
        Destroy();
    }

    // has to be on the object that has the renderer component!
    public void OnBecameInvisible()
    {
        Destroy();
    }

    public void SetDirection(Vector2 direction)
    {
        this.direction.Set(direction.x, direction.y);
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
