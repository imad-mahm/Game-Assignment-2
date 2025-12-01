using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiftCloser : MonoBehaviour
{
    [SerializeField] private GameObject arrow1;
    [SerializeField] private GameObject arrow2;
    private void Start()
    {
        //rift arrows
        arrow1.SetActive(true);
        arrow2.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Rift"))
        {
            //load Scene 2
        }
    }
}
