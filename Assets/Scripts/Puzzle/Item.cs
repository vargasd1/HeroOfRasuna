using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    //not attached to any item, but is found on objects in "Assets/Interactable Items". Change names there, then add the item into objects with ItemPickup.cs
    new public string name = "New Item";
    public Sprite icon = null;
}
