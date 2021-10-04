using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightRoom : MonoBehaviour {
    public SpawnerChoreographer choreographer;
    public GameObject wallParent;
    public Collider2D RoomArea;
    public float FocusX;
    private PlayerCamera _camera;

    private double startTime;
    private double endTime;
    private bool activated;

    [HideInInspector] public double completionSpeed;
    public double optimalCompletionSpeed;

    private void Awake()
    {
        _camera = Camera.main.GetComponent<PlayerCamera>();
        RoomArea = transform.Find("CameraBounds").GetComponent<BoxCollider2D>();
        FocusX = gameObject.GetComponentInChildren<BoxCollider2D>().bounds.center.x;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !activated) {
            Debug.Log("Fightroom active");
            activated = true;
            Begin();
        }
    }

    void Begin() {
        _camera.LockCameraOnTarget(RoomArea);
        //Fight room fx
        startTime = Time.realtimeSinceStartupAsDouble;
        Walls(true);
        choreographer.BeginChoreography();
    }

    void Walls(bool enable) {
        wallParent.SetActive(enable);
    }

    public void Complete() {
        End();
        endTime = Time.realtimeSinceStartupAsDouble;
        completionSpeed = endTime - startTime;
        //TODO: Send to score based on optimalCompletionSpeed
    }

    public void End()
    {
        _camera.UnlockCamera();
        Walls(false);
        choreographer.EndChoreography();
    }

    public void Reset() {
        End();
        choreographer.KillEverything();
        activated = false;
    }
}
