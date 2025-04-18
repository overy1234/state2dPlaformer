using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace SamuraiVagabond
{
	public class AudioManager : MonoBehaviour
	{
		public Sound[] sound;
		void Awake()
		{
			foreach (Sound s in sound)
			{
				s.source = gameObject.AddComponent<AudioSource>();
				s.source.clip = s.clip;

				s.source.volume = s.volume;
				s.source.pitch = s.pitch;
				s.source.loop = s.loop;
				s.source.playOnAwake = s.playOnAwake;
			}
		}

		public void play(string name)
		{
			bool exist = false;
			foreach (Sound s in sound)
			{
				if (name == s.name)
				{
					exist = true;
					s.source.Play();
					break;
				}
			}

			if (!exist)
			{
				Debug.Log("Error! The audio is not found");
				return;
			}
		}

		public void pause(string name)
		{
			bool exist = false;
			foreach(Sound s in sound)
            {
				if(name == s.name)
                {
					exist = true;
					s.source.Pause();
					break;
				}
            }

			if (!exist)
            {
				Debug.Log("Error! The audio is not found");
				return;
			}
				
		}

		public void unPause(string name)
		{
			bool exist = false;
			foreach (Sound s in sound)
			{
				if (name == s.name)
				{
					exist = true;
					s.source.UnPause();
					break;
				}
			}

			if (!exist)
			{
				Debug.Log("Error! The audio is not found");
				return;
			}
		}

		public void playOnAwake(string name , bool activated)
		{
			bool exist = false;
			foreach (Sound s in sound)
			{
				if (name == s.name)
				{
					exist = true;
					s.source.playOnAwake = activated;
					break;
				}
			}

			if (!exist)
			{
				Debug.Log("Error! The audio is not found");
				return;
			}
		}
	}
}
