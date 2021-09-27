using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraFader : MonoBehaviour
{
    public PostProcessVolume postprocess;
    private PostProcessProfile profile;
    private GBCameraSettings gbc;
    
    private static CameraFader _instance;
    public static CameraFader Instance {
        get { return _instance; }
    }
    void Awake()
    {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }
        profile = postprocess.profile;
        profile.TryGetSettings(out gbc);
    }

    public IEnumerator FadeCoroutine(float target, float time) {
        float startValue = gbc._Fade.value;

        float elapsedTime = 0;

        while (elapsedTime < time) {
            elapsedTime += Time.deltaTime;
            gbc._Fade.value = Mathf.Lerp(startValue, target, elapsedTime/time);
            yield return null;
        }
    }
}
