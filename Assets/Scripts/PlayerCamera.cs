using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
    private Vector3 pos;
    public Transform player;
    private void LateUpdate() {
        pos = player.position + Vector3.back;
        transform.position = pos;
    }
}
