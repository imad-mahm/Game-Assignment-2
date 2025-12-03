using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collecting : MonoBehaviour
{
   [SerializeField] private GameObject weapon;
   [SerializeField] private GameObject arrow1;
   [SerializeField] private GameObject arrow2;
   
   
   private float collected=0f;
   private void OnTriggerEnter(Collider other)
   {
      
      if (other.gameObject.CompareTag("Collectable"))
      {
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
            weapon.SetActive(true);
      }
   }
}
