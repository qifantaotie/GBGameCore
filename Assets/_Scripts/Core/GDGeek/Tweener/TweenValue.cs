using System;
using UnityEngine;
using System.Collections;
namespace Core{
	public class TweenValue : Tween {

		public float from = 0f;
		public float to = 1f;
		public string fun = "fun";
		public GameObject receiver = null;

        //自己加的方法
        public Action<float> addValue = null; 
		//public GameObject obj = null;
		
		override protected void OnUpdate (float factor, bool isFinished)
		{	
			float val = from * (1f - factor) + to * factor;
		    if (receiver!=null)
		    {
                this.receiver.SendMessage(fun, val, SendMessageOptions.DontRequireReceiver);
		        
		    }

		    if (addValue != null)
		    {
                addValue(val);
		    }
			
		}

		/// <summary>
		/// Start the tweening operation.
		/// </summary>

		static public TweenValue Begin (GameObject go, float duration, float from, float to, GameObject receiver, string fun)
		{
			TweenValue comp = Tween.Begin<TweenValue>(go, duration);
			comp.from = from;
			comp.to = to;
			comp.fun = fun;
			comp.receiver = receiver;
			if (duration <= 0f)
			{
				comp.Sample(1f, true);
				comp.enabled = false;
			}
			return comp;
		}

        /// <summary>
        ///  利用委托增加数值
        /// </summary>
        /// <param name="go">需要改变的值</param>
        /// <param name="duration">延迟时间</param>
        /// <param name="from">开始值</param>
        /// <param name="to">结束值</param>
        /// <param name="addvalue">增加的方法</param>
        /// <returns></returns>
        static public TweenValue Begin(GameObject go, float duration, float from, float to, Action<float> addvalue )
        {
            TweenValue comp = Tween.Begin<TweenValue>(go, duration);
            comp.from = from;
            comp.to = to;
            comp.addValue = addvalue;
            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }
	}
}
