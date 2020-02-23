using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrackerUI : MonoBehaviour
{
  public GameObject row1;
  public GameObject row2;
  public GameObject row3;

  private GameObject[] goalRowArray;
  private GameManager gameManager;
  private GoalManager goalManager;

  private Goal currentGoal;

  // Start is called before the first frame update
  void Start()
  {
    goalRowArray = new GameObject[] { row1, row2, row3 };
    gameManager = FindObjectOfType<GameManager>();
    goalManager = FindObjectOfType<GoalManager>();

    var currentLevel = gameManager.CurrentLevel;
    currentGoal = goalManager.currentGoal;

    InitGoalRows(currentGoal);
    goalManager.GoalProgressEvent.AddListener(HandleGoalProgressEvent);
  }

  private void HandleGoalProgressEvent()
  {
    for (int i = 0; i < currentGoal.goalTags.Length; i++)
    {
      var row = goalRowArray[i];
      var required = currentGoal.goalQuantitiesRequired[i];
      var current = currentGoal.goalQuantitiesCurrent[i];
      row.transform.Find("GoalTrackText").GetComponent<TMPro.TextMeshProUGUI>().text = current + "/" + required;
    }
  }

  private void InitGoalRows(Goal goal)
  {
    for (int i = 0; i < goal.goalTags.Length; i++)
    {
      var row = goalRowArray[i];
      var colorIndex = int.Parse(goal.goalTags[i].Substring(1)) - 1;
      var color = gameManager.colors[colorIndex];
      row.SetActive(true);
      row.transform.Find("GoalSprite").GetComponent<SpriteRenderer>().color = color;
      var required = currentGoal.goalQuantitiesRequired[i];
      var current = currentGoal.goalQuantitiesCurrent[i];
      row.transform.Find("GoalTrackText").GetComponent<TMPro.TextMeshProUGUI>().text = current + "/" + required;
    }
  }

  // Update is called once per frame
  void Update()
  {

  }
}
