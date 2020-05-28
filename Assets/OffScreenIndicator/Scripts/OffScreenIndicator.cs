using UnityEngine;
using UnityEngine.VR;
using System.Collections;

/// <summary>
/// Off screen indicator.
/// Classic wrapper, user doesn't need to worry about implementation
/// </summary>
namespace Greyman{
	public class OffScreenIndicator : MonoBehaviour {

		public bool	enableDebug = true;
		public bool VirtualRealitySupported = false;
		public float VR_cameraDistance = 5;
		public float VR_radius = 1.8f;
		public float VR_indicatorScale = 0.1f;
		public GameObject canvas;
        public Camera renderCamera;
		public int Canvas_circleRadius = 5; //size in pixels
		public int Canvas_border = 10; // when Canvas is Square pixels in border
		public int Canvas_indicatorSize = 100; //size in pixels
		public Indicator[] indicators;
		public FixedTarget[] targets;
        private int startIndex;
        private int endIndex;
        public GameObject player;
        public GameObject startPoints;
        public GameObject endPoints;
        private string condition;
        //public 
        private OffScreenIndicatorManager manager;

		void Awake () {

            SettingOption();
            SettingRoute();

            if (condition == "C" || condition == "D")
            {
                if (VirtualRealitySupported)
                {
                    manager = gameObject.AddComponent<OffScreenIndicatorManagerVR>();
                    (manager as OffScreenIndicatorManagerVR).cameraDistance = VR_cameraDistance;
                    (manager as OffScreenIndicatorManagerVR).radius = VR_radius;
                    (manager as OffScreenIndicatorManagerVR).indicatorScale = VR_indicatorScale;
                    (manager as OffScreenIndicatorManagerVR).CreateIndicatorsParent();
                }
                else
                {
                    manager = gameObject.AddComponent<OffScreenIndicatorManagerCanvas>();
                    (manager as OffScreenIndicatorManagerCanvas).indicatorsParentObj = canvas;
                    (manager as OffScreenIndicatorManagerCanvas).circleRadius = Canvas_circleRadius;
                    (manager as OffScreenIndicatorManagerCanvas).border = Canvas_border;
                    (manager as OffScreenIndicatorManagerCanvas).indicatorSize = Canvas_indicatorSize;
                    (manager as OffScreenIndicatorManagerCanvas).renderCamera = renderCamera;
                }
                manager.indicators = indicators;
                manager.enableDebug = enableDebug;
                manager.CheckFields();

                // 是否加入參考點箭頭
                if (condition == "D")
                {
                    // 到摩天輪附近才加入箭頭
                    StartCoroutine(AddArrow());
                }
                // 只加入目的地箭頭
                else
                {
                    AddIndicator(targets[0].target, targets[0].indicatorID);
                }
                
            }
		}

        public IEnumerator AddArrow()
        {
            // 與摩天輪距離五十公尺內才顯示
            while (Vector3.Distance(new Vector3(-36.1f, 0, 27.6f), player.transform.position) > 62f)
            {
                yield return null;
            }
            AddIndicator(targets[0].target, targets[0].indicatorID);
        }

        private void SettingOption()
        {
            startIndex = VROption.startPoint;
            endIndex = VROption.endPoint;
            condition = VROption.condition;
        }

		public void AddIndicator(Transform target, int indicatorID){
			manager.AddIndicator(target, indicatorID);
		}

		public void RemoveIndicator(Transform target){
			manager.RemoveIndicator(target);
		}

        private void SettingRoute()
        {
            Vector3 startPos, endPos;
            startPos = startPoints.transform.Find(startIndex.ToString()).position;
            endPos = endPoints.transform.Find(endIndex.ToString()).position;
            VROption.endPos = endPos;

            // 更改Destination位置
            targets[0].target = endPoints.transform.Find(endIndex.ToString());
        }

    }

	/// <summary>
	/// Indicator.
	/// References and colors for indicator sprites
	/// </summary>
	[System.Serializable]
	public class Indicator{
		public Sprite onScreenSprite;
		public Color onScreenColor = Color.white;
		public bool onScreenRotates;
		public Sprite offScreenSprite;
		public Color offScreenColor = Color.white;
		public bool offScreenRotates;
		public Vector3 targetOffset;
		/// <summary>
		/// Both sprites need to have the same transition
		/// aswell both sprites need to have the same duration.
		/// </summary>
		public Transition transition;
		public float transitionDuration = 1;
		[System.NonSerialized]
		public bool showOnScreen;
		[System.NonSerialized]
		public bool showOffScreen;

		public enum Transition{
			None,
			Fading,
			Scaling
		}
	}

	[System.Serializable]
	public class FixedTarget{
		public Transform target;
		public int indicatorID;
	}
}