using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RiftCloser2 : MonoBehaviour
{   
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (!this.gameObject.activeSelf) return;

        if (other.gameObject.CompareTag("Rift"))
        {
            audioSource.PlayOneShot(audioClip);
            other.gameObject.SetActive(false);
        }
            
    }
}
