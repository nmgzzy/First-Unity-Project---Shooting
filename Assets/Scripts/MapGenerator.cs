using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab;
    public Coord maxMapSize;
    [Range(0, 0.5f)]
    public float outlinePercent;
    [Range(0.1f, 5)]
    public float tileSize = 1;
    [System.Serializable]
    public struct Coord
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
    [System.Serializable]
    public class Map
    {
        public Coord mapSize;
        public Coord mapCentre
        {
            get
            {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
        [Range(0, 0.8f)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;
    }
    Map currentMap;
    public Map[] maps;
    public int mapIndex;
    Queue<Coord> shuffledOpenTileCoords;
    Transform[,] tileMap;

    void Start()
    {
        GenerateMap();
    }
    public void GenerateMap()
    {
        currentMap = maps[mapIndex];

        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random prng = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, 0.02f, currentMap.mapSize.y * tileSize);
        //init allTileCoords
        List<Coord> allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        Queue<Coord> shuffledTileCoords = new Queue<Coord>(Utility.shuffleArray(allTileCoords.ToArray(), currentMap.seed));

        //create map holder
        string hoderName = "Generated Map";
        if (transform.Find(hoderName))
        {
            DestroyImmediate(transform.Find(hoderName).gameObject);
        }
        Transform mapHolder = new GameObject(hoderName).transform;
        mapHolder.parent = transform;

        //Generate tile
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 tilePisition = Coord2Positon(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePisition, Quaternion.Euler(Vector3.right * 90), mapHolder);
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                tileMap[x, y] = newTile;
            }
        }

        //Generate obstacle
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        bool[,] obstacleMap = new bool[(int)(currentMap.mapSize.x), (int)(currentMap.mapSize.y)];
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord(shuffledTileCoords);
            currentObstacleCount++;
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            if (randomCoord != currentMap.mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                Vector3 obstaclePosition = Coord2Positon(randomCoord.x, randomCoord.y) + Vector3.up * obstacleHeight / 2;
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity, mapHolder);
                newObstacle.localScale = (Vector3.forward + Vector3.right) * (1 - outlinePercent) * tileSize + Vector3.up * obstacleHeight;

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allTileCoords.Remove(randomCoord);
            }
            else
            {
                currentObstacleCount--;
                obstacleMap[randomCoord.x, randomCoord.y] = false;
            }
        }

        shuffledOpenTileCoords = new Queue<Coord>(Utility.shuffleArray(allTileCoords.ToArray(), currentMap.seed));

        //generate nav mesh mask
        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (maxMapSize.x + currentMap.mapSize.x) / 4f * tileSize, Quaternion.identity, mapHolder);
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, maxMapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (maxMapSize.x + currentMap.mapSize.x) / 4f * tileSize, Quaternion.identity, mapHolder);
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, maxMapSize.y) * tileSize;

        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (maxMapSize.y + currentMap.mapSize.y) / 4f * tileSize, Quaternion.identity, mapHolder);
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (maxMapSize.y + currentMap.mapSize.y) / 4f * tileSize, Quaternion.identity, mapHolder);
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y, 1) * tileSize;
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        int sizex = obstacleMap.GetLength(0), sizey = obstacleMap.GetLength(1);
        bool[,] mapFlag = new bool[sizex, sizey];
        Queue<Coord> bfsQueue = new Queue<Coord>();
        mapFlag[currentMap.mapCentre.x, currentMap.mapCentre.y] = true;
        bfsQueue.Enqueue(currentMap.mapCentre);
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
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }
    Coord GetRandomCoord(Queue<Coord> shuffledTileCoords)
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }

}

