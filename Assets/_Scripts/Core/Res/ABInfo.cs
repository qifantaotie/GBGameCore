using UnityEngine;
using System.Collections;

public class ABInfo {
        /// <summary>
        /// 资源名
        /// </summary>
        public string AssetName { get; set; }

        /// <summary>
        /// 资源完整的名称（包含后缀名）
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// AB包名
        /// </summary>
        public string ABName { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_assetName">资源名称</param>
        /// <param name="_fullName">资源全名</param>
        /// <param name="_abName">资源包名</param>
        public ABInfo(string _assetName, string _fullName, string _abName)
        {
            this.ABName = _abName;
            this.AssetName = _assetName;
            this.FullName = _fullName;
        }
	
}
