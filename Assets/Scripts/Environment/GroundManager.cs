using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class Voxel : MonoBehaviour
{
    public int gridSize = 20;
    public int generationRadius = 100;
    public GameObject player;
    public GameObject[] groundTiles;
    public GameObject[] enemies;
    public Dictionary<Vector3, GameObject> spawnedTiles = new Dictionary<Vector3, GameObject>();
    public Dictionary<Vector3, GameObject> spawnedEnemies = new Dictionary<Vector3, GameObject>();
    private Dictionary<GameObject, Vector3> tileSizes = new Dictionary<GameObject, Vector3>();


    public GameObject initialIsland;

    public LayerMask whatIsGround;

    private void Start()
    {
        foreach (var tile in groundTiles)
        {
            SetLayerRecursively(tile.gameObject, 10);

            if (tile.GetComponent<Renderer>() != null)
            {
                Vector3 size = tile.GetComponent<Renderer>().bounds.size;
                tileSizes[tile] = size;
            }
            else
            {
                // Default size or log error if no renderer is found
                tileSizes[tile] = new Vector3(gridSize, 0, gridSize);
            }
        }
        spawnedTiles.Add(initialIsland.transform.position, initialIsland);


    }

    public void Update()
    {
        Vector3 playerPosition = player.transform.position;

        GenerateChunksAroundPlayer(playerPosition);

        RemoveDistantChunks(playerPosition);

    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }


        obj.layer = newLayer;
        obj.tag = "Island";

        foreach (Transform child in obj.transform)
        {
            if (child == null)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }


    void GenerateChunksAroundPlayer(Vector3 playerPosition)
    {
        for (int x = (((int)playerPosition.x) - generationRadius) / gridSize; x < (((int)playerPosition.x) + generationRadius) / gridSize; x++)
        {
            for (int z = (((int)playerPosition.z) - generationRadius) / gridSize; z < (((int)playerPosition.z) + generationRadius) / gridSize; z++)
            {
                int randomIndex = Random.Range(0, groundTiles.Length);
                GameObject tilePrefab = groundTiles[randomIndex];
                Vector3 tileSize = tileSizes[tilePrefab];
                Vector3 pos = new Vector3(x * tileSize.x, 0, z * tileSize.z);

                if (spawnedTiles.ContainsKey(pos) || IsTileOccupied(pos))
                {
                    continue;
                }

                GameObject obj = Instantiate(tilePrefab, pos, Quaternion.identity);

                // Set rotation
                int randomRotation = Random.Range(1, 4);
                float rotationAngle = 90 * randomRotation;
                obj.transform.rotation = Quaternion.Euler(0, rotationAngle, 0);

                spawnedTiles.Add(pos, obj);
                SpawnEnemyNearTile(pos, randomIndex);
            }
        }
    }

    void SpawnEnemyNearTile(Vector3 tilePos, int tileIndex)
    {
        Vector3 enemySpawnPos = tilePos + new Vector3(0, 1, 0); // Adjust the Y coordinate as needed
        if (!spawnedEnemies.ContainsKey(enemySpawnPos))
        {
            GameObject enemyPrefab = enemies[Random.Range(0, enemies.Length)];
            GameObject enemy = Instantiate(enemyPrefab, enemySpawnPos, Quaternion.identity);
            spawnedEnemies.Add(enemySpawnPos, enemy);
        }
    }

    bool IsTileOccupied(Vector3 position)
    {
        // Check if there's already an object at this position.
        // Adjust the radius and layerMask as needed.
        return Physics.CheckSphere(position, gridSize / 2f, whatIsGround);
    }


    void RemoveDistantChunks(Vector3 playerPosition)
    {
        List<Vector3> tilesRemove = new List<Vector3>();
        List<Vector3> enemiesRemove = new List<Vector3>();

        foreach (KeyValuePair<Vector3, GameObject> tile in spawnedTiles)
        {
            Vector3 pos = tile.Key;
            if (Vector3.Distance(playerPosition, pos) > generationRadius)
            {
                Destroy(tile.Value);
                tilesRemove.Add(pos);
            }
        }

        foreach (Vector3 tile in tilesRemove)
        {
            spawnedTiles.Remove(tile);
        }

        foreach (KeyValuePair<Vector3, GameObject> enemy in spawnedEnemies)
        {
            Vector3 pos = enemy.Key;
            if (Vector3.Distance(playerPosition, pos) > generationRadius)
            {
                Destroy(enemy.Value);
                enemiesRemove.Add(pos);
            }
        }

        foreach (Vector3 enemy in enemiesRemove)
        {
            spawnedEnemies.Remove(enemy);
        }
    }
}