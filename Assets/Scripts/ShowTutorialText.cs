using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ShowTutorialText : MonoBehaviour
{
    public GameObject tutorialText; // Assign the UI text GameObject in the inspector
    public Vector3 expandedScale = new Vector3(1.2f, 1.2f, 1f); // Target scale when expanded
    public float expandDuration = 0.5f;

    private RectTransform rectTransform;
    private RectTransform parentRectTransform;
    private Vector3 originalScale;

    private void Awake()
    {
        rectTransform = tutorialText.GetComponent<RectTransform>();
        parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        tutorialText.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialText.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(ExpandText());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(ShrinkText());
        }
    }

    private IEnumerator ExpandText()
    {
        rectTransform.localScale = originalScale;
        float elapsed = 0f;

        // Calculate max allowed scale based on parent and self size
        Vector2 parentSize = parentRectTransform.rect.size;
        Vector2 textSize = rectTransform.rect.size;
        float maxScaleX = parentSize.x / textSize.x;
        float maxScaleY = parentSize.y / textSize.y;
        float maxUniformScale = Mathf.Min(maxScaleX, maxScaleY, expandedScale.x);

        Vector3 targetScale = new Vector3(
            Mathf.Min(expandedScale.x, maxUniformScale),
            Mathf.Min(expandedScale.y, maxUniformScale),
            expandedScale.z
        );

        while (elapsed < expandDuration)
        {
            float t = elapsed / expandDuration;
            rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.localScale = targetScale;
    }

    private IEnumerator ShrinkText()
    {
        Vector3 startScale = rectTransform.localScale;
        float elapsed = 0f;

        while (elapsed < expandDuration)
        {
            float t = elapsed / expandDuration;
            rectTransform.localScale = Vector3.Lerp(startScale, originalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.localScale = originalScale;
        tutorialText.SetActive(false);
    }
}