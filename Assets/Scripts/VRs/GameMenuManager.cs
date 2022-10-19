using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{
    public Transform PlayerHead;
    public float SpawnDistance = 2f;

    public GameObject MenuCanvas;
    public InputActionProperty ShowButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ShowButton.action.WasPerformedThisFrame())
        {
            MenuCanvas.SetActive(!MenuCanvas.activeSelf);
            MenuCanvas.transform.position = PlayerHead.position + new Vector3(PlayerHead.forward.x, 0, PlayerHead.forward.z).normalized * SpawnDistance;
        }

        MenuCanvas.transform.LookAt(new Vector3(PlayerHead.position.x, MenuCanvas.transform.position.y, PlayerHead.position.z));
        MenuCanvas.transform.forward *= -1;
    }
}
