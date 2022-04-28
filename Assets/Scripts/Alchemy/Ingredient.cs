using System;
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

        public new string name;
        public Status status = Status.Normal;
        public TemperatureStatus tempStatus = TemperatureStatus.Warm;
        
        [Range(0, 100)] public float temperature = 50;

        // Call after ImageTracking Awake()
        private void Start()
        {
            name = gameObject.name;
        }

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

        private void OnCollisionEnter(Collision collision)
        {
            // Don't mix if ingredient is wasted
            if (status == Status.Wasted) return;
            
            var obj = collision.gameObject;
            
            // Do nothing if object isn't an ingredient or is wasted
            if (!obj.TryGetComponent(out Ingredient ingr) || ingr.status == Status.Wasted) return;
            
            Debug.Log(gameObject.name + " collided with " + obj.name);
            
            CraftingManager.Instance.AddIngredient(this);
        }

        public void SetToWasted()
        {
            status = Status.Wasted;
        }
    }
}
