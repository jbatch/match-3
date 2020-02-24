using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 [RequireComponent(typeof(Image))]
public class MuteController : MonoBehaviour
{
    public Sprite muteSprite;
    public Sprite unmuteSprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggleMute() {
        var audioSource = MusicSource.Instance.GetComponent<AudioSource>();
        audioSource.mute = !audioSource.mute;

        if (audioSource.mute) {
            this.GetComponent<Image>().sprite = muteSprite;
        } else {
            this.GetComponent<Image>().sprite = unmuteSprite;
        }
    }
}
