using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Offscreen indicator manager VR.
/// Implementation of OffscreenIndicatorManager for Canvas.
/// </summary>
namespace Greyman{
	public class OffScreenIndicatorManagerCanvas : OffScreenIndicatorManager {
		
		//public Indicator[] indicators = new Indicator[3];
		public GameObject indicatorsParentObj;
		public float cameraDistance = 5; //default distance
		public int circleRadius = 100;
		public int border = 10;
		public int indicatorSize = 100;
        public Camera renderCamera;
        private GameObject distanceText;
		private float realBorder;
		private Vector2 referenceResolution;
		private float screenScaleX;
		private float screenScaleY;
		private bool screenScaled =false;
        private Canvas canvas;
		
		void Start (){
			//Create empty transform
			if(indicatorsParentObj == null || indicatorsParentObj.GetComponent<Canvas>() == null){
				Debug.LogError("OffScreenIndicator Canvas field requieres a Canvas GameObject");
				Debug.Break();
			}

            canvas = indicatorsParentObj.GetComponent<Canvas>();

            /*
            //Get Reference resolution to obtain scale
            if (indicatorsParentObj.GetComponent<CanvasScaler>().uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize){
				referenceResolution = indicatorsParentObj.GetComponent<CanvasScaler>().referenceResolution;
				Vector2 screenResolution = new Vector2(Screen.width, Screen.height);
				screenScaleX = screenResolution.x / referenceResolution.x;
				screenScaleY = screenResolution.y / referenceResolution.y;
				screenScaled = true;
				screenScaled = false;

				Debug.Log("ReferenceResolution = " + referenceResolution.ToString());
				Debug.Log("ScreenResolution = " + screenResolution.ToString());
				Debug.Log("ScreenScaleX = " + screenScaleX.ToString());
				Debug.Log("ScreenScaleY = " + screenScaleY.ToString());
			} else {
				screenScaled = false;
			}
            */
			//indicatorsSize depends on screen scale

			if(screenScaled){
				indicatorSize = Mathf.RoundToInt(indicatorSize * screenScaleX);
			}
			realBorder = (indicatorSize/2f) + border;
		}
		
		void LateUpdate(){
			//update enemies arrows
			foreach(ArrowIndicator arrowIndicator in arrowIndicators){
				UpdateIndicatorPosition(arrowIndicator);
				arrowIndicator.UpdateEffects();
			}
		}
		
		public override void AddIndicator(Transform target, int indicatorID){
			if(indicatorID >= indicators.Length){
				Debug.LogError("Indicator ID not valid. Check Off Screen Indicator Indicators list.");
				return;
			}
			if (!ExistsIndicator(target)){
				ArrowIndicatorCanvas newArrowIndicator = new ArrowIndicatorCanvas();
				newArrowIndicator.target = target;
				newArrowIndicator.arrow = new GameObject();
				newArrowIndicator.arrow.transform.SetParent(indicatorsParentObj.transform);
				newArrowIndicator.arrow.name = "Indicator";
                newArrowIndicator.arrow.transform.SetAsLastSibling();
				newArrowIndicator.arrow.transform.localScale = Vector3.one;
				newArrowIndicator.arrow.AddComponent<Image>();
				newArrowIndicator.indicator = indicators[indicatorID];
				newArrowIndicator.arrow.GetComponent<Image>().sprite = newArrowIndicator.indicator.offScreenSprite;
				newArrowIndicator.arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(indicatorSize, indicatorSize);
				newArrowIndicator.arrow.GetComponent<Image>().color = newArrowIndicator.indicator.offScreenColor;
				if(!newArrowIndicator.indicator.showOffScreen){
					newArrowIndicator.arrow.SetActive(false);
				}

				newArrowIndicator.onScreen = false;
				arrowIndicators.Add(newArrowIndicator);
			} else {
				Debug.LogWarning ("Target already added: " + target.name);
			}
		}

		public override void RemoveIndicator(Transform target){
			if(ExistsIndicator(target)){
				ArrowIndicator oldArrowTarget = arrowIndicators.Find(x=>x.target == target);
				int id = arrowIndicators.FindIndex(x=>x.target == target);
				arrowIndicators.RemoveAt(id);
				GameObject.Destroy(oldArrowTarget.arrow);
				ArrowIndicator.Destroy(oldArrowTarget);
			} else {
				Debug.LogWarning ("Target no longer exists: " + target.name);
			}
		}
		
		protected override void UpdateIndicatorPosition(ArrowIndicator arrowIndicator, int id = 0){
			Vector3 v2DPos = renderCamera.WorldToScreenPoint(arrowIndicator.target.localPosition + arrowIndicator.indicator.targetOffset);
			float angle;
			bool behindCamera;
            RectTransform objectRectTransform = canvas.GetComponent<RectTransform>();
            //Debug.Log("width: " + objectRectTransform.rect.width + ", height: " + objectRectTransform.rect.height);
            float width = objectRectTransform.rect.width;
            float height = objectRectTransform.rect.height;

            Vector3 heading = arrowIndicator.target.position - renderCamera.transform.position;
			behindCamera = (Vector3.Dot(renderCamera.transform.forward, heading) < 0);

			if(v2DPos.x > width - realBorder || v2DPos.x < realBorder || v2DPos.y > height - realBorder || v2DPos.y < realBorder || behindCamera){
				//Debug.Log ("OUT OF SCREEN");
				arrowIndicator.onScreen = false;
				//Cut position on the sides
				angle = Mathf.Atan2(v2DPos.y - (height/2), v2DPos.x - (width/2));
				float xCut, yCut;
				if(v2DPos.x - width/2 > 0){
					//Right side
					xCut = width/2 - realBorder;
					yCut = xCut * Mathf.Tan(angle);
				} else {
					//Left side
					xCut = -width/2 + realBorder;
					yCut = xCut * Mathf.Tan(angle);
				}
				//Check cut position up and down
				if(yCut > height/2 - realBorder){
					//Up
					yCut = height/2 - realBorder;
					xCut = yCut / Mathf.Tan(angle);
				}
				if(yCut < -height/2 + realBorder){
					//Down
					yCut = -height/2 + realBorder;
					xCut = yCut / Mathf.Tan(angle);
				}
				if(behindCamera){
					xCut = -xCut;
					yCut = -yCut;
				}
				if(screenScaled){
					xCut /= screenScaleX;
					yCut /= screenScaleY;
				}
				arrowIndicator.arrow.transform.localPosition = new Vector3(xCut, yCut, 0);
                arrowIndicator.arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(indicatorSize, indicatorSize);


            } else {
				//Debug.Log ("INSIDE OF SCREEN");
				arrowIndicator.onScreen = true;
				float xScaled = v2DPos.x - (width/2);
				float yScaled = v2DPos.y - (height/2);
                // Debug.Log(xScaled);

                // 避免tag跑出視窗外
                xScaled = Mathf.Clamp(xScaled, -170, 170);

				if(screenScaled){
					xScaled /= screenScaleX;
					yScaled /= screenScaleY;
				}
				arrowIndicator.arrow.transform.localPosition = new Vector3(xScaled, yScaled, 0);
                // 寬度變長來顯示tag
                arrowIndicator.arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(indicatorSize*3, indicatorSize);
            }

            Quaternion textRotation = new Quaternion(0,0,0,0);

            // 增加距離文字,如果文字還不存在
            if (arrowIndicator.arrow.GetComponent<Transform>().Find("DistanceText(Clone)") == null)
            {
                GameObject distanceText = (GameObject)Resources.Load("DistanceText");
                Color parentColor = arrowIndicator.arrow.GetComponent<Image>().color;
                distanceText = Instantiate(distanceText);
                distanceText.transform.SetParent(arrowIndicator.arrow.transform);
                distanceText.transform.localPosition = new Vector3(0, -90, 0);
                distanceText.transform.localScale = new Vector3(1, 1, 1);
                textRotation = distanceText.transform.rotation;
                Text myText = distanceText.GetComponent<Text>();
                myText.color = parentColor;
            }
            else
            {
                Transform myTextTransform = arrowIndicator.arrow.GetComponent<Transform>().Find("DistanceText(Clone)");
                Text myText = myTextTransform.GetComponent<Text>();
                myTextTransform.rotation = textRotation;
                myText.text = Mathf.FloorToInt(Vector3.Distance(renderCamera.transform.position, arrowIndicator.target.position)) + "m";
            }

            //rotatearrow
            if ((arrowIndicator.onScreen && arrowIndicator.indicator.onScreenRotates) || (!arrowIndicator.onScreen && arrowIndicator.indicator.offScreenRotates)){
				if(behindCamera){
					angle = Mathf.Atan2(-(v2DPos.y - (height/2)), -(v2DPos.x - (width/2)));
				} else {
					angle = Mathf.Atan2(v2DPos.y - (height/2), v2DPos.x - (width/2));
				}
			} else {
				angle = 90 * Mathf.Deg2Rad;
			}
			arrowIndicator.arrow.transform.localEulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg - 90);
		}
	}
}