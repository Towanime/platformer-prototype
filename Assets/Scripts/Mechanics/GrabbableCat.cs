using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableCat : GrabbableEntity
{
    [Tooltip("Game object to attach to the player arm if the grab is successful.")]
    public GameObject headPrefab;
    [Tooltip("Damagable cat component, used to \"kill\" the cat when is grabbed.")]
    public Cat entity;
    // 
    public Hazard hazardComponent;

    public override GameObject OnGrab()
    {
        AnimatorStateInfo animatorInfo = entity.animator.GetCurrentAnimatorStateInfo(0);
        if (hazardComponent.enabled && (animatorInfo.IsName("Attack") && (animatorInfo.normalizedTime > 0.35f && animatorInfo.normalizedTime < 0.70f)))
        {
            // do damage
            return null;
        }else
        {
            // "kill" the cat and set it as spawn point
            entity.GrabKill();
            GameObject head = Instantiate(headPrefab, gameObject.transform) as GameObject;
            // set spawn point
            head.GetComponent<EnemyDamageableEntity>().spawn = entity;
            return head;
        }
    }
}
