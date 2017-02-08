using UnityEngine;
using System.Collections;

public class GrababbleEntity : MonoBehaviour {

    private bool beingThrown;
    private float elapsedTime;
    private float throwTravelTime;
    private AnimationCurve throwSpeedCurve;
    private Vector2 throwDirection;
    private float throwSpeed;

    /// <summary>
    /// Checks conditions and prepares a body to be able attached to the arm.
    /// </summary>
    /// <returns>Returns true if the grab is successful or false if there's a condition to meet before grabbing an enemy.</returns>
	public virtual bool OnGrab()
    {
        return !beingThrown;
    }

    void FixedUpdate()
    {
        if (!beingThrown) return;
        if (elapsedTime >= throwTravelTime)
        {
            FinishThrow();
            return;
        }
        UpdateThrowPosition();
        elapsedTime += Time.fixedDeltaTime;
    }

    private void UpdateThrowPosition()
    {
        // Get current value interval between 0 and 1 from the animation curve depending on the elapsed time
        float t = 1 - throwSpeedCurve.Evaluate(elapsedTime / throwTravelTime);
        // Use that value to get the current speed, 0 = Fullspeed and 1 = Stopped.
        float currentSpeed = Mathf.Lerp(throwSpeed, 0, t);
        transform.Translate(throwDirection * currentSpeed * Time.fixedDeltaTime, Space.World);
    }

    /// <summary>
    /// Kill the enemy at the end of the throw without dropping a soul
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

    public void BeginThrow(Vector2 direction, float speed, float throwTravelTime, AnimationCurve throwSpeedCurve)
    {
        this.throwDirection = direction;
        this.throwSpeed = speed;
        this.throwTravelTime = throwTravelTime;
        this.throwSpeedCurve = throwSpeedCurve;
        beingThrown = true;
        elapsedTime = 0;
    }
}
