using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneConstructor : MonoBehaviour
{
    public GameObject[] ToInstatiate;
    public GameObject Player;
    private void Awake()
    {
        //Spawn instances
        Transform marker = new GameObject("==Instances==").transform;
        foreach (GameObject gameObject in ToInstatiate)
        {
            Instantiate(gameObject);
        }
        marker.SetAsFirstSibling();

        //Spawn player
        Transform spawnPoint = GameObject.Find("SpawnPoint").transform;
        Vector2 spawnPos = spawnPoint == null ? Vector3.zero : spawnPoint.position;
        Instantiate(Player, spawnPos, Quaternion.identity);

        Destroy(gameObject);
    }
}
