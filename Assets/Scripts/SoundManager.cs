using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public AudioSource[] destroyNoise;

    public void PlayRandomDestroyNoise()
	{
		//choisir un nombre aléatoire
		int clipToPlay = Random.Range(0, destroyNoise.Length);
		//jouer le son en question
		destroyNoise[clipToPlay].Play();
	}
}
