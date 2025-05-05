using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Character
{
    /// <summary>
    /// Used for when multiple resource types need to be stored.
    /// </summary>
    [CreateAssetMenu(menuName = "Character/Resource Inventory", fileName = "NewResourceInventory")]
    public class ResourceInventory : SerializedScriptableObject
    {
        [SerializeField] private int _maxAmount;
        [SerializeField] private int _amount;
        [SerializeField] private bool _unlimited = false;

        public bool Full => (_amount >= _maxAmount && !_unlimited);
        public bool HalfFull => (_amount >= _maxAmount * 0.5f && !_unlimited);
        public bool Empty => (_amount == 0 && !_unlimited);
        public string ResourceDataToString => _unlimited ? $"{StringUtils.GetShortenedNumberAsString(_amount)}" : $"{StringUtils.GetShortenedNumberAsString(_amount)}/{StringUtils.GetShortenedNumberAsString(_maxAmount)}";

        public int MaxAmount
        {
            get => _maxAmount;
            set
            {
                _maxAmount = value;
                OnMaxAmountChanged();
            }
        }

        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnAmountChanged();
            }
        }

        public void Init(int startingAmount, int maxAmount, bool unlimited = false)
        {
            _amount = startingAmount;
            _maxAmount = maxAmount;
            _unlimited = unlimited;

            OnAmountChanged();
            OnMaxAmountChanged();
        }

        private void OnAmountChanged()
        {
            if (_amount > _maxAmount && !_unlimited)
                _amount = _maxAmount;

            if (_amount < 0)
                _amount = 0;
        }

        private void OnMaxAmountChanged()
        {
            if (_maxAmount < 0)
                _maxAmount = 0;
        }

        /// <summary>
        /// Creates and initializes a new ResourceInventory ScriptableObject instance.
        /// </summary>
        public static ResourceInventory CreateInventory(int start, int max, bool unlimited = false)
        {
            var inventory = ScriptableObject.CreateInstance<ResourceInventory>();
            inventory.Init(start, max, unlimited);
            return inventory;
        }
    }
}
