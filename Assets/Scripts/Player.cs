using System;
using System.Collections;
using System.ComponentModel.Design;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public GameObject m_pGrenadeObject;
    public Camera m_pFollowCamera = null;

    public int score;

    float hAxis;
    float vAxis;
    bool walkDown;
    bool jumpDown;
    bool itemDown;
    bool ReloadDown;

    bool fireDown;
    public bool GrenadeDown;
    bool isFireReady;
    float fireDelay;

    public bool[] swaps = {false,false,false};
    int iCurWeaponIndex= -1;

    bool isJump = false;
    bool isDodge = false;
    bool isSwapping = false;
    bool isReload = false;
    bool isBorder = false; //경계에 닿았는가 벽충돌문제
    bool isDamage = false;
    bool isShop = false;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigidBody;
    Animator anim;
    MeshRenderer[] meshs;

    GameObject m_pNearObject = null;
    public Weapon m_pEquipWeapon = null;



    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        //PlayerPrefs.SetInt("MaxScore", 1120);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interaction();
    }

    private void Grenade()
    {
        int HasGrenades = GameMgr.GetInstance.GetItemValue(Item.ItemType.ITEM_GRANADE); 

        if(HasGrenades < 0){
            return;
        }
        if(GrenadeDown && !isReload &&!isSwapping){
             Ray ray = m_pFollowCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit,100f)){
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 3f;

                GameObject instantGrenade = Instantiate(m_pGrenadeObject,transform.position,transform.rotation);
                Debug.Log("instantGrenade??");

                Rigidbody rigidGrenade =instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec,ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back *10, ForceMode.Impulse);

                GameMgr.GetInstance.SetItem(Item.ItemType.ITEM_GRANADE,-1);
                grenades[HasGrenades].SetActive(false);
            }
        }
    }

    void FreezeRotation(){
        rigidBody.angularVelocity = Vector3.zero;

    }

    void StopToWall(){
        Debug.DrawRay(transform.position, transform.forward * 5 , Color.green);
        isBorder = Physics.Raycast(transform.position,transform.forward,
                                    5f,LayerMask.GetMask("Wall"));

    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    private void Reload()
    {
        if(m_pEquipWeapon == null||m_pEquipWeapon.type == Weapon.Type.Melee)
            return;
        
        if(isReload || GameMgr.GetInstance.GetItemValue(Item.ItemType.ITEM_AMMO )==0)
            return;
        
        if(ReloadDown&& !isJump && !isDodge && !isSwapping && isFireReady){
            anim.SetTrigger("DoReload");
            isReload = true;
            Invoke("ReloadOut",0.5f);
        }
    }

    void ReloadOut(){
        int CurPlayerItemAmmo = GameMgr.GetInstance.GetItemValue(Item.ItemType.ITEM_AMMO);
        int reloadAmmo = CurPlayerItemAmmo < m_pEquipWeapon.maxAmmo ? CurPlayerItemAmmo : m_pEquipWeapon.maxAmmo;

        m_pEquipWeapon.curAmmo = reloadAmmo;
        GameMgr.GetInstance.SetItem(Item.ItemType.ITEM_AMMO,-reloadAmmo);

        isReload = false;
    }

    void Attack(){
        if(m_pEquipWeapon == null)
            return;


        fireDelay +=Time.deltaTime;
        isFireReady = m_pEquipWeapon.rate < fireDelay;
        if(!isShop && fireDown && isFireReady && !isDodge &&!isSwapping){
            m_pEquipWeapon.Use();
            anim.SetTrigger(m_pEquipWeapon.type == Weapon.Type.Melee? "DoSwing": "DoShot");
            fireDelay = 0f;
        }

    }
    void Swap()
    {
        int iWeaponIndex= -1;
        for(int i=0;i<3;++i ){
            if(swaps[i]){
                iWeaponIndex = i;
                if((iCurWeaponIndex ==i) || hasWeapons[i] == false)
                    return;
                break;
            }
        }
        if((swaps[0]||swaps[1]||swaps[2]) && !isJump && !isDodge){
            if(m_pEquipWeapon){
                m_pEquipWeapon.gameObject.SetActive(false);
                m_pEquipWeapon = null;
            }
            m_pEquipWeapon = weapons[iWeaponIndex].GetComponent<Weapon>();
            m_pEquipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("DoSwap");
            isSwapping = true;
            Invoke("SwapOut",0.3f);
            iCurWeaponIndex= iWeaponIndex;
        }

    }

    void SwapOut(){
        isSwapping = false;
    }

    void Interaction(){
        if(itemDown && m_pNearObject!= null && !isJump && !isDodge){
            if(m_pNearObject.tag == "Weapon"){
                Debug.Log("Interaction " + m_pNearObject.name);

                Item item = m_pNearObject.GetComponent<Item>();
                if(item.type == Item.ItemType.ITEM_GRANADE)
                    return;
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(m_pNearObject);
            }
            else if(m_pNearObject.tag == "Shop"){
                Shop shopscript = m_pNearObject.GetComponent<Shop>();
                shopscript.Enter(this);
                isShop = true;
            }

        }
    }

      private void Dodge()
    {
        if(!isJump && moveVec != Vector3.zero && jumpDown&& !isDodge){
            dodgeVec = moveVec;
            speed *= 2f;
            isDodge = true;
            anim.SetTrigger("DoDodge");

            Invoke("DodgeOut",0.5f);
        }

    }

    void DodgeOut(){
        speed *= 0.5f;
        isDodge = false;
    }

    private void Jump()
    {
        if(!isJump && moveVec == Vector3.zero && jumpDown && !isDodge){
            isJump = true;
            anim.SetBool("IsJump", true);
            anim.SetTrigger("DoJump");
            rigidBody.AddForce(Vector3.up * 15f,ForceMode.Impulse);
        }

    }

    private void Turn()
    {
        transform.LookAt(transform.position + moveVec);
        if(fireDown){
            Ray ray = m_pFollowCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit,100f)){
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = transform.position.y;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    private void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if(isDodge)
            moveVec = dodgeVec;

        if(isSwapping||isReload)
            moveVec = Vector3.zero;
        if(!isBorder)
            transform.position += moveVec * speed * (walkDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("IsRun", moveVec != Vector3.zero);
        anim.SetBool("IsWalk", walkDown);
    }

    private void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkDown = Input.GetButton("Walk");
        jumpDown = Input.GetButtonDown("Jump");
        fireDown = Input.GetButton("Fire1");
        GrenadeDown = Input.GetKeyDown(KeyCode.Mouse1);
        ReloadDown = Input.GetButton("Reload");
        itemDown = Input.GetButton("Interaction");
        for(int i=0; i<3; ++i){
            swaps[i] = Input.GetButton("Swap" + i);
        }

        // //Detect if the right mouse button is pressed
        // if (Input.GetKeyUp(KeyCode.Mouse1))
        // {
        //     Debug.Log("Mouse 1");
        // }

    }

    virtual public void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Floor")
        {
            anim.SetBool("IsJump", false);
            isJump = false;
        }
    }

    void OnCollisionStay(Collision other)
    {
        if(other.gameObject.tag == "Weapon")
        {
            m_pNearObject = other.gameObject;
            Debug.Log("Stay" + m_pNearObject.name);
        }
    }

    void OnCollisionExit(Collision other)
    {
        if(other.gameObject.tag == "Weapon")
        {
            Debug.Log("Exit " + m_pNearObject.name);
            m_pNearObject= null;

        }
    }

    void OnTriggerEnter(Collider other){
        if(other.tag =="Item"){
            Item item = other.GetComponent<Item>();
            GameMgr.GetInstance.SetItem(item.type,item.value);
            int TotalItemValue = GameMgr.GetInstance.GetItemValue(item.type);
            
            if((TotalItemValue >= 0)&&item.type == Item.ItemType.ITEM_GRANADE)
            {
                grenades[TotalItemValue].SetActive(true);
            }
            Destroy(item.gameObject);
        }
        else if(other.tag == "EnemyBullet"){
            if(!isDamage){
                Bullet enemyBullet = other.GetComponent<Bullet>();
                GameMgr.GetInstance.SetItem(Item.ItemType.ITEM_HEART,-enemyBullet.damage);

                bool isBossAttack = other.name == "Boss Melee Area";

                
                StartCoroutine(OnDamage(isBossAttack)); 

            }
            if(other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);            

        }   
    }

    void OnTriggerStay(Collider other){
        if(other.tag == "Weapon"|| other.tag == "Shop"){
            m_pNearObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other){
        if(other.tag == "Weapon"){
            m_pNearObject = null;
        }
        else if (other.tag == "Shop"){
            Shop shop = m_pNearObject.GetComponent<Shop>();
            shop.Exit();

            m_pNearObject = null;
            isShop =false;
        }
    }
    

    IEnumerator OnDamage(bool isBossAttack){
            isDamage = true;
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.red;
            }
            if(isBossAttack)
                rigidBody.AddForce(transform.forward * -25f,ForceMode.Impulse);

            yield return new WaitForSeconds(1f);
            isDamage = false;

            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.white;
            }
            if(isBossAttack)
                rigidBody.linearVelocity = Vector3.zero;

        }
}
