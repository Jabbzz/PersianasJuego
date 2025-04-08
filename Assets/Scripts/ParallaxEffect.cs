using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera; // Reference to the main camera
    public Transform followTarget; // The target object to follow

    [Header("Parallax Settings")]
    [Range(0f, 1f)]
    public float parallaxStrength = 0.5f; // Strength of the parallax effect

    private Vector2 startPosition; // The initial position of the background
    private float startingZ;

    private Vector2 camMoveSinceStart => (Vector2)mainCamera.transform.position - startPosition; // Camera movement since the start
    private float zDistanceFromTarget => transform.position.z - followTarget.transform.position.z; // Distance from the target to the background

    // Determines which clipping plane to use based on the object's position relative to the target
    private float clippingPlane => (mainCamera.transform.position.z + 
        (zDistanceFromTarget > 0 ? mainCamera.farClipPlane : mainCamera.nearClipPlane));

    private float parallaxFactor => Mathf.Abs(zDistanceFromTarget) / clippingPlane * parallaxStrength;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position; // Store the initial position of the background
        startingZ = transform.position.z; // Store the initial Z position
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the new position based on the parallax effect
        Vector2 newPosition = startPosition + camMoveSinceStart * parallaxFactor;

        // Update the position of the background
        transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
    }
}
