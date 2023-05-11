using Target;
using UnityEngine;

namespace Combat 
{
    public class Projectile : MonoBehaviour 
	{
		public TargetableHealth Target { get; set; }
		public float MoveSpeed { get; set; }
		public int Damage { get; set; }

		private void OnHitTarget()
		{
			transform.position = Vector3.zero;
			Target.HealthHandler.TakeDamage(Damage, null);
			gameObject.SetActive(false);
		}

		private void Update()
		{
			if (Target == null)
				return;

			float sqrDistToTarget = Vector3.SqrMagnitude(transform.position - (Target.gameObject.transform.position + (Vector3.up * 2)));

			if (sqrDistToTarget <= 1f)
				OnHitTarget();

			transform.position = Vector3.MoveTowards(transform.position, Target.transform.position + (Vector3.up * 2), Time.deltaTime * MoveSpeed);

			Vector3 lookPos = Target.transform.position - transform.position;
            lookPos.y = 0;

			Quaternion rot = Quaternion.LookRotation(lookPos);
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 50);
		}
	}
}