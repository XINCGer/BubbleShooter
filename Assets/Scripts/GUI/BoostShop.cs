using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BoostShop : MonoBehaviour {
    public Sprite[] icons;
    public string[] titles;
    public string[] descriptions;
    public int[] prices;
    public Image icon;
    public Text title;
    public Text description;
    public Text price;

    BoostType boostType;

	// Use this for initialization
	void Start () {
	
	}

    void Update()
    {
        GamePlay.Instance.GameStatus = GameState.Pause;
    }
	
	// Update is called once per frame
	public void SetBoost (BoostType _boostType ) 
    {
        GamePlay.Instance.GameStatus = GameState.Pause;
        boostType = _boostType;
        gameObject.SetActive( true );
        icon.sprite = icons[(int)_boostType];
        title.text = titles[(int)_boostType];
        description.text = descriptions[(int)_boostType];
        price.text = "" + prices[(int)_boostType];
	}

    public void BuyBoost()
    {
        GetComponent<AnimationManager>().BuyBoost( boostType, prices[(int)boostType] );
    }
}
