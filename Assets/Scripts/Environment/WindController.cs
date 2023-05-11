using UnityEngine;

namespace Environment
{
    /// <summary>
    /// Wind Controller will modify the wind direction and strength in each asigned material
    /// </summary>
    public class WindController : MonoBehaviour
    {
        [SerializeField]
        private Material[] _materials;

        [SerializeField, Space(10), Range(0.0f, 5.0f)]
        private float _speed = 1.0f;

        [SerializeField]
        private float _textureSize = 1.0f;
        //Converts Radian to Vector2
        private static Vector2 RadianToVector2(float _radian)
        {
            return new Vector2(Mathf.Cos(_radian), Mathf.Sin(_radian));
        }
      
        //Converts objects rotation ("_degree") to a vector 2
        private static Vector2 DegreeToVector2(float _degree)
        {
            return RadianToVector2(_degree * Mathf.Deg2Rad);
        }

        //Align materials wind direction function
        public void AlignWind()
        {
            //Finds the objects rotation then converts it to a vector 2
            Vector2 _direction = DegreeToVector2(transform.eulerAngles.y);
            if (_materials.Length > 0)
            {
                for (int i = 0; i < _materials.Length; i++)
                {
                    //Changes each material wind direction
                    _materials[i].SetVector("_windDirection", _direction);
                }
            }
        }

        //Changes materials wind strength
        public void ChangeWindStrength()
        {
            for (int i = 0; i < _materials.Length; i++)
            {
                //Changes the winds strength in the material
                _materials[i].SetFloat("_windStrength", _speed);
            }
        }

        public void ChangeWindTextureSize()
        {
            for (int i = 0; i < _materials.Length; i++)
            {
                //Changes the winds strength in the material
                _materials[i].SetFloat("_textureSize", _textureSize);
            }
        }

        //Used to display the direction of the wind
        private void OnDrawGizmos()
        {
            //Middle line
            Gizmos.DrawLine(new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z), transform.localPosition + transform.forward);
            //Top line
            Gizmos.DrawLine(new Vector3(transform.localPosition.x, transform.localPosition.y + 0.1f, transform.localPosition.z), transform.localPosition + transform.forward);
            //Bottom line
            Gizmos.DrawLine(new Vector3(transform.localPosition.x, transform.localPosition.y - 0.1f, transform.localPosition.z), transform.localPosition + transform.forward);
        }
    }
}