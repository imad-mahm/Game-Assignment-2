using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RiftCloser : MonoBehaviour
{
    [SerializeField] private GameObject arrow1;
    [SerializeField] private GameObject arrow2;
    
    private void Start()
    {
        arrow1.SetActive(true);
        arrow2.SetActive(true);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Rift"))
        {
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
    }
}
