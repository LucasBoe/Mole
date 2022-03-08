using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnHandler : SingletonBehaviour<PlayerSpawnHandler>
{
    public GameObject Player;
    private static Transform spawnPoint;
    private static Vector2 spawnPos => spawnPoint == null ? Vector3.zero : spawnPoint.position;

    internal static void Respawn()
    {
        PlayerHealth playerHealth = Instance.Player.GetComponent<PlayerHealth>();
        playerHealth.Heal(int.MaxValue);
        Instance.Player.transform.position = spawnPos;
    }

    internal void Spawn()
    {
        spawnPoint = GameObject.Find("SpawnPoint").transform;
        Player = Instantiate(Player, spawnPos, Quaternion.identity);

    }
}
