using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/SpriteDictionaryObject")]
/// <summary>
/// Functions:
/// This class takes a list of sprites and strings, sets a dictionary using the list.
/// The key is set with the string attached to each sprite
/// </summary>
public class SpriteDictionaryObject : ListToDictionaryObject<string, SpriteDictionaryObject.KeyValuePair>
{
    [System.Serializable]
    public struct KeyValuePair
    {
        [SerializeField] private string _name;
        [SerializeField] private Sprite _sprite;

        public string Name => _name;
        public Sprite Sprite => _sprite;
    }

    protected override string GetKey(KeyValuePair value)
    {
        return value.Name;
    }

    public override bool TryGetValue(string key, out KeyValuePair value)
    {

       if  (!base.TryGetValue(key, out value))
       {
           return base.TryGetValue("default", out value);
       }

       return base.TryGetValue(key, out value);
    }
}