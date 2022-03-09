using System;
using System.Collections;
using UnityEngine;

public interface IPlayerComponent
{
    //the bigger the number the earlier the update
    int UpdatePrio { get; }
    void UpdatePlayerComponent(PlayerContext context);
    void Init(PlayerContext context);

}

public class PlayerItemCollector : PlayerBehaviour
{

    [SerializeField] private Collider2D trigger;
    [SerializeField, ReadOnly] private CollectablePlayerItemWorldObject playerItem;
    InputAction current = null;

    public void Start()
    {
        PlayerItemHolder.AddedItem += OnItemChanged;
        PlayerItemHolder.RemovedItem += OnItemChanged;
        PlayerItemHolder.Changedtem += OnItemChanged;
    }

    private void OnItemChanged(PlayerItem item)
    {
        UpdateTrigger();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollectablePlayerItemWorldObject c = collision.GetComponent<CollectablePlayerItemWorldObject>();
        if (c != null)
        {
            //TODO: Move this to input action register look at PlayerAboveInputActionProvider
            current = new InputAction() { Text = "Take " + c.Item.name, Target = transform, Stage = InputActionStage.WorldObject, ActionCallback = TryCollect };
            PlayerInputActionRegister.Instance.RegisterInputAction(current);
            playerItem = c;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CollectablePlayerItemWorldObject c = collision.GetComponent<CollectablePlayerItemWorldObject>();
        if (c != null && PlayerInputActionRegister.Instance.UnregisterAllInputActions(transform)) playerItem = null;
    }

    private void UpdateTrigger()
    {
        Vector2 offset = trigger.offset;
        trigger.enabled = false;
        trigger.offset = Vector2.down;
        trigger.enabled = true;
        trigger.offset = offset;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            UpdateTrigger();
    }

    private void TryCollect()
    {
        CollectablePlayerItemWorldObject itemObject = playerItem;
        if (itemObject != null && PlayerItemHolder.Instance.AddItem(itemObject.Item))
        {
            if (current != null)
                PlayerInputActionRegister.Instance.UnregisterInputAction(current);

            itemObject.Collect();
        }
    }
}
