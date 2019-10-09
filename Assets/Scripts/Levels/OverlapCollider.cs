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
    [FormerlySerializedAs("OnEnter")]
    [SerializeField]
    public OnTriggerEvent m_OnEnter = new OnTriggerEvent();

    [FormerlySerializedAs("OnExit")]
    [SerializeField]
    public OnTriggerEvent m_OnExit = new OnTriggerEvent();

    private void OnTriggerEnter(Collider collider)
    {
        m_OnEnter.Invoke(collider.gameObject);
    }

    private void OnTriggerExit(Collider collider)
    {
        m_OnExit.Invoke(collider.gameObject);
    }
}

[Serializable]
public class OnTriggerEvent : UnityEvent<GameObject> { }
