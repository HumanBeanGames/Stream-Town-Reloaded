using Combat;
using Managers;
using Sensors;
using Target;
using UnityEngine;

namespace Buildings
{
	public class ProjectileShooter : MonoBehaviour
	{
		[SerializeField]
		private string ProjectilePoolName;

		[SerializeField]
		private float _moveSpeed;

		[SerializeField]
		private int _damage;

		[SerializeField]
		private float _range = 10f;

		[SerializeField]
		private float _fireRate;
		private float _timeUntilAttack;

		private TargetSensor _targetSensor;

		private void Start()
		{
			_targetSensor = GetComponent<TargetSensor>();
			_timeUntilAttack = _fireRate;
		}

		private void Update()
		{
			_timeUntilAttack -= Time.deltaTime;

			if (_timeUntilAttack <= 0)
			{

				if (!_targetSensor.HasTarget)
					return;
				_timeUntilAttack = _fireRate;

				if (Vector3.SqrMagnitude(transform.position - _targetSensor.CurrentTarget.transform.position) > (_range * _range))
				{
					_targetSensor.ClearTarget();
					return;
				}

				Projectile proj = GameManager.Instance.PoolingManager.GetPooledObject(ProjectilePoolName, true).GetComponent<Projectile>();
				proj.gameObject.transform.position = transform.position;
				proj.Damage = _damage;
				proj.MoveSpeed = _moveSpeed;
				proj.Target = (TargetableHealth)_targetSensor.CurrentTarget;
				proj.gameObject.SetActive(true);
			}
		}

	}
}