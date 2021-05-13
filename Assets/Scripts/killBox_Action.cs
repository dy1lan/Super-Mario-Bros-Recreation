using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killBox_Action : MonoBehaviour
{
    private enemy parentScript;
    // Start is called before the first frame update
    void Start()
    {
        parentScript = transform.parent.GetComponent<enemy>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            parentScript.HitPlayer(collision);
        }
    }
}
