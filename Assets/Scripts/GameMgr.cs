using Unity.VisualScripting;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    //게임매니저의 인스턴스를 담는 전역변수(static 변수이지만 이해하기 쉽게 전역변수라고 하겠다).
    //이 게임 내에서 게임매니저 인스턴스는 이 instance에 담긴 녀석만 존재하게 할 것이다.
    //보안을 위해 private으로.

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
            //이 클래스 인스턴스가 탄생했을 때 전역변수 instance에 게임매니저 인스턴스가 담겨있지 않다면, 자신을 넣어준다.
            m_pInstance = this;

            //씬 전환이 되더라도 파괴되지 않게 한다.
            //gameObject만으로도 이 스크립트가 컴포넌트로서 붙어있는 Hierarchy상의 게임오브젝트라는 뜻이지만, 
            //나는 헷갈림 방지를 위해 this를 붙여주기도 한다.
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            //만약 씬 이동이 되었는데 그 씬에도 Hierarchy에 GameMgr이 존재할 수도 있다.
            //그럴 경우엔 이전 씬에서 사용하던 인스턴스를 계속 사용해주는 경우가 많은 것 같다.
            //그래서 이미 전역변수인 instance에 인스턴스가 존재한다면 자신(새로운 씬의 GameMgr)을 삭제해준다.
            Destroy(this.gameObject);
        }
    }

    //게임 매니저 인스턴스에 접근할 수 있는 프로퍼티. static이므로 다른 클래스에서 맘껏 호출할 수 있다.
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
