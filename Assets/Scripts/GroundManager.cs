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
    public GameObject[] connectiveTiles;
    public GameObject[] enemies;
    public Dictionary<Vector3, GameObject> spawnedTiles = new Dictionary<Vector3, GameObject>();
    public Dictionary<Vector3, GameObject> spawnedEnemies = new Dictionary<Vector3, GameObject>();
    private Dictionary<GameObject, Vector3> tileSizes = new Dictionary<GameObject, Vector3>();
    HashSet<Tuple<Vector3, Vector3>> connections = new HashSet<Tuple<Vector3, Vector3>>();


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
        foreach (var tile in connectiveTiles)
        {
            SetLayerRecursively(tile.gameObject, 10);
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
                Vector3 pos = new Vector3(x * gridSize, 0, z * gridSize);
                if (!spawnedTiles.ContainsKey(pos) && !IsTileOccupied(pos))
                {
                    int randomIndex = Random.Range(0, groundTiles.Length);
                    GameObject tilePrefab = groundTiles[randomIndex];
                    GameObject obj = Instantiate(tilePrefab, pos, Quaternion.identity);

                    // Set rotation
                    int randomRotation = Random.Range(1, 4);
                    float rotationAngle = 90 * randomRotation;
                    obj.transform.rotation = Quaternion.Euler(0, rotationAngle, 0);

                    spawnedTiles.Add(pos, obj);

                    // Enemy Spawning Logic
                    SpawnEnemyNearTile(pos, randomIndex);
                    GenerateConnectiveTiles(pos, obj);
                }
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
    void GenerateConnectiveTiles(Vector3 islandPosition, GameObject island)
    {
        GameObject nearestIsland = FindNearestIsland(islandPosition);

        if (nearestIsland != null && !IsAlreadyConnected(islandPosition, nearestIsland.transform.position))
        {
            InstantiateBridge(islandPosition, nearestIsland.transform.position, island);
            // Add connection to HashSet
            var connection = new Tuple<Vector3, Vector3>(islandPosition, nearestIsland.transform.position);
            connections.Add(connection);
        }
    }

    GameObject FindNearestIsland(Vector3 position)
    {
        GameObject nearestIsland = null;
        float minDistance = float.MaxValue;

        foreach (var tile in spawnedTiles)
        {
            if (Array.IndexOf(connectiveTiles, tile.Value) >= 0)
            {
                // Skip if the tile is a connective tile
                continue;
            }

            float distance = Vector3.Distance(position, tile.Key);
            if (distance < minDistance && distance != 0)
            {
                minDistance = distance;
                nearestIsland = tile.Value;
            }
        }

        return nearestIsland;
    }


    void InstantiateBridge(Vector3 startPos, Vector3 endPos, GameObject island)
    {
        Vector3 direction = (endPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, endPos);

        GameObject bridgePrefab = connectiveTiles[Random.Range(0, connectiveTiles.Length)];
        Vector3 bridgeSize = bridgePrefab.transform.localScale;

        // Calculate the number of bridges needed based on the distance and the length of a single bridge piece
        int numberOfBridges = Mathf.CeilToInt(0.6f * distance / bridgeSize.z); // Assuming the bridge is oriented along the z-axis

        // Adjust the start position so the bridge starts at the edge of the island
        Vector3 currentPos = startPos + direction * (island.transform.localScale.z * 1.5f);

        // Instantiate bridge pieces end-to-end
        for (int i = 0; i < numberOfBridges; i++)
        {
            // Instantiate the bridge prefab at the current position
            GameObject bridgePiece = Instantiate(bridgePrefab, currentPos, Quaternion.LookRotation(direction));
            spawnedTiles.Add(currentPos, bridgePiece);

            // Optionally, adjust the scale of the bridge piece if necessary
            bridgePiece.transform.localScale = bridgeSize;

            // Update the current position for the next bridge piece
            currentPos += direction * bridgeSize.z;
        }
    }



    // You can optimize RemoveDistantChunks by creating a list of tiles to remove
    // and then iterating over that list to remove them. This avoids modifying the
    // dictionary while iterating over it.


    bool IsAlreadyConnected(Vector3 pos1, Vector3 pos2)
    {
        // Implement logic to check if two positions are already connected by a bridge
        // You can use a HashSet, a Dictionary, or any other collection to keep track of connections
        // For simplicity, the code for this is not shown here
        // Tuple creates an immutable pair of values, which you can use as a key in a HashSet

        var connection = new Tuple<Vector3, Vector3>(pos1, pos2);
        var reverseConnection = new Tuple<Vector3, Vector3>(pos2, pos1);

        // Check both directions since connection can be bidirectional
        if (connections.Contains(connection) || connections.Contains(reverseConnection))
        {
            return true;
        }

        // Default case: if no connection is found, return false
        return false;
    }

    void RemoveDistantChunks(Vector3 playerPosition)
    {
        // Iterate through all existing chunks.
        // If a chunk's distance from the player exceeds the generation radius,
        // remove or deactivate the chunk.

        foreach (KeyValuePair<Vector3, GameObject> tile in spawnedTiles)
        {
            Vector3 pos = tile.Key;
            if (Vector3.Distance(playerPosition, pos) > generationRadius)
            {
                Destroy(tile.Value);
                spawnedTiles.Remove(tile.Key);
            }
        }

        foreach (KeyValuePair<Vector3, GameObject> enemy in spawnedEnemies)
        {
            Vector3 pos = enemy.Key;
            if (Vector3.Distance(playerPosition, pos) > generationRadius)
            {
                Destroy(enemy.Value);
                spawnedTiles.Remove(enemy.Key);
            }
        }
    }
}