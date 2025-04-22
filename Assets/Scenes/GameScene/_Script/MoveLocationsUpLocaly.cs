using UnityEngine;

public class MoveLocationsUpLocaly : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position += transform.up * 3;
    }
}
