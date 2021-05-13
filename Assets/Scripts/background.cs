using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class background : MonoBehaviour
{
    [SerializeField] private AudioClip aPause;
    [SerializeField] private AudioClip aCoin;

    private AudioSource aSource;
    bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        aSource = GetComponent<AudioSource>();
        aSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            paused = !paused;
            PauseGame(paused);
        }
    }

    /// <summary>
    /// PauseGame: Checks if the player paused the game by using p or m.
    /// <param name="p">If the game is paused currently</param>
    /// </summary>
    public void PauseGame(bool p)
    {
        if (p)//if we want to pause
        {
            aSource.Stop();
            aSource.PlayOneShot(aPause);
            Time.timeScale = 0;
            return;
        }
        aSource.Play();
        Time.timeScale = 1;
    }
    /// <summary>
    /// StopBackgroundMusic: Stops the Background music. Can be called from any script.
    /// </summary>
    public void StopBackgroundMusic()
    {
        if(aSource.isPlaying)
            aSource.Stop();
    }
    /// <summary>
    /// StartBackgroundMusic: Starts the Background music. Can be called from any script.
    /// </summary>
    public void StartBackgroundMusic()
    {
        if(!aSource.isPlaying)
            aSource.Play();
    }
    /// <summary>
    /// PlaySFX: Plays an Audio Clip one time
    /// <param name="name">Name of Clip to Play</param>
    /// </summary>
    public void PlaySFX(string name)
    {
        if(name == "coin")
        {
            aSource.PlayOneShot(aCoin);
        }
    }
}
