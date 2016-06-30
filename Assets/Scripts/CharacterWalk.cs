using UnityEngine;
using System.Collections;

public class CharacterWalk : MonoBehaviour
{

    //Movement
    private float moveValue = 10f;
    public float moveSpeed;
    public float flightmoveSpeed;
    private float sprintSpeed = 1;
    public float sprintValue = 2f;
    //Jump
    public float jumppower;
    public Rigidbody player;
    //Grounded
    public bool grounded;


    // Update is called once per frame
    void Update()
    {

        float h = Input.GetAxis("Horizontal");

        moveSpeed = moveValue * sprintSpeed;
       
        // Bewegung


        if (grounded == true)
        {
            transform.Translate(new Vector3(h, 0, 0) * Time.deltaTime * moveSpeed);
        }
        if  (grounded == false)
        {
            transform.Translate(new Vector3(h, 0, 0) * Time.deltaTime * moveSpeed * flightmoveSpeed );
        }

        if (Input.GetButtonDown("Jump") && grounded == true)
        {
            player.AddForce(new Vector3(0, 100f, 0) * jumppower);
            grounded = false;
        }
        if (Input.GetButton("Fire3"))
        {
            sprintSpeed = sprintValue;
        }
        else
            sprintSpeed = 1;
    }
    void OnCollisionStay(Collision ground)
    {
        if (ground.gameObject.CompareTag("Platform"))
        {
        grounded = true;
        }
    }
}

    