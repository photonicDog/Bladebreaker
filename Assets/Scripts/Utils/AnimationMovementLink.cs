using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMovementLink : MonoBehaviour
{
    public EntityMovement targetScript;

    public void PushEntityHorizontal(float impulse) {
        targetScript.PushEntity(new Vector2(targetScript._facing * impulse, 0));
    }

    public void PushEntityVertical(float impulse) {
        targetScript.PushEntity(new Vector2(0, impulse));
    }

    public void Antigravity(int on) {
        if (on==1) targetScript.Antigravity();
        else targetScript.StopAntigravity();
    }

    public void Stop() {
        targetScript.Stop();
    }
}
