using UnityEngine;

public class AnimationBehaviour : MonoBehaviour
{
    [SerializeField]
    Animator SBearAnim;
    [SerializeField]
    Animator SFoxAnim;
    public int animationIndex;
    public void SDecideAnimation()
    {
        animationIndex = Random.Range(1,21);
        if (animationIndex == 1)
        {
            SBearAnim.SetTrigger("Hurt");
            GameManagerScript.Instance.Dying();
            GameManagerScript.Instance.repsDone--;
            GameManagerScript.Instance.ModifyReps();
        }
        else if (animationIndex == 20)
        {
            SBearAnim.SetTrigger("Breakdance");
            GameManagerScript.Instance.Winning();
            GameManagerScript.Instance.repsDone++;
            GameManagerScript.Instance.ModifyReps();
        }
        else
        {
            SBearAnim.SetTrigger("Tapped");
            GameManagerScript.Instance.Punch();
        }
    }
    public void SFoxDecideAnimation()
    {
        animationIndex = Random.Range(1,21);
        if (animationIndex == 1)
        {
            SFoxAnim.SetTrigger("Deadge");
            GameManagerScript.Instance.Dying();
            GameManagerScript.Instance.repsDone--;
            GameManagerScript.Instance.ModifyReps();
        }
        else if (animationIndex == 20)
        {
            SFoxAnim.SetTrigger("Crit");
            GameManagerScript.Instance.Winning();
            GameManagerScript.Instance.repsDone++;
            GameManagerScript.Instance.ModifyReps();
        }
        else
        {
            SFoxAnim.SetTrigger("Tapped");
            GameManagerScript.Instance.Slash();   
        }
    }
}
