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
        public Transform transform;

        private void UpdateParticlePosition()
        {
            _visualEffect.SetVector3("particlePosition", new Vector3(transform.position.x, base.transform.position.y, transform.position.z));
        }

        private void Update()
        {
            UpdateParticlePosition();
        }
    }
}