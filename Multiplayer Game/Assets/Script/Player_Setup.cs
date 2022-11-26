using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;

//MonoBehaviourPunCallbacks
public class Player_Setup : MonoBehaviourPunCallbacks
{
    // Decalaring Variable/Objects
    public GameObject []localPlayerItems;
    public GameObject []remotePlayerItems;
    public GameObject PlayerCanvas;
    public GameObject CameraHolder;
    private Animator playerAnimator;
    public Button FireButton;
    private Shooting shooting;
    public GameObject HealthCanvas;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize Variable/Objects
        playerAnimator = GetComponent<Animator>();
        shooting = GetComponent<Shooting>();

        //Condition
        if(photonView.IsMine) 
        {   // for localPlayerItems
            foreach(GameObject g in localPlayerItems)
            {
                g.SetActive(true);
            }
            // for remotePlayerItems
            foreach(GameObject g in remotePlayerItems)
            {
                g.SetActive(false);
            }
            // Initialize Variable/Objects
            GetComponent<RigidbodyFirstPersonController>().enabled = true;
            GetComponent<Player_Movement_Controller>().enabled = true;
            HealthCanvas.SetActive(false);
            PlayerCanvas.SetActive(true);
            CameraHolder.SetActive(true);
            playerAnimator.SetBool("Soldier",true);
            // Subscribing Method/Function to Button
            FireButton.onClick.AddListener(()=> shooting.Fire()); 
        }
        else
        {   
            // for localPlayerItems
            foreach(GameObject g in localPlayerItems)
            {
                g.SetActive(false);
            }
             // for remotePlayerItems
            foreach(GameObject g in remotePlayerItems)
            {
                g.SetActive(true);
            }
            // Initialize Variable/Objects
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            GetComponent<Player_Movement_Controller>().enabled = false;
            PlayerCanvas.SetActive(false);
            CameraHolder.SetActive(false);
            HealthCanvas.SetActive(true);
            playerAnimator.SetBool("Soldier",false);
        }
    }
}