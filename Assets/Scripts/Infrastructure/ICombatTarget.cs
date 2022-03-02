using UnityEngine;

public interface ICombatTarget : IInterfaceable
{
    void Kill();
    Rigidbody2D Rigidbody2D { get; }

    //Strangle
    Vector2 StranglePosition { get; }
    bool StartStrangling();
    void StopStrangling(Vector2 playerPos);
}
