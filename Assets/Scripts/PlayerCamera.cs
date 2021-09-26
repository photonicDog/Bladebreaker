using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerCamera : MonoBehaviour {
    private Vector3 newPos;
    public Transform player;
    public Collider2D CurrentArea;
    public float FocusX;
    public float FocusY;

    public bool LockVertical;
    public bool LockHorizontal;
    public bool isSmoothing;

    public float UpperYBound;
    public float LowerYBound;
    public float UpperXBound;
    public float LowerXBound;

    private float _xFromCenter;
    private float _yFromCenter;

    private void Awake()
    {
        _xFromCenter = 10f;
        _yFromCenter = 9f;
        FocusX = 0f;
        FocusY = 0f;
    }

    private void LateUpdate()
    {
        if (!isSmoothing)
        {
            UpperXBound = CurrentArea.bounds.max.x;
            UpperYBound = CurrentArea.bounds.max.y;
            LowerXBound = CurrentArea.bounds.min.x;
            LowerYBound = CurrentArea.bounds.min.y;

            newPos = new Vector3(
                FocusX == 0 ? player.position.x : FocusX,
                FocusY == 0 ? player.position.y : FocusY,
                player.position.z - 1);

            newPos = KeepCameraInStage(newPos);

            if (LockVertical)
            {
                newPos.y = transform.position.y;
            }

            if (LockHorizontal)
            {
                newPos.x = transform.position.x;
            }


            transform.position = newPos;
        }
    }

    public Vector3 KeepCameraInStage(Vector3 pos)
    {
        if (pos.x - _xFromCenter < LowerXBound)
        {
            pos.x = LowerXBound + _xFromCenter;
        }

        if (pos.x + _xFromCenter > UpperXBound)
        {
            pos.x = UpperXBound - _xFromCenter;
        }

        if (pos.y - _yFromCenter < LowerYBound)
        {
            pos.y = LowerYBound + _yFromCenter;
        }

        if (pos.y + _yFromCenter > UpperYBound)
        {
            pos.y = UpperYBound - _yFromCenter;
        }

        return pos;
    }

    public void LockCameraOnTarget(float focusX, float focusY)
    {
        FocusX = focusX;
        FocusY = focusY;
        newPos = new Vector3(
            FocusX == 0 ? transform.position.x : FocusX,
            FocusY == 0 ? transform.position.y : FocusY,
            player.position.z - 1);
        isSmoothing = true;
        StartCoroutine(SmoothLock(newPos, 0.5f));
    }

    public void UnlockCamera()
    {
        FocusX = 0;
        FocusY = 0;
        isSmoothing = true;
        StartCoroutine(SmoothUnlock(0.5f));
    }

    private IEnumerator SmoothLock(Vector3 endPos, float duration)
    {
        Vector3 startPos = transform.position;
        float ellapsedTime = 0f;
        while(ellapsedTime < duration)
        {
            ellapsedTime += Time.deltaTime;
            transform.position = KeepCameraInStage(Vector3.Lerp(startPos, endPos, ellapsedTime/duration));
            yield return null;
        }
        isSmoothing = false;
    }
    private IEnumerator SmoothUnlock(float duration)
    {
        Vector3 startPos = transform.position;
        float ellapsedTime = 0f;
        while (ellapsedTime < duration)
        {
            Vector3 endPos = player.position + Vector3.back;
            ellapsedTime += Time.deltaTime;
            transform.position = KeepCameraInStage(Vector3.Lerp(startPos, endPos, ellapsedTime / duration));
            yield return null;
        }
        isSmoothing = false;
    }
}
