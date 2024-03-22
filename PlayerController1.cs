using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controlador1 : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed = 10;
    public float rotationSpeed = 1.0f;
    public float jumpForce = 15;

    private bool isJumping;
    private bool isGrounded;

    private Vector3 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    void Update()
    {
       
    //Define the speed at which the object moves.

        float horizontalInput = Input.GetAxis("Horizontal");

        float verticalInput = Input.GetAxis("Vertical");

        rb.AddForce(new Vector3(horizontalInput, 0, verticalInput) * moveSpeed * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.Space)){
            Debug.Log("Espacio pulsado");
            //Physics.AddForce(new Vector3(), ForceMode.Impulse);//Del tutorial
            rb.AddForce(new Vector3(0,jumpForce,0), ForceMode.Impulse);
            isJumping = true;
        }
        if(rb.velocity.y<0 && isJumping){
            isJumping = false;
            rb.AddForce(new Vector3(0,0,0), ForceMode.Impulse);
        }
    }

    void onTriggerEnter (Collider col){
        if(col.CompareTag("Finish")){

        }
        if(col.CompareTag("Enemy")){
            transform.position = startPosition;
        }
    }
}
