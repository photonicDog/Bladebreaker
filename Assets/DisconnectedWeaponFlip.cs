using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectedWeaponFlip : MonoBehaviour {
    public EntityMovement _em;
    // Update is called once per frame
    void Update() {
        transform.localScale = new Vector3(_em._facing, 1, 1);
    }
}
