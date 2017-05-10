using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProgressBarScript : MonoBehaviour {
	Slider slider;
	public static ProgressBarScript Instance;
	float maxWidth;
	// Use this for initialization
	void Start () {
		Instance = this;
        slider = GetComponent<Slider>();
		maxWidth = slider.value;
		ResetBar();
        PrepareStars();
	}
	
	public void UpdateDisplay (float x) {	
		slider.value = maxWidth * x;
		if(maxWidth * x >= maxWidth){
			slider.value = maxWidth;

		//	ResetBar();
		}
	}
	
	public void AddValue (float x) {	
		UpdateDisplay ( slider.value*100/maxWidth/100 + x);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public bool IsFull()
	{
		if(slider.value >= maxWidth){ 
			ResetBar();
			return true;
		}
		else return false;
	}
	
	public void ResetBar(){
		UpdateDisplay(0.0f);
	}

    void PrepareStars()
    {
        float width = GetComponent<RectTransform>().rect.width;
        transform.Find( "Star1" ).localPosition = new Vector3( LevelData.star1 * 100 / LevelData.star3 * width / 100 - ( width / 2f ), transform.Find( "Star1" ).localPosition.y, 0 );
        transform.Find( "Star2" ).localPosition = new Vector3( LevelData.star2 * 100 / LevelData.star3 * width / 100 - ( width / 2f ), transform.Find( "Star2" ).localPosition.y, 0 );
    }

}
