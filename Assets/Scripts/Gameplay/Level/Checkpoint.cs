using BladeBreaker.Gameplay.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BladeBreaker.Gameplay.Level
{
    public class Checkpoint : MonoBehaviour
    {
        public Collider2D CameraBounds;
        private int BoundsLayer;

        private void Awake()
        {
            BoundsLayer = LayerMask.GetMask("PlayerZone");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                int currentIndex = LevelManager.Instance.checkpoints.FindIndex(
                    a => a == this
                );
                if (currentIndex > LevelManager.Instance.checkpointMarker)
                    LevelManager.Instance.checkpointMarker = currentIndex;
            }
        }

        private void Update()
        {
            if (CameraBounds == null)
            {
                Physics2D.queriesStartInColliders = true;
                RaycastHit2D boundsCheck = Physics2D.Raycast(transform.position, Vector2.right, 2f, BoundsLayer);
                if (boundsCheck && boundsCheck.collider.gameObject.CompareTag("CameraBounds"))
                {
                    CameraBounds = boundsCheck.collider.gameObject.GetComponent<BoxCollider2D>();
                    Physics2D.queriesStartInColliders = false;
                }
                else
                    Physics2D.queriesStartInColliders = false;
            }
        }

        public void TeleportToCheckpoint()
        {
            PlayerStats.Instance.transform.position = transform.position;
            Camera.main.GetComponent<PlayerCamera>().CurrentArea = CameraBounds;
            PlayerStats.Instance.GetComponent<EntityMovement>().velocity = Vector2.zero;
        }
    }
}

