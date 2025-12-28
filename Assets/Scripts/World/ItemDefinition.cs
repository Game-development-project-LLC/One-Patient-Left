using UnityEngine;

public enum ItemType
{
    Key,
    Consumable,
    Ammo,
    Quest,
    Misc
}

[CreateAssetMenu(menuName = "OnePatientLeft/Items/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id = "item_id";
    [SerializeField] private string displayName = "New Item";

    [Header("UI")]
    [SerializeField] private Sprite icon;
    [TextArea(2, 6)]
    [SerializeField] private string description;

    [Header("Gameplay")]
    [SerializeField] private ItemType type = ItemType.Misc;

    [Tooltip("If false: inventory can only hold 0/1 of this item.")]
    [SerializeField] private bool stackable = false;

    [Min(1)]
    [SerializeField] private int maxStack = 1;

    [Header("Optional: Use Effect")]
    [SerializeField] private ItemUseEffect useEffect;

    public string Id => id;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public string Description => description;
    public ItemType Type => type;
    public bool Stackable => stackable;
    public int MaxStack => stackable ? Mathf.Max(1, maxStack) : 1;
    public ItemUseEffect UseEffect => useEffect;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!stackable) maxStack = 1;
        if (stackable && maxStack < 1) maxStack = 1;
    }
#endif
}
