using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Move a character according to keyboard input. Character Controller component required
/// </summary>
[RequireComponent(typeof(CharacterController), typeof(RaycastSystem))]
public class PlayerInputSystem : MonoBehaviour
{
    /// <summary>
    /// Speed mofidier
    /// </summary>
    [MinValue(0)]
    public float speedMod = 1;

    /// <summary>
    /// Constant speed modifier
    /// </summary>
    public float ConstantSpeedMod { get; private set; } = 350;

    /// <summary>
    /// Whether sprint is enabled
    /// </summary>
    public bool canSprint = true;
    /// <summary>
    /// Sprint speed modifier
    /// </summary>
    [ShowIf("canSprint")]
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
    public float jumpSpeed = 8.0f;

    /// <summary>
    /// Whether right and left rotate or move
    /// </summary>
    public bool rotateOnRightLeft = false;
    /// <summary>
    /// Constant rotate speed modifier
    /// </summary>
    [ShowIf("rotateOnRightLeft")]
    public float constantRotateMod = 250;

    public bool canOpenMap = false;
    [ShowIf("canOpenMap"), Required]
    public RectTransform sublevelGridmap = null;
    [ShowIf("canOpenMap"), Required]
    public RectTransform sublevelGridmapBorder = null;

    private FullscreenSettings savedGridmap, savedBorder;
    private FullscreenSettings fullscreenGridmap = new FullscreenSettings()
    {
        anchorMin = new Vector2(0, 0),
        anchorMax = new Vector2(1, 1),
        offsetMax = new Vector2(0, 0),
        offsetMin = new Vector2(0, 0),
    };

    private class FullscreenSettings
    {
        public Vector2 anchorMin, anchorMax;
        public Vector2 offsetMin, offsetMax;

        public FullscreenSettings() { }

        public FullscreenSettings(RectTransform rectTransform)
        {
            anchorMin = rectTransform.anchorMin;
            anchorMax = rectTransform.anchorMax;
            offsetMin = rectTransform.offsetMin;
            offsetMax = rectTransform.offsetMax;
        }

        public void SetSettings(RectTransform rectTransform)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;
        }
    }

    /// <summary>
    /// The Character controller
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

    private BaseWeapon weapon = null;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        player = GetComponent<Transform>();
        weaponSystem = GetComponent<WeaponSystem>();
        weapon = new Weapon_AssaultRifle().CreateTarget();
        weaponSystem.EquipWeapon(weapon);
        raycastSystem = GetComponent<RaycastSystem>();
    }

    void Update()
    {
        /*** Movements ***/
        // Get axis input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        // Get delta time
        float delta = Time.deltaTime;
        if (delta >= 0.05f)
            delta = 0.05f;

        // Get forward vector
        Vector3 move = player.forward * v;
        // Move if rotate disabled
        if (!rotateOnRightLeft)
            move += player.right * h;
        // Rotate if enabled
        else
            player.Rotate(player.up, Input.GetAxis("Horizontal") * constantRotateMod * delta);

        // If magnitude greater than 1, normalize to be 1
        if (move.magnitude > 1)
            move = move.normalized;

        /*** Sprint ***/
        if (canSprint && Input.GetButton("Sprint"))
        {
            // Apply sprint speed modifier
            move *= sprintSpeedMod;
        }
        /*** Jump ***/
        if (Input.GetButton("Jump"))
        {
            move.y = jumpSpeed;
        }

        // Apply speed mod and deltaTime
        move = move * speedMod * ConstantSpeedMod * delta;

        // Do simple move (with gravity)
        controller.SimpleMove(move);

        /*** Map ***/
        if (Input.GetButtonDown("OpenMap"))
        {
            savedGridmap = new FullscreenSettings(sublevelGridmap);

            sublevelGridmap.anchorMin = new Vector2(0, 0);
            sublevelGridmap.anchorMax = new Vector2(1, 1);
            sublevelGridmap.offsetMin = new Vector2(0, 0);
            sublevelGridmap.offsetMax = new Vector2(0, 0);
        }
        if (Input.GetButtonUp("OpenMap"))
        {
            sublevelGridmap.anchorMin = new Vector2(1, 0);
            sublevelGridmap.anchorMax = new Vector2(1, 0);
        }

        /*** Zoom ***/
        // If zoom enabled
        if (canZoom)
        {
            // Get scrollWheel input
            float zoomMod = Input.GetAxis("Mouse ScrollWheel");

            bool state = false;
            if (quickZoomOnSprintKey)
                state = Input.GetButton("Sprint");

            // Zoom the camera
            if (zoomMod < 0)
                cam.Zoom(state);
            else if (zoomMod > 0)
                cam.UnZoom(state);
        }

        /*** Shoot ***/
        // Left click
        if (Input.GetMouseButton(0))
        {
            if (weaponSystem.CanShoot())
                weaponSystem.Shoot(raycastSystem.lastHit.point);
        }
    }
}
