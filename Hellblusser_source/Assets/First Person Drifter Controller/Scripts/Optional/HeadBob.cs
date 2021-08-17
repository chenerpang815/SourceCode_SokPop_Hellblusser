// original by Mr. Animator
// adapted to C# by @torahhorse
// http://wiki.unity3d.com/index.php/Headbobber

using UnityEngine;
using System.Collections;

public class HeadBob : MonoBehaviour
{	
	private float bobbingSpeed = 0.25f; 
	public float bobbingAmount = 0.05f; 
	public float  midpoint = 0.6f; 
	
	private float timer = 0.0f; 
 
	void Update ()
	{
		if (!SetupManager.instance.inFreeze)
		{
			float waveslice = 0.0f;
			float horizontal = InputManager.instance.moveDirection.x; //0f;//Input.GetAxis("Horizontal");
			float vertical = InputManager.instance.moveDirection.y; //0f;//Input.GetAxis("Vertical");

			float bobThreshold = .125f;
			if ( (Mathf.Abs(horizontal) <= bobThreshold && Mathf.Abs(vertical) <= bobThreshold) || SetupManager.instance.runDataRead.playerDead || GameManager.instance.playerHurt || GameManager.instance.playerFirstPersonDrifter.playerBlocking || SetupManager.instance.defeatedFinalBoss || SetupManager.instance.curProgressData.settingsData.cameraMotion == 0 )
			{
				timer = 0.0f;
			}
			else
			{
				waveslice = Mathf.Sin(timer);
				timer = timer + bobbingSpeed;
				if (timer > Mathf.PI * 2f)
				{
					timer = timer - (Mathf.PI * 2f);
				}
			}
			if (waveslice != 0f)
			{
				float translateChange = waveslice * bobbingAmount;
				float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
				totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
				translateChange = totalAxes * translateChange;

				Vector3 localPos = transform.localPosition;
				localPos.y = midpoint + translateChange * Time.timeScale;
				transform.localPosition = localPos;
			}
			else
			{
				Vector3 localPos = transform.localPosition;
				localPos.y = midpoint;
				transform.localPosition = localPos;
			}
		}
	}
}
