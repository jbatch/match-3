using UnityEngine;
using UnityEngine.UI;

public static class GlobalState
{
  [Header("Global State")]
    public static int CurrentLevel = 1;
    public static int CurrentScore = 0;
    public static int LevelsUnlocked = 1;

    public static Color[] colors = new Color[] {
      new Color(0.776f, 0.867f, 0.322f),
      new Color(0.984f, 0.722f, 0.098f),
      new Color(0.875f, 0.239f, 0.369f),
      new Color(0.427f, 0.290f, 0.463f),
      new Color(0.212f, 0.592f, 0.557f)
    };

    
    public static void incrementLevelsUnlocked() {
        Debug.Log("Unlocked was " + LevelsUnlocked);
        LevelsUnlocked += 1;
        Debug.Log("Unlocked is now " + LevelsUnlocked);
    }
}