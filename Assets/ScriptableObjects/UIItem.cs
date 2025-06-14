using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIItem : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData data;
    //[SerializeField] Image image;
    [SerializeField] RectTransform rect;
    [SerializeField] Image image;

    private const int TileSize = 86;
    private const int TileSpace = 4;
    private Vector2 baseRectSize;
    private Vector2 equippedRectSize;
    private Vector2 currentRectSize;
    public Vector2 homePosition = new Vector2();

    public Vector2 EquippedRectOffset { get; private set; }
    public Vector2Int Spot { get; private set; } = new Vector2Int(-1, -1);

    //private InventoryGrid inventoryGrid;

    Vector2 offset = new Vector2();


    private void Start()
    {
        //inventoryGrid = FindObjectOfType<InventoryGrid>();
    }

    public void UpdatePosition()
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, Camera.main, out position);
        transform.position = rect.TransformPoint(position);
    }

    public void SetHomePositionAndSpot(Vector2 pos, Vector2Int spotIn)
    {
        // Define items spot
        Spot = spotIn;
        if (Spot == new Vector2Int(-1, -1))
            homePosition = pos + EquippedRectOffset;
        else
            homePosition = pos;

        // Place item
        transform.localPosition = homePosition;

        ResetPosition();
    }

    public void SetData(ItemData dataIn)
    {
        // Define this UI Item at creation
        data = dataIn;
        UpdateItem();
        //baseRectSize = new Vector2(data.size.y * TileSize + (data.size.y - 1) * TileSpace, data.size.x * TileSize + (data.size.x - 1) * TileSpace);
        //SetToBaseRectSize();
    }

    public void SetData(ItemData dataIn, Vector2 equippedSize)
    {
        // Define this UI Item at creation
        equippedRectSize = new Vector2(equippedSize.x, equippedSize.y);
        EquippedRectOffset = new Vector2(-equippedSize.x / 2, equippedSize.y / 2);
        SetData(dataIn);
    }

    private void SetToBaseRectSize()
    {
        currentRectSize = baseRectSize;
        UpdateItem();
    }

    private void DetermineRectSize()
    {
        // If -1 -1 item is equipped and not on the inventory gird
        if (Spot == new Vector2Int(-1, -1))
            currentRectSize = equippedRectSize;
        else
            currentRectSize = baseRectSize;
    }

    private void UpdateItem()
    {
        //transform.localScale = Vector3.one;
        //rect.sizeDelta = currentRectSize;
        rect.sizeDelta = new Vector2(100f, 100f);
        image.sprite = data.Picture;
    }

    public void ResetPosition()
    {
        // Return the item to its home location
        transform.localPosition = homePosition;
        DetermineRectSize();
        UpdateItem();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        /*
        // Clicking item in inventory
        if (eventData.button == PointerEventData.InputButton.Left && Inputs.Instance.Controls.Player.Shift.IsPressed()) {
            // Shift Clicking item in inventory - Drop
            inventoryGrid.DropItem(this);
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Right) {
            // Right Clicking item in inventory - Equip
            if (data.itemType == ItemType.Equipable) {
                // Limit equip calls from being re-called to rapidly
                if (inventoryGrid.ClickTimerLimited)
                    return;
                StartCoroutine(inventoryGrid.ClickTimerLimiter());

                // try to equip
                inventoryGrid.RequestEquip(this);
            }
            else if (data.itemType == ItemType.Consumable) {
                // Right Clicking consumable in inventory - Consume

                // Sheck if full health allready
                if (PlayerStats.Instance.AtMaxHealth) {
                    HUDMessage.Instance.ShowMessage("Already at max health!");
                    return;
                }

                // Consume
                ConsumableData consumeData = (ConsumableData)data;
                PlayerStats.Instance.Consume(consumeData);

                // Remove the consumed item
                inventoryGrid.RemoveFromInventory(this);
                HUDMessage.Instance.ShowMessage("You regained some health!", false, SoundName.HUDPositive);
            }

        }*/
    }

    public void OnDrag(PointerEventData eventData)
    {

        // If not left button is held dont drag
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // Place dragged item unde cursor by an offset
        transform.position = eventData.position;// + offset;

        // Make the rect correct scale when dragged
        //SetToBaseRectSize();
    }

    public void SetParent(Transform p) => transform.SetParent(p);

    public void OnEndDrag(PointerEventData eventData)
    {

        // If not left button is held dont drag
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // Check if dropped position is a valid spot
        Vector2 drop = eventData.position + offset;

        // Check if mouse is above a UISlot that is a UI Image
        UISlot targetSlot = GetReleaseSlot(eventData);
        if(targetSlot == null) {
        
            Debug.Log("No slot here, return item to old position");
            DragObject.Instance.ReturnItem();
            return;
        }

        // Target is an UISlot
        UISlot originalSlot = DragObject.Instance.OriginalSlot();


        // Find out if transfers fail
        if(targetSlot is CharacterEquipSlot && ((CharacterEquipSlot)targetSlot).EquipmentType != data.EquipmentType){
            Debug.Log("((CharacterEquipSlot)targetSlot).EquipmentType ="+((CharacterEquipSlot)targetSlot).EquipmentType);
            Debug.Log("data.EquipmentType =" + data.EquipmentType);

            Debug.Log("Dragging into the equipmentSlot but using wrong itemtype");
            DragObject.Instance.ReturnItem();
            return;
        }else if(originalSlot is CharacterEquipSlot && targetSlot.HeldItem != null && ((CharacterEquipSlot)originalSlot).EquipmentType != targetSlot.HeldItem.data.EquipmentType) {
                Debug.Log("Dragging from equipmentSlot but swapping with wrong itemtype");
                DragObject.Instance.ReturnItem();
                return;
        }else if(targetSlot is SellSlot) {
            Debug.Log("Selling Item ");
            DragObject.Instance.SellItem();
                return;
        }
        // Handle releasing over valid Slot
        DragObject.Instance.MoveItemToSlot(targetSlot);
        
    }

    private UISlot GetReleaseSlot(PointerEventData eventData)
    {
        // Raycast UI elements under the mouse
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Select(r => r.gameObject.GetComponent<UISlot>()).FirstOrDefault(slot => slot != null);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        // If not left button is held dont drag
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // Define the offset
        offset = (Vector2)transform.position - eventData.position;

        // Set dragged item
        DragObject.Instance.SetDragedItem(this);
    }

    public bool IsInInventory() => Spot != new Vector2Int(-1, -1);

    public void SetNormalRectScale() => SetToBaseRectSize();

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Determine if there is one of these equipped already
        ItemData equipped = EquippedManager.Instance.GetEquipped(data.EquipmentType);

        // Update info for current item
        InfoPanelController.Instance.ShowInfo(data,equipped);
    }

    public void OnPointerExit(PointerEventData eventData) => InfoPanelController.Instance.HideInfo();
}

