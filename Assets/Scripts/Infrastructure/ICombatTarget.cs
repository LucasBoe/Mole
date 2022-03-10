using UnityEngine;

public interface ICombatTarget : IInterfaceable
{
    void Kill();
    Rigidbody2D Rigidbody2D { get; }

    //Strangle
    ICollisionModifier CollisionModifier { get; }
    bool IsAlive { get; }
    Vector2 Position { get; }
    Direction2D Forward { get; }

    bool StartStrangling();
    void StopStrangling(Vector2 playerPos);
    void Knock(Vector2 vector2);

    EnemyMemoryModule Memory { get; }
}
