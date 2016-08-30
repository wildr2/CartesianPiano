using UnityEngine;
using System.Collections;

public class GazeCaster : MonoBehaviour
{
    public Transform reticle;
    private GazeObject gazed;
    public float reticle_offset = 0.1f;
    public float default_distance = 10f;


    private void Update()
    {
        Vector3 dir = transform.forward;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, 100);
        GazeObject g = hits.Length > 0 ? hits[0].collider.GetComponent<GazeObject>() : null;

        // Update currently gazed and events
        if (g != gazed)
        {
            // Exit old gaze object
            if (gazed != null) gazed.OnGazeExit();

            // Enter new gaze object
            if (g != null) g.OnGazeEnter();

            // Gazed could now be null or a new object
            gazed = g;
        }
        else
        {
            // Stay in same gaze object (or no gaze object)
            if (g != null) g.OnGazeStay();
        }


        // Reticle
        if (reticle != null)
        {
            if (g == null)
            {
                //reticle.gameObject.SetActive(false);
                reticle.position = transform.position + dir * default_distance;
            }
            else
            {
                //reticle.gameObject.SetActive(true);
                reticle.position = hits[0].point - dir * reticle_offset;
            }

            reticle.rotation = Quaternion.LookRotation(dir);
        }
    }
}
