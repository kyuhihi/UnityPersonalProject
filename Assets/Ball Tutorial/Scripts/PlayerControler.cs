// Removed System.Numerics to avoid ambiguity with UnityEngine.Vector3
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


public class PlayerControler : MonoBehaviour
{

    private Rigidbody PlayerRigidbody;
    public TextMeshProUGUI countText;
    private float speed = 10f;

    
    private int count = 0;

    private UnityEngine.Vector2 movementInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerRigidbody = GetComponent<Rigidbody>();
        SetCountText();
    }

    void OnMove(InputValue movementValue){
        movementInput = movementValue.Get<UnityEngine.Vector2>();
        
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
    }


    void FixedUpdate()
    {
        UnityEngine.Vector3 movement = new UnityEngine.Vector3(movementInput.x, 0f, movementInput.y);
        PlayerRigidbody.AddForce(movement * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            ++count;
            SetCountText();

        }
    }

}
