using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
	/// <summary>
	/// Handles the Buildings Models and Construction Staging
	/// </summary>
	[System.Serializable]
	public class BuildingModelHandler : MonoBehaviour
	{
		public GameObject FullModel;
		public GameObject Stage1;
		public GameObject Stage2;
		public GameObject Stage3;
		public List<GameObject> Upgrades;
		public List<GameObject> OtherModels;

		/// <summary>
		/// Used for the start of Construction.
		/// </summary>
		public void OnConstructionStart()
		{
			foreach (GameObject gameObject in OtherModels)
				gameObject.SetActive(false);
			
			for(int i = 0; i < Upgrades.Count; i++)
				Upgrades[i].SetActive(false);		

			Stage1.gameObject.SetActive(true);
			FullModel.gameObject.SetActive(false);
			Stage3.gameObject.SetActive(false);
			Stage2.gameObject.SetActive(false);
		}

		/// <summary>
		/// Used to show first stage of construction.
		/// </summary>
		public void OnStage2()
		{
			Stage1.SetActive(false);
			Stage2.SetActive(true);
		}

		/// <summary>
		/// Used to show the second stage of construction.
		/// </summary>
		public void OnStage3()
		{
			Stage2.SetActive(false);
			Stage3.SetActive(true);
		}

		/// <summary>
		/// Used to show the full building model.
		/// </summary>
		public void OnFinishedConstruction()
		{
			Stage1.SetActive(false);
			Stage2.SetActive(false);
			Stage3.SetActive(false);
			FullModel.SetActive(true);

			for(int i = 0;i < Upgrades.Count;i++)
				Upgrades[i].SetActive(true);
		}
	}
}