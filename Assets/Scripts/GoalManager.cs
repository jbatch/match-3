using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GoalManager : MonoBehaviour
{

  private GameManager gameManager;

  private static Dictionary<int, Goal> AllLevelGoals = new Dictionary<int, Goal> () {
    {1, new Goal(("T1", 10), ("T2", 10))}
  };

  public Goal currentGoal;

  private void Start()
  {
    gameManager = FindObjectOfType<GameManager>();
    currentGoal = AllLevelGoals[gameManager.CurrentLevel];
  }

  private void Update()
  {

  }

  public void HandleTileDestroyedEvent(GameObject go) {
    var tag = go.tag;
    if(currentGoal.IsGoal(tag)) {
      currentGoal.IncrementGoal(tag);
    }
  }
}

[System.Serializable]
public class Goal
{
  public string[] goalTags;
  public int[] goalQuantitiesRequired;
  public int[] goalQuantitiesCurrent;

  public Goal(params (string tag, int required)[] subGoals)
  {
    goalTags = new string[subGoals.Length];
    goalQuantitiesRequired = new int[subGoals.Length];
    goalQuantitiesCurrent = new int[subGoals.Length];
    for(var i = 0; i < subGoals.Length; i++) {
      goalTags[i] = subGoals[i].tag;
      goalQuantitiesRequired[i] = subGoals[i].required;
      goalQuantitiesCurrent[i] = 0;
    }
  }

  public bool IsGoal(string tag) {
    return goalTags.Contains(tag);
  }

  public void IncrementGoal(string tag)
  {
    var index =  Array.IndexOf(goalTags, tag);
    goalQuantitiesCurrent[index]++;
  }
}