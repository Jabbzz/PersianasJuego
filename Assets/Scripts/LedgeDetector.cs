using System;
using UnityEngine;

public class LedgeDetector : MonoBehaviour
{
    public bool ledgeAvailable = false;
    public Collider2D currentLedge;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ledge"))
        {
            ledgeAvailable = true;
            currentLedge = other;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == currentLedge)
        {
            ledgeAvailable = false;
            currentLedge = null;
        }
    }
}
