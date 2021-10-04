using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerCamera : MonoBehaviour {
    private Vector3 newPos;
    public Transform player;
    public Collider2D CurrentArea;

    public bool isSmoothing;

    public float UpperYBound;
    public float LowerYBound;
    public float UpperXBound;
    public float LowerXBound;

    private float _xFromCenter;
    private float _yFromCenter;

    private bool _inFightRoom;
    private Collider2D _levelArea;
    private Vector3 _intendedPos;

    private void Awake()
    {
        _xFromCenter = 10f;
        _yFromCenter = 9f;
    }

    private void LateUpdate()
    {
        UpperXBound = CurrentArea.bounds.max.x;
        UpperYBound = CurrentArea.bounds.max.y;
        LowerXBound = CurrentArea.bounds.min.x;
        LowerYBound = CurrentArea.bounds.min.y;

        newPos = new Vector3(
            player.position.x,
            player.position.y,
            player.position.z - 1);

        newPos = KeepCameraInStage(newPos);

        _intendedPos = newPos;

        if (!isSmoothing)
        {
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

    public void LockCameraOnTarget(Collider2D fightRoomArea)
    {
        newPos = new Vector3(
            transform.position.x,
            transform.position.y,
            player.position.z - 1);
        isSmoothing = true;
        if (fightRoomArea)
        {
            _levelArea = CurrentArea;
            CurrentArea = fightRoomArea;
            _inFightRoom = true;
        }
        StartCoroutine(SmoothTransition(0.5f));
    }

    public void UnlockCamera()
    {
        isSmoothing = true;
        StartCoroutine(SmoothTransition(0.5f));
        if (_inFightRoom) {
            CurrentArea = _levelArea;
            _inFightRoom = false;
        }
    }

    private IEnumerator SmoothTransition(float duration)
    {
        Vector3 startPos = transform.position;
        float ellapsedTime = 0f;
        while (ellapsedTime < duration)
        {
            Vector3 endPos = KeepCameraInStage(player.position + Vector3.back);
            ellapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, ellapsedTime / duration);
            yield return null;
        }
        isSmoothing = false;
    }
}
