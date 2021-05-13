using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Post_action : MonoBehaviour
{

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        anim.SetFloat("Hit", 9f);
        //anim.Play("Post_hit");

        if (col.GetComponent<PlayerMovement>() != null)
        {
            PlayerMovement player = col.GetComponent<PlayerMovement>();
            player.allowMove = false;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        }
        //anim.Play("Post_down");
        //col.GetComponent<Animator>().SetBool("Finish", true);
        //anim.SetBool("Done", true);

    }
}
