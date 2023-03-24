using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        private static T s_Instance;

        /// <summary>
        /// singleton property
        /// </summary>
        public static T instance
        {
            get
            {

                if (s_Instance == null)
                {
                    s_Instance = GameObject.FindObjectOfType(typeof(T)) as T;

                    if (s_Instance == null)
                    {
                        GameObject gameObject = new GameObject(typeof(T).Name);
                        s_Instance = gameObject.AddComponent(typeof(T)) as T;
                        GameObject.DontDestroyOnLoad(gameObject);
                    }

                }

                return s_Instance;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (s_Instance)
                GameObject.Destroy(s_Instance);

            s_Instance = null;
        }
    }
