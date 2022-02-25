using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IUpdateMeWithInput
{
    void UpdateUI(PlayerInput input);
}

public class PlayerItemUI : UIBehaviour
{
    [SerializeField] private List<ItemSlot> itemSlots = new List<ItemSlot>();
    [SerializeField] private int selectedItemSlotIndex = 0;
    private Image currentSelectionImage;

    [SerializeField] RectTransform parent;
    [SerializeField] Text itemNameText;

    [SerializeField] RectTransform itemSlotPrefab;
    [SerializeField] Image itemSlotModePrefab;

    InputAction ac_useItem, ac_deselectItem, ac_stopUsing, ac_confirmUsage;
    private void Start()
    {
        UIHandler.Instance.Show(this);
        PlayerItemHolder.OnAddNewItem += OnAddItem;
        PlayerItemHolder.OnRemoveItem += OnRemoveItem;

        ac_useItem = new InputAction() { Stage = InputActionStage.ModeSpecific, Target = transform, Input = ControlType.Use, Text = "Use Item", ActionCallback = TryUseItem };
        ac_deselectItem = new InputAction() { Stage = InputActionStage.ModeSpecific, Target = transform, Input = ControlType.Back, Text = "Hide Item", ActionCallback = DeselectItem };
        ac_stopUsing = new InputAction() { Stage = InputActionStage.ModeSpecific, Target = transform, Input = ControlType.Back, Text = "Stop", ActionCallback = StopUse };
        ac_confirmUsage = new InputAction() { Stage = InputActionStage.ModeSpecific, Target = transform, Input = ControlType.Interact, Text = "Confirm", ActionCallback = ConfirmUse };
    }
    private void SelectItem(PlayerItem item)
    {
        Debug.LogWarning("SelectItem: " + item);
        PlayerInputActionRegister.Instance.UnregisterAllInputActions(transform);
        PlayerItemUser.Instance.OverrideSelectedItem(item, drop: false);

        if (item != null)
        {
            PlayerInputActionRegister.Instance.RegisterInputAction(ac_useItem);
            PlayerInputActionRegister.Instance.RegisterInputAction(ac_deselectItem);
            SetSelectionIndicatorAlpha(1f);
        }
    }

    private void DeselectItem()
    {
        PlayerItemUser.Instance.Stop();
        SelectItem(null);
        SetSelectionIndicatorAlpha(0.1f);
    }

    private void TryUseItem()
    {
        PlayerInputActionRegister.Instance.UnregisterAllInputActions(transform);
        PlayerItemUser.Instance.Use(itemSlots[selectedItemSlotIndex].Item);
        PlayerInputActionRegister.Instance.RegisterInputAction(ac_confirmUsage);
        PlayerInputActionRegister.Instance.RegisterInputAction(ac_stopUsing);
    }

    private void StopUse()
    {
        PlayerItemUser.Instance.Stop();
        SelectItem(itemSlots[selectedItemSlotIndex].Item);
    }

    private void ConfirmUse()
    {
        PlayerItemUser.Instance.Confirm();
    }


    private void OnAddItem(PlayerItem item, bool forceSelection)
    {
        ItemSlot newSlot = new ItemSlot() { Item = item, Modes = item.GetItemModes() };
        itemSlots.Add(newSlot);
        CreateInstancesForSlot(newSlot);

        if (forceSelection)
            selectedItemSlotIndex = itemSlots.IndexOf(newSlot);

        UpdateSelectedItem();
    }
    private void OnRemoveItem(PlayerItem item)
    {
        for (int i = itemSlots.Count - 1; i >= 0; i--)
        {
            ItemSlot slot = itemSlots[i];

            if (slot.Item == item)
            {
                Destroy(slot.RectInstance.gameObject);
                itemSlots.RemoveAt(i);
            }
        }

        UpdateSelectedItem();
        DeselectItem();
    }
    private void SetSelectionIndicatorAlpha(float alpha)
    {
        if (currentSelectionImage != null)
            currentSelectionImage.color = new Color(255, 255, 255, alpha);
    }

    private void CreateUIElementsForAllItems()
    {
        foreach (ItemSlot slot in itemSlots)
        {
            CreateInstancesForSlot(slot);
        }
    }

    private void CreateInstancesForSlot(ItemSlot slot)
    {
        slot.RectInstance = Instantiate(itemSlotPrefab, parent);

        foreach (ItemMode mode in slot.Modes)
        {
            Image selectedImage = Instantiate(itemSlotModePrefab, slot.RectInstance);
            Image iconImage = selectedImage.GetComponentInChildrenExcludeOwn<Image>();
            mode.IconImage = iconImage;
            mode.SelectedImage = selectedImage;
            iconImage.sprite = mode.Icon;
        }
    }

    private void UpdateSelectedItem()
    {
        if (itemSlots.Count == 0)
        {
            //clear selected item
            SelectItem(null);
            return;
        }
        else
        {
            //prevent overflow when old index is outside range
            selectedItemSlotIndex = Mathf.Min(selectedItemSlotIndex, itemSlots.Count - 1);
        }

        Debug.LogWarning(selectedItemSlotIndex);

        SelectItem(itemSlots[selectedItemSlotIndex].Item);

        //visuals
        for (int i = 0; i < itemSlots.Count; i++)
        {
            ItemSlot slot = itemSlots[i];


            int index = i < selectedItemSlotIndex ? selectedItemSlotIndex + i : i - selectedItemSlotIndex;
            slot.RectInstance.SetSiblingIndex(index);

            float alpha = Mathf.Lerp(1f, 0f, index / 3f);

            for (int j = 0; j < slot.Modes.Length; j++)
            {
                ItemMode mode = slot.Modes[j];
                bool isSelected = i == selectedItemSlotIndex && j == slot.SelectedModeIndex;
                if (isSelected) currentSelectionImage = mode.SelectedImage;
                mode.SelectedImage.enabled = isSelected;
                mode.IconImage.color = new Color(1, 1, 1, alpha * (isSelected ? 1f : 0.6f));

                if (isSelected)
                    itemNameText.text = mode.Name;
            }
        }
    }

    public override void Show()
    {
        base.Show();

        CreateUIElementsForAllItems();
        UpdateSelectedItem();
    }

    public override void Hide()
    {
        //do never hide
    }

    public override void UpdateUI(PlayerInput input)
    {
        if (itemSlots.Count == 0 || PlayerItemUser.Instance.IsAiming)
            return;

        bool update = false;
        ItemSlot slot = itemSlots[selectedItemSlotIndex];

        if (input.ItemMenuUp)
        {
            if (selectedItemSlotIndex == 0)
                selectedItemSlotIndex = itemSlots.Count - 1;
            else
                selectedItemSlotIndex--;

            update = true;
        }
        else if (input.ItemMenuDown)
        {
            if (selectedItemSlotIndex == itemSlots.Count - 1)
                selectedItemSlotIndex = 0;
            else
                selectedItemSlotIndex++;

            update = true;
        }
        else if (slot.Modes.Length > 1)
        {
            if (input.ItemMenuLeft)
            {
                if (slot.SelectedModeIndex == 0)
                    slot.SelectedModeIndex = slot.Modes.Length - 1;
                else
                    slot.SelectedModeIndex--;

                update = true;
            }
            else if (input.ItemMenuRight)
            {
                if (slot.SelectedModeIndex == slot.Modes.Length - 1)
                    slot.SelectedModeIndex = 0;
                else
                    slot.SelectedModeIndex++;

                update = true;
            }
        }

        if (update)
            UpdateSelectedItem();
    }
}

[System.Serializable]
public class ItemSlot
{
    public PlayerItem Item;
    public ItemMode[] Modes;
    public int SelectedModeIndex;
    public RectTransform RectInstance;
}

[System.Serializable]
public class ItemMode
{
    public Sprite Icon;
    public string Name;
    public Image IconImage;
    public Image SelectedImage;
}
