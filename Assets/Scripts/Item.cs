using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType {ITEM_AMMO, ITEM_COIN, ITEM_GRANADE, ITEM_HEART, ITEM_WEAPON,ITEM_END};
    
    public ItemType type;
    public int value;
    
    const float m_fRotateSpeed = 20f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * m_fRotateSpeed * Time.deltaTime);

    }
}
