using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardUIManager : MonoBehaviour
{
    public GameObject boardRef;
    public GameObject endRoundCanvas;
    public TMPro.TextMeshProUGUI levelText;
    public TMPro.TextMeshProUGUI scoreText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowEndRoundScreen() {
        var curLevel = GlobalState.CurrentLevel;
        var curScore = GlobalState.CurrentScore;
        levelText.text = "Level " + (curLevel - 1) + " Completed!";
        scoreText.text = curScore + "pts";
        boardRef.SetActive(false);
        endRoundCanvas.SetActive(true);
    }

    public void ContinueClicked() {
        Debug.Log("Continnue called");
        var curLevel = GlobalState.CurrentLevel;
        var levelsUnlocked = GlobalState.LevelsUnlocked;
        if (curLevel + 1 > levelsUnlocked) {
            GlobalState.incrementLevelsUnlocked();
        }

        SceneManager.LoadScene("LevelSelectScene");
    }
}
