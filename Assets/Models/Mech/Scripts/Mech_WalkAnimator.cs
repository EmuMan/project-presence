using UnityEngine;

public class Mech_WalkAnimator : MonoBehaviour
{
	float rootMotionOffsetWalk = 5.2f;
	float rootMotionOffsetRun = 9.98176f;
//	float startDelayCounter = 0;
	Animator animator;


	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		animator.speed = 1;
	}
	

	void LateUpdate() {
//		startDelayCounter += Time.deltaTime;
//		if (startDelayCounter > 3)
			animator.speed = 1;
		//animator.SetTrigger("Walk");
	}
}
