using UnityEngine;
using System.Collections;

public class bonus : MonoBehaviour {
	public int revertButterFly;
	Vector3 target;
	float timed;
	// Use this for initialization
	void Start () {
		StartCoroutine( flyUp());
		timed = Time.time;
		if(name.Contains("bonus_score")) transform.position = new Vector3(transform.position.x, Random.Range(-2, 5),-10f);
		revertButterFly = Random.Range(0,2);
		if(revertButterFly == 1){
			transform.position = new Vector3(3,  transform.position.y, -10f);
			target =  new Vector3(transform.position.x-8, transform.position.y, transform.localScale.z);
		//	iTween.MoveTo(gameObject,iTween.Hash("position", new Vector3(transform.position.x-8, transform.position.y, transform.localScale.z), "time",3f,"easetype",iTween.EaseType.easeInOutSine));
		}
		else{
			transform.position = new Vector3(-3,  transform.position.y, -10f);
			transform.localScale = new Vector3(transform.localScale.x*-1, transform.localScale.y, transform.localScale.z);  //flip horrizontal
			target =  new Vector3(transform.position.x+8, transform.position.y, transform.localScale.z);
			//	iTween.MoveTo(gameObject,iTween.Hash("position", new Vector3(transform.position.x+8, transform.position.y, transform.localScale.z), "time",3f,"easetype",iTween.EaseType.easeInOutSine));
		}

	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Lerp(transform.position, target, (Time.time - timed)*0.02f);
		if(transform.position.x < -4 || transform.position.x > 4) Destroy(gameObject);
	}


	IEnumerator flyUp(){
        yield return new WaitForSeconds(0.2f);
        /*	while(true){
                iTween.MoveAdd(gameObject,iTween.Hash("y", 0.2f, "time",0.2f,"easetype",iTween.EaseType.easeInOutSine));
                yield return new WaitForSeconds(0.2f);
                iTween.MoveAdd(gameObject,iTween.Hash("y", -0.2f, "time",0.2f,"easetype",iTween.EaseType.easeInOutSine));
                yield return new WaitForSeconds(0.2f);
            }*/
	}

	public void OnTriggerEnter2D(Collider2D owner)
	{
		// check if we collided with a top block and adjust our speed and rotation accordingly
		if (owner.gameObject.name.IndexOf("ball")==0 && owner.gameObject.GetComponent<ball>().setTarget)
		{
			//SoundBase.Instance.audio.PlayOneShot(SoundBase.Instance.Pickup);
			if(name.Contains("bonus_liana")){
		//		SoundBase.Instance.audio.PlayOneShot(SoundBase.Instance.electric);
				if(!MainScript.ElectricBoost)
					MainScript.Instance.SwitchLianaBoost();
			}
			else if(name.Contains("bonus_score")){
		//		Score.Instance.addMultiplyer();
			}
			Destroy(gameObject);
		}
	}

}
