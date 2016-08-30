using UnityEngine;
using System.Collections;

public class CameraShake : ShakingObj
{
    public static Note<CamShakeParams> note_shake = new Note<CamShakeParams>();
    public static Note note_shake_small = new Note();
    public static Note note_shake_med = new Note();
    public static Note note_shake_big = new Note();

       
    public void Shake(CamShakeParams shake_params)
    {
        FreezeFrames(shake_params.freeze_frames);
        Shake(shake_params.shake_params);
    }
    public void ShakeSmall()
    {
        Shake(0.2f, 0.5f, 1);
    }
    public void ShakeMed()
    {
        FreezeFrames(3);
        Shake(0.2f, 2, 0.3f);
    }
    public void ShakeBig()
    {
        FreezeFrames(3);
        Shake(0.3f, 2, 0.8f);
    }
    public void FreezeFrames(int frames)
    {
        StartCoroutine(Freeze(frames));
    }
    protected override void Awake()
    {
        base.Awake();

        NoticeBoard<CamShakeParams>.Watch(note_shake, Shake);
        NoticeBoard.Watch(note_shake_small, ShakeSmall);
        NoticeBoard.Watch(note_shake_med, ShakeMed);
        NoticeBoard.Watch(note_shake_big, ShakeBig);
    }

    private IEnumerator Freeze(float frames = 3)
    {
        TimeScaleManager.Instance.SetFactor(0);
        for (int i = 0; i < frames; ++i)
        {
            yield return null;
        }
        TimeScaleManager.Instance.SetFactor(1);
    }   
}
public class CamShakeParams
{
    public ShakeParams shake_params;
    public int freeze_frames = 0;
    
    public CamShakeParams()
    {

    }
    public CamShakeParams(float duration, float intensity, float speed, int freeze_frames)
    {
        shake_params = new ShakeParams(duration, intensity, speed);
        this.freeze_frames = freeze_frames;
    }
}

