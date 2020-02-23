using UnityEngine;
using UnityEngine.UI;

public static class GlobalState
{
  [Header("Global State")]
    public static int CurrentLevel = 1;
    public static int CurrentScore = 0;
    public static int LevelsUnlocked = 1;

    public static Color[] colors;

    
    public static void incrementLevelsUnlocked() {
        Debug.Log("Unlocked was " + LevelsUnlocked);
        LevelsUnlocked += 1;
        Debug.Log("Unlocked is now " + LevelsUnlocked);
    }
}