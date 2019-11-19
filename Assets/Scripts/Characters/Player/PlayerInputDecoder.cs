using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Decode inputs
/// </summary>
public class PlayerInputDecoder : MonoBehaviour
{
    #region Inspector

    /// <summary>
    /// Whether the lastData is automatically updated each Update call
    /// </summary>
    public bool AutoUpdate { get; set; } = false;

    #endregion

    /// <summary>
    /// Last Input data
    /// </summary>
    public InputData LastData { get; private set; }

    /// <summary>
    /// Whether sprint is held down
    /// </summary>
    public bool GetSprint()
    {
        return Input.GetButton("Sprint");
    }

    /// <summary>
    /// Whether Jump is pressed
    /// </summary>
    /// <returns></returns>
    public bool GetJump()
    {
        return Input.GetButtonDown("Jump");
    }

    /// <summary>
    /// Whether OpenMap is pressed
    /// </summary>
    public bool GetMap()
    {
        return Input.GetButtonDown("OpenMap");
    }

    void Update()
    {
        if (!AutoUpdate)
            return;

        LastData = GetData();
    }

    /// <summary>
    /// Retrieve input data
    /// </summary>
    public InputData GetData()
    {
        LastData = new InputData()
        {
            Axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")),
            DeltaTime = Time.deltaTime,
            Jump = GetJump(),
            Sprint = GetSprint(),
            Map = GetMap(),
            Zoom = Input.GetAxis("Mouse ScrollWheel"),
            LeftClick = Input.GetMouseButton(0),
            RightClick = Input.GetMouseButton(1),
        };
        return LastData;
    }
}

/// <summary>
/// Wrapper for input data
/// </summary>
public class InputData
{
    public Vector2 Axis { get; set; }
    public float DeltaTime { get; set; }
    public bool Sprint { get; set; }
    public bool Jump { get; set; }
    public bool Map { get; set; }
    public float Zoom { get; set; }

    public bool LeftClick { get; set; }
    public bool RightClick { get; set; }
}

