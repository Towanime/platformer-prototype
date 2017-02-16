using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageableEntityWithKnockback : DamageableEntity {

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
    public Text lblTemp;
    private bool renderingEnabled;
    private bool immune;
    private bool stunned;
    private float elapsedTime;
    private float elapsedFickerTime;
    // spawn point in case of death
    private PlayerCheckpoint checkpoint;

    public override bool OnDamage(GameObject origin, float damage)
    {
        bool damaged = false;
        float damageToApply = 1; // Make it 1 always because 1 unit = 1 heart
        // Only apply damage if it's not immune or stunned
        if (!immune && !stunned)
        {
            damaged = base.OnDamage(origin, damageToApply);
            // If damage was applied then apply knockback and start stunned state
            if (damaged)
            {
                stunned = true;
                ApplyKnockback(origin);
                elapsedTime = 0;
            }
        }
        return damaged;
    }

    protected override void OnDeath()
    {
        // spawn to last checkpoint if life <= 0
        if (currentLife <= 0)
        {
            GetComponentInChildren<PlayerCheckpoint>().Spawn();
        }
    }

    private void ApplyKnockback(GameObject origin)
    {
        // Use the position of the hazard to see if the attack came from the left or the right
        // to determine the direction of the knockback
        bool attackFromTheRight = origin.transform.position.x >= transform.position.x;
        float knockbackXDirection = attackFromTheRight ? -1 : 1;
        characterMovement.SetSpeed(knockbackForceX * knockbackXDirection, knockbackForceY);
    }

	void Update () {
        if (stunned)
        {
            UpdateStunned();
        } else if (immune)
        {
            if (flickerEnabled)
            {
                UpdateFlicker();
            }
            UpdateImmune();
        }
    }

    void LateUpdate()
    {
        UpdateLabel();
    }

    private void UpdateStunned()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= timeStunned)
        {
            immune = true;
            stunned = false;
            elapsedTime = 0;
            renderingEnabled = false;
            elapsedFickerTime = 0;
            if (flickerEnabled)
            {
                EnableRendering(renderingEnabled);
            }
        }
    }

    private void UpdateImmune()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= timeImmune)
        {
            EnableRendering(true);
            immune = false;
        }
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

    private void UpdateLabel()
    {
        lblTemp.text = "Life: " + currentLife + " / " + life;
    }

}
