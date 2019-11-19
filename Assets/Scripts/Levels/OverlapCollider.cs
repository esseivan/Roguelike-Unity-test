using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// Detection of a player or ennemy fall
/// </summary>
[RequireComponent(typeof(Collider))]
public class OverlapCollider : MonoBehaviour
{

    [SerializeField]
    public OnTriggerEvent OnEnter = new OnTriggerEvent();

    [SerializeField]
    public OnTriggerEvent OnExit = new OnTriggerEvent();

    private void OnTriggerEnter(Collider collider)
    {
        OnEnter.Invoke(collider);
    }

    private void OnTriggerExit(Collider collider)
    {
        OnExit.Invoke(collider);
    }
}

[Serializable]
public class OnTriggerEvent : UnityEvent<Collider> { }
