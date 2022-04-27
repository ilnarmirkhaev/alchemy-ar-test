using UnityEngine;

namespace AlchemyAR.Alchemy
{
    public class TemperatureCrystal : MonoBehaviour
    {
        [Range(-1f, 1f)]
        [SerializeField] private float tempOffset;

        private void OnCollisionEnter(Collision collision)
        {
            var obj = collision.gameObject;
            Debug.Log(gameObject.name + " collided with " + obj.name);
        }

        private void OnCollisionStay(Collision collision)
        {
            var obj = collision.gameObject;
            if (obj.TryGetComponent(out Ingredient ingredient))
            {
                ingredient.ChangeTemperature(tempOffset);
            }
        }
    }
}
