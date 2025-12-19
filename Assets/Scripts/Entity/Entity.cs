using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    
}

public class Entity<T> : EntityBase where T : Entity<T>
{ 
    
}