using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage the use of inputs
/// </summary>
[RequireComponent(typeof(PlayerInputDecoder), typeof(RaycastSystem), typeof(CharacterController))]
public class PlayerInputSystem : MonoBehaviour
{
    #region Inspector

    /// <summary>
    /// Speed mofidier
    /// </summary>
    [MinValue(0)]
    public float speedMod = 1;

    /// <summary>
    /// Constant speed modifier
    /// </summary>
    public const float SPEED_MOD_CONST = 350;

    /// <summary>
    /// Whether sprint is enabled
    /// </summary>
    public bool canSprint = true;
    /// <summary>
    /// Sprint speed modifier
    /// </summary>
    [ShowIf("canSprint")]
    [MinValue(1)]
    public float sprintSpeedMod = 2;

    /// <summary>
    /// Whether zoom is enabled
    /// </summary>
    public bool canZoom = true;
    /// <summary>
    /// Whether quick zoom is enabled (with "Sprint" button mapped)
    /// </summary>
    [ShowIf("canZoom")]
    public bool quickZoomOnSprintKey = false;
    /// <summary>
    /// The camera to zoom
    /// </summary>
    [ShowIf("canZoom")]
    public CameraFollow cam = null;

    /// <summary>
    /// Whether jump is enabled
    /// </summary>
    public bool canJump = true;
    /// <summary>
    /// The jump factor
    /// </summary>
    [ShowIf("canJump")]
    [MinValue(0)]
    public float jumpSpeed = 8.0f;

    /// <summary>
    /// Whether the map can be open
    /// </summary>
    public bool canOpenMap = false;
    [ShowIf("canOpenMap"), Required]
    public MapSystem mapSystem = null;

    #endregion

    /// <summary>
    /// Input decoder
    /// </summary>
    private PlayerInputDecoder playerInputDecoder = null;
    /// <summary>
    /// Character controller
    /// </summary>
    private CharacterController controller = null;
    /// <summary>
    /// The player
    /// </summary>
    private Transform player = null;
    /// <summary>
    /// Weapon system
    /// </summary>
    private WeaponSystem weaponSystem = null;
    /// <summary>
    /// Raycast system
    /// </summary>
    private RaycastSystem raycastSystem = null;

    // Start is called before the first frame update
    void Start()
    {
        playerInputDecoder = GetComponent<PlayerInputDecoder>();
        playerInputDecoder.AutoUpdate = false;
        player = GetComponent<Transform>();
        controller = GetComponent<CharacterController>();
        weaponSystem = GetComponent<WeaponSystem>();
        raycastSystem = GetComponent<RaycastSystem>();
    }

    void Update()
    {
        // Get input data
        InputData inputData = playerInputDecoder.GetData();

        // Set maximum delta time to prevent very quick movement when freezes happens
        if (inputData.DeltaTime > 0.05f)
            inputData.DeltaTime = 0.05f;

        // Get movement vector
        Vector3 move = player.forward * inputData.Axis.y + player.right * inputData.Axis.x;
        // Normalize if greater than 1
        if (move.magnitude > 1)
            move.Normalize();

        // Apply speed mod
        move *= speedMod * SPEED_MOD_CONST * inputData.DeltaTime;

        // Apply sprint
        if (inputData.Sprint && canSprint)
            move *= sprintSpeedMod;

        // Apply jump
        if (inputData.Jump && canJump)
            move.y = jumpSpeed;

        // Toggle map
        if (inputData.Map && canOpenMap)
            mapSystem.ToggleMap();

        // Zoom
        if (canZoom && inputData.Zoom != 0)
        {
            bool quickZoom = inputData.Sprint && quickZoomOnSprintKey;
            if (inputData.Zoom > 0)
                cam.Zoom(quickZoom);
            else
                cam.UnZoom(quickZoom);
        }

        // Do simple move (with gravity)
        controller.SimpleMove(move);

        // Shoot with left click
        if (inputData.LeftClick)
            weaponSystem.Shoot(raycastSystem.lastHit.point);
    }
}
