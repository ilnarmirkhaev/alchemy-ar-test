using UnityEngine;

namespace AlchemyAR.Alchemy
{
    public class TemperatureCrystal : MonoBehaviour
    {
        [Range(-1f, 1f)]
        [SerializeField] private float tempOffset;

        private void OnCollisionStay(Collision collision)
        {
            var obj = collision.gameObject;
            if (obj.TryGetComponent(out Ingredient ingredient) && ingredient.status == Ingredient.Status.Normal)
            {
                ingredient.ChangeTemperature(tempOffset);
            }
        }
    }
}
