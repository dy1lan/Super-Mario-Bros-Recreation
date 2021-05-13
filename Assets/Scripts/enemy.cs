using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
	public float moveSpeed = 2f;        // The speed the enemy moves at.
	public Sprite deadEnemy;            // A sprite of the enemy when it's dead.
	public AudioClip aBump;
	public AudioClip aStomp;

	private Transform frontCheck;       // Reference to the position of the gameobject used for checking if something is in front.
	private bool Destroyed = false;
	private Animator anim;
	private AudioSource aSource;
	private bool HitTrigger = false;

    // Start is called before the first frame update
    void Awake()
    {
		// Setting up the references.
		HitTrigger = false;
		frontCheck = transform.Find("frontCheck").transform;
		anim = GetComponent<Animator>();
		aSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -20)
        {
			Destroy(gameObject);
        }
    }

	void FixedUpdate()
	{
		Vector3 ps = GameObject.Find("Player").transform.position;
		float distToPlayer = transform.position.x - ps.x;

		if (distToPlayer > 50f)
        {
			return;
        }
		// Create an array of all the colliders in front of the enemy.
		Collider2D[] frontHits = Physics2D.OverlapCircleAll(frontCheck.position, .2f);

		// Check each of the colliders.
		foreach (Collider2D c in frontHits)
		{
			// If any of the colliders is an Obstacle...
			if (c.name == "Sm_Pipe" || 
				c.name == "Med_Pipe" || 
				c.name == "Lg_Pipe" || 
				(c.CompareTag("Enemy") && c.gameObject != gameObject && c.gameObject.transform.parent.gameObject != gameObject) || 
				c.CompareTag("Stairs"))   //(c.CompareTag("Obstacle"))
			{
				// ... Flip the enemy and stop checking the other colliders.
				Flip();
				break;
			}
		}

		// Set the enemy's velocity to moveSpeed in the x direction.
		GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player") && !Destroyed)
		{
			HitTrigger = true;
			aSource.PlayOneShot(aStomp);
			PlayerMovement ps = col.GetComponent<PlayerMovement>();
			anim.Play("goomba_flat");
			ps.ScoreCount += 100;
			Destroy(gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
			Destroyed = true;
		}
	}
	/// <summary>
	/// HitPlayer: Called if the Goomba hit a Player object and proceeds to destroy it.
	/// <param name="col">Player Objects collider</param>
	/// </summary>
	public void HitPlayer(Collision2D col)
    {
		if (!HitTrigger)
        {
			background b = GameObject.Find("BackgroundAudio").GetComponent<background>();
			b.StopBackgroundMusic();
			aSource.PlayOneShot(aBump);
			col.gameObject.GetComponent<PlayerMovement>().allowMove = false;
			col.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			col.gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
			Destroy(col.gameObject.GetComponent<CircleCollider2D>());
			Destroy(col.gameObject.GetComponent<BoxCollider2D>());
			StartCoroutine(WaitForRestart(col));
        }
    }



    public void Flip()
	{
		// Multiply the x component of localScale by -1.
		Vector3 enemyScale = transform.localScale;
		enemyScale.x *= -1;
		transform.localScale = enemyScale;
	}

	IEnumerator WaitForRestart(Collision2D col)
	{
		yield return new WaitForSeconds(aBump.length);
		if (col.gameObject.GetComponent<PlayerMovement>() != null)
			col.gameObject.GetComponent<PlayerMovement>().RestartGame();
    }
}
