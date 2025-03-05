using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public string requiredTag; // Set this in Inspector (e.g., "Fruits", "Meat")

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            Debug.LogError("🚨 ERROR: pointerDrag is NULL! Drop event not detected.");
            return;
        }

        GameObject draggedObject = eventData.pointerDrag;
        Debug.Log("🔥 OnDrop CALLED on slot: " + gameObject.name + " with item: " + draggedObject.name);

        DragAndDrop dragScript = draggedObject.GetComponent<DragAndDrop>();

        string actualTag = draggedObject.tag;
        Debug.Log("📌 Actual Tag of Dropped Item: " + actualTag);

        if (actualTag == requiredTag)
        {
            Debug.Log("✅ Correct item placed in slot: " + requiredTag);
            dragScript.SetPlacedCorrectly(true);

            // Parent the item to the slot without resetting its position
            draggedObject.transform.SetParent(transform, false);
        }
        else
        {
            Debug.Log("❌ Wrong slot! Returning item.");
            dragScript.SetPlacedCorrectly(false);
            dragScript.ResetPosition();
        }
    }
}