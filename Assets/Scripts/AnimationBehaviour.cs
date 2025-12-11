using UnityEngine;

public class AnimationBehaviour : MonoBehaviour
{
    Animator anim;
    public void Punching()
    {
        anim.SetTrigger("Punch");
    }
    
}
