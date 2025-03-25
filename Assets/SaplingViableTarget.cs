using UnityEngine;
using GameResources;
using Managers;
using Utils.Pooling;

[RequireComponent(typeof(ResourceHolder))]
public class SaplingViableTarget : MonoBehaviour
{
    private ResourceHolder _resourceHolder;
    [SerializeField] private string treePoolName = "Tree"; // Matches name in Object Pool

    private void OnEnable()
    {
        _resourceHolder = GetComponent<ResourceHolder>();
        _resourceHolder.OnAmountChange += HandleResourceChanged;
    }

    private void HandleResourceChanged(ResourceHolder rh)
    {
        if (rh.Amount <= 1)
            SpawnTree();
    }

    private void SpawnTree()
    {
        var pool = GameManager.Instance.PoolingManager;
        GameObject tree = pool.GetPooledObject(treePoolName, false).gameObject;
        tree.transform.position = transform.position;
        tree.transform.rotation = Quaternion.identity;
        tree.SetActive(true);
    }

    private void OnDisable()
    {
        _resourceHolder.OnAmountChange -= HandleResourceChanged;
    }
}
