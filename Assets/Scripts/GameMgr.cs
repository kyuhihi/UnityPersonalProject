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
            //이 클래스 인스턴스가 탄생했을 때 전역변수 instance에 게임매니저 인스턴스가 담겨있지 않다면, 자신을 넣어준다.
            m_pInstance = this;

            //씬 전환이 되더라도 파괴되지 않게 한다.
            //gameObject만으로도 이 스크립트가 컴포넌트로서 붙어있는 Hierarchy상의 게임오브젝트라는 뜻이지만, 
            //나는 헷갈림 방지를 위해 this를 붙여주기도 한다.
            DontDestroyOnLoad(this.gameObject);
            maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));  //string.Format 함수로 문자열 양식 적용

            
        }
        else
        {
            //만약 씬 이동이 되었는데 그 씬에도 Hierarchy에 GameMgr이 존재할 수도 있다.
            //그럴 경우엔 이전 씬에서 사용하던 인스턴스를 계속 사용해주는 경우가 많은 것 같다.
            //그래서 이미 전역변수인 instance에 인스턴스가 존재한다면 자신(새로운 씬의 GameMgr)을 삭제해준다.
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


    void LateUpdate() //Ui 안성맞춤 LateUpdate
    {
        //상단 UI
        scoreText.text = string.Format("{0:n0}", player.score);
        stageText.text = "STAGE " + stage;

        int hour = (int)(playTime / 3600); //초단위 시간을 3600, 60으로 나누어 시분초로 계산
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);

        playTimeText.text = string.Format("{0:00}", hour) + ":" 
                                        + string.Format("{0:00}", min) + ":" 
                                         + string.Format("{0:00}", second);

        //플레이어 UI
        playerHealthText.text = playerInfo.Health + " / " + playerInfo.maxHealth;
        playerCoinText.text = string.Format("{0:n0}", playerInfo.Coin);
        if (player.m_pEquipWeapon == null)
            playerAmmoText.text = "- / " + playerInfo.Ammo;
        else if (player.m_pEquipWeapon.type == Weapon.Type.Melee)
            playerAmmoText.text = "- / " + playerInfo.Ammo;
        else
            playerAmmoText.text = player.m_pEquipWeapon.curAmmo + " / " + playerInfo.Ammo;

        //무기 UI
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, playerInfo.HasGrenades > 0 ? 1 : 0);
        
        //몬스터 숫자 UI
        enemyAText.text = enemyCntA.ToString();
        enemyBText.text = enemyCntB.ToString();
        enemyCText.text = enemyCntC.ToString();

        //보스 체력 UI 이미지의 scale을 남을 체력 비율에 따라 변경
        //보스 변수가 비어있을 때 UI업데이트 하지 않도록 조건 추가    
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
