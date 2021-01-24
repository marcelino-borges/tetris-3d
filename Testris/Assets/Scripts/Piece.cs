using UnityEngine;

public class Piece : MonoBehaviour
{
    public int X { get => (int)transform.position.x; }
    public int Y { get => (int)transform.position.y; }
    
    internal void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }
}
