using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightRoom : MonoBehaviour {
    public SpawnerChoreographer choreographer;
    public GameObject wallParent;

    private double startTime;
    private double endTime;
    private bool activated;

    [HideInInspector] public double completionSpeed;
    public double optimalCompletionSpeed;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !activated) {
            Debug.Log("Fightroom active");
            activated = true;
            Begin();
        }
    }

    void Begin() {
        //Lock camera
        //Fight room fx
        startTime = Time.realtimeSinceStartupAsDouble;
        Walls(true);
        choreographer.BeginChoreography();
    }

    void Walls(bool enable) {
        wallParent.SetActive(enable);
    }

    public void End() {
        Walls(false);
        endTime = Time.realtimeSinceStartupAsDouble;
        choreographer.EndChoreography();

        completionSpeed = endTime - startTime;
        //TODO: Send to score based on optimalCompletionSpeed
    }
}
