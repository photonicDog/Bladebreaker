using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTrail : MonoBehaviour {
    private ParticleSystem ps;
    private ParticleSystemRenderer psr;
    public EntityMovement em;

    private bool enabled;

    private void Awake() {
        ps = GetComponent<ParticleSystem>();
        psr = GetComponent<ParticleSystemRenderer>();
        ps.Stop();
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

        psr.flip = new Vector3(em._facing == 1 ? 0 : 1, 0);
    }
}
