using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Events;

public class GoalManager : MonoBehaviour
{

  public GoalAchievedEvent GoalAchievedEvent;
  public GoalProgressEvent GoalProgressEvent;
  private GameManager gameManager;

  private static Dictionary<int, Goal> AllLevelGoals = new Dictionary<int, Goal>() {
    {1, new Goal(("T1", 5), ("T2", 5))},
    {2, new Goal(("T2", 10), ("T5", 10))},
    {3, new Goal(("T3", 15), ("T2", 15))},
    {4, new Goal(("T1", 20), ("T2", 20))},
    {5, new Goal(("T3", 10), ("T2", 10), ("T1", 10))},
    {6, new Goal(("T2", 15), ("T4", 15), ("T1", 15))},
    {7, new Goal(("T3", 20), ("T2", 20), ("T1", 20))},
    {8, new Goal(("T4", 25), ("T2", 25), ("T5", 25))},
    {9, new Goal(("T5", 30), ("T4", 30), ("T1", 30))},
    {10, new Goal(("T1", 30), ("T2", 30), ("T1", 30))},
  };

  public Goal currentGoal;

  private void Awake()
  {
    gameManager = FindObjectOfType<GameManager>();
    currentGoal = AllLevelGoals[gameManager.CurrentLevel];
  }

  private void Update()
  {

  }

  public void HandleTileDestroyedEvent(GameObject go)
  {
    var tag = go.tag;
    if (currentGoal.IsGoal(tag))
    {
      gameManager.CurrentScore += 1;
      currentGoal.IncrementGoal(tag);
      GoalProgressEvent.Invoke();
      if (currentGoal.AllGoalsAchieved())
      {
        gameManager.CurrentScore += 20;
        GoalAchievedEvent.Invoke();
      }
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
    for (var i = 0; i < subGoals.Length; i++)
    {
      goalTags[i] = subGoals[i].tag;
      goalQuantitiesRequired[i] = subGoals[i].required;
      goalQuantitiesCurrent[i] = 0;
    }
  }

  public bool IsGoal(string tag)
  {
    return goalTags.Contains(tag);
  }

  public void IncrementGoal(string tag)
  {
    if(AllGoalsAchieved()) {
      return;
    }
    var index = Array.IndexOf(goalTags, tag);
    goalQuantitiesCurrent[index]++;
  }

  public bool AllGoalsAchieved()
  {
    bool allGoalsAchieved = true;
    for (int i = 0; i < goalTags.Length; i++)
    {
      if (goalQuantitiesRequired[i] > goalQuantitiesCurrent[i])
      {
        allGoalsAchieved = false;
        break;
      }
    }
    return allGoalsAchieved;
  }
}

[System.Serializable]
public class GoalAchievedEvent : UnityEvent
{

}

[System.Serializable]
public class GoalProgressEvent : UnityEvent
{

}