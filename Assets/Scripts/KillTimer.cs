using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTimer : MonoBehaviour {
    public int framesActive = 5;
    // Start is called before the first frame update
    void Start() {
        StartCoroutine(DieInFrames());
    }

    IEnumerator DieInFrames() {
        int i = 0;
        while (i < framesActive) {
            i++;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
