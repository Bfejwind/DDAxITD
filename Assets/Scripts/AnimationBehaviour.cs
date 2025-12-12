using UnityEngine;

public class AnimationBehaviour : MonoBehaviour
{
    [SerializeField]
    Animator SBearAnim;
    public int animationIndex;
    public void SDecideAnimation()
    {
        animationIndex = Random.Range(1,21);
        if (animationIndex == 1)
        {
            SBearAnim.SetTrigger("Hurt");
            GameManagerScript.Instance.repsDone--;
            GameManagerScript.Instance.ModifyReps();
        }
        else if (animationIndex == 20)
        {
            SBearAnim.SetTrigger("Breakdance");
            GameManagerScript.Instance.repsDone++;
            GameManagerScript.Instance.ModifyReps();
        }
        else
        {
            SBearAnim.SetTrigger("Tapped");    
        }
    }
}
