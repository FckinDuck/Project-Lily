using UnityEngine;

public interface IDeflectable 
{
    public float DeflectSpeed { get; set; }
    public void Deflect(Vector2 deflectDirection);
    public bool IsDeflected { get; set; }
    }
