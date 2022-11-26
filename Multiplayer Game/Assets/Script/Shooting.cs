using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

//MonoBehaviourPunCallbacks
public class Shooting : MonoBehaviourPunCallbacks 
{
    // Decalaring Variable/Objects
    public Camera FPS_CAMERA;
    public GameObject BulletEffectPrefab;
    
    [Header("Health Stuff")]
    public float startHealth = 100f;
    public float health = 100f;
    public Image healthSlider;
    private Animator animator;
    [SerializeField] private AudioSource jumpSoundEffect;
    public void Start()
    {
        // Initialize Variable/Objects
        health = startHealth;
        healthSlider.fillAmount = health/startHealth; 
        animator = GetComponent<Animator>();
    }
    public void Fire()
    {
        // Bullet Script for Raycast
        Ray ray = FPS_CAMERA.ViewportPointToRay(new Vector3(0.5f,0.5f));
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,1000f)) // 
        {
            jumpSoundEffect.Play();
            Debug.Log(hit.collider.gameObject.name);
            GameObject effect = Instantiate(BulletEffectPrefab, hit.point, Quaternion.identity); 
            Destroy(effect,1f);
            if(hit.collider.gameObject.CompareTag("Player")&& !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                //RPC
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("DamageHealth",RpcTarget.AllBuffered,10f);
            }
        }
    }
    [PunRPC]
    public void DamageHealth(float damage,PhotonMessageInfo photonMessageInfo)
    {
        health-= damage;
        healthSlider.fillAmount = health/startHealth;
        if(health <= 0)
        {
            Die();
            Debug.Log(photonMessageInfo.Sender.NickName+"to"+photonMessageInfo.photonView.Owner.NickName);
        }
    }

    public void Die()
    {
        animator.SetBool("IsDead",true);
        GetComponent<Player_Movement_Controller>().enabled = false;
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        GameObject resptext = GameObject.Find("RespawnText");
        float RespawnTime = 2f;
        while (RespawnTime > 0f)
        {
            yield return new WaitForSeconds(1f);
            RespawnTime -= 1;
            resptext.GetComponent<TMP_Text>().text = "Player Respwaning in :" + RespawnTime.ToString("0.00");
            GetComponent<Player_Movement_Controller>().enabled = false;

        }
            animator.SetBool("IsDead", false);
            resptext.GetComponent<TMP_Text>().text = "";
            float RandomPoint = Random.Range(10,-10);
            transform.position = new Vector3(RandomPoint,0f, RandomPoint);
            GetComponent<Player_Movement_Controller>().enabled = true;
            // Health 100
            photonView.RPC("RegainHealth",RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {   // Regain Health
        health = startHealth;
        healthSlider.fillAmount = health/startHealth;
    }

    [PunRPC]
    public void CreateEffect(Vector3 position)
    {   // Fire Effect
        GameObject effect = Instantiate(BulletEffectPrefab,position, Quaternion.identity);
        Destroy(effect, 1f);
    }
}
