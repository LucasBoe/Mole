using UnityEngine;

public interface IPlayerComponent
{
    //the bigger the number the earlier the update
    int UpdatePrio { get; }
    void UpdatePlayerComponent(PlayerContext context);
    void Init(PlayerContext context);

}

public class PlayerItemCollector : MonoBehaviour
{
    CollectablePlayerItemWorldObject playerItem;
    InputAction current = null;

    public void Init(PlayerContext context) { }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollectablePlayerItemWorldObject c = collision.GetComponent<CollectablePlayerItemWorldObject>();
        if (c != null)
        {
            //TODO: Move this to input action register look at PlayerAboveInputActionProvider
            current = new InputAction() { Text = "Take " + c.Item.name, Target = transform, Stage= InputActionStage.WorldObject, ActionCallback = TryCollect };
            PlayerInputActionRegister.Instance.RegisterInputAction(current);
            playerItem = c;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CollectablePlayerItemWorldObject c = collision.GetComponent<CollectablePlayerItemWorldObject>();
        if (c != null && PlayerInputActionRegister.Instance.UnregisterAllInputActions(transform)) playerItem = null;
    }

    private void TryCollect()
    {
        if (playerItem != null && PlayerItemHolder.Instance.AddItem(playerItem.Item))
        {
            if (current != null)
                PlayerInputActionRegister.Instance.UnregisterInputAction(current);

            playerItem.Collect();
        }
    }
}
