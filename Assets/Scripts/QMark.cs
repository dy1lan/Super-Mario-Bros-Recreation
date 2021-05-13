using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QMark : MonoBehaviour
{

    [SerializeField] private GameObject[] items;		// Array of item prefabs.
    [SerializeField] private AudioClip coin;
    [SerializeField] private AudioClip powerUp;

    private Animator anim;
    private AudioSource aSource;
    private bool HasBeenHit = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        aSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        anim.SetBool("Hit", true);
        if(!HasBeenHit)
        {
            Spawn();
            HasBeenHit = true;
        }


        //can't get the question mark to bounce.

        //Vector3 temp = transform.position;
        //Vector3 newPos = transform.position;
        //newPos.y += 10;
        //temp.y -= 8;
        //transform.position = new Vector3(transform.position.x, Mathf.PingPong(Time.time * 2, newPos.y) + transform.position.y, transform.position.z);
        //transform.position = temp;
        //transform.position = Vector3.Lerp(transform.position, newPos, smooth * Time.deltaTime);
        //transform.position = Vector3.Lerp(transform.position, temp, smooth * Time.deltaTime);
    }

    /// <summary>
    /// Spawn: Called when the Player hits the Question Mark from underneath, this Spawns a new instance of an item.
    /// </summary>
    void Spawn()
    {
        // Instantiate a random enemy.
        int enemyIndex = Random.Range(0, items.Length);
        Instantiate(items[enemyIndex], transform.position, transform.rotation);

        if(items[enemyIndex].name == "coin")
        {
            aSource.PlayOneShot(coin);
            PlayerMovement m = GameObject.Find("Player").GetComponent<PlayerMovement>();
            m.CoinCount += 1;
            m.ScoreCount += 100;
        }
        else if (items[enemyIndex].name == "mushroom")
        {
            aSource.PlayOneShot(powerUp);
        }
    }

}
