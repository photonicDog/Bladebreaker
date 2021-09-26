using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            GameManager.Instance.checkpointMarker = 
                GameManager.Instance.checkpoints.FindIndex(
                a => a == this
                );
        }
    }

    public void TeleportToCheckpoint() {
        GameManager.Instance.player.transform.position = transform.position;
    }
}
