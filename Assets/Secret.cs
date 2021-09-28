using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Controllers;
using UnityEngine;

public class Secret : MonoBehaviour {
    public int index;
    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) return;
        GameManager.Instance.player.GetComponent<PlayerStatsController>().Secrets[index] = 1;
        gameObject.SetActive(false);
    }
}
