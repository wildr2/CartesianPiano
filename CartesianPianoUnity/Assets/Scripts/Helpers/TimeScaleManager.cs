using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// A singleton that manages multiple assignments to Time.timescale
/// </summary>
public class TimeScaleManager : MonoBehaviour
{
    private static TimeScaleManager _instance;
    public static TimeScaleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TimeScaleManager>();

                if (_instance == null) Debug.LogError("Missing TimeScaleManager");
                else
                {
                    DontDestroyOnLoad(_instance);
                }
            }
            return _instance;
        }
    }

    public float default_timescale = 1;
    public bool maintain_fixedtimestep_ratio = true;
    public float fixed_timestep = 0.016f;

    private FloatProduct product;
    private UID default_timescale_id;


    // PUBLIC MODIFIERS

    public void SetFactor(float factor, UID id)
    {
        product.SetFactor(factor, id);
        UpdateTimeScale();
    }
    public void SetFactor(float first_factor)
    {
        product.SetFactor(first_factor);
        UpdateTimeScale();
    }
    public float GetFactor(UID id)
    {
        return product.GetFactor(id);
    }


    // PRIVATE MODIFIERS

    private void Awake()
    {
        // if this is the first instance, make this the singleton
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
            Initialize();
        }
        else
        {
            // destroy other instances that are not the already existing singleton
            if (this != _instance)
            {
                // save new inspector parameters
                //_instance.default_timescale = this.default_timescale;
                //_instance.maintain_fixedtimestep_ratio = this.maintain_fixedtimestep_ratio;
                //_instance.fixed_timestep = this.fixed_timestep;

                Destroy(this.gameObject);
            }

        }
    }
    private void Initialize()
    {
        product = new FloatProduct();
        default_timescale_id = new UID();
        SetFactor(default_timescale, default_timescale_id);
    }
    private void OnLevelWasLoaded(int level)
    {
        if (this != _instance) return;

        // reset first factor
        SetFactor(1);
    }
    private void Update()
    {
        if (Time.timeScale != product.Value)
        {
            Debug.LogWarning("Time.timeScale was set by something other than TimeScaleManager");

            // insure that time scale is controlled by this manager
            Time.timeScale = product.Value;
        }
    }
    private void UpdateTimeScale()
    {
        Time.timeScale = product.Value;
        if (maintain_fixedtimestep_ratio)
        {
            Time.fixedDeltaTime = fixed_timestep * Time.timeScale;
        }
    }
}