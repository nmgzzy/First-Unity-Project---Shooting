using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Vector2 mapSize;
    [Range(0,1)]
    public float outlinePercent;

    void Start() {
        GenerateMap();
    }
    public void GenerateMap()
    {
        string hoderName = "Generated Map";
        if (transform.Find(hoderName))
        {
            DestroyImmediate(transform.Find(hoderName).gameObject);
        }
        Transform mapHolder = new GameObject(hoderName).transform;
        mapHolder.parent = transform;
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePisition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
                Transform newTile = Instantiate(tilePrefab, tilePisition, Quaternion.Euler(Vector3.right*90), mapHolder);
                newTile.localScale = Vector3.one * (1-outlinePercent);
            }
        }
    }
}
