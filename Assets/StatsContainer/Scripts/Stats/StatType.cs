using UnityEngine;

/// <summary>
/// I'm using this scriptable object "stat type" class instead of enums. 
/// So you don't need to change the code if you need to add new types.
/// </summary>
[CreateAssetMenu]
public class StatType : ScriptableObject 
{
#if UNITY_EDITOR
    // Optional. To display an icon in the stats container.
    [field: SerializeField] public Sprite Icon { get; private set; }
#endif

    public StatType(string name) =>
        this.name = name;

    public static bool Compare(StatType o1, StatType o2)
    {
#if ADDRESSABLES
        // When you use Addressables, because of the specifics, 
        // it is best to compare by o1.name == o2.name because the same stat type can have different instance ids, 
        // especially if you serialize object references in the inspector.
        return o1.name == o2.name;
#else
        return o1 == o2;
#endif
    }
}
