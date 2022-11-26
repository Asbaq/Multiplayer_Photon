using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    // Initialize Variable
    [SerializeField]    
    GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        // // Initialize Player on PhotonNetwork
        if(PhotonNetwork.IsConnectedAndReady)
        {
            int randomNumber = Random.Range(10,-10);
            PhotonNetwork.Instantiate(playerPrefab.name,new Vector3(randomNumber,0.10f,randomNumber),Quaternion.identity);
        }
    }
}
