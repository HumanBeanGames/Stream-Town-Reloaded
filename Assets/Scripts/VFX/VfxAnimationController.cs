using UnityEngine;
using UnityEngine.VFX;
namespace VFX 
{
    public class VfxAnimationController : MonoBehaviour 
	{
		[SerializeField]
		private VisualEffect _vfx;
		
		[SerializeField]
		private bool _destoryAfterAnimation;
		
		[SerializeField]
		private float _destoryTime;

		public void Play()
		{
			Debug.Log("Playing " + _vfx.name);
			_vfx.Play();
		}

		public void Stop()
		{
			Debug.Log("Stopping " + _vfx.name);
			_vfx.Stop();
		}

		private void Awake()
		{
			_vfx = GetComponent<VisualEffect>();
			_vfx.playRate = 2f;
			if(_destoryAfterAnimation)
			{
				Destroy(gameObject, _destoryTime);
			}
		}
	}
}