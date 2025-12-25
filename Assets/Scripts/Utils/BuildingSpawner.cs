using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    [Header("Building Prefabs")]
    [SerializeField] private GameObject[] buildingPrefabs = new GameObject[4];
    
    [Header("Spawn Settings")]
    [SerializeField] private float groundHeight = 0f;
    [SerializeField] private float buildingGap = 100f;
    [SerializeField] private float spawnAreaSize = 500f;
    [SerializeField] private float minDistanceFromPlayer = 50f;
    [SerializeField] private Vector2 heightRange = new Vector2(10f, 30f);
    [SerializeField] private Vector2 widthRange = new Vector2(5f, 15f);
    [SerializeField] private int maxSpawnAttempts = 10;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnBuildings();
    }

    void SpawnBuildings()
    {
        if (buildingPrefabs.Length == 0)
        {
            Debug.LogWarning("No building prefabs assigned!");
            return;
        }
        
        // Find player position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPosition = player != null ? player.transform.position : Vector3.zero;
        
        for (int i = 0; i < buildingPrefabs.Length; i++)
        {
            if (buildingPrefabs[i] == null)
            {
                Debug.LogWarning($"Building prefab at index {i} is null!");
                continue;
            }
            
            Vector3 spawnPosition = Vector3.zero;
            bool validPositionFound = false;
            
            // Try to find a valid spawn position
            for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
            {
                // Generate random position within spawn area
                float randomX = Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f);
                float randomZ = Random.Range(-spawnAreaSize / 2f, spawnAreaSize / 2f);
                
                spawnPosition = new Vector3(
                    transform.position.x + randomX,
                    groundHeight,
                    transform.position.z + randomZ
                );
                
                // Check distance from player
                if (player != null && Vector3.Distance(spawnPosition, playerPosition) < minDistanceFromPlayer)
                {
                    continue; // Too close to player, try again
                }
                
                // Check distance from other spawned buildings (minimum gap)
                bool tooCloseToOtherBuilding = false;
                GameObject[] existingBuildings = GameObject.FindGameObjectsWithTag("Building");
                foreach (GameObject existingBuilding in existingBuildings)
                {
                    if (Vector3.Distance(spawnPosition, existingBuilding.transform.position) < buildingGap)
                    {
                        tooCloseToOtherBuilding = true;
                        break;
                    }
                }
                
                if (!tooCloseToOtherBuilding)
                {
                    validPositionFound = true;
                    break;
                }
            }
            
            if (!validPositionFound)
            {
                Debug.LogWarning($"Could not find valid position for building {i} after {maxSpawnAttempts} attempts");
                continue;
            }
            
            // Spawn the building
            GameObject building = Instantiate(buildingPrefabs[i], spawnPosition, Quaternion.identity);
            building.tag = "Building"; // Tag for distance checking
            
            // Randomize height and width
            float randomHeight = Random.Range(heightRange.x, heightRange.y);
            float randomWidth = Random.Range(widthRange.x, widthRange.y);
            
            // Apply random scale
            building.transform.localScale = new Vector3(randomWidth, randomHeight, randomWidth);
            
            // Random rotation
            building.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            
            Debug.Log($"Spawned building {i} at {spawnPosition} with height: {randomHeight}, width: {randomWidth}");
        }
    }
}
