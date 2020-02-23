using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelSelectManager : MonoBehaviour
{
    public Button[] buttons;

    // Start is called before the first frame update
    void Start()
    {
         

    }

    void Awake() {
         var levelsUnlocked = GlobalState.LevelsUnlocked;
         Debug.Log(buttons.LongLength + " " + levelsUnlocked);

         for (int i = 0; i < buttons.Length; i++)
         {
             if (i < levelsUnlocked) {
                 Debug.Log(i + " interactable");
             buttons[i].interactable = true;
             } else {
                 buttons[i].interactable = false;
             }
         }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectLevel(int level) {
        GlobalState.CurrentLevel = level + 1;
        SceneManager.LoadScene("GameScene");
    }

    public void Quit() {
        SceneManager.LoadScene("TitleScene");
    }
}
