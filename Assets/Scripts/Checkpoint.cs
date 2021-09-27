using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            int currentIndex = GameManager.Instance.checkpoints.FindIndex(
                a => a == this
            );
            if (currentIndex > GameManager.Instance.checkpointMarker)
                GameManager.Instance.checkpointMarker = currentIndex;
        }
    }

    public void TeleportToCheckpoint() {
        GameManager.Instance.player.transform.position = transform.position;
    }
}
