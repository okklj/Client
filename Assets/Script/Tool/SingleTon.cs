﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MySingleTon
{
    public class SingleTon<T> : MonoBehaviour where T : SingleTon<T>//约束
    {
        private static T instance;
        public static T Instance
        {
            get { return instance; }
        }

        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = (T)this;
            }
        }

        public static bool IsInitialized
        {
            get { return instance != null; }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }


    }

}