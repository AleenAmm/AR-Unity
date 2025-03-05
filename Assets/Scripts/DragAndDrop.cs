using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Vector3 originalPosition;
    private bool isPlacedCorrectly = false;
    private string droppedOnSlot = null;
    private PointsManager pointsManager;
    public bool startPlay = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        originalPosition = rectTransform.position;
        pointsManager = FindObjectOfType<PointsManager>();

        Debug.Log("🎯 Awake: RectTransform, CanvasGroup, and Canvas initialized for " + gameObject.name);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isPlacedCorrectly)
        {
            eventData.pointerDrag = null;
            return;
        }

        Debug.Log("🚀 Begin Drag: " + gameObject.name);
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        rectTransform.SetAsLastSibling();
        transform.SetParent(canvas.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPlacedCorrectly) return;
        rectTransform.position += (Vector3)eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        startPlay = true;
        if (isPlacedCorrectly) return;

        Debug.Log("🎯 End Drag: " + gameObject.name);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (droppedOnSlot != null)
        {
            Debug.Log("🎯 Dropped on slot: " + droppedOnSlot);
            if (droppedOnSlot == gameObject.tag)
            {
                Debug.Log("✅ Correct item placed in slot: " + droppedOnSlot);
                isPlacedCorrectly = true;

                // Do NOT change the item's position
                // It will stay exactly where it was dropped

                pointsManager.AddPoints(3);
                SetOriginalPosition(rectTransform.position);
            }
            else
            {
                Debug.Log("❌ Wrong slot! Returning item.");
                ResetPosition();
            }
        }
        else
        {
            Debug.Log("⚠ No valid drop target. Resetting position.");
            ResetPosition();
            pointsManager.GetHint(gameObject.name);
        }
    }

    private GameObject FindCorrectSlot(string tag)
    {
        GameObject[] slots = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject slot in slots)
        {
            if (slot.GetComponent<DragAndDrop>() == null)
            {
                return slot;
            }
        }
        return null;
    }

    public void SetOriginalPosition(Vector3 pos)
    {
        originalPosition = pos;
        Debug.Log("🗺️ Original position set to: " + pos);
    }

    public void SetPlacedCorrectly(bool placed)
    {
        isPlacedCorrectly = placed;
    }

    public void ResetPosition()
    {
        rectTransform.position = originalPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"🎯 Item: {gameObject.name}, Slot: {other.gameObject.name}, Slot Tag: {other.gameObject.tag}");
        if (other.CompareTag(gameObject.tag))
        {
            droppedOnSlot = other.gameObject.tag;
            Debug.Log($"🎯 Item entered correct slot: {droppedOnSlot}");
        }
        else
        {
            Debug.Log($"❌ Tag mismatch! Item tag: {gameObject.tag}, Slot tag: {other.gameObject.tag}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(gameObject.tag))
        {
            droppedOnSlot = null;
            Debug.Log($"🎯 Item exited slot: {other.gameObject.tag}");
        }
    }
}