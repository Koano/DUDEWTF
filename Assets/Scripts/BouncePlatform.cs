using UnityEngine;
using System.Collections;


public class BouncePlatform: MonoBehaviour
{

    public float jumpness = 25;
    public Rigidbody player;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision obj)
    {
        
        {
            Debug.Log("Auf dem Jumppad");
            player.AddForce(new Vector3(0, 10f, 0) * jumpness);
        }
    }
}