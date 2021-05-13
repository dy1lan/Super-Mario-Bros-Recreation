using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class item : MonoBehaviour
{
    [SerializeField] string itemName;
    [SerializeField] private AudioClip aPowerup_ate;
    [SerializeField] private AudioClip aCoin;
    public Font pointFont;
    bool Destroyed = false;
    private AudioSource aSource;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 newPOS = transform.position;
        newPOS.y += 5;
        transform.position = Vector3.Lerp(transform.position, newPOS, 10 * Time.deltaTime);

        if (itemName == "Coin")
        {
            StartCoroutine(WaitAndDie());
        }
        if(gameObject.GetComponent<AudioSource>() != null)
        {
            aSource = GetComponent<AudioSource>();
        }
    }

    IEnumerator WaitAndDie()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !Destroyed)
        {
            PlayerMovement ps = col.GetComponent<PlayerMovement>();
            if (itemName == "FloatingCoin")
            {
                GameObject.Find("BackgroundAudio").GetComponent<background>().PlaySFX("coin");
                ps.CoinCount += 1;
                ps.ScoreCount += 200;
            }
            if(itemName == "Flower")
            {
                ps.ScoreCount += 1000;
            }
            if(itemName == "Mushroom")
            {
                aSource.PlayOneShot(aPowerup_ate);
                ps.ScoreCount += 1000;
            }
            Destroy(gameObject);
            Destroyed = true;
        }
    }
}
