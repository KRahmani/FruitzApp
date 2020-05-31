using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Level")]
public class Level : ScriptableObject
{
    public int width, height;

    public TileType[] boadLayout;

    public GameObject[] fruits;

    public int[] scoreGoals;

    public EndGameRequirements endGameRequirements;

    public BlankGoal[] levelGoals;

}
