using System.Collections.Generic;
using BladeBreaker.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts {
    public class EntityNoises : SerializedMonoBehaviour {
        public List<AudioClip> sounds;

        public void PlaySound(int i) {
            AudioController.Instance.PlayPlayerSFX(sounds[i]);
        }
    }
}