using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject tableLabel;
    public GameObject table;
    public List<Text> tableLabels;
    public LobbyManager intermediate;
    public GameObject playerPrefab;
    public List<Vector3> playerPositions;
    public bool sessionExists;
    public List<Transform> commands;
    public List<RectTransform> tableCommands;
    public List<PlayerManager> players;
    List<PlayerManager> sortedPlayers;
    public List<AudioClip> clips;
    public List<string> weapons = new List<string>{
        "Sword",
        "Axe",
        "Bow"
    };
    void Start() {
        sessionExists = true;
        intermediate = GameObject.FindWithTag("Lobby").GetComponent<LobbyManager>();
        Invoke("StartGame", intermediate.startTime);
    }

    void StartGame() {
        for(int playerIndex = 0; playerIndex < intermediate.countPlayers; playerIndex++){
            float rotationAxisY = 0f;
            Transform command = commands[0];
            bool needFreeze = false;
            bool needPaint = false;
            // bool needOponentEnemyCommand = false;
            if(playerIndex % 2 != 0){
                rotationAxisY = 180f;
                command = commands[1];
                needPaint = true;
                // needOponentEnemyCommand = true;
            } else if(playerIndex % 2 == 0 && playerIndex + 1 == intermediate.countPlayers){
                needFreeze = true;
            }
            GameObject player = Instantiate(playerPrefab, playerPositions[playerIndex], Quaternion.Euler(0, rotationAxisY, 0f), command);
            PlayerManager playerManager = player.GetComponent<PlayerManager>();
            playerManager.playerIndex = playerIndex;
            // if(needOponentEnemyCommand){
            //     playerManager.oponent = players[playerManager.playerIndex - 1].gameObject;
            // } else {
            //     playerManager.oponent = players[playerManager.playerIndex + 1].gameObject;
            // }
            players.Add(playerManager);
            if(needFreeze){
                player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                playerManager.near = false;
            }
            if(needPaint){
                player.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
            }
        }
        Invoke("EndGame", intermediate.durationTime);
    }

    public void AllEnemiesKilled(string winnerCommand){
        if(sessionExists){
            sessionExists = false;
            CancelInvoke();
            TableOutput(winnerCommand);
        }
    }

    void EndGame(){
        if(sessionExists){
            sessionExists = false;
            TableOutput("undefined");
        }
    }

    void TableOutput(string winnerCommand){
        intermediate.EpicHeroMusic();
        StopAllCoroutines();

        sortedPlayers = players.OrderByDescending(player => player.experience).ToList();    

        foreach(var sortedPlayer in sortedPlayers){
            if(sortedPlayer != null){
                RectTransform tableCommand = tableCommands[0];
                if(sortedPlayer.playerIndex % 2 != 0){
                    tableCommand = tableCommands[1];
                }
                GameObject tempLabel = Instantiate(tableLabel, Vector2.zero, Quaternion.Euler(0f, 0f, 0f), tableCommand);
                tempLabel.transform.localPosition = new Vector3(0f, 0f, 0f);
                tempLabel.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                Text tempLabelText = tempLabel.GetComponent<Text>();
                int orderNumber = sortedPlayer.playerIndex + 1;
                string playerRecord = orderNumber.ToString() + ") " + sortedPlayer.gameObject.GetHashCode().ToString() + " \n" + sortedPlayer.experience + "exp";
                tempLabelText.text = playerRecord;
                tableLabels.Add(tempLabelText);
            }   
        }

        table.SetActive(true);
        if(winnerCommand.Contains("undefined")){
            if(commands[0].childCount > commands[1].childCount){
                tableLabels[1].text = "Вы проиграли!";
            } else if(commands[1].childCount > commands[0].childCount){
                tableLabels[0].text = "Вы проиграли!";
            } else if(commands[1].childCount == commands[0].childCount){
                int totalHpFirstCommand = 0;
                int totalExperienceFirstCommand = 0;
                int totalPointsFirstCommand = 0;
                int totalHpSecondCommand = 0;
                int totalExperienceSecondCommand = 0;
                int totalPointsSecondCommand = 0;
                for(var fighterIndex = 0; fighterIndex < commands[0].childCount; fighterIndex++){
                    totalHpFirstCommand += commands[0].GetChild(fighterIndex).gameObject.GetComponent<PlayerManager>().hp;
                    totalExperienceFirstCommand += commands[0].GetChild(fighterIndex).gameObject.GetComponent<PlayerManager>().experience;
                }
                for(var fighterIndex = 0; fighterIndex < commands[1].childCount; fighterIndex++){
                    totalHpSecondCommand += commands[1].GetChild(fighterIndex).gameObject.GetComponent<PlayerManager>().hp;
                    totalExperienceSecondCommand += commands[1].GetChild(fighterIndex).gameObject.GetComponent<PlayerManager>().experience;
                }
                totalPointsFirstCommand = totalHpFirstCommand + totalExperienceFirstCommand;
                totalPointsSecondCommand = totalHpFirstCommand + totalExperienceSecondCommand;
                if(totalPointsFirstCommand > totalPointsSecondCommand){
                    tableLabels[1].text = "Вы проиграли!";
                } else if(totalPointsFirstCommand < totalPointsSecondCommand){
                    tableLabels[0].text = "Вы проиграли!";
                }
            }
        } else if(winnerCommand.Contains("First")){
            tableLabels[1].text = "Вы проиграли!";
        } else if(winnerCommand.Contains("Second")){
            tableLabels[0].text = "Вы проиграли!";
        }
    }

    void Update() {
        
    }
}
