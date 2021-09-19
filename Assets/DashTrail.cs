using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTrail : MonoBehaviour {
    private ParticleSystem ps;
    public EntityMovement em;

    private bool enabled;

    private void Awake() {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update() {
        if (em.dash && !enabled) {
            ps.Play();
            enabled = true;
        }

        if (enabled && !em.dash) {
            ps.Stop();
            enabled = false;
        }
    }
}
