using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerCamera : MonoBehaviour {
    private Vector3 pos;
    public Transform player;
    public Collider2D StartingArea;

    public bool LockVertical;
    public bool LockHorizontal;

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
    }

    private void LateUpdate()
    {
        UpperXBound = StartingArea.bounds.max.x;
        UpperYBound = StartingArea.bounds.max.y;
        LowerXBound = StartingArea.bounds.min.x;
        LowerYBound = StartingArea.bounds.min.y;

        pos = player.position + Vector3.back;

        pos = KeepCameraInStage(pos);

        if (LockVertical)
        {
            pos.y = transform.position.y;
        }

        if (LockHorizontal)
        {
            pos.x = transform.position.x;
        }


        transform.position = pos;
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
}
