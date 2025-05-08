using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo{
        public int Ammo = 100;
        public int Coin = 4000;
        public int Health = 100;
        public int HasGrenades = -1;

        public int maxAmmo = 999;
        public int maxCoin = 99999;
        public int maxHealth = 100;
        public int maxHasGrenades = 4;
    }
    public PlayerInfo playerInfo;
    private static GameMgr m_pInstance = null;

    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject StartZone;

    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;


    public GameObject menuPanel;
    public GameObject gamePanel;
    public Text maxScoreText;
    public Text scoreText;
    public Text stageText;
    public Text playTimeText;
    public Text playerHealthText;
    public Text playerAmmoText;
    public Text playerCoinText;
     
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;

    public Text enemyAText;
    public Text enemyBText;
    public Text enemyCText;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    

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
            maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));  //string.Format �Լ��� ���ڿ� ��� ����

            
        }
        else
        {
            //���� �� �̵��� �Ǿ��µ� �� ������ Hierarchy�� GameMgr�� ������ ���� �ִ�.
            //�׷� ��쿣 ���� ������ ����ϴ� �ν��Ͻ��� ��� ������ִ� ��찡 ���� �� ����.
            //�׷��� �̹� ���������� instance�� �ν��Ͻ��� �����Ѵٸ� �ڽ�(���ο� ���� GameMgr)�� �������ش�.
            Destroy(this.gameObject);
        }
    }

    public void GameStart(){
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);    
    }

    void Update()
    {
        if(isBattle)
            playTime += Time.deltaTime;
    }

    public void StageStart(){
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        StartZone.SetActive(false);

        isBattle = true;
        StartCoroutine(InBattle());

    }
    public void StageEnd(){
        player.transform.position = Vector3.up * 0.8f;
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        StartZone.SetActive(true);
        ++stage;
        isBattle = false;

    }
    IEnumerator InBattle(){
        yield return new WaitForSeconds(5f);
        StageEnd();
    }


    void LateUpdate() //Ui �ȼ����� LateUpdate
    {
        //��� UI
        scoreText.text = string.Format("{0:n0}", player.score);
        stageText.text = "STAGE " + stage;

        int hour = (int)(playTime / 3600); //�ʴ��� �ð��� 3600, 60���� ������ �ú��ʷ� ���
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);

        playTimeText.text = string.Format("{0:00}", hour) + ":" 
                                        + string.Format("{0:00}", min) + ":" 
                                         + string.Format("{0:00}", second);

        //�÷��̾� UI
        playerHealthText.text = playerInfo.Health + " / " + playerInfo.maxHealth;
        playerCoinText.text = string.Format("{0:n0}", playerInfo.Coin);
        if (player.m_pEquipWeapon == null)
            playerAmmoText.text = "- / " + playerInfo.Ammo;
        else if (player.m_pEquipWeapon.type == Weapon.Type.Melee)
            playerAmmoText.text = "- / " + playerInfo.Ammo;
        else
            playerAmmoText.text = player.m_pEquipWeapon.curAmmo + " / " + playerInfo.Ammo;

        //���� UI
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, playerInfo.HasGrenades > 0 ? 1 : 0);
        
        //���� ���� UI
        enemyAText.text = enemyCntA.ToString();
        enemyBText.text = enemyCntB.ToString();
        enemyCText.text = enemyCntC.ToString();

        //���� ü�� UI �̹����� scale�� ���� ü�� ������ ���� ����
        //���� ������ ������� �� UI������Ʈ ���� �ʵ��� ���� �߰�    
        if (boss != null)
        {
            //bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;          
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
   
    public void SetItem(Item.ItemType eType, int itemValue){
        switch (eType)
            {
                case Item.ItemType.ITEM_AMMO:
                    playerInfo.Ammo += itemValue;
                    if(playerInfo.Ammo > playerInfo.maxAmmo)
                        playerInfo.Ammo = playerInfo.maxAmmo;
                    break;
                case Item.ItemType.ITEM_COIN:
                    playerInfo.Coin += itemValue;
                    if(playerInfo.Coin > playerInfo.maxCoin)
                        playerInfo.Coin = playerInfo.maxCoin;
                    break;
                case Item.ItemType.ITEM_HEART:
                    playerInfo.Health += itemValue;
                    if(playerInfo.Health > playerInfo.maxHealth)
                        playerInfo.Health = playerInfo.maxHealth;
                    break;                    
                case Item.ItemType.ITEM_GRANADE:
                    playerInfo.HasGrenades += itemValue;
                    if(playerInfo.HasGrenades > playerInfo.maxHasGrenades)
                        playerInfo.HasGrenades = playerInfo.maxHasGrenades;
                    break;
                default:
                    break;
            }

            
    }

    public int GetItemValue(Item.ItemType eType){
        switch (eType)
            {
                case Item.ItemType.ITEM_AMMO:
                    return playerInfo.Ammo;
                    
                case Item.ItemType.ITEM_COIN:
                    return playerInfo.Coin;

                case Item.ItemType.ITEM_HEART:
                    return playerInfo.Health;            
                case Item.ItemType.ITEM_GRANADE:
                    return playerInfo.HasGrenades;
                default:
                    return -1;
            }
    }

    
}
