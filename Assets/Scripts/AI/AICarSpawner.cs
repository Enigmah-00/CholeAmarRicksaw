using System.Collections;
using UnityEngine;

public class AICarSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] carAIprefabs;

    GameObject[] carAIPool = new GameObject[20];

    Transform playerCarTransform;

    [SerializeField]
    LayerMask otherCarsLayerMask ;
    Collider[] overlappedCheckCollider = new Collider[1];



    float timeCarLastSpawned = 0;
        WaitForSeconds wait = new WaitForSeconds(0.5f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;
        int PrefabIndex = 0;
        for(int i = 0; i < carAIPool.Length; i++)
        {
            carAIPool[i] = Instantiate(carAIprefabs[PrefabIndex]);
            carAIPool[i].SetActive(false);
            PrefabIndex ++;

            if(PrefabIndex > carAIprefabs.Length - 1)
            {
                PrefabIndex = 0;
            }
        }
        StartCoroutine(UpdateLessOftenCO());
    }

    IEnumerator UpdateLessOftenCO()
    {
        while (true)
        {
            CleanUpCarsBeyondView();
            SpawnNewCars();
            yield return wait;
        }
    }

    void SpawnNewCars()
    {
        if(Time.time - timeCarLastSpawned < 2)
        {
            return;
        }
        GameObject carToSpawn = null;
        foreach(GameObject aiCar in carAIPool)
        {
            if(aiCar.activeInHierarchy) continue;
            carToSpawn = aiCar;
            break;
        }
        if(carToSpawn == null) return;

        Vector3 spawnPosition = new Vector3(0,0,playerCarTransform.transform.position.z + 100);

    if(Physics.OverlapBoxNonAlloc(spawnPosition,Vector3.one * 2 , overlappedCheckCollider,Quaternion.identity,otherCarsLayerMask) > 0){
        return;
    } 

        carToSpawn.transform.position = spawnPosition;
        carToSpawn.SetActive(true);

        timeCarLastSpawned = Time.time;
    }
    void CleanUpCarsBeyondView()
    {
        foreach(GameObject aiCar in carAIPool)
        {
            if(!aiCar.activeInHierarchy) continue;
            if(aiCar.transform.position.z - playerCarTransform.position.z < -50)
              aiCar.SetActive(false);
        }
    }
}
