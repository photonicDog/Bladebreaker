using BladeBreaker.Gameplay.Player;
using UnityEngine;

namespace BladeBreaker.Gameplay.Level
{
    public class Secret : MonoBehaviour
    {
        public int index;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            PlayerStats.Instance.Secrets[index] = 1;
            gameObject.SetActive(false);
        }
    }
}