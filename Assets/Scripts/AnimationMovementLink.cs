using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMovementLink : MonoBehaviour
{
    //TODO: add facing check to the horizontal push method so it works left and right correctly
    public EntityMovement targetScript;

    public void PushEntityHorizontal(float impulse) {
        targetScript.PushEntity(new Vector2(targetScript._facing * impulse, 0));
    }

    public void PushEntityVertical(float impulse) {
        targetScript.PushEntity(new Vector2(0, impulse));
    }
}
