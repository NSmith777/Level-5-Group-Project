using UnityEngine;

public class PlayerHandAnim : MonoBehaviour
{
    public Animator m_HandAnim;

    public void StopHandShootAnim()
    {
        m_HandAnim.SetBool("m_IsShooting", false);
    }
}
