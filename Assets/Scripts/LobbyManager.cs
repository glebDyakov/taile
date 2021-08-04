using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {
    
    public InputField startTimeOfUser;
    public InputField durationTimeOfUser;
    public InputField countPlayersOfUser;
    public int startTime;
    public int durationTime;
    public int countPlayers;
    AudioSource audioSource;
    public AudioClip epicMusic;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void EpicHeroMusic(){
        audioSource.clip = epicMusic;
        audioSource.Play();
    }

    public void LaunchGame() {
        
        try{
            if(int.Parse(startTimeOfUser.text) >= 1 && int.Parse(startTimeOfUser.text) <= 10){
                startTime = int.Parse(startTimeOfUser.text);
            } else {
                startTime = 1;
            }
            if(int.Parse(durationTimeOfUser.text) >= 20){
                durationTime = int.Parse(durationTimeOfUser.text);
            } else {
                durationTime = 20;
            }
            if(int.Parse(countPlayersOfUser.text) >= 2 && int.Parse(countPlayersOfUser.text) <= 8){
                countPlayers = int.Parse(countPlayersOfUser.text);
            } else {
                countPlayers = 2;
            }
            SceneManager.LoadScene("Main");
        } catch(FormatException error){
            audioSource.Play();
            Debug.Log("FormatException: " + error.ToString());
        }
    }
}
