using Animation;
using SavingAndLoading.Structs;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Character
{
	/// <summary>
	/// Handles the models and visuals of a player character.
	/// </summary>
	public class CharacterModelHandler : MonoBehaviour
	{
		[SerializeField] private Transform _modelTransform;
		public int ChosenEyeIndex;
		public int ChosenHairIndex;
		public int ChosenFacialHairIndex;
		public int ChosenEquipmentSetIndex;
		public int ChosenBodyTypeIndex;
		public int chosenHairColorIndex;
		public int ChosenEyeColorIndex;
		public int ChosenSkinColorIndex;

		private bool _hairEnabled = true;

		/// <summary>
		/// Holds all eye models.
		/// </summary>
		[SerializeField]
		private List<GameObject> _eyes = new List<GameObject>();

		/// <summary>
		/// Holds all hair models.
		/// </summary>
		[SerializeField]
		private List<GameObject> _hairs = new List<GameObject>();

		/// <summary>
		/// Holds all facial hair models.
		/// </summary>
		[SerializeField]
		private List<GameObject> _facialHairs = new List<GameObject>();

		/// <summary>
		/// Holds data for each role's equipment set.
		/// </summary>
		[SerializeField]
		private List<RoleEquipment> _equipmentSets;

		private AnimationHandler _animationHandler;
		private RoleHandler _roleHander;

		/// <summary>
		/// All colors used for character skin.
		/// </summary>
		[SerializeField]
		private List<Color> _skinColors = new List<Color>();

		/// <summary>
		/// All colors used for character hair and facial hair.
		/// </summary>
		[SerializeField]
		private List<Color> _hairColors = new List<Color>();

		/// <summary>
		/// All colors used for character eyes.
		/// </summary>
		[SerializeField]
		private List<Color> _eyeColors = new List<Color>();

		/// <summary>
		/// The current body type of the character.
		/// </summary>
		private BodyType _currentBodyType = BodyType.Slim;

		private List<Renderer> _hairRenderers;
		private List<Renderer> _eyeRenderers;

		// Properties.
		public Transform ModelTransform => _modelTransform;
		public BodyType CurrentBodyType => _currentBodyType;

		public PlayerCustomizationSaveData ToSaveData()
		{
			return new PlayerCustomizationSaveData(ChosenEyeIndex, ChosenHairIndex, ChosenFacialHairIndex, ChosenSkinColorIndex, chosenHairColorIndex, ChosenEyeColorIndex, ChosenBodyTypeIndex);
		}

		public void LoadFromSaveData(PlayerCustomizationSaveData data)
		{
			SetBodyTypeByIndex(data.ChosenBodyTypeIndex + 1);
			SetHairByIndex(data.ChosenHairIndex + 1);
			SetHairColorByIndex(data.ChosenHairColourIndex + 1);
			SetEyeColorByIndex(data.ChosenEyeColourIndex + 1);
			SetEyesByIndex(data.ChosenEyeColourIndex + 1);
			SetFacialHairByIndex(data.ChosenFacialHairIndex + 1);
			// TODO: set skin color
		}

		/// <summary>
		/// Changes the player's currently active role if possible.
		/// </summary>
		/// <param name="prevRole"></param>
		/// <param name="newRole"></param>
		/// <param name="bodyType"></param>
		public void SwitchRole(PlayerRole prevRole, PlayerRole newRole, BodyType bodyType)
		{
			DisableEquipment(GetEquipmentByRole(prevRole));
			EnableEquipment(GetEquipmentByRole(newRole), bodyType);
		}

		/// <summary>
		/// Switches the player's current body type.
		/// </summary>
		/// <param name="role"></param>
		/// <param name="bodyType"></param>
		public void SwitchBodyType(PlayerRole role, BodyType bodyType)
		{
			SwitchBodyType(GetEquipmentByRole(role), role, bodyType);
		}

		/// <summary>
		/// Switches the player's current body type.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="role"></param>
		/// <param name="bodyType"></param>
		public void SwitchBodyType(RoleEquipment e, PlayerRole role, BodyType bodyType)
		{
			e.BodieBulk.SetActive(false);
			e.BodieFeminine.SetActive(false);
			e.BodieSlim.SetActive(false);

			switch (bodyType)
			{
				case BodyType.Slim:
					e.BodieSlim.SetActive(true);
					break;
				case BodyType.Bulk:
					e.BodieBulk.SetActive(true);
					break;
				case BodyType.Feminine:
					e.BodieFeminine.SetActive(true);
					break;
				default:
					break;
			}

			_currentBodyType = bodyType;
			ChosenBodyTypeIndex = (int)bodyType;
		}

		/// <summary>
		/// Sets the body type of the current role based on the index.
		/// </summary>
		/// <param name="index"></param>
		public bool SetBodyTypeByIndex(int index)
		{
			int adjustedIndex = index - 1;
			if (adjustedIndex < 0 || adjustedIndex >= (int)BodyType.Count)
				return false;

			SwitchBodyType(_roleHander.CurrentRole, (BodyType)adjustedIndex);
			ChosenBodyTypeIndex = adjustedIndex;
			return true;
		}

		/// <summary>
		/// Returns the equipment set for the role.
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public RoleEquipment GetEquipmentByRole(PlayerRole role)
		{
			for (int i = 0; i < _equipmentSets.Count; i++)
			{
				if (_equipmentSets[i].PlayerRole == role)
				{
					return _equipmentSets[i];
				}
			}
			return null;
		}

		/// <summary>
		/// Enables to visual carry object of a role if it exists.
		/// </summary>
		/// <param name="role"></param>
		/// <param name="withAnimation"></param>
		public void EnableCarry(PlayerRole role, bool withAnimation = true)
		{
			RoleEquipment equipment = GetEquipmentByRole(role);
			if (equipment.LeftHand == null || equipment.LeftHandPermanent)
				return;

			equipment.LeftHand.SetActive(true);

			if (equipment.HasCarryAnimation && withAnimation)
				_animationHandler.SetBool(equipment.CarryAnimation, true);
		}

		/// <summary>
		/// Disables the visual carry object of a role if it exists.
		/// </summary>
		/// <param name="role"></param>
		/// <param name="withAnimation"></param>
		public void DisableCarry(PlayerRole role, bool withAnimation = true)
		{
			RoleEquipment equipment = GetEquipmentByRole(role);
			if (equipment.LeftHand == null || equipment.LeftHandPermanent)
				return;

			equipment.LeftHand.SetActive(false);

			if (equipment.HasCarryAnimation && withAnimation)
				_animationHandler.SetBool(equipment.CarryAnimation, false);
		}

		/// <summary>
		/// Enables the current role's carry object if it exists.
		/// </summary>
		/// <param name="withAnimation"></param>
		public void EnableCarry(bool withAnimation = true)
		{
			EnableCarry(_roleHander.CurrentRole, withAnimation);
		}

		/// <summary>
		/// Disables the current role's carry object if it exists.
		/// </summary>
		/// <param name="withAnimation"></param>
		public void DisableCarry(bool withAnimation = true)
		{
			DisableCarry(_roleHander.CurrentRole, withAnimation);
		}

		/// <summary>
		/// Sets the hair to active if index is valid.
		/// </summary>
		/// <param name="index"></param>
		public bool SetHairByIndex(int index)
		{
			int adjustedIndex = index - 1;

			if (adjustedIndex >= _hairs.Count || adjustedIndex < -1)
				return false;

			// Disable all hairs.
			for (int i = 0; i < _hairs.Count; i++)
			{
				_hairs[i].SetActive(false);
			}

			if (adjustedIndex == -1)
				return false;

			if (_hairEnabled)
				_hairs[adjustedIndex].SetActive(true);
			ChosenHairIndex = adjustedIndex;

			return true;
		}

		/// <summary>
		/// Sets the eyes to active if index is valid.
		/// </summary>
		/// <param name="index"></param>
		public bool SetEyesByIndex(int index)
		{
			int adjustedIndex = index - 1;

			if (adjustedIndex >= _eyes.Count || adjustedIndex < -1)
				return false;

			// Disable all eyes.
			for (int i = 0; i < _eyes.Count; i++)
			{
				_eyes[i].SetActive(false);
			}

			if (adjustedIndex == -1)
				return false;

			_eyes[adjustedIndex].SetActive(true);
			ChosenEyeIndex = adjustedIndex;
			return true;
		}

		/// <summary>
		/// Sets the facial hair to active if index is valid.
		/// </summary>
		/// <param name="index"></param>
		public bool SetFacialHairByIndex(int index)
		{
			int adjustedIndex = index - 1;

			if (adjustedIndex >= _facialHairs.Count || adjustedIndex < -1)
				return false;

			// Disable all facial hairs.
			for (int i = 0; i < _facialHairs.Count; i++)
			{
				_facialHairs[i].SetActive(false);
			}

			if (adjustedIndex == -1)
				return false;

			_facialHairs[adjustedIndex].SetActive(true);
			ChosenFacialHairIndex = adjustedIndex;
			return true;
		}

		/// <summary>
		/// Sets the Players Hair and Facial Hair color.
		/// </summary>
		/// <param name="index"></param>
		public bool SetHairColorByIndex(int index)
		{
			int adjustedIndex = index - 1;

			if (adjustedIndex < 0 || adjustedIndex >= _hairColors.Count)
				return false;

			MaterialPropertyBlock matBlock = new MaterialPropertyBlock();

			for (int i = 0; i < _hairRenderers.Count; i++)
			{
				if (_hairRenderers[i] == null)
					continue;

				_hairRenderers[i].GetPropertyBlock(matBlock);
				matBlock.SetColor("_albedoColor", _hairColors[adjustedIndex]);
				_hairRenderers[i].SetPropertyBlock(matBlock);
			}
			chosenHairColorIndex = adjustedIndex;

			return true;

		}

		/// <summary>
		/// Sets the Player's Eye Color by index.
		/// </summary>
		/// <param name="index"></param>
		public bool SetEyeColorByIndex(int index)
		{
			int adjustedIndex = index - 1;

			if (adjustedIndex < 0 || adjustedIndex >= _eyeColors.Count)
				return false;

			MaterialPropertyBlock matBlock = new MaterialPropertyBlock();

			for (int i = 0; i < _eyeRenderers.Count; i++)
			{
				if (_eyeRenderers[i] == null)
					continue;

				_eyeRenderers[i].GetPropertyBlock(matBlock);
				matBlock.SetColor("_albedoColor", _eyeColors[adjustedIndex]);
				_eyeRenderers[i].SetPropertyBlock(matBlock);
			}
			ChosenEyeColorIndex = adjustedIndex;

			return true;

		}

		/// <summary>
		/// Enables a role's equipment.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="bodyType"></param>
		private void EnableEquipment(RoleEquipment e, BodyType bodyType)
		{
			if (e != null)
				SwitchBodyType(e.PlayerRole, bodyType);

			if (e.Helmet)
			{
				e.Helmet.SetActive(true);
				_hairEnabled = false;
				_hairs[ChosenHairIndex].SetActive(false);
			}
			else
			{
				_hairEnabled = true;
				_hairs[ChosenHairIndex].SetActive(true);
			}

			if (e.LeftHand && e.LeftHandPermanent)
				e.LeftHand.SetActive(true);
			if (e.RightHand)
				e.RightHand.SetActive(true);
		}

		/// <summary>
		/// Disable a role's equipment.
		/// </summary>
		/// <param name="e"></param>
		private void DisableEquipment(RoleEquipment e)
		{
			if (e != null)
			{
				e.BodieBulk.SetActive(false);
				e.BodieSlim.SetActive(false);
				e.BodieFeminine.SetActive(false);

				if (e.Helmet)
					e.Helmet.SetActive(false);
				if (e.LeftHand)
					e.LeftHand.SetActive(false);
				if (e.RightHand)
					e.RightHand.SetActive(false);
			}
		}

		private void Init()
		{
			for (int i = 0; i < _equipmentSets.Count; i++)
			{
				DisableEquipment(_equipmentSets[i]);
			}
		}

		// Untiy Functions.
		private void Awake()
		{
			Init();

			_animationHandler = GetComponentInChildren<AnimationHandler>();
			_roleHander = GetComponent<RoleHandler>();

			_hairRenderers = new List<Renderer>();

			for (int i = 0; i < _hairs.Count; i++)
			{
				_hairRenderers.Add(_hairs[i].GetComponent<Renderer>());
			}

			for (int i = 0; i < _facialHairs.Count; i++)
			{
				_hairRenderers.Add(_facialHairs[i].GetComponent<Renderer>());
			}

			_eyeRenderers = new List<Renderer>();

			for (int i = 0; i < _eyes.Count; i++)
			{
				_eyeRenderers.Add(_eyes[i].GetComponent<Renderer>());
			}

			SetHairByIndex(Random.Range(0, _hairs.Count));
			SetBodyTypeByIndex(Random.Range(0, 3));
			SetEyesByIndex(Random.Range(0, _eyes.Count));
			SetHairColorByIndex(Random.Range(0, _hairColors.Count));
			SetFacialHairByIndex(Random.Range(0, _facialHairs.Count));
			SetEyeColorByIndex(Random.Range(0, _eyeColors.Count));
		}

#if UNITY_EDITOR
		public void AddEquipmentSet(RoleEquipment e)
		{
			_equipmentSets.Add(e);
		}

		public void ResetEquipmentSet()
		{
			_equipmentSets = new List<RoleEquipment>();
		}
#endif
	}
}