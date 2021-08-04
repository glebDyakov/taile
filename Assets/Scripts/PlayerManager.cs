using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    
    AudioSource audio;
    // public GameObject oponent;
    public int playerIndex;
    string weapon;
    public int experience = 0;
    public int hp = 100;
    public bool near = false;
    bool killed = false;
    GameManager gameManager;
    PlayerManager enemyManager;
    Rigidbody rb;
    public GameObject explosion;
    float speed;
    Coroutine repairHpCoroutine;
    public Rigidbody enemyRb;
    float hpRepairInterval = 10f;
    int damage = 0;

    void Start() {
        audio = GetComponent<AudioSource>();
        gameManager = GameObject.FindWithTag("MainCamera").GetComponent<GameManager>();
        weapon = gameManager.weapons[Random.Range(0, gameManager.weapons.Count)];
        rb = GetComponent<Rigidbody>();
        float tempo = 1f;
        
        RaycastHit enemyRbHit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        if(Physics.Raycast(rb.position, fwd, out enemyRbHit)){
            enemyRb = enemyRbHit.rigidbody;
        }

        if(weapon.Contains("Sword")){
            tempo = 3f;
            speed = 3f;
            damage = 30;
            audio.clip = gameManager.clips[0];
        } else if(weapon.Contains("Axe")){
            tempo = 1f;
            speed = 1f;
            damage = 20;
            audio.clip = gameManager.clips[1];
        } else if(weapon.Contains("Bow")){
            tempo = 1f;
            speed = 5f;
            damage = 10;
            audio.clip = gameManager.clips[2];
        }
        InvokeRepeating("Damage", gameManager.intermediate.startTime, tempo);
    }

    public IEnumerator RepairHealth() {
        while(hp < 100){
            hp += 2;
            yield return new WaitForSeconds(hpRepairInterval);
        }
    } 

    bool TargetFinished(PlayerManager player) {
        return player.enemyRb == null;
        // return player.oponent == null;
        // return !player.near;
    }

    void Damage(){
        if(near && gameManager.sessionExists && !killed && enemyManager != null){
            audio.Play();
            enemyManager.hp -= damage;
            enemyManager.repairHpCoroutine = StartCoroutine(enemyManager.RepairHealth());
            experience += 10;
            if(enemyManager.hp <= 0){
                StopCoroutine(enemyManager.repairHpCoroutine);
                enemyManager.killed = true;
                near = false;
                enemyManager.explosion.SetActive(true);
                enemyRb = null;
                Destroy(enemyManager.gameObject, 2f);
                Invoke("CheckResultsOfBattle", 3f);
            }
        }
    }

    void FixedUpdate() {
        if(!killed){
            rb.MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);
            if(enemyRb != null && weapon.Contains("Bow") && !near){
                if(Vector3.Distance(rb.position, enemyRb.position) <= 5f){
                    near = true;
                    enemyManager = enemyRb.gameObject.GetComponent<PlayerManager>();
                }
            }
        }
    }

    void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player")){
            near = true;
            enemyManager = other.gameObject.GetComponent<PlayerManager>();
        }
    }

    void OnCollisionExit(Collision other) {
        near = false;
    }

    void CheckResultsOfBattle(){
        if(gameManager.commands[0].childCount == 0){
            gameManager.AllEnemiesKilled("Second");
        } else if(gameManager.commands[1].childCount == 0){
            gameManager.AllEnemiesKilled("First");
        }
        if(gameManager.players.TrueForAll(TargetFinished)){
            gameManager.AllEnemiesKilled("undefined");
        }
    }
}
