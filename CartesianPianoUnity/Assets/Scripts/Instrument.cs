using UnityEngine;
using System;  // Needed for Math
using System.Collections.Generic;

public enum Hand { Left, Right }

public class Instrument : MonoBehaviour
{
    // Sound Parameters
    public float gain = 0.05f;
    public float offset;
    public AnimationCurve curve;
    public float curve_scale = 1;
    public int samplerate = 44100;

    // frequencies of notes
    private Dictionary<string, float> notes;


    // Playback
    public AudioSource source_prefab;
    private Dictionary<string, AudioClip> note_clips = new Dictionary<string, AudioClip>();


    // Control
    private int num_octaves = 7;
    public int left_octave = 0, right_octave = 1;
    private bool sharpen_next = false;

    private List<SoundKey> left_keys;
    private List<SoundKey> right_keys;

    public OctaveBlock block_prefab;



    // PUBLIC ACCESSORS

    public AudioClip GetNoteClip(string note, int octave, bool sharpen=false)
    {
        if (octave < 0 || octave >= num_octaves) return null;

        if (sharpen && notes.ContainsKey(note + '#')) note += '#';

        note += octave;

        return note_clips[note];
    }
    public int GetNumOctaves()
    {
        return num_octaves;
    }
    public List<SoundKey> GetKeys(Hand hand)
    {
        return hand == Hand.Left ? left_keys : right_keys;
    }



    // PUBLIC HELPERS

    public int ClampOctave(int octave)
    {
        return Mathf.Clamp(octave, 0, num_octaves-1);
    }


    // PRIVATE MODIFIERS

    private void Awake()
    {
        DefineNoteFrequencies();
        CreateNoteClips();
        CreateSoundKeys2();
        CreateOctaveBlocks();
    }
    private void Update()
    {
        bool pedal = Input.GetKey(KeyCode.Space);

        foreach (SoundKey key in left_keys)
        {
            key.Update(left_octave, pedal);
        }
        foreach (SoundKey key in right_keys)
        {
            key.Update(right_octave, pedal);
        }
    }

    private void SetData(AudioClip clip, float frequency)
    {
        float[] data = new float[clip.samples];

        for (int i = 0; i < data.Length; i = i + clip.channels)
        {
            data[i] = Mathf.Sign(Mathf.Sin(i * frequency * clip.channels * 2f * Mathf.PI / samplerate));
            if (clip.channels == 2) data[i + 1] = data[i];
        }




        // update increment in case frequency has changed
        //double increment = frequency * channels * Math.PI / samplerate;
        //double phase = 0;

        //for (var i = 0; i < data.Length; i = i + 2)
        //{
        //    phase = phase + increment;

        //    // this is where we copy audio data to make them “available” to Unity
        //    data[i] = (float)(gain * Math.Sin(phase));

        //    // if we have stereo, we copy the mono data to each channel
        //    if (channels == 2) data[i + 1] = data[i];
        //    if (phase > 2 * Math.PI) phase = 0;
        //}

        clip.SetData(data, 0);
    }
    private void DefineNoteFrequencies()
    {
        // Set note frequencies for one octave
        int defined_octave = 3;
        notes = new Dictionary<string, float>();
        notes["A"   + defined_octave] = 220.000f;
        notes["A#"  + defined_octave] = 233.082f;
        notes["B"   + defined_octave] = 246.942f;
        notes["C"   + defined_octave] = 261.626f;
        notes["C#"  + defined_octave] = 277.183f;
        notes["D"   + defined_octave] = 293.665f;
        notes["D#"  + defined_octave] = 311.127f;
        notes["E"   + defined_octave] = 329.628f;
        notes["F"   + defined_octave] = 349.228f;
        notes["F#"  + defined_octave] = 369.994f;
        notes["G"   + defined_octave] = 391.995f;
        notes["G#"  + defined_octave] = 415.305f;

        // Calculate note frequencies for other octaves        
        List<KeyValuePair<string, float>> initial_notes = new List<KeyValuePair<string, float>>();
        foreach (KeyValuePair<string, float> note in notes) initial_notes.Add(note);

        for (int octave = 0; octave < num_octaves; ++octave)
        {
            if (octave == defined_octave) continue;
            foreach (KeyValuePair<string, float> note in initial_notes)
            {
                string note_name = note.Key.Substring(0, note.Key.Length - 1) + octave;
                notes[note_name] = note.Value * Mathf.Pow(2, octave - defined_octave);
            }
        }
    }
    private void CreateNoteClips()
    {
        // Create audio clips for each note
        foreach (KeyValuePair<string, float> note in notes)
        {
            AudioClip clip = AudioClip.Create(note.Key, samplerate * 2, 1, samplerate, false);
            SetData(clip, note.Value);
            note_clips[note.Key] = clip;
        }
    }
    private void CreateSoundKeys1()
    {
        // A#    C# D#
        // A  B  C  D
        // E  F  G  A
        //    F# G# A#

        // Left keys
        left_keys = new List<SoundKey>();
        left_keys.Add(new SoundKey(this, KeyCode.Q, "A", 0));
        left_keys.Add(new SoundKey(this, KeyCode.Alpha2, "A#", 0));
        left_keys.Add(new SoundKey(this, KeyCode.W, "B", 0));
        left_keys.Add(new SoundKey(this, KeyCode.E, "C", 0));
        left_keys.Add(new SoundKey(this, KeyCode.Alpha4, "C#", 0));
        left_keys.Add(new SoundKey(this, KeyCode.R, "D", 0));
        left_keys.Add(new SoundKey(this, KeyCode.Alpha5, "D#", 0));

        left_keys.Add(new SoundKey(this, KeyCode.A, "E", 0));
        left_keys.Add(new SoundKey(this, KeyCode.S, "F", 0));
        left_keys.Add(new SoundKey(this, KeyCode.X, "F#", 0));
        left_keys.Add(new SoundKey(this, KeyCode.D, "G", 0));
        left_keys.Add(new SoundKey(this, KeyCode.C, "G#", 0));
        left_keys.Add(new SoundKey(this, KeyCode.F, "A", 1));

        // Right keys
        right_keys = new List<SoundKey>();
        right_keys.Add(new SoundKey(this, KeyCode.U, "A", 0));
        right_keys.Add(new SoundKey(this, KeyCode.Alpha8, "A#", 0));
        right_keys.Add(new SoundKey(this, KeyCode.I, "B", 0));
        right_keys.Add(new SoundKey(this, KeyCode.O, "C", 0));
        right_keys.Add(new SoundKey(this, KeyCode.Alpha0, "C#", 0));
        right_keys.Add(new SoundKey(this, KeyCode.P, "D", 0));
        right_keys.Add(new SoundKey(this, KeyCode.Minus, "D#", 0));

        right_keys.Add(new SoundKey(this, KeyCode.J, "E", 0));
        right_keys.Add(new SoundKey(this, KeyCode.K, "F", 0));
        right_keys.Add(new SoundKey(this, KeyCode.Comma, "F#", 0));
        right_keys.Add(new SoundKey(this, KeyCode.L, "G", 0));
        right_keys.Add(new SoundKey(this, KeyCode.Period, "G#", 0));
        right_keys.Add(new SoundKey(this, KeyCode.Semicolon, "A", 1));

    }
    private void CreateSoundKeys2()
    {
        // G# G  A  A#
        // D# D  E  F  F#
        // A# A  B  C  C#

        // Left keys
        left_keys = new List<SoundKey>();
        AddLKey(KeyCode.Z, "A#"); AddLKey(KeyCode.X, "A"); AddLKey(KeyCode.C, "B"); AddLKey(KeyCode.V, "C"); AddLKey(KeyCode.B, "C#");
        AddLKey(KeyCode.A, "D#"); AddLKey(KeyCode.S, "D"); AddLKey(KeyCode.D, "E"); AddLKey(KeyCode.F, "F"); AddLKey(KeyCode.G, "F#");
        AddLKey(KeyCode.Q, "G#"); AddLKey(KeyCode.W, "G"); AddLKey(KeyCode.E, "A", 1); AddLKey(KeyCode.R, "A#", 1);

        // Right keys
        right_keys = new List<SoundKey>();
        AddRKey(KeyCode.N, "A#"); AddRKey(KeyCode.M, "A"); AddRKey(KeyCode.Comma, "B"); AddRKey(KeyCode.Period, "C"); AddRKey(KeyCode.Slash, "C#");
        AddRKey(KeyCode.H, "D#"); AddRKey(KeyCode.J, "D"); AddRKey(KeyCode.K, "E"); AddRKey(KeyCode.L, "F"); AddRKey(KeyCode.Semicolon, "F#");
        AddRKey(KeyCode.Y, "G#"); AddRKey(KeyCode.U, "G"); AddRKey(KeyCode.I, "A", 1); AddRKey(KeyCode.O, "A#", 1);
    }
    private void CreateSoundKeys3()
    {
        // D# E  F  F# G  G#
        // A  A# B  C  C# D

        // Left keys
        left_keys = new List<SoundKey>();
        left_keys.Add(new SoundKey(this, KeyCode.A, "A", 0));
        left_keys.Add(new SoundKey(this, KeyCode.S, "A#", 0));
        left_keys.Add(new SoundKey(this, KeyCode.D, "B", 0));
        left_keys.Add(new SoundKey(this, KeyCode.F, "C", 0));
        left_keys.Add(new SoundKey(this, KeyCode.G, "C#", 0));
        left_keys.Add(new SoundKey(this, KeyCode.H, "D", 0));

        left_keys.Add(new SoundKey(this, KeyCode.Q, "D#", 0));
        left_keys.Add(new SoundKey(this, KeyCode.W, "E", 0));
        left_keys.Add(new SoundKey(this, KeyCode.E, "F", 0));
        left_keys.Add(new SoundKey(this, KeyCode.R, "F#", 0));
        left_keys.Add(new SoundKey(this, KeyCode.T, "G", 0));
        left_keys.Add(new SoundKey(this, KeyCode.Y, "G#", 0));

        // Right keys
        right_keys = new List<SoundKey>();
        right_keys.Add(new SoundKey(this, KeyCode.J, "A", 0));
        right_keys.Add(new SoundKey(this, KeyCode.K, "A#", 0));
        right_keys.Add(new SoundKey(this, KeyCode.L, "B", 0));
        right_keys.Add(new SoundKey(this, KeyCode.Semicolon, "C", 0));
        right_keys.Add(new SoundKey(this, KeyCode.Quote, "C#", 0));
        right_keys.Add(new SoundKey(this, KeyCode.Return, "D", 0));

        right_keys.Add(new SoundKey(this, KeyCode.U, "D#", 0));
        right_keys.Add(new SoundKey(this, KeyCode.I, "E", 0));
        right_keys.Add(new SoundKey(this, KeyCode.O, "F", 0));
        right_keys.Add(new SoundKey(this, KeyCode.P, "F#", 0));
        right_keys.Add(new SoundKey(this, KeyCode.LeftBracket, "G", 0));
        right_keys.Add(new SoundKey(this, KeyCode.RightBracket, "G#", 0));
    }
    private void CreateSoundKeys4()
    {
        // E  F  G  A
        // A  B  C  D

        // Left keys
        left_keys = new List<SoundKey>();

        AddLKey(KeyCode.Alpha2, "F#"); AddLKey(KeyCode.Alpha3, "G#"); AddLKey(KeyCode.Alpha4, "A#", 1);
        AddLKey(KeyCode.Q, "E"); AddLKey(KeyCode.W, "F"); AddLKey(KeyCode.E, "G"); AddLKey(KeyCode.R, "A", 1);
        AddLKey(KeyCode.A, "A"); AddLKey(KeyCode.S, "B"); AddLKey(KeyCode.D, "C"); AddLKey(KeyCode.F, "D");
        AddLKey(KeyCode.Z, "A#"); AddLKey(KeyCode.C, "C#"); AddLKey(KeyCode.V, "D#");


        // Right keys
        right_keys = new List<SoundKey>();

        AddRKey(KeyCode.Alpha8, "F#"); AddRKey(KeyCode.Alpha9, "G#"); AddRKey(KeyCode.Alpha0, "A#", 1);
        AddRKey(KeyCode.U, "E"); AddRKey(KeyCode.I, "F"); AddRKey(KeyCode.O, "G"); AddRKey(KeyCode.P, "A", 1);
        AddRKey(KeyCode.J, "A"); AddRKey(KeyCode.K, "B"); AddRKey(KeyCode.L, "C"); AddRKey(KeyCode.Semicolon, "D");
        AddRKey(KeyCode.M, "A#"); AddRKey(KeyCode.Period, "C#"); AddRKey(KeyCode.Slash, "D#");
    }
    private void CreateOctaveBlocks()
    {
        bool odd = false;

        float width = num_octaves * 1;
        for (int x = 0; x < num_octaves; ++x)
        {
            for (int y = 0; y < num_octaves; ++y)
            {
                OctaveBlock block = Instantiate(block_prefab);
                block.transform.SetParent(transform);
                block.transform.position = new Vector3(x - width / 2f, y - width / 2f, 0);

                if (odd) block.SetTint(new Color(0.9f, 0.9f, 0.9f));

                int lo = y;
                int ro = x;
                block.on_gaze_enter += () => { left_octave = lo; right_octave = ro; };

                odd = !odd;
            }
        }
    }

    private void AddLKey(KeyCode key, string base_note, int octave_shift = 0)
    {
        left_keys.Add(new SoundKey(this, key, base_note, octave_shift));
    }
    private void AddRKey(KeyCode key, string base_note, int octave_shift = 0)
    {
        right_keys.Add(new SoundKey(this, key, base_note, octave_shift));
    }

    public class SoundKey
    {
        private Instrument parent;
        private AudioSource[] sources;
        private AudioSource held_source;
        public Action on_down, on_stay, on_up;

        public KeyCode Key { get; private set; }
        public string BaseNote { get; private set; }
        public int OctaveShift { get; private set; }

        public SoundKey(Instrument parent, KeyCode key, string base_note, int octave_shift)
        {
            this.parent = parent;
            Key = key;
            BaseNote = base_note;
            OctaveShift = octave_shift;

            // Create an audio source for each note this key can play (each octave)
            int n = parent.GetNumOctaves();
            sources = new AudioSource[n];
            for (int octave = 0; octave < n; ++octave)
            {
                AudioSource s = Instantiate(parent.source_prefab);
                s.transform.SetParent(parent.transform);
                s.clip = parent.GetNoteClip(BaseNote, octave);
                s.volume = 0;
                sources[octave] = s;
            }
        }
        public void Update(int octave, bool pedal)
        {
            if (Input.GetKeyDown(Key))
            {
                // Hit
                octave += OctaveShift;
                if (octave >= 0 && octave < parent.GetNumOctaves())
                {
                    held_source = sources[octave];

                    held_source.volume = 1;
                    held_source.Play();

                    if (on_down != null) on_down();
                }
            }
            else if (Input.GetKeyUp(Key))
            {
                // Release
                held_source = null;
                if (on_up != null) on_up();
            }
            else if (held_source != null)
            {
                // Hold
                if (on_stay != null) on_stay();
            }

            // Diminish
            foreach (AudioSource source in sources)
            {
                source.volume *= source == held_source || pedal ? 0.99f : 0.9f;
            }
        }
        public float GetVolume()
        {
            float a = 0;
            foreach (AudioSource source in sources)
            {
                a += source.volume;
            }
            return Mathf.Min(a, 1);
        }
    }
}