using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HE_Explosion : Explosion
{

    [SerializeField] private Tilemap destructibleMap;
    [SerializeField] private Tilemap decDestructibleMap;
    [SerializeField] private float radius = 1.5f;

    private Vector3Int pos_temp = new Vector3Int(0, 0, 0);

    void Start()
    {
        destructibleMap = TileMapManager.DestructibleMap;
        decDestructibleMap = TileMapManager.DecDestructibleMap;
    }

    public void ExplodeDestructableTiles()
    {

        BoundsInt bounds = new BoundsInt(
        destructibleMap.WorldToCell(transform.position - Vector3.one * radius),
                new Vector3Int(Mathf.CeilToInt(radius * 2), Mathf.CeilToInt(radius * 2), 1));


        foreach (var pos in bounds.allPositionsWithin)
        {

            var tile = destructibleMap.GetTile(pos);
            if (tile != null)
            {
                Debug.Log($"Removing tile at {pos}");
                destructibleMap.SetTile(pos, null);
                DestroyDecorativeTileAbove(pos);
            }
            else
            {
                Debug.Log($"No tile at {pos}");
            }


        }

        //destructibleMap.RefreshAllTiles();
    }
    
    private void DestroyDecorativeTileAbove(Vector3Int tilePos)
    {
        if (decDestructibleMap == null) return;

        Vector3Int abovePos = new Vector3Int(tilePos.x, tilePos.y + 1, tilePos.z);

        if (decDestructibleMap.HasTile(abovePos))
        {
            decDestructibleMap.SetTile(abovePos, null);
        }
    }

    

   
}
