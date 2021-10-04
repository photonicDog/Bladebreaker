using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Collider2D CameraBounds;
    private int BoundsLayer;

    private void Awake()
    {
        BoundsLayer = LayerMask.GetMask("PlayerZone");
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            int currentIndex = GameManager.Instance.checkpoints.FindIndex(
                a => a == this
            );
            if (currentIndex > GameManager.Instance.checkpointMarker)
                GameManager.Instance.checkpointMarker = currentIndex;
        }
    }

    private void Update()
    {
        if (CameraBounds == null)
        {
            Physics2D.queriesStartInColliders = true;
            RaycastHit2D boundsCheck = Physics2D.Raycast(transform.position, Vector2.right, 2f, BoundsLayer);
            if (boundsCheck.collider.gameObject.CompareTag("CameraBounds"))
            {
                CameraBounds = boundsCheck.collider.gameObject.GetComponent<BoxCollider2D>();
                Physics2D.queriesStartInColliders = false;
            }
            Physics2D.queriesStartInColliders = false;
        }
    }

    public void TeleportToCheckpoint() {
        GameManager.Instance.player.transform.position = transform.position;
        Camera.main.GetComponent<PlayerCamera>().CurrentArea = CameraBounds;
        GameManager.Instance.player.GetComponent<EntityMovement>().velocity = Vector2.zero;
    }
}
