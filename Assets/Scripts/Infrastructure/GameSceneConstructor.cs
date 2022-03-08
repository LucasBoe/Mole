using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneConstructor : MonoBehaviour
{
    public GameObject[] ToInstatiate;
    [SerializeField] PlayerSpawnHandler playerSpawnHandler;
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
        playerSpawnHandler.Spawn();

        Destroy(gameObject);
    }
}
