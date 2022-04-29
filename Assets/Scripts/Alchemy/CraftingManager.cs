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
            Debug.Log("AddIngredient is called");
            if (_ingredientsToMix.ingr1 == null)
                _ingredientsToMix.ingr1 = ingredient;
            else if (_ingredientsToMix.ingr2 == null)
                _ingredientsToMix.ingr2 = ingredient;
            LogIngredients();
            
            if (_ingredientsToMix.ingr1 != null && _ingredientsToMix.ingr2 != null)
            {
                TryMixIngredients(_ingredientsToMix.ingr1, _ingredientsToMix.ingr2);
                ClearIngredients();
            }
        }

        private void LogIngredients()
        {
            if (_ingredientsToMix.ingr1 == null && _ingredientsToMix.ingr2 == null)
            {
                Debug.Log("No ingredients added");
                return;
            }
            if (_ingredientsToMix.ingr1 != null)
                Debug.Log($"Ingredient1: {_ingredientsToMix.ingr1.name}");
            if (_ingredientsToMix.ingr2 != null)
                Debug.Log($"Ingredient2: {_ingredientsToMix.ingr2.name}");
        }

        private void ClearIngredients()
        {
            _ingredientsToMix.ingr1 = null;
            _ingredientsToMix.ingr2 = null;
        }

        private void TryMixIngredients(Ingredient ingr1, Ingredient ingr2)
        {
            Debug.Log($"Mix called on ingredients: {ingr1.name} ({ingr1.tempStatus}) and {ingr2.name} ({ingr2.tempStatus})");

            foreach (var recipe in recipes)
            {
                recipe.LogRecipe();
                if (recipe.IsCorrect(ingr1, ingr2))
                {
                    Debug.Log("Success!");
                    
                    var result = imageTrackingManager.ARObjects[recipe.result.name];
                    
                    // Add LeanDragTranslate to drag object via touch
                    if (!result.TryGetComponent(out LeanDragTranslate _))
                        result.AddComponent<LeanDragTranslate>();
                    
                    result.transform.position = Vector3.Lerp(
                        ingr1.gameObject.transform.position,
                        ingr2.gameObject.transform.position,
                        0.5f
                    ) + Vector3.up * 0.25f; // Move result up, so it doesn't collide with others
                    
                    result.SetActive(true);
                    
                    if (result.TryGetComponent(out Ingredient ingredient))
                        ingredient.ResetValues();
                    
                    return;
                }
            }
            
            Debug.Log("Fail");
            ingr1.SetToWasted();
            ingr2.SetToWasted();
        }
    }
}
