using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navmeshFloor;
	public Transform navmeshMaskPrefab;
    public Vector2 mapSize;
    public Vector2 maxMapSize;
    public int seed = 10;
    [Range(0, 0.5f)]
    public float outlinePercent;
    [Range(0, 0.8f)]
    public float obstaclePercent = 0.1f;
    [Range(0.1f, 5)]
    public float tileSize = 1;
    struct Coord
    {
        public int x;
        public int y;
        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }
    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    Coord mapCentre;

    void Start()
    {
        GenerateMap();
    }
    public void GenerateMap()
    {
        //init allTileCoords
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.shuffleArray(allTileCoords.ToArray(), seed));
        mapCentre = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);
        //Generate tile
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
                Vector3 tilePisition = Coord2Positon(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePisition, Quaternion.Euler(Vector3.right * 90), mapHolder);
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
            }
        }
        //Generate obstacle
        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;
        bool[,] obstacleMap = new bool[(int)(mapSize.x), (int)(mapSize.y)];
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            currentObstacleCount++;
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            if (randomCoord != mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = Coord2Positon(randomCoord.x, randomCoord.y) + Vector3.up * 0.5f;
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity, mapHolder);
                newObstacle.localScale = ((Vector3.right + Vector3.forward) * (1-outlinePercent) + Vector3.up) * tileSize;
            }
            else
            {
                currentObstacleCount--;
                obstacleMap[randomCoord.x, randomCoord.y] = false;
            }
        }

        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (maxMapSize.x+mapSize.x) / 4 * tileSize, Quaternion.identity, mapHolder);
        maskLeft.localScale = new Vector3((maxMapSize.x - mapSize.x) / 2, 1, maxMapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (maxMapSize.x+mapSize.x) / 4 * tileSize, Quaternion.identity, mapHolder);
        maskRight.localScale = new Vector3((maxMapSize.x - mapSize.x) / 2, 1, maxMapSize.y) * tileSize;
        
        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (maxMapSize.y+mapSize.y) / 4 * tileSize, Quaternion.identity, mapHolder);
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - mapSize.y) / 2) * tileSize;
        
        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (maxMapSize.y+mapSize.y) / 4 * tileSize, Quaternion.identity, mapHolder);
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - mapSize.y) / 2) * tileSize;

        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y, 1) * tileSize;
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        int sizex = obstacleMap.GetLength(0), sizey = obstacleMap.GetLength(1);
        bool[,] mapFlag = new bool[sizex, sizey];
        Queue<Coord> bfsQueue = new Queue<Coord>();
        mapFlag[mapCentre.x, mapCentre.y] = true;
        bfsQueue.Enqueue(mapCentre);
        int accessibleTileCount = 1;
        Coord[] neighbor = { new Coord(0, 1), new Coord(0, -1), new Coord(1, 0), new Coord(-1, 0) };

        while (bfsQueue.Count > 0)
        {
            Coord tile = bfsQueue.Dequeue();
            foreach (Coord n in neighbor)
            {
                int x = tile.x + n.x;
                int y = tile.y + n.y;
                if (x >= 0 && y >= 0 && x < sizex && y < sizey)
                {
                    if (!mapFlag[x, y] && !obstacleMap[x, y])
                    {
                        mapFlag[x, y] = true;
                        accessibleTileCount++;
                        bfsQueue.Enqueue(new Coord(x, y));
                    }
                }
            }
        }
        return accessibleTileCount == (sizex * sizey - currentObstacleCount);
    }

    Vector3 Coord2Positon(int x, int y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y) * tileSize;
    }
    Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }
}
