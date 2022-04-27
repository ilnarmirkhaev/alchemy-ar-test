using UnityEngine;

namespace AlchemyAR.Alchemy
{
    public class Ingredient : MonoBehaviour
    {
        public enum Status
        {
            Normal,
            Wasted
        }
        
        public enum TemperatureStatus
        {
            Cold,
            Warm,
            Hot
        }

        public Status status = Status.Normal;
        public TemperatureStatus tempStatus = TemperatureStatus.Warm;
        
        [Range(0, 100)] public float temperature = 50;

        public void ChangeTemperature(float tempOffset)
        {
            temperature += tempOffset;
            temperature = Mathf.Clamp(temperature, 0, 100);
            
            tempStatus = temperature switch
            {
                <= 33 => TemperatureStatus.Cold,
                >= 66 => TemperatureStatus.Hot,
                _ => TemperatureStatus.Warm
            };

            gameObject.GetComponent<Renderer>().material.color = tempStatus switch
            {
                TemperatureStatus.Cold => Color.cyan,
                TemperatureStatus.Hot => Color.red,
                _ => Color.white
            };
        }

        // TODO: fix getting called twice
        private void OnCollisionEnter(Collision collision)
        {
            var obj = collision.gameObject;
            
            if (!obj.TryGetComponent(out Ingredient _)) return;
            
            Debug.Log(gameObject.name + " collided with " + obj.name);
            
            if (!CraftingManager.Instance.TryMixIngredients(gameObject.name, obj.name))
            {
                status = Status.Wasted;
            }
        }
    }
}
