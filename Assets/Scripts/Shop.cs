 using UnityEngine;
using UnityEngine.UI;
public class Shop : MonoBehaviour
{

    public RectTransform uiGroup;
    public Animator anim;

    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos;
    Player enterPlayer;
    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    // Update is called once per frame
    public void Exit()
    {
        anim.SetTrigger("DoHello");
        uiGroup.anchoredPosition = Vector3.down *1000;

    }

    public void Buy(int index){
        int price = itemPrice[index];
        GameMgr gamemgr = GameMgr.GetInstance;
        if(price > gamemgr.GetItemValue(Item.ItemType.ITEM_COIN)){
            
            
            return;
        }
        gamemgr.SetItem(Item.ItemType.ITEM_COIN,-price);
        Instantiate(itemObj[index], itemPos[index].position,itemPos[index].rotation);
        }
}
