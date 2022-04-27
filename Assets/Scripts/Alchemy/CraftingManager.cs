using System.Collections.Generic;
using UnityEngine;

namespace AlchemyAR.Alchemy
{
    public class CraftingManager : MonoBehaviour
    {
        public static CraftingManager Instance { get; private set; }

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

        [SerializeField] private List<Recipe> recipes;

        public bool TryMixIngredients(string ingredient1, string ingredient2)
        {
            foreach (var recipe in recipes)
            {
                if (recipe.ingredient1 == ingredient1 && recipe.ingredient2 == ingredient2)
                {
                    Debug.Log("Success!");
                    // TODO: don't let multiple objects to spawn, add to _arObjects
                    var result = Instantiate(recipe.result, Vector3.zero, Quaternion.identity);
                    result.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    return true;
                }
            }

            Debug.Log("Fail");
            return false;
        }
    }
}
