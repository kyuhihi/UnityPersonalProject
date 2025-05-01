using Unity.VisualScripting;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    //���ӸŴ����� �ν��Ͻ��� ��� ��������(static ���������� �����ϱ� ���� ����������� �ϰڴ�).
    //�� ���� ������ ���ӸŴ��� �ν��Ͻ��� �� instance�� ��� �༮�� �����ϰ� �� ���̴�.
    //������ ���� private����.

    public int Ammo;
    public int Coin;
    public int Health;
    public int HasGrenades;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;


     private static GameMgr m_pInstance = null;

    void Awake()
    {
        if (null == m_pInstance)
        {
            //�� Ŭ���� �ν��Ͻ��� ź������ �� �������� instance�� ���ӸŴ��� �ν��Ͻ��� ������� �ʴٸ�, �ڽ��� �־��ش�.
            m_pInstance = this;

            //�� ��ȯ�� �Ǵ��� �ı����� �ʰ� �Ѵ�.
            //gameObject�����ε� �� ��ũ��Ʈ�� ������Ʈ�μ� �پ��ִ� Hierarchy���� ���ӿ�����Ʈ��� ��������, 
            //���� �򰥸� ������ ���� this�� �ٿ��ֱ⵵ �Ѵ�.
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            //���� �� �̵��� �Ǿ��µ� �� ������ Hierarchy�� GameMgr�� ������ ���� �ִ�.
            //�׷� ��쿣 ���� ������ ����ϴ� �ν��Ͻ��� ��� ������ִ� ��찡 ���� �� ����.
            //�׷��� �̹� ���������� instance�� �ν��Ͻ��� �����Ѵٸ� �ڽ�(���ο� ���� GameMgr)�� �������ش�.
            Destroy(this.gameObject);
        }
    }

    //���� �Ŵ��� �ν��Ͻ��� ������ �� �ִ� ������Ƽ. static�̹Ƿ� �ٸ� Ŭ�������� ���� ȣ���� �� �ִ�.
    public static GameMgr GetInstance
    {
        get
        {
            if (null == m_pInstance)
            {
                return null;
            }
            return m_pInstance;
        }
    }
   
    public void GetItem(Item.ItemType eType, int itemValue, Item item){
        switch (eType)
            {
                case Item.ItemType.ITEM_AMMO:
                    Ammo += itemValue;
                    if(Ammo > maxAmmo)
                        Ammo = maxAmmo;
                    break;
                case Item.ItemType.ITEM_COIN:
                    Coin += itemValue;
                    if(Coin > maxCoin)
                        Coin = maxCoin;
                    break;
                case Item.ItemType.ITEM_HEART:
                    Health += itemValue;
                    if(Health > maxHealth)
                        Health = maxHealth;
                    break;                    
                case Item.ItemType.ITEM_GRANADE:
                    HasGrenades += itemValue;
                    if(HasGrenades > maxHasGrenades)
                        HasGrenades = maxHasGrenades;
                    break;
                default:
                    break;
            }

            Destroy(item.gameObject);
    }

    public int GetItemValue(Item.ItemType eType){
        switch (eType)
            {
                case Item.ItemType.ITEM_AMMO:
                    return Ammo;
                    
                case Item.ItemType.ITEM_COIN:
                    return Coin;

                case Item.ItemType.ITEM_HEART:
                    return Health;            
                case Item.ItemType.ITEM_GRANADE:
                    return HasGrenades;
                default:
                    return -1;
            }
    }

    public void InitGame()
    {

    }

    public void PauseGame()
    {

    }

    public void ContinueGame()
    {

    }

    public void RestartGame()
    {

    }

    public void StopGame()
    {

    }
}
