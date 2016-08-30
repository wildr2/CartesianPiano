using UnityEngine;
using System.Collections;

public class OctaveBlock : GazeObject
{
    private MeshRenderer mesh;
    public System.Action on_gaze_enter;

    private Color tint = Color.white;
    public Color gazed_color;


    public override void OnGazeEnter()
    {
        base.OnGazeEnter();

        mesh.material.color = tint * Color.blue;
        if (on_gaze_enter != null) on_gaze_enter();
    }
    public override void OnGazeStay()
    {
        base.OnGazeStay();
    }
    public override void OnGazeExit()
    {
        base.OnGazeExit();

        mesh.material.color = tint * Color.white;
    }

    public void SetTint(Color color)
    {
        tint = color;
        mesh.material.color = tint;
    }

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
    }
}
