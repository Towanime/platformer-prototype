using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : DamageableEntity {

    public float invinsibilityTime;
    public float timePerFlick;
    public bool flickerEnabled;
    public List<Renderer> renderersToFlick;
    private bool rendering;
    private bool invinsible;
    private float elapsedInvinsibilityTime;
    private float elapsedFickerTime;

    public override bool OnDamage(float damage)
    {
        bool damaged = false;
        if (!invinsible)
        {
            damaged = base.OnDamage(damage);
            if (damaged)
            {
                invinsible = true;
                if (flickerEnabled)
                {
                    rendering = false;
                    elapsedFickerTime = 0;
                }
                elapsedInvinsibilityTime = 0;
            }
        }
        return damaged;
    }
	
	// Update is called once per frame
	void Update () {
		if (invinsible)
        {
            if (flickerEnabled)
            {
                foreach (Renderer renderer in renderersToFlick)
                {
                    renderer.enabled = rendering;
                }
                elapsedFickerTime += Time.deltaTime;
                if (elapsedFickerTime >= timePerFlick)
                {
                    elapsedFickerTime = 0;
                    rendering = !rendering;
                }
            }
            elapsedInvinsibilityTime += Time.deltaTime;
            if (elapsedInvinsibilityTime >= invinsibilityTime)
            {
                invinsible = false;
                foreach (Renderer renderer in renderersToFlick)
                {
                    renderer.enabled = true;
                }
            }

        }
	}
}
