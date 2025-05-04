using Managers;
using UnityEngine;
using Utils;

namespace Buildings
{
	/// <summary>
	/// Handles the resource models for a building that holds resources.
	/// </summary>
	public class BuildingResourceModelHandler : MonoBehaviour
	{
		public GameObject EmptyModel;
		public GameObject HalfFullModel;
		public GameObject FullModel;
		private BuildingBase _building;

		/// <summary>
		/// The current status of the storage.
		/// </summary>
		private StorageStatus _status = StorageStatus.Full;

		// Required Components.
		private ResourceStorageModifier _resourceModifier;

		/// <summary>
		/// Hides all resource models.
		/// </summary>
		public void HideAll()
		{
			EmptyModel.SetActive(false);
			HalfFullModel.SetActive(false);
			FullModel.SetActive(false);
		}

		/// <summary>
		/// Shows the empty resource model.
		/// </summary>
		public void OnEmpty()
		{
			EmptyModel.SetActive(true);
			HalfFullModel.SetActive(false);
			FullModel.SetActive(false);
		}

		/// <summary>
		/// Shows the half full resource model.
		/// </summary>
		public void OnHalfFull()
		{
			EmptyModel.SetActive(false);
			HalfFullModel.SetActive(true);
			FullModel.SetActive(false);
		}

		/// <summary>
		/// Shows the full resource model.
		/// </summary>
		public void OnFull()
		{
			EmptyModel.SetActive(false);
			HalfFullModel.SetActive(false);
			FullModel.SetActive(true);
		}

		/// <summary>
		/// Called via the resource change event.
		/// </summary>
		/// <param name="status"></param>
		private void HandleResourceChange(StorageStatus status)
		{
			// If the status has not changed, do nothing and return.
			if (status == _status)
				return;

			// If the building is still under construction, do nothing and return.
			if (!_building.Built)
				return;

			// Enable appropriate resource model based on status.
			switch (status)
			{
				case StorageStatus.Empty:
					OnEmpty();
					break;
				case StorageStatus.HalfFull:
					OnHalfFull();
					break;
				case StorageStatus.Full:
					OnFull();
					break;
			}
		}

		// Unity Functions.
		private void Awake()
		{
			HideAll();
			_building = GetComponentInParent<BuildingBase>();
			_resourceModifier = GetComponentInParent<ResourceStorageModifier>();
		}

		private void Start()
		{
			// Subscribe to resource change event based on resource type
			GameManager.Instance.TownResourceManager.GetResourceChangeEvent(_resourceModifier.ResourceType).AddListener(HandleResourceChange);
		}
	}
}