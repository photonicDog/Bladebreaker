using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class AudioController : MonoBehaviour
    {
        public AudioClip Music;
        public AudioClip Loop;

        private AudioSource _musicSource;

        private AudioSource _playerSFXSource1;
        private AudioSource _playerSFXSource2;

        private AudioSource _enemySFXSource1;
        private AudioSource _enemySFXSource2;

        private AudioSource _stageSFXSource1;
        private AudioSource _stageSFXSource2;

        private static AudioController _instance;
        public static AudioController Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            _musicSource = transform.Find("Music").GetComponent<AudioSource>();
            _playerSFXSource1 = transform.Find("PlayerSFX1").GetComponent<AudioSource>();
            _playerSFXSource2 = transform.Find("PlayerSFX2").GetComponent<AudioSource>();
            _enemySFXSource1 = transform.Find("EnemySFX1").GetComponent<AudioSource>();
            _enemySFXSource2 = transform.Find("EnemySFX2").GetComponent<AudioSource>();
            _stageSFXSource1 = transform.Find("StageSFX1").GetComponent<AudioSource>();
            _stageSFXSource2 = transform.Find("StageSFX2").GetComponent<AudioSource>();

            _musicSource.clip = Loop ? Loop : Music;
            _musicSource.loop = true;
            //DontDestroyOnLoad(gameObject);
        }

        public void PlayMusic(AudioClip music)
        {
            _musicSource.PlayOneShot(music);
            _musicSource.PlayScheduled(AudioSettings.dspTime + music.length);
        }

        public void PlayMusic() {
            _musicSource.PlayOneShot(Music);
            _musicSource.PlayScheduled(AudioSettings.dspTime + Music.length);
        }

        public void StopMusic()
        {
            _musicSource.Stop();
        }

        public void PlayPlayerSFX(AudioClip sfx)
        {
            if (_playerSFXSource1.isPlaying)
            {
                _playerSFXSource2.clip = sfx;
                _playerSFXSource2.Play();
            } else
            {
                _playerSFXSource1.clip = sfx;
                _playerSFXSource1.Play();
            }
        }

        public void StopPlayerSFX()
        {
            _playerSFXSource1.Stop();
            _playerSFXSource2.Stop();
        }

        public void PlayEnemySFX(AudioClip sfx)
        {

            if (_enemySFXSource1.isPlaying)
            {
                _enemySFXSource2.clip = sfx;
                _enemySFXSource2.Play();
            }
            else
            {
                _enemySFXSource1.clip = sfx;
                _enemySFXSource1.Play();
            }
        }

        public void StopEnemySFX()
        {
            _enemySFXSource1.Stop();
            _enemySFXSource2.Stop();
        }

        public void PlayStageSFX(AudioClip sfx)
        {
            if (_stageSFXSource1.isPlaying)
            {
                _stageSFXSource2.clip = sfx;
                _stageSFXSource2.Play();
            }
            else
            {
                _stageSFXSource1.clip = sfx;
                _stageSFXSource1.Play();
            }
        }

        public void StopStageSFX()
        {
            _stageSFXSource1.Stop();
            _stageSFXSource2.Stop();
        }

        public void StopAllSFX()
        {
            _playerSFXSource1.Stop();
            _playerSFXSource2.Stop();
            _enemySFXSource1.Stop();
            _enemySFXSource2.Stop();
            _stageSFXSource1.Stop();
            _stageSFXSource2.Stop();
        }

        public void OnPause()
        {
            _musicSource.Pause();
            _playerSFXSource1.Stop();
            _playerSFXSource2.Stop();
            _enemySFXSource1.Stop();
            _enemySFXSource2.Stop();
            _stageSFXSource1.Stop();
            _stageSFXSource2.Stop();
        }

        public void OnResume()
        {
            _musicSource.Play();
        }
    }
}
