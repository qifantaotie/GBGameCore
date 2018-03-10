#region

using UnityEngine;

#endregion

namespace Core
{
    public class GlobalUI : MonoBehaviour
    {
        private static GlobalUI instance_;
        public LoadingWindow _loading = null;

        public LoadingWindow loading
        {
            get { return _loading; }
        }

        public static GlobalUI GetInstance()
        {
            return instance_;
        }

        private void Awake()
        {
            instance_ = this;
        }
    }
}