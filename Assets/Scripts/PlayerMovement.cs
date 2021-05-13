using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

/*
 * notes;
 * 50 points for each second they have left.
 * 5000 point for reaching the end
 * 100 points for killing an enemy
 * 1000 points for eating a mushroom or flower
 */

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private Animator animator;

    [SerializeField] private float runSpeed = 50f;
    [SerializeField] private Text m_points;
    [SerializeField] private Text m_coins;
    [SerializeField] private Text m_time;
    [SerializeField] private Text m_lives;
    [SerializeField] private AudioClip aJump;
    [SerializeField] private AudioClip aDie;
    [SerializeField] private AudioClip aGameOver;
    [SerializeField] private AudioClip aFlag;
    [SerializeField] private AudioClip aStageClear;



    private AudioSource aSource;
    private Rigidbody2D rbody;
    float horizontalMove = 0.0f;
    public bool allowMove = true;
    bool jump = false;
    public int ScoreCount = 0;
    public int time = 400;
    public int CoinCount = 0;
    int Previous_Score = 0;
    int livesScore = 0;
    int Num_Of_Lives = 3;
    bool inReset = false;
    bool StopTimer = false;


    private void Start()
    {
        aSource = GetComponent<AudioSource>();
        Previous_Score = 0;
        if (DataSave.Instance.CoinCount_bak > 0) CoinCount = DataSave.Instance.CoinCount_bak;
        if (DataSave.Instance.Num_Of_Lives_bak > 0) Num_Of_Lives = DataSave.Instance.Num_Of_Lives_bak;
        ScoreCount = 0;
        time = 400;
        inReset = false;
        StopTimer = false;
        StartCoroutine(StartTime());
    }

    // Update is called once per frame
    void Update()
    {
        Backup();
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (allowMove) { animator.SetFloat("Speed", Mathf.Abs(horizontalMove)); }
        
        if (Input.GetButtonDown("Jump")){
            aSource.PlayOneShot(aJump);
            jump = true;
            animator.SetBool("Jump", true);
        }

        if (transform.position.y < -20 && !inReset){ RestartGame(); } // restart game

        if (CoinCount < 10)  { m_coins.text = "Coins: x0" + CoinCount; }
        if (CoinCount >= 10) { m_coins.text = "Coins: x" + CoinCount; }

        if (Previous_Score != ScoreCount)
        {   
            livesScore += ScoreCount - Previous_Score;
            if (Mathf.Floor(livesScore / 2000) >= 1)
            {
                livesScore -= 2000;
                Num_Of_Lives++;
            }
            UpdateScore();
        }
        m_lives.text = "Lives:\n" + Num_Of_Lives.ToString();
    }
    private void FixedUpdate()
    {
        //move our character
        if (allowMove)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
            jump = false;
        }
    }

    /// <summary>
    /// UpdateScore: Updates the Score constantly, Can be called by any function or Script.
    /// </summary>
    public void UpdateScore()
    {
        int Sc_Len = ScoreCount.ToString().Length;
        int z_len = 6 - Sc_Len;
        string score = "";
        for (int i = 0; i < z_len; i++)
        {
            score += "0";
        }
        score += ScoreCount;
        m_points.text = score;
        Previous_Score = ScoreCount;
    }
    /// <summary>
    /// UpdateTime: Updates the Time constantly, Can be called by any function or Script.
    /// </summary>
    public void UpdateTime()
    {
        m_time.text = "Time\n" + time;
    }

    /// <summary>
    /// Backup: Backs up the CoinCount and Num_Of_Lives variables.
    /// </summary>
    public void Backup()
    {
        DataSave.Instance.CoinCount_bak = CoinCount; //back up incase player dies.
        DataSave.Instance.Num_Of_Lives_bak = Num_Of_Lives; //backup incase player dies.
    }

    /// <summary>
    /// OnLanding: Called by Invoke when player hits the ground.
    /// </summary>
    public void OnLanding()
    {
        animator.SetBool("Jump", false);
    }

    /// <summary>
    /// RestartGame: Restarts the game if the Player dies.
    /// </summary>
    public void RestartGame()
    {
        StopTimer = true;
        GameObject.Find("BackgroundAudio").GetComponent<background>().StopBackgroundMusic();
        inReset = true;
        Num_Of_Lives--;
        Backup();
        if (Num_Of_Lives <= 0)
        {
            //Game Over
            EndGame();
            return;
        }
        aSource.PlayOneShot(aDie);
        StartCoroutine(WaitForClipAndReset());
    }
    /// <summary>
    /// StopCharacter: Sets the Characters velocity to zero and doesn't allow it to move.
    /// </summary>
    public void StopCharacter()
    {
        allowMove = false;
        rbody.velocity = Vector2.zero;
        rbody.angularVelocity = 0f;
    }

    /// <summary>
    /// EndGame: Quits the Application.
    /// </summary>
    public void EndGame()
    {
        aSource.PlayOneShot(aGameOver);
        Application.Quit();
    }

    /// <summary>
    /// StartTime: Begins teh Timer coundown
    /// </summary>
    IEnumerator StartTime()
    {
        while (time != 0 & !StopTimer)
        {
            yield return new WaitForSeconds(1);
            time--;
            UpdateTime();
        }
    }

    /// <summary>
    /// WaitForClipAndReset: Waits for the audio clip to finish then resets scene.
    /// </summary>
    IEnumerator WaitForClipAndReset()
    {
        yield return new WaitForSeconds(aDie.length);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// OnTriggerEnter2D: Only runs when the player reaches the end post.
    /// <param name="col">GameObject that collided with this</param> 
    /// </summary>
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("EndPost"))
        {
            StopTimer = true;
            StartCoroutine(AddFinalPoints());
            GameObject.Find("BackgroundAudio").GetComponent<background>().StopBackgroundMusic();
            StartCoroutine(PlayFlagSound());
            animator.SetFloat("Speed", 0.0f);
            StartCoroutine(MoveCharacter());

            animator.SetFloat("Finish", 9f);
            Vector3 top = col.transform.position;
            Vector3 bottom = col.transform.position;
            top.y += 5;
            bottom.y -= 4;

            transform.position = Vector3.Lerp(top, bottom, 8 * Time.deltaTime); 
            StartCoroutine(WaitToEnd());
        }
    }

    IEnumerator AddFinalPoints()
    {
        while (time > 0)
        {
            ScoreCount += 50;
            time--;
            UpdateScore();
            UpdateTime();
            yield return new WaitForSeconds(0.005f);
        }
    }

    IEnumerator PlayFlagSound()
    {
        aSource.PlayOneShot(aFlag);
        yield return new WaitForSeconds(aFlag.length);

        aSource.PlayOneShot(aStageClear);
        yield return new WaitForSeconds(aStageClear.length);
    }

    IEnumerator MoveCharacter()
    {
        yield return new WaitForSeconds(2);
        Vector3 temp = transform.position;
        temp.x += 25f;
        transform.position = Vector3.Lerp(transform.position, temp, 8 * Time.deltaTime);
    }

    IEnumerator WaitToEnd()
    {
        yield return new WaitUntil(() => !aSource.isPlaying);
        Application.Quit();
    }
}
