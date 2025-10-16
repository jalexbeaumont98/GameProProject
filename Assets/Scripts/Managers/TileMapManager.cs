using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
    public static Tilemap DestructibleMap { get; private set; }
    public static Tilemap DecDestructibleMap { get; private set; }

    private void Awake()
    {
        DestructibleMap = GameObject.FindGameObjectWithTag("Destructible")?.GetComponent<Tilemap>();
        DecDestructibleMap = GameObject.FindGameObjectWithTag("Dec_Destructible")?.GetComponent<Tilemap>();
    }
}
