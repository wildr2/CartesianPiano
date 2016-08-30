using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SoundKeyIcon : MonoBehaviour
{
    public Color default_color, held_color;

    private Image image;
    private Instrument.SoundKey soundkey;



    public void Initialize(Instrument.SoundKey soundkey)
    {
        this.soundkey = soundkey;

        // Events
        soundkey.on_down += OnKeyDown;

        // Text
        GetComponentInChildren<Text>().text = soundkey.BaseNote;

        // Image
        image = GetComponentInChildren<Image>();
        image.color = default_color;
    }

    private void OnKeyDown()
    {
        image.color = held_color;
    }
    private void Update()
    {
        image.color = Color.Lerp(default_color, held_color, soundkey.GetVolume());
    }
}
