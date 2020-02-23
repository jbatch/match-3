using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUIManager : MonoBehaviour
{
    public GameObject PlayingCanvas;
    public GameObject EndRoundCanvas;
    // Start is called before the first frame update
    void Start()
    {
        EndRoundCanvas.SetActive(false);
        PlayingCanvas.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showEndRoundCanvas() {
        EndRoundCanvas.SetActive(true);
        PlayingCanvas.SetActive(false);
    }
}
