using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : DamageableEntity {

    public ActionStateMachine mainStateMachine;
    [Tooltip("Time that the player will stay immune from attacks but able to control the character.")]
    public float timeImmune;
    [Tooltip("Time that the player will stay immune from attacks and unable to control the character.")]
    public float timeStunned;
    [Tooltip("Time in seconds before the rendering is switched between on/off when flickering.")]
    public float timePerFlick;
    [Tooltip("Enable flickering when immune?")]
    public bool flickerEnabled;
    [Tooltip("Renderers that will be enabled/disabled when flickering.")]
    public List<Renderer> renderersForFlicker;
    [Tooltip("Force in X to apply for the knockback when hit, positive.")]
    public float knockbackForceX;
    [Tooltip("Force in Y to apply for the knockback whenhit, positive.")]
    public float knockbackForceY;
    public CharacterMovement characterMovement;
    public AimingDirectionResolver aimingDirectionResolver;
    private bool renderingEnabled;
    private float elapsedKnockbackTime;
    private float elapsedInvulnerableTime;
    private float elapsedFickerTime;

    public override bool OnDamage(GameObject origin, float damage)
    {
        // Let the state machine handle damage
        return mainStateMachine.OnDamage(origin, damage);
    }

    protected override void OnDeath()
    {
        // Don't handle death directly in here
    }

    public bool ApplyDamage(GameObject origin, float damage)
    {
        return base.OnDamage(origin, damage);
    }

    public void BeginKnockback(GameObject origin)
    {
        // Use the position of the hazard to see if the attack came from the left or the right
        // to determine the direction of the knockback
        float knockBackDirection = -aimingDirectionResolver.FacingDirection;
        if (origin != null)
        {
            bool attackFromTheRight = origin.transform.position.x >= transform.position.x;
            float knockbackXDirection = attackFromTheRight ? -1 : 1;
        }
        characterMovement.SetSpeed(knockbackForceX * knockBackDirection, knockbackForceY);
        elapsedKnockbackTime = 0;
    }

    public bool UpdateKnockback()
    {
        elapsedKnockbackTime += Time.deltaTime;
        return elapsedKnockbackTime >= timeStunned;
    }

    public void BeginInvulnerable()
    {
        elapsedInvulnerableTime = 0;
        elapsedFickerTime = 0;
        renderingEnabled = false;
        if (flickerEnabled)
        {
            EnableRendering(renderingEnabled);
        }
    }

    public bool UpdateInvulnerable()
    {
        if (flickerEnabled)
        {
            UpdateFlicker();
        }
        elapsedInvulnerableTime += Time.deltaTime;
        if (elapsedInvulnerableTime >= timeImmune)
        {
            EnableRendering(true);
            return true;
        }
        return false;
    }

    private void UpdateFlicker()
    {
        elapsedFickerTime += Time.deltaTime;
        if (elapsedFickerTime >= timePerFlick)
        {
            elapsedFickerTime = 0;
            renderingEnabled = !renderingEnabled;
            EnableRendering(renderingEnabled);
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
