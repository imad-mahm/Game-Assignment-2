using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject riftCloser;
    [SerializeField] private Collecting collectingscript;
    // Start is called before the first frame update
    void Start()
    {
        gun.SetActive(false);
        riftCloser.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (collectingscript.crafted)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                riftCloser.SetActive(true);
                gun.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            riftCloser.SetActive(false);
            gun.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            riftCloser.SetActive(!riftCloser.activeSelf);
            gun.SetActive(!gun.activeSelf);
        }
    }
}
