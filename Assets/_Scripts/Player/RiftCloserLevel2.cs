using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RiftCloser2 : MonoBehaviour
{   
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (!this.gameObject.activeSelf) return;
        
        if (other.gameObject.CompareTag("Rift"))
            other.gameObject.SetActive(false);
    }
}
