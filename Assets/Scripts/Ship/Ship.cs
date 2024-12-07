using UnityEngine;

public class Ship : MonoBehaviour
{
    [Header("Stats")]
    public float maxSpeed = 8f;
    public float acceleration = 10f;
    public float steeringSpeed = 100f;

    [Header("References")]
    public Transform windInfluencePoint;

    public float GetSpeed()
    {
        return maxSpeed;
    }

    public float GetAcceleration()
    {
        return acceleration;
    }

    public float GetSteeringSpeed()
    {
        return steeringSpeed;
    }
}
