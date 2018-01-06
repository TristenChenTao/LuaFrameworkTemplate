using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using UnityEngine;

namespace LuaFramework {
    public class SoundManager : Manager {
        private AudioSource audio;
        private Hashtable sounds = new Hashtable ();

        void Start () {
            audio = GetComponent<AudioSource> ();
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
                    Add (url, ac);
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
            if (canPlay) {
                audio.loop = true;
                audio.clip = LoadAudioClip (name);
                audio.Play ();
            } else {
                audio.Stop ();
                audio.clip = null;
                Util.ClearMemory ();
            }
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
            AudioSource.PlayClipAtPoint (clip, position);
        }
    }
}