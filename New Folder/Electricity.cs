using UnityEngine;
using System.Collections;

namespace BLINDED_AM_ME{

	[RequireComponent(typeof(Path_Comp))]
	public class Electricity : MonoBehaviour {

		public float strikeFrequency = 0.5f;

		[Range(0.01f, 1.0f)]
		public float zigZagIntensity = 5.0f;
		public float zigZagPerMeter = 5.0f;
		
		public LineRenderer[] lineRenderers;

		private int       _line_iterator = 0;
		private float     _strikeTracker = 0.0f;
		private Path_Comp _pathComp;

		void Reset(){

			GetComponent<Path_Comp>().isSmooth = false;
			GetComponent<Path_Comp>().isCircuit = false;

		}

		void OnValidate(){

			zigZagIntensity = Mathf.Clamp(zigZagIntensity, 0.01f, 100.0f);
			zigZagPerMeter = Mathf.Clamp(zigZagPerMeter, 0.01f, 1000.0f);
		}


		// Use this for initialization
		void Start () {

			_pathComp = GetComponent<Path_Comp>();
				
		}

		// Update is called once per frame
		void Update () {
		
			_strikeTracker += Time.deltaTime;
			if(_strikeTracker >= strikeFrequency){ // time for another
				_strikeTracker = 0.0f;


				CreateBolt(lineRenderers[_line_iterator]);

				lineRenderers[_line_iterator].GetComponent<Animator>().Play("Fade", 0, 0.0f);


				_line_iterator = (_line_iterator + 1) % lineRenderers.Length;
			}
		}

		private void CreateBolt(LineRenderer line){

			//lineObject.material.SetTextureScale("_MainTex", new Vector2(distance * zigZagPerMeter, 1.0f));
			//lineObject.numPositions = vertexCount;

			float totalDistance = _pathComp.TotalDistance;
			int   numPositions = Mathf.CeilToInt(totalDistance * zigZagPerMeter);
			Vector3[] points = new Vector3[numPositions];

			line.positionCount = numPositions;
			line.material.SetTextureScale("_MainTex", new Vector2(totalDistance * zigZagPerMeter, 1.0f));

			// set the ends
			points[0] = _pathComp.GetPathPoint(0.0f).point;
			points[numPositions-1] = _pathComp.GetPathPoint(totalDistance).point;


			Vector2 previousOffset = Vector2.zero;

			for(int i=1; i<numPositions-1; i++){

				Path_Point pathPoint = _pathComp.GetPathPoint(Math_Functions.Value_from_another_Scope(i,0,numPositions-1,0,totalDistance));


				Vector2 offset = new Vector2( Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));

				offset *= zigZagIntensity;
				previousOffset = offset;

				points[i] = pathPoint.point + (pathPoint.right * offset.x) + (pathPoint.up * offset.y);

			}

			line.SetPositions(points);
		
		}

	}
}