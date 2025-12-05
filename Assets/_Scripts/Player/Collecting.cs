using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collecting : MonoBehaviour
{
   [SerializeField] private GameObject weapon;
   [SerializeField] private GameObject arrow1;
   [SerializeField] private GameObject arrow2;
   [SerializeField] private AudioSource audioSource;
   [SerializeField] private AudioClip audioClip;
   [SerializeField] private AudioClip CraftingAudio;
   public bool crafted = false;
   
   
   private float collected=0f;


   private void Start()
   {
      if (SceneManager.GetActiveScene().buildIndex == 2)
      {
         crafted = true;
      }
   }

   private void OnTriggerEnter(Collider other)
   {
      
      if (other.gameObject.CompareTag("Collectable"))
      {
         audioSource.PlayOneShot(audioClip);
         collected+=1f;
         other.gameObject.SetActive(false);
      }

      if (collected == 5f)
      {
         arrow1.SetActive(true);
         arrow2.SetActive(true);
      }
      
   }

   private void OnTriggerStay(Collider other)
   {
      if (other.gameObject.CompareTag("WorkBench") && collected == 5f)
      {
         Debug.Log("Press E");
         if (Input.GetKey(KeyCode.E))
         {
            audioSource.PlayOneShot(CraftingAudio);
            weapon.SetActive(true);
            crafted = true;
         }
      }
   }
}
