using System;
using Target;
using Units;
using UnityEngine;

namespace Buildings 
{
    public class BuildingDamageMaterialHandler : MonoBehaviour 
	{
		private BuildingBase _building;
		private HealthHandler _healthHandler;
		private Renderer _renderer;
		private MaterialPropertyBlock _materialPropertyBlock;
		private bool _initialized = false;

		private void Awake()
		{
			_renderer = GetComponent<Renderer>();
			_building = GetComponentInParent<BuildingBase>();
			_building.DamageHandler = this;
			_healthHandler = GetComponentInParent<HealthHandler>();
			_healthHandler.OnHealthChange += OnHealthChanged;
			_materialPropertyBlock = new MaterialPropertyBlock();
			_initialized = true;
			SetDamageByPercentage(_healthHandler.HealthPercentage);
		}

		private void OnEnable()
		{
			if (!_initialized)
				return;

			SetDamageByPercentage(_healthHandler.HealthPercentage);
		}

		public void OnHealthChanged(HealthHandler obj)
		{
			if (_building.BuildingState == Utils.BuildingState.Construction)
				return;

			SetDamageByPercentage(obj.HealthPercentage);
		}

        private void SetDamageByPercentage(float percentage)
        {
            if (_materialPropertyBlock == null)
                _materialPropertyBlock = new MaterialPropertyBlock();

            if (_renderer == null)
                _renderer = GetComponent<Renderer>();

            _renderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetFloat("_DestructionValue", percentage);
            _renderer.SetPropertyBlock(_materialPropertyBlock);
        }

    }
}