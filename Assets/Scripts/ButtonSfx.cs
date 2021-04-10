using UnityEngine;

public class ButtonSfx : MonoBehaviour
{
    public void Beep(AudioSource source)
    {
        source.Play();
    }
}
