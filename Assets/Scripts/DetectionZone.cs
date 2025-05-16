using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
    public UnityEvent NoCollidersRemain;
    public List<Collider2D> detectedColliders = new List<Collider2D>();
    Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        detectedColliders.Add(collision);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        detectedColliders.Remove(collision);

        if (detectedColliders.Count <= 0)
        {
            NoCollidersRemain.Invoke();
        }
    }
}
