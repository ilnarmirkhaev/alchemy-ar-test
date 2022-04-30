using System.Collections.Generic;
using AlchemyAR.AR;
using Lean.Touch;
using UnityEngine;

namespace AlchemyAR.Alchemy
{
    public class CraftingManager : MonoBehaviour
    {
        public static CraftingManager Instance { get; private set; }

        public ImageTracking imageTrackingManager;

        [SerializeField] private List<Recipe> recipes;

        private (Ingredient ingr1, Ingredient ingr2) _ingredientsToMix;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public void AddIngredient(Ingredient ingredient)
        {
            if (_ingredientsToMix.ingr1 == null)
                _ingredientsToMix.ingr1 = ingredient;
            else if (_ingredientsToMix.ingr2 == null)
                _ingredientsToMix.ingr2 = ingredient;

            if (_ingredientsToMix.ingr1 == null || _ingredientsToMix.ingr2 == null) return;
            
            TryMixIngredients(_ingredientsToMix.ingr1, _ingredientsToMix.ingr2);
            ClearIngredients();
        }

        private void ClearIngredients()
        {
            _ingredientsToMix.ingr1 = null;
            _ingredientsToMix.ingr2 = null;
        }

        private void TryMixIngredients(Ingredient ingr1, Ingredient ingr2)
        {
            Debug.Log($"Mix called on ingredients: {ingr1.name} ({ingr1.TempStatus}) and {ingr2.name} ({ingr2.TempStatus})");

            foreach (var recipe in recipes)
            {
                if (!recipe.IsCorrect(ingr1, ingr2)) continue;
                
                var result = imageTrackingManager.ARObjects[recipe.result.name];
                
                // Do nothing if result is already active in scene
                if (result.activeSelf) return;

                // Add LeanDragTranslate to drag object via touch
                if (!result.TryGetComponent(out LeanDragTranslate _))
                    result.AddComponent<LeanDragTranslate>();
                    
                // Move result up, so it doesn't collide with others
                result.transform.position = Vector3.Lerp(
                    ingr1.gameObject.transform.position,
                    ingr2.gameObject.transform.position,
                    0.5f
                ) + Vector3.up * 0.25f;
                    
                result.SetActive(true);
                    
                if (result.TryGetComponent(out Ingredient ingredient))
                    ingredient.ResetValues();
                    
                return;
            }
            
            ingr1.SetToWasted();
            ingr2.SetToWasted();
        }
    }
}
