using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartScreenManager : MonoBehaviour
{
  public GameObject startPanel;
  public GameObject optionsPanel;

  // Start is called before the first frame update
  void Start()
  {
            Debug.Log("Color" + Color.black);
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void CloseGame()
  {
#if UNITY_EDITOR
    // Application.Quit() does not work in the editor so
    // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
    UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
  }

  public void OptionsMenuBackButtonPressed()
  {
    optionsPanel.SetActive(false);
    startPanel.SetActive(true);
  }

  public void MainScreenOptionsButtonPressed()
  {
    optionsPanel.SetActive(true);
    startPanel.SetActive(false);
  }

  public void startGame() {
    SceneManager.LoadScene("LevelSelectScene");
  }
}
