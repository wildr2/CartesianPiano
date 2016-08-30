using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class LowerPanel : MonoBehaviour
{
    public Instrument instrument;
    public SoundKeyIcon key_icon_prefab;
    public GridLayoutGroup left_grid, right_grid;

    
    private void Start()
    {
        CreateKeyIcons();
    }
    private void CreateKeyIcons()
    {
        foreach (Hand hand in System.Enum.GetValues(typeof(Hand)))
        {
            GridLayoutGroup parent = hand == Hand.Left ? left_grid : right_grid;

            foreach (Instrument.SoundKey key in instrument.GetKeys(hand))
            {
                SoundKeyIcon icon = Instantiate(key_icon_prefab);
                icon.transform.SetParent(parent.transform, false);
                icon.Initialize(key);
            }
        }
        
    }
    

}
