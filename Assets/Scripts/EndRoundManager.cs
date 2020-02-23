using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EndRoundManager : MonoBehaviour
{
    private GameManager gameManager;
    public TMPro.TextMeshProUGUI roundOverTitle;
    public TMPro.TextMeshProUGUI pointsLabel;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        var curLevel = gameManager.CurrentLevel;
        var curScore = gameManager.CurrentScore;
        roundOverTitle.text = "Level " + curLevel + " Complete!";
        pointsLabel.text = curScore + "pts";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
