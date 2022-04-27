using System.Collections.Generic;
using AlchemyAR.AR;
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
            else
            {
                TryMixIngredients(_ingredientsToMix.ingr1, _ingredientsToMix.ingr2);
                ClearIngredients();
            }
        }

        private void ClearIngredients()
        {
            _ingredientsToMix.ingr1 = null;
            _ingredientsToMix.ingr2 = null;
        }

        private void TryMixIngredients(Ingredient ingr1, Ingredient ingr2)
        {
            if (ingr1.status == Ingredient.Status.Wasted || ingr2.status == Ingredient.Status.Wasted)
            {
                ingr1.SetToWasted();
                ingr2.SetToWasted();
                return;
            }
            
            foreach (var recipe in recipes)
                if (recipe.IsCorrect(ingr1, ingr2))
                {
                    Debug.Log("Success!");
                    
                    var result = imageTrackingManager.ARObjects[recipe.result.name];
                    result.SetActive(true);
                    result.transform.position = Vector3.Lerp(
                        ingr1.gameObject.transform.position,
                        ingr2.gameObject.transform.position,
                        0.5f
                        );
                    
                    return;
                }

            Debug.Log("Fail");
            ingr1.SetToWasted();
            ingr2.SetToWasted();
        }
    }
}
