using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
  public int height;
  public int width;
  public GameObject[] tilePrefabs;
  private bool freefalling = false;

  public GoalManager goalManager;

  public enum SwapDirection
  {
    UP,
    DOWN,
    LEFT,
    RIGHT
  }

  public enum Pattern
  {
    HLrg,
    VLrg,
    L1,
    L2,
    L3,
    L4,
    T1,
    T2,
    T3,
    T4,
    SQR,
    HMed,
    VMed,
    HSml,
    VSml
  }

  // Values are the Y/X offsets to use to match against each pattern in priority order. Vector2.Y = the X Offset.
  private static Dictionary<Pattern, List<Vector2>> patterns = new Dictionary<Pattern, List<Vector2>>() {
        { Pattern.HLrg, new List<Vector2>{ new Vector2(0,1), new Vector2(0, 2), new Vector2(0, 3), new Vector2(0, 4) } },
        { Pattern.VLrg, new List<Vector2>{ new Vector2(-1,0), new Vector2(-2, 0), new Vector2(-3, 0), new Vector2(-4, 0) } },
        { Pattern.L1, new List<Vector2>{ new Vector2(1,0), new Vector2(2, 0), new Vector2(0, 1), new Vector2(0, 2) } },
        { Pattern.L2, new List<Vector2>{ new Vector2(0,1), new Vector2(0, 2), new Vector2(-1, 0), new Vector2(-2, 0) } },
        { Pattern.L3, new List<Vector2>{ new Vector2(0,-1), new Vector2(0, -2), new Vector2(-1, 0), new Vector2(-2, 0) } },
        { Pattern.L4, new List<Vector2>{ new Vector2(1,0), new Vector2(2, 0), new Vector2(0, -1), new Vector2(0, -2) } },
        { Pattern.T1, new List<Vector2>{ new Vector2(1,0), new Vector2(-1, 0), new Vector2(0, 1) } },
        { Pattern.T2, new List<Vector2>{ new Vector2(1,0), new Vector2(-1, 0), new Vector2(0, -1)} },
        { Pattern.T3, new List<Vector2>{ new Vector2(0,-1), new Vector2(0, 1), new Vector2(-1, 0) } },
        { Pattern.T4, new List<Vector2>{ new Vector2(0,-1), new Vector2(0, 1), new Vector2(1, 0) } },
        // { Pattern.SQR, new List<Vector2>{ new Vector2(0,1), new Vector2(-1, 0), new Vector2(-1, 1) } },
        { Pattern.HMed, new List<Vector2>{ new Vector2(0,1), new Vector2(0, 2), new Vector2(0, 3) } },
        { Pattern.VMed, new List<Vector2>{ new Vector2(-1,0), new Vector2(-2, 0), new Vector2(-3, 0) } },
        { Pattern.HSml, new List<Vector2>{ new Vector2(0,1), new Vector2(0, 2) } },
        { Pattern.VSml, new List<Vector2>{ new Vector2(-1,0), new Vector2(-2, 0) } },
    };
  private Tile[,] tiles;

  // Start is called before the first frame update
  void Start()
  {
    tiles = new Tile[height, width];
    InstantiateBoard();
    // Listen for goal achievment
    this.goalManager.GoalAchievedEvent.AddListener(HandleGoalAchievedEvent);
  }

  private void InstantiateBoard()
  {
    for (int i = 0; i < height; i++)
    {
      for (int j = 0; j < width; j++)
      {
        CreateNewTile(i, j, false);
      }
    }
  }

  Tile CreateNewTile(int y, int x, bool canHaveMatches)
  {
    var position = new Vector2(transform.position.x + x, transform.position.y + y);
    int offset = Random.Range(0, tilePrefabs.Length);
    bool foundSafeOffset = false;

    while (!canHaveMatches && !foundSafeOffset)
    {
      offset = Random.Range(0, tilePrefabs.Length);
      var potentialTag = tilePrefabs[offset].gameObject.tag;
      foundSafeOffset = IsSafeOffset((x, y), potentialTag);
    }

    var tileObjt = Instantiate(tilePrefabs[offset], position, Quaternion.identity);
    var tile = tileObjt.GetComponent<Tile>();
    tile.SwapEvent.AddListener(HandleSwapEvent);
    tile.GridPosition = (x, y);
    tileObjt.transform.parent = this.transform;
    tileObjt.transform.name = "( " + x + ", " + y + " )";
    tiles[y, x] = tile.GetComponent<Tile>();
    return tile;
  }

  bool IsSafeOffset((int x, int y) pos, string tag)
  {
    var (x, y) = pos;
    bool safeLeft = x < 2 || (tiles[y, x - 1].gameObject.tag != tag || tiles[y, x - 2].gameObject.tag != tag);
    bool safeDown = y < 2 || (tiles[y - 1, x].gameObject.tag != tag || tiles[y - 2, x].gameObject.tag != tag);
    return safeLeft && safeDown;
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.C))
    {
      //CheckForMatches();
    }

  }

  bool CheckForMatches()
  {
    var matches = FindAllMatches();
    bool anyPatternMatched = false;
    foreach (var pattern in patterns.Keys)
    {
      // Debug.Log("Checking Groups for pattern " + pattern);
      var matchedTiles = CheckMatchAnyGroup(pattern, matches);
      if (matchedTiles != null)
      {
        anyPatternMatched = true;
        // Debug.Log("Found matched pattern");
        MarkTilesToBeDestroyed(matchedTiles);
      }
    }
    if (anyPatternMatched)
    {
      freefalling = true;
      InvokeRepeating("CheckFreeFall", 0.3f, 0.3f);
    }
    return anyPatternMatched;
  }

  IEnumerator AsyncCheckForMatches(GameObject tile, SwapDirection direction)
  {
    yield return new WaitForSeconds(0.3f);
    bool anyPatternMatched = CheckForMatches();
    if (!anyPatternMatched)
    {
      Swap(tile, direction);
    }

  }

  void CheckFreeFall()
  {
    if (!freefalling)
    {
      CancelInvoke("CheckFreeFall");
      CheckForMatches();
      return;
    }
    bool newFreefalling = false;
    for (int i = 1; i < height; i++)
    {
      for (int j = 0; j < width; j++)
      {
        var tile = tiles[i, j];
        if (tile == null)
        {
          continue;
        }
        var tileBeneath = GetTileAtPos(i - 1, j);
        if (tileBeneath == null)
        {
          newFreefalling = true;
          tile.MoveTo((j, i - 1));
          tiles[i, j] = null;
          tiles[i - 1, j] = tile;
        }
      }
    }

    for (var j = 0; j < width; j++)
    {
      var tile = tiles[height - 1, j];
      if (tile == null)
      {
        newFreefalling = true;
        var newTile = CreateNewTile(height - 1, j, true);
        newTile.transform.localPosition = new Vector2(j, height);
        newTile.MoveTo((j, height - 1));
      }
    }
    freefalling = newFreefalling;
  }

  List<Tile> CheckMatchAnyGroup(Pattern pattern, List<HashSet<Tile>> groups)
  {
    var i = 0;
    foreach (var group in groups)
    {
      // Debug.Log("Checking Group " + i + " for matching patterns. Size: " + group.Count);
      foreach (var checkingTile in group)
      {
        if (checkingTile.ToBeDestroyed)
        {
          continue;
        }
        var matchedTiles = GroupMatchesPattern(pattern, group, checkingTile);
        if (matchedTiles != null)
        {
          return matchedTiles;
        }
      }
      i++;
    }
    return null;
  }

  GameObject Swap(GameObject tile, SwapDirection direction)
  {
    var tile1 = tile.GetComponent<Tile>();
    var (x1, y1) = tile1.GridPosition;
    var (offesetX, offsetY) = VectorFromDirection(direction);
    // Debug.Log("Swapping " + (x1, y1) + " + " + VectorFromDirection(direction));
    var (x2, y2) = (x1 + offesetX, y1 + offsetY);
    var tile2 = tiles[y2, x2];

    tile1.MoveTo((x2, y2));
    tile2.MoveTo((x1, y1));
    tiles[y1, x1] = tile2;
    tiles[y2, x2] = tile1;
    return tile2.gameObject;
  }

  void HandleSwapEvent(GameObject tile, SwapDirection direction)
  {
    var tile2 = Swap(tile, direction);
    StartCoroutine(AsyncCheckForMatches(tile2, direction));
  }

  void HandleGoalAchievedEvent()
  {
    Debug.Log("Goal Acheived");
  }

  private void MarkTilesToBeDestroyed(List<Tile> matchedTiles)
  {
    foreach (var tile in matchedTiles)
    {
      goalManager.HandleTileDestroyedEvent(tile.gameObject);
      tiles[tile.GridPosition.y, tile.GridPosition.x] = null;
      Destroy(tile.gameObject);
    }
  }

  private List<Tile> GroupMatchesPattern(Pattern pattern, HashSet<Tile> group, Tile checkingTile)
  {
    // Debug.Log("Checking tile " + checkingTile.ToPosString() + " against pattern " + pattern);
    var offsetsToCheck = patterns[pattern];
    List<Tile> matchedTiles = new List<Tile>();
    matchedTiles.Add(checkingTile);
    var (x, y) = checkingTile.GridPosition;
    // Debug.Log("\tTile has pos ( " + x + ", " + y + " )");
    foreach (var offset in offsetsToCheck)
    {
      var t = GetTileAtPos(y + (int)offset.x, x + (int)offset.y);
      if (t == null)
      {
        // Debug.Log("NULL " + (y + (int)offset.x) + " ," + (x + (int)offset.y));
        return null;
      }
      // Debug.Log("\tChecking inner tile ( " + t.GridPosition + " against pattern " + pattern);
      if (t == null || !group.Contains(t))
      {
        return null;
      }
      matchedTiles.Add(t);
    }
    return matchedTiles;
  }

  private Tile GetTileAtPos(int y, int x)
  {
    if (y >= 0 && x >= 0 && y < height && x < width)
    {
      return tiles[y, x];
    }
    return null;
  }

  private List<HashSet<Tile>> FindAllMatches()
  {
    List<HashSet<Tile>> matchedGroups = new List<HashSet<Tile>>();

    // Identify all contiguous groups.
    for (int i = 0; i < height; i++)
    {
      for (int j = 0; j < width; j++)
      {
        Tile tile = tiles[i, j];
        if (!tile.Visited)
        {
          HashSet<Tile> group = new HashSet<Tile>();
          BuildConnectedGroup(group, tile);
          if (group.Count > 2)
          {
            matchedGroups.Add(group);
          }
        }
      }
    }

    // Clear visited
    for (int i = 0; i < height; i++)
    {
      for (int j = 0; j < width; j++)
      {
        tiles[i, j].Visited = false;
      }
    }
    return matchedGroups;
  }

  private void BuildConnectedGroup(HashSet<Tile> group, Tile tile)
  {
    var matchingTag = tile.transform.tag;
    List<Tile> toVisit = new List<Tile>();
    toVisit.Add(tile);
    while (toVisit.Count > 0)
    {
      var visitingTile = toVisit[0];
      toVisit.RemoveAt(0);

      if (visitingTile.Visited) continue;

      if (visitingTile.transform.tag == matchingTag)
      {
        group.Add(visitingTile);
        visitingTile.Visited = true;
        toVisit.AddRange(GetNeighboursOfTile(visitingTile));
      }
    }
  }

  private List<Tile> GetNeighboursOfTile(Tile tile)
  {
    List<Tile> neighbours = new List<Tile>();
    var (x, y) = tile.GridPosition;
    if (x - 1 >= 0)
    {
      neighbours.Add(tiles[y, x - 1]);
    }
    if (y - 1 >= 0)
    {
      neighbours.Add(tiles[y - 1, x]);
    }
    if (x + 1 < width)
    {
      neighbours.Add(tiles[y, x + 1]);
    }
    if (y + 1 < height)
    {
      neighbours.Add(tiles[y + 1, x]);
    }
    return neighbours;
  }

  private (int, int) VectorFromDirection(SwapDirection direction)
  {
    switch (direction)
    {
      case SwapDirection.UP: return (0, 1);
      case SwapDirection.DOWN: return (0, -1);
      case SwapDirection.LEFT: return (-1, 0);
      default: return (1, 0);
    }
  }
}
