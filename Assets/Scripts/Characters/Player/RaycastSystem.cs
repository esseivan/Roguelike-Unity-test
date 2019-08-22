using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastSystem : MonoBehaviour
{
    public RaycastHit lastHit = new RaycastHit();

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Exclude NonMinimap layer
        int layermask = (1 << LayerMask.NameToLayer("NonMinimap")) | (1 << LayerMask.NameToLayer("IgnoreRaycast"));
        layermask = ~layermask;

        if (Physics.Raycast(ray, out RaycastHit hit, 1000, layermask))
        {
            lastHit = hit;
            Debug.DrawLine(ray.origin, hit.point, Color.red);   
        }
        else
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
    }
}
