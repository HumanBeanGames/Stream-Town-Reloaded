using UnityEngine;
using UnityEngine.VFX;

namespace VFX 
{
    /// <summary>
    /// Will set the secondary effect to the correct position in world space
    /// </summary>
    public class VfxParticlePosition : MonoBehaviour 
	{
        [SerializeField]
        private VisualEffect _visualEffect;
        [SerializeField]
        private Transform _transform;

        private void UpdateParticlePosition()
        {
            _visualEffect.SetVector3("particlePosition", new Vector3(_transform.position.x, transform.position.y, _transform.position.z));
        }

        private void Update()
        {
            UpdateParticlePosition();
        }
    }
}