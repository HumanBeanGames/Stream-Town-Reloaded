using GUIDSystem;
using Target;
using Utils.Pooling;

namespace SavingAndLoading.SavableObjects 
{
    public class SaveableObject 
	{
        public Targetable Target;
        public GUIDComponent GUIDComponent;
        public string PoolName;
        public PoolableObject PoolableObject;

        public virtual object SaveData() {return new object(); }
        public virtual void LoadData(object data) {}

        public void SetVariables(Targetable target, GUIDComponent component, string poolName, PoolableObject poolableObject)
        {
            Target = target;
            GUIDComponent = component;
            PoolName = poolName;
            PoolableObject = poolableObject;
        }
    }
}