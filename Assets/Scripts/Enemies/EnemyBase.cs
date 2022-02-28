using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    Dictionary<System.Type, object> modules = new Dictionary<System.Type, object>();
    public System.Action OnEnemyDeath;

    public void Register<T>(object module)
    {
        modules.Add(typeof(T), module);
    }

    public T GetModule<T>()
    {
        if (modules.ContainsKey(typeof(T)))
            return (T)modules[typeof(T)];

        Debug.LogWarning("tried getting module of type " + typeof(T).ToString());
        return default(T);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector2(transform.position.x, transform.position.y), 0.5f);
        Gizmos.DrawSphere(new Vector2(transform.position.x, transform.position.y + 0.5f), 0.5f);
        Gizmos.DrawSphere(new Vector2(transform.position.x, transform.position.y - 0.5f), 0.5f);
    }
}
