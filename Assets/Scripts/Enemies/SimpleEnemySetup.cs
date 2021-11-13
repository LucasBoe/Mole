using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SimpleEnemySetup : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        SimpleEnemy simpleEnemy = GetComponent<SimpleEnemy>();
        simpleEnemy.UpdateViewcone();
    }
}
