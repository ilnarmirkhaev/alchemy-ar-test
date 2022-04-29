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
        private Material _material;
        private Texture2D _texture;
        private static readonly int RimColor = Shader.PropertyToID("_RimColor");
        private static readonly int Texture1 = Shader.PropertyToID("_Texture");
        private static readonly int IngrTexture = Shader.PropertyToID("_IngrTexture");


        private void Start()
        {
            name = gameObject.name;
            
            // Get material
            _material = gameObject.GetComponent<MeshRenderer>().material;
            
            // Load texture by name and add to material
            _texture = Resources.Load<Texture2D>($"Textures/{name}");
            _material.SetTexture(IngrTexture, _texture);
            
            // No rim light
            _material.SetColor(RimColor, Color.black);
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
            
            // Change color of emission
            _material.SetColor(RimColor, tempStatus switch
            {
                TemperatureStatus.Cold => Color.cyan,
                TemperatureStatus.Hot => Color.red,
                _ => Color.black
            });
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Don't mix if ingredient is wasted
            if (status == Status.Wasted) return;
            
            var obj = collision.gameObject;
            
            // Do nothing if other object isn't an ingredient or is wasted
            if (!obj.TryGetComponent(out Ingredient ingr) || ingr.status == Status.Wasted) return;
            
            Debug.Log(gameObject.name + " collided with " + obj.name);
            
            CraftingManager.Instance.AddIngredient(this);
        }

        public void SetToWasted()
        {
            status = Status.Wasted;
            
            _material.SetColor(RimColor, Color.green);
        }

        public void ResetValues()
        {
            status = Status.Normal;
            temperature = 50;
            tempStatus = TemperatureStatus.Warm;
            
            if (_material != null && _material.name == "Temperature (Instance)") 
                _material.SetColor(RimColor, Color.black);
        }
    }
}
