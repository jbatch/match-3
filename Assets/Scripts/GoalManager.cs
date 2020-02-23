using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Events;

public class GoalManager : MonoBehaviour
{

  public GoalAchievedEvent GoalAchievedEvent;
  private GameManager gameManager;

  private static Dictionary<int, Goal> AllLevelGoals = new Dictionary<int, Goal>() {
    {1, new Goal(("T1", 5), ("T2", 5))}
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

  public void HandleTileDestroyedEvent(GameObject go)
  {
    var tag = go.tag;
    gameManager.CurrentScore += 1;
    if (currentGoal.IsGoal(tag))
    {
      currentGoal.IncrementGoal(tag);
      if (currentGoal.AllGoalsAchieved())
      {
        // I dunno, 10 points for winning
        gameManager.CurrentScore += 10;
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