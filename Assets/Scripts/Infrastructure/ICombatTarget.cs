using UnityEngine;

public interface ICombatTarget : IInterfaceable
{
    void Kill();
    Rigidbody2D Rigidbody2D { get; }

    //Strangle
    ICollisionModifier CollisionModifier { get; }
    bool IsAlive { get; }
    Vector2 Position { get; }

    bool StartStrangling();
    void StopStrangling();
    void FinishStrangling();
    void Knock(Vector2 vector2);

    EnemyMemoryModule Memory { get; }

}
