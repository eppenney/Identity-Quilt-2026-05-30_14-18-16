using UnityEngine;
using TMPro; // Required to communicate with TextMeshPro assets

public class CreditScroller : MonoBehaviour
{
    [Header("Scroll Settings")]
    [Tooltip("The speed the text moves upward in UI pixels per second.")]
    [SerializeField] private float scrollSpeed = 40f;

    private RectTransform textRectTransform;

    void Awake()
    {
        // Grab the RectTransform component responsible for UI positioning
        textRectTransform = GetComponent<RectTransform>();

        // Optional safety check: Warning if you forgot to use TextMeshPro
        if (GetComponent<TextMeshProUGUI>() == null)
        {
            Debug.LogWarning($"[CreditScroller] No TextMeshPro component found on {gameObject.name}. Make sure this is a UI Text element!", this);
        }
    }

    void Update()
    {
        // 1. Calculate how far to move this specific frame
        float movementAmount = scrollSpeed * Time.deltaTime;

        // 2. Translate the UI element vertically upward over time
        textRectTransform.anchoredPosition += new Vector2(0f, movementAmount);
    }
}