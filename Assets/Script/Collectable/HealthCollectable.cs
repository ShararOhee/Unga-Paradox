using UnityEngine;

public class HealthCollectable : MonoBehaviour, ICollectable
{
    [SerializeField]
    private float _healthAmount;
    
    public void OnCollected(GameObject player)
    {
        player.GetComponent<Health>().AddHealth(_healthAmount);
    }
}
