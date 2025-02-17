using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class LevelTile : Tile
{
    public enum LevelStatus
    {
        Locked,
        Unlocked,
        Cleared
    }

    [SerializeField] private string _levelName;
    public string LevelName => _levelName;
}
