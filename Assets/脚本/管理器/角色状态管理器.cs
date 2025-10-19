using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateManager : MonoBehaviour
{
    public static bool is012Sitting = false;

    public static void Set012Sitting(bool value)
    {
        is012Sitting = value;
        PlayerPrefs.SetInt("012_Sitting", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool Get012Sitting()
    {
        return PlayerPrefs.GetInt("012_Sitting", 0) == 1;
    }
}
