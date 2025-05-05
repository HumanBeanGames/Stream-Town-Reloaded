using GridSystem.Partitioning;
using GUIDSystem;
using Managers;
using UnityEngine;
using UserInterface;
using Utils;

namespace Target
{
	/// <summary>
	/// Base class for all Targetable objects in the game
	/// </summary>
	public class Targetable : MonoBehaviour
	{
		[SerializeField]
		protected TargetMask _targetType;
		[SerializeField]
		protected bool _updatePartitionIndex = false;
		[SerializeField]
		protected float _partitionUpdateRate = 3f;
		protected float _partitionUpdateTime = 0;
		protected int _cellIndex = -1;

		[SerializeField, Tooltip("If false, use the box colliders bounds. This is used for determining how close a unit should get to the target.")]
		private bool _useCustomSize = false;
		[SerializeField]
		private float _customSize = 0;
		protected BoxCollider _boxCollider;
		protected float _sizeSqr;

		[SerializeField, Tooltip("Cost for each additional unit assigned to this target.")]
		private float _assignmentPenaltyMod = 15;
		[SerializeField, Tooltip("Cost per distance unit.")]
		private float _distancePenaltyMod = 0.5f;
		private int _currentAssignedCount = 0;

		protected bool _wasPooled = false;

		[SerializeReference]
		private Transform _textDisplayTransform;
		protected Transform _transform;

		private GUIDComponent _gUIDComponent;

		// Properties
		public Transform TextDisplayTransform => _textDisplayTransform;
		public float SizeSqr => _sizeSqr;
		public TargetMask TargetType => _targetType;
		public Transform CachedTransform => _transform;
		public GUIDComponent GUIDComponent => _gUIDComponent;

		/// <summary>
		/// Sets the Target Type.
		/// </summary>
		/// <param name="type"></param>
		public void SetTargetType(TargetMask type)
		{
			RemoveThisTarget();
			_targetType = type;
			AddThisTargetToCell();
		}

		/// <summary>
		/// Calculates the score for targeting this object.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public float CalculateScore(Vector3 position)
		{
			return (Vector3.Distance(position, transform.position) * _distancePenaltyMod) + (_currentAssignedCount * _assignmentPenaltyMod);
		}

		/// <summary>
		/// Increases the assigned units count.
		/// </summary>
		public void AssignToTarget()
		{
			_currentAssignedCount++;
		}

		/// <summary>
		/// Decreases the assigned units count.
		/// </summary>
		public void UnassignFromTarget()
		{
			_currentAssignedCount--;
		}

		/// <summary>
		/// Used for initializing any required data.
		/// </summary>
		protected virtual void Init() { }

		/// <summary>
		/// Adds this target to the cell index of the cell space partition.
		/// </summary>
		protected void AddThisTargetToCell()
		{
			if (_cellIndex == -1)
				return;

			CSPManager.GetCellAtIndex(_cellIndex).AddTarget(this);
		}

		/// <summary>
		/// Removes this target from the cell index of the cell space partition.
		/// </summary>
		protected void RemoveThisTarget()
		{
			if ( _cellIndex == -1)
				return;

			CSPManager.GetCellAtIndex(_cellIndex).RemoveTarget(this);
		}

		/// <summary>
		/// Calculates and stores the size of the box collider squared.
		/// </summary>
		private void CalculateSizeSquared()
		{
			if (!_useCustomSize && TryGetComponent(out _boxCollider))
			{
				if (_boxCollider.size.x > _boxCollider.size.z)
					_sizeSqr = _boxCollider.size.x;
				else
					_sizeSqr = _boxCollider.size.z;

				_sizeSqr *= _sizeSqr;
			}
			else if (_useCustomSize)
			{
				_sizeSqr = _customSize * _customSize;
			}
			else
				_sizeSqr = 0;
		}

		/// <summary>
		/// Updates the cell index that this target belongs to.
		/// </summary>
		protected void UpdateIndex()
		{
			int newCellIndex = CSPManager.PositionToIndex(transform.position);

			if (newCellIndex != _cellIndex)
			{
				RemoveThisTarget();
				_cellIndex = newCellIndex;
				AddThisTargetToCell();
			}
		}

		/// <summary>
		/// Checks if enough time has elasped to update which partition and cell index the target belongs to.
		/// </summary>
		protected void CheckUpdatePartitionTime()
		{
			if (!_updatePartitionIndex)
				return;

			_partitionUpdateTime += Time.deltaTime;

			if (_partitionUpdateTime >= _partitionUpdateRate)
			{
				_partitionUpdateTime -= _partitionUpdateRate;
				UpdateIndex();
			}
		}

		// Unity Functions.
		public void Update()
		{
			CheckUpdatePartitionTime();
		}

		protected void OnEnable()
		{
			_wasPooled = true;
			AddThisTargetToCell();

			TargetManager.AddTarget(this);
		}

		protected void OnDisable()
		{
			RemoveThisTarget();

			TargetManager.RemoveTarget(this);
		}

		private void Awake()
		{
			_gUIDComponent = GetComponent<GUIDComponent>();
			Init();
			
			CalculateSizeSquared();
		}

		protected void Start()
		{
			_transform = transform;
			_cellIndex = CSPManager.PositionToIndex(transform.position); ;
			AddThisTargetToCell();
		}
	}
}