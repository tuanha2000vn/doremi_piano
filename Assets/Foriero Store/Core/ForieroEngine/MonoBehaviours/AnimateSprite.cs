using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class AnimateSprite : MonoBehaviour {

	public SpriteRenderer spriteRenderer;
	public Image image;

	public enum AnimateState{
		None,
		Playing,
		Pausing,
		Completed
	}

	public enum AnimateType{
		PlayOnce, 
		PingPongOnce,
		Loop,
		PingPong,
	}

	public enum AnimateDirection{
		Forward = 0,
		Backward = 1
	}

	[HideInInspector]
	public float _spritePerSecond = 10f;
	[HideInInspector]
	public float _spriteDuration = 0.1f;

	public float spritesPerSecond {
		set{
			_spritePerSecond = value;
			spriteDuration = 1f/_spritePerSecond;
		}
		get{
			return _spritePerSecond;
		}
	}

	public float spriteDuration {
		set{
			_spriteDuration = value;
			_spritePerSecond = 1f/_spriteDuration;
		}
		get{
			return _spriteDuration;
		}
	}

	private AnimateState _state = AnimateState.None;

	public AnimateState state {
		set{
			if(_state != value && OnStateChanged != null) OnStateChanged(value);
			_state = value;
		}
		get{
			return _state;
		}
	}

	public Action<AnimateState> OnStateChanged;

	public AnimateType type = AnimateType.PlayOnce;
	public AnimateDirection direction = AnimateDirection.Forward;
	public bool playOnAwake = false;
	public bool delayed = false;
	public float delayMIN = 0f;
	public float delayMAX = 1f;
	[HideInInspector]
	public float delay = 0f;
	public bool playOnceRepeated = false;

	public AudioClip audioClip;

	[HideInInspector]
	public Sprite[] sprites;

	//PRIVATE FIELDS//
	int index = 0;
	AnimateDirection indexDirection = AnimateDirection.Forward;
	float timeElapsed = 0f;
	float delayElapsed = 0f;
	int loops = 0;

	bool setSpriteIndexOnPlay = false;

	void Awake(){
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		image = gameObject.GetComponent<Image>();
		Stop ();
		if(playOnAwake) Play ();
	}

	public void Play(){
		state = AnimateState.Playing;
		if(audioClip) {
			GetComponent<AudioSource>().clip = audioClip;
			GetComponent<AudioSource>().Play();
		}
	}

	public void Pause(){
		state = AnimateState.Pausing;
	}

	public void Stop(){
		Reset ();
		if(!setSpriteIndexOnPlay) ResetSprite();
	}

	void ResetSprite(){
		switch(direction){
		case AnimateDirection.Forward:
			index = 0;
			SetSprite();
			break;
		case AnimateDirection.Backward:
			index = sprites.Length - 1; 
			SetSprite ();
			break;
		}
	}

	void Reset(){
		state = AnimateState.None;
		indexDirection = direction;
		timeElapsed = 0f;
		loops = 0;
		delayElapsed = 0f;
		delay = UnityEngine.Random.Range(delayMIN, delayMAX);
		setSpriteIndexOnPlay = false;
	}

	void SetSprite(){
		if(sprites.Length > 0 && index >= 0 && index < sprites.Length){
			if(spriteRenderer) spriteRenderer.sprite = sprites[index];
			if(image) image.sprite = sprites[index];
		} 
	}

	bool firstSpriteAfterDelay = false;

	void Update(){
		if(state != AnimateState.Playing) return;

		if(delayElapsed >= delay || delayed == false){

			if(!firstSpriteAfterDelay && setSpriteIndexOnPlay) ResetSprite ();
			firstSpriteAfterDelay = true;

			if(timeElapsed >= spriteDuration){
				PlayAnimation();
				timeElapsed = timeElapsed - spriteDuration;
			} else {
				timeElapsed += Time.deltaTime;
			}
		} else {

			delayElapsed += Time.deltaTime;
			firstSpriteAfterDelay = false;

		}
	}

	void PlayAnimation(){
		switch(indexDirection){
		case AnimateDirection.Forward:
			if(index < sprites.Length - 1){
				index++;
				SetSprite();
			} else {
				PlayAnimationFinished();
			}
			break;
		case AnimateDirection.Backward:
			if(index > 0){
				index--;
				SetSprite();
			} else {
				PlayAnimationFinished();
			}
			break;
		}
	}

	void PlayAnimationFinished(){
		loops++;
		switch(type){
		case AnimateType.PlayOnce:
			if(playOnceRepeated){
				indexDirection = direction;
				timeElapsed = 0f;
				loops = 0;
				delayElapsed = 0f;
				delay = UnityEngine.Random.Range(delayMIN, delayMAX);
				setSpriteIndexOnPlay = true;
				_state = AnimateState.Playing;
			} else {
				state = AnimateState.Completed;
			}
			break;
		case AnimateType.PingPongOnce:
			if(loops >= 2) {
				if(playOnceRepeated){
					indexDirection = direction;
					timeElapsed = 0f;
					loops = 0;
					delayElapsed = 0f;
					delay = UnityEngine.Random.Range(delayMIN, delayMAX);
					setSpriteIndexOnPlay = false;
					_state = AnimateState.Playing;
				} else {
					state = AnimateState.Completed;
				}
			} else {
				switch(indexDirection){
				case AnimateDirection.Forward:
					indexDirection = AnimateDirection.Backward;
					index--;
					SetSprite();
					break;
				case AnimateDirection.Backward:
					indexDirection = AnimateDirection.Forward;
					index++;
					SetSprite();
					break;
				}
			}
			break;
		case AnimateType.Loop:
			switch(direction){
			case AnimateDirection.Forward:
				index = 0;
				SetSprite();
				break;
			case AnimateDirection.Backward:
				index = sprites.Length - 1;
				SetSprite();
				break;
			}
			break;
		case AnimateType.PingPong:
			switch(indexDirection){
			case AnimateDirection.Forward:
				indexDirection = AnimateDirection.Backward;
				index--;
				SetSprite();
				break;
			case AnimateDirection.Backward:
				indexDirection = AnimateDirection.Forward;
				index++;
				SetSprite();
				break;
			}
			break;
		}
	}
}
