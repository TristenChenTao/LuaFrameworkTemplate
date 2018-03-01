using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using UnityEngine;

namespace LuaFramework {
    public class SoundManager : Manager {
        private AudioSource audio; //音乐 背景音乐

        private AudioSource soundAudio; //音效 出牌等提示音
        private Hashtable sounds = new Hashtable ();

        void Start () {
            audio = GetComponent<AudioSource> ();
            soundAudio = GameObject.FindWithTag ("SoundObject").GetComponent<AudioSource> ();
        }

        /// <summary>
        /// ���һ������
        /// </summary>
        void Add (string key, AudioClip value) {
            if (sounds[key] != null || value == null) return;
            sounds.Add (key, value);
        }

        /// <summary>
        /// ��ȡһ������
        /// </summary>
        AudioClip Get (string key) {
            if (sounds[key] == null) return null;
            return sounds[key] as AudioClip;
        }

        /// <summary>
        /// ����һ����Ƶ
        /// </summary>
        public AudioClip LoadAudioClip (string path) {

            string url = AppConst.AudioDir + path;
            string assetBundleURL = Util.DataPath + AppConst.AudioDir.ToLower () + path.ToLower () + AppConst.ExtName;
            bool isExists = System.IO.File.Exists (assetBundleURL);
            AudioClip ac;
            if (isExists) {
                ac = Get (assetBundleURL);
                if (ac == null) {
                    AssetBundle ab = AssetBundle.LoadFromFile (assetBundleURL);
                    string[] names = path.Split ('/');
                    ac = ab.LoadAsset (names[names.Length - 1]) as AudioClip;
                    ab.Unload (false);
                    Add (assetBundleURL, ac);
                }
            } else {
                ac = Get (url);
                if (ac == null) {
                    ac = (AudioClip) Resources.Load (url, typeof (AudioClip));
                    if (ac == null) {
                        Debug.Log (url + "路径找不到");
                    } else {
                        Add (url, ac);
                    }
                }
            }

            return ac;
        }

        /// <summary>
        /// �Ƿ񲥷ű������֣�Ĭ����1������
        /// </summary>
        /// <returns></returns>
        public bool CanPlayBackSound () {
            string key = AppConst.AppPrefix + "BackSound";
            int i = PlayerPrefs.GetInt (key, 1);
            return i == 1;
        }

        /// <summary>
        /// ���ű�������
        /// </summary>
        /// <param name="canPlay"></param>
        public void PlayBacksound (string name, bool canPlay) {
            string url = AppConst.AudioDir + name;
            if(audio == null) {
Debug.Log("<><><><>><><>PlayBacksoundPlayBacksound 10");
            }
            
            if (audio.clip != null) {
                if (url.IndexOf (audio.clip.name) > -1) {
                    if (!canPlay) {
                        audio.Stop ();
                        audio.clip = null;
                        Util.ClearMemory ();
                    }
                    return;
                }
            }
            Debug.Log("<><><><>><><>PlayBacksoundPlayBacksound 1");
            if (canPlay) {
                audio.loop = true;
                audio.clip = LoadAudioClip (name);
                audio.Play ();
            } else {
                audio.Stop ();
                audio.clip = null;
                Util.ClearMemory ();
            }
            Debug.Log("<><><><>><><>PlayBacksoundPlayBacksound 2");
        }

        /// <summary>
        /// �Ƿ񲥷���Ч,Ĭ����1������
        /// </summary>
        /// <returns></returns>
        public bool CanPlaySoundEffect () {
            string key = AppConst.AppPrefix + "SoundEffect";
            int i = PlayerPrefs.GetInt (key, 1);
            return i == 1;
        }

        /// <summary>
        /// ������Ƶ��
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="position"></param>
        public void Play (AudioClip clip, Vector3 position) {
            if (!CanPlaySoundEffect ()) return;
            // AudioSource.PlayClipAtPoint (clip, position);
            soundAudio.clip = clip;
            soundAudio.Play ();
        }

        /// <summary>
        /// 停掉音效播放
        /// </summary>
        public void StopPlay () {
            soundAudio.Stop ();
            soundAudio.clip = null;
            // Util.ClearMemory ();
        }


        //继续播放背景音乐
        public void RePlay() {
            if (audio.clip) {
                audio.Play();
            }
        }

        //继续播放音效
        public void SoundRePlay() {
            if (soundAudio.clip) {
                soundAudio.Play();
            }
        }

        //暂停播放背景音乐
        public void Pause() {
            if (audio.isPlaying) {
                audio.Pause();
            }
        }

        //暂停播放音效
        public void SoundPause() {
            if (soundAudio.isPlaying) {
                soundAudio.Pause();
            }
        }


        //设置音乐量大小
        public void Volume (float number) {
            audio.volume = number;
        }

        //设置音效量大小
        public void SoundVolume (float number) {
            soundAudio.volume = number;
        }

        public double GetBgmVolume(){
            return audio.volume ;
        }
         public double GetSoundVolume(){
            return soundAudio.volume ;
        }
    }
}