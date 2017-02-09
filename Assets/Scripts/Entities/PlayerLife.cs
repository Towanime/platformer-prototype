using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : DamageableEntity {

    public float timeInvinsible;
    public float timeStunned;
    public float timePerFlick;
    public bool flickerEnabled;
    public List<Renderer> renderersForFlicker;
    public float knockbackForceX;
    public float knockbackForceY;
    public CharacterMovement characterMovement;
    private bool rendering;
    private bool invinsible;
    private bool stunned;
    private float elapsedTime;
    private float elapsedFickerTime;

    public override bool OnDamage(GameObject origin, float damage)
    {
        bool damaged = false;
        float damageToApply = 1; // Make it 1 always
        if (!invinsible && !stunned)
        {
            damaged = base.OnDamage(origin, damageToApply);
            if (damaged)
            {
                stunned = true;
                ApplyKnockback(origin);
                elapsedTime = 0;
            }
        }
        return damaged;
    }

    private void ApplyKnockback(GameObject origin)
    {
        bool attackFromTheRight = origin.transform.position.x >= transform.position.x;
        float knockbackXDirection = attackFromTheRight ? -1 : 1;
        characterMovement.AddForce(knockbackForceX * knockbackXDirection, knockbackForceY);
    }

	void Update () {
        if (stunned)
        {
            UpdateStunned();
        } else if (invinsible)
        {
            if (flickerEnabled)
            {
                UpdateFlicker();
            }
            UpdateInvinsible();
        }
	}

    private void UpdateStunned()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= timeStunned)
        {
            invinsible = true;
            stunned = false;
            elapsedTime = 0;
            rendering = false;
            elapsedFickerTime = 0;
            if (flickerEnabled)
            {
                EnableRendering(rendering);
            }
        }
    }

    private void UpdateInvinsible()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= timeInvinsible)
        {
            EnableRendering(true);
            invinsible = false;
        }
    }

    private void UpdateFlicker()
    {
        elapsedFickerTime += Time.deltaTime;
        if (elapsedFickerTime >= timePerFlick)
        {
            elapsedFickerTime = 0;
            rendering = !rendering;
            EnableRendering(rendering);
        }
    }

    private void EnableRendering(bool enabled)
    {
        foreach (Renderer renderer in renderersForFlicker)
        {
            renderer.enabled = enabled;
        }
    }
}
