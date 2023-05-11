using Character;
using UnityEngine;

namespace Audio
{
	public class PlayerAudioHandler : AudioHandler
	{
		private RoleHandler _roleHandler;

		protected override void Initialize()
		{
			base.Initialize();
			_roleHandler = GetComponentInParent<RoleHandler>();
		}
		public void PlayRoleActionAudio()
		{
			PlayerRoleData roleData = _roleHandler.PlayerRoleData;

			if (roleData.ActionClips == null)
			{
				Debug.LogWarning($"Missing action audio clip for {_roleHandler.CurrentRole}");
				return;
			}

			PlayClip(roleData.ActionClips[Random.Range(0, roleData.ActionClips.Length)]);
		}

	}
}