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

        // Ingredient properties
        public new string name;
        public Status status = Status.Normal;
        [Range(MinTemperature, MaxTemperature)] public float temperature;
        
        // Temperature constants
        private const float MinTemperature = 0;
        private const float MaxTemperature = 30;
        private const float DefaultTemperature = (MaxTemperature + MinTemperature) / 2;
        private const float ColdTemperature = MinTemperature + (MaxTemperature - MinTemperature) / 3;
        private const float HotTemperature = MaxTemperature - (MaxTemperature - MinTemperature) / 3;

        private TemperatureStatus _tempStatus = TemperatureStatus.Warm;

        public TemperatureStatus TempStatus
        {
            get => _tempStatus;

            private set
            {
                if (_tempStatus == value) return;
                
                _tempStatus = value;
                ChangeTempColor(value);
            }
        }
        
        // Shader properties
        private Material _material;
        private Texture2D _texture;
        private static readonly int RimColor = Shader.PropertyToID("_RimColor");
        private static readonly int IngrTexture = Shader.PropertyToID("_IngrTexture");


        private void Start()
        {
            name = gameObject.name;
            temperature = DefaultTemperature;
            
            // Get material
            _material = gameObject.GetComponent<MeshRenderer>().material;
            
            // Load texture by name and add to material
            _texture = Resources.Load<Texture2D>($"Textures/{name}");
            _material.SetTexture(IngrTexture, _texture);
            
            // No rim light
            SetShaderColor(Color.black);
        }

        public void ChangeTemperature(float tempOffset)
        {
            temperature += tempOffset;
            temperature = Mathf.Clamp(temperature, MinTemperature, MaxTemperature);
            
            TempStatus = temperature switch
            {
                <= ColdTemperature => TemperatureStatus.Cold,
                >= HotTemperature => TemperatureStatus.Hot,
                _ => TemperatureStatus.Warm
            };
        }
        
        private void ChangeTempColor(TemperatureStatus tempStatus)
        {
            // Change color of emission
            SetShaderColor(tempStatus switch
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
            
            // If object is an ingredient and isn't wasted, add to ingredients to mix
            if (obj.TryGetComponent(out Ingredient ingr) && ingr.status != Status.Wasted)
                CraftingManager.Instance.AddIngredient(this);
        }

        public void SetToWasted()
        {
            status = Status.Wasted;
            
            SetShaderColor(Color.green);
        }

        public void ResetValues()
        {
            status = Status.Normal;
            temperature = DefaultTemperature;
            _tempStatus = TemperatureStatus.Warm;
            
            if (_material != null && _material.name == "Temperature (Instance)") 
                SetShaderColor(Color.black);
        }

        private void SetShaderColor(Color color)
        {
            _material.SetColor(RimColor, color);
        }
    }
}
