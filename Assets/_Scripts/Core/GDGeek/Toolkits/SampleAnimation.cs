#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Core
{
    public class SampleAnimation : MonoBehaviour
    {
        public Image _image = null;
        public float _interval = 1.0f;
        public Sprite[] _sprites = null;
        private float allTime_;
        private int index_;
        // Update is called once per frame
        private void Update()
        {
            allTime_ += Time.deltaTime;
            while (allTime_ > _interval)
            {
                allTime_ -= _interval;
                index_++;
                index_ %= _sprites.Length;
                _image.sprite = _sprites[index_];
            }
        }
    }
}