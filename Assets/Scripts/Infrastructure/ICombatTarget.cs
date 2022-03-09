using UnityEngine;

public interface ICombatTarget : IInterfaceable
{
    void Kill();
    Rigidbody2D Rigidbody2D { get; }

    //Strangle
    Vector2 StranglePosition { get; }
    ICollisionModifier CollisionModifier { get; }
    bool IsAlive { get; }
    Vector2 Position { get; }

    bool StartStrangling();
    void StopStrangling(Vector2 playerPos);
    void Knock(Vector2 vector2);

    EnemyMemoryModule Memory { get; }
}
