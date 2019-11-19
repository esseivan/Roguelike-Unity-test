using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementSystem : MonoBehaviour
{
    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 movement = new Vector3();

        if (Keyboard.current.wKey.isPressed) { movement.z += 1; }
        if (Keyboard.current.sKey.isPressed) { movement.z -= 1; }
        if (Keyboard.current.aKey.isPressed) { movement.x += 1; }
        if (Keyboard.current.dKey.isPressed) { movement.x -= 1; }


    }
}
