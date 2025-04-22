using UnityEngine;

public class PlayOnstart : MonoBehaviour
{
    private float sectimer = 1;
    bool didthing = false;
    private void Update()
    {
        if(sectimer <= 0 && !didthing)
        {
            didthing = true;
            GetComponent<AudioSource>().Play();
        }
        else
        {
            sectimer -= Time.deltaTime;
        }
    }

}
