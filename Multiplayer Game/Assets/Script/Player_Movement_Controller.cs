using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class Player_Movement_Controller : MonoBehaviour
{
    // Decalaring Variable
    public Joystick joystick;
    public FixedTouchField touchField;
    private RigidbodyFirstPersonController rigidbodyFirstPersonController;
    private Animator playerAnimator;
    // Start is called before the first frame update
    void Start()
    {       // Initialize Variable
            rigidbodyFirstPersonController = GetComponent<RigidbodyFirstPersonController>();
            playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {       // movement on x-axis
            rigidbodyFirstPersonController.joystickInputAxis.x = joystick.Horizontal; // 0 to 1  
            // movement on y-axis
            rigidbodyFirstPersonController.joystickInputAxis.y = joystick.Vertical;
            // mouse look
            rigidbodyFirstPersonController.mouseLook.lookInputAxis = touchField.TouchDist;
            // Animation on movement on x-axis
            playerAnimator.SetFloat("Horizontal", joystick.Horizontal);
            // Animation on movement on y-axis
            playerAnimator.SetFloat("Vertical", joystick.Vertical);

        if(Mathf.Abs(joystick.Horizontal)>0.9f || Mathf.Abs(joystick.Vertical)>0.9f)
        {
            // Making Animation true on running
            playerAnimator.SetBool("IsRunning ",true);
            rigidbodyFirstPersonController.movementSettings.ForwardSpeed = 16f;
        }
        else
        {
            // Making Animation false on running
            playerAnimator.SetBool("IsRunning ", false);
            rigidbodyFirstPersonController.movementSettings.ForwardSpeed = 8f;
        }
    }
}
