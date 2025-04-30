using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType {ITEM_AMMO, ITEM_COIN, ITEM_GRANADE, ITEM_HEART, ITEM_WEAPON,ITEM_END};
    
    public ItemType type;
    public int value;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
