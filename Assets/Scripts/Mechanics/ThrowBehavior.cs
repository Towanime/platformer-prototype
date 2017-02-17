using UnityEngine;
using System.Collections;

/// <summary>
/// Marks a game object as grabbable and returns a game object to attach to the player when the grab is successful.
/// </summary>
public class ThrowBehavior : MonoBehaviour {

    private bool beingThrown;
    private float elapsedTravelTime;
    private float throwTravelTime;
    private float elapsedDeathTime;
    private float throwDeathTime;
    private AnimationCurve throwSpeedCurve;
    private Vector2 throwDirection;
    private float throwSpeed;

    void FixedUpdate()
    {
        if (!beingThrown) return;
        if (elapsedDeathTime >= throwDeathTime)
        {
            FinishThrow();
            return;
        }
        UpdateThrowPosition();
        elapsedTravelTime += Time.fixedDeltaTime;
        elapsedDeathTime += Time.fixedDeltaTime;
    }

    private void UpdateThrowPosition()
    {
        // Get current value interval between 0 and 1 from the animation curve depending on the elapsed time
        float t = 1 - throwSpeedCurve.Evaluate(elapsedTravelTime / throwTravelTime);
        // Use that value to get the current speed, 0 = Fullspeed and 1 = Stopped.
        float currentSpeed = Mathf.Lerp(throwSpeed, 0, t);
        transform.Translate(throwDirection * currentSpeed * Time.fixedDeltaTime, Space.World);
    }

    /// <summary>
    /// Kill the enemy when the death timer runs out without dropping a soul
    /// </summary>
    private void FinishThrow()
    {
        beingThrown = false;
        EnemyDamageableEntity damageableEntity = gameObject.GetComponent<EnemyDamageableEntity>();
        if (damageableEntity != null)
        {
            damageableEntity.DropSoul = false;
        }
        gameObject.SendMessage("OnDeath");
        Destroy(gameObject);
    }

    public void BeginThrow(Vector2 direction, float speed, float throwTravelTime, float throwDeathTime, AnimationCurve throwSpeedCurve)
    {
        this.throwDirection = direction;
        this.throwSpeed = speed;
        this.throwTravelTime = throwTravelTime;
        this.throwSpeedCurve = throwSpeedCurve;
        this.throwDeathTime = throwDeathTime;
        beingThrown = true;
        elapsedTravelTime = 0;
        elapsedDeathTime = 0;
    }

    void OnDamageApplied()
    {
        // Reset death timer every time it receives damage
        elapsedDeathTime = 0;
    }
}
