using UnityEngine;

namespace AlchemyAR.Alchemy
{
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Alchemy/Recipe")]
    public class Recipe : ScriptableObject
    {
        public Ingredient ingredient1;
        public Ingredient.TemperatureStatus tempStatus1;
        
        public Ingredient ingredient2;
        public Ingredient.TemperatureStatus tempStatus2;
        
        public GameObject result;

        public bool IsCorrect(Ingredient ing1, Ingredient ing2)
        {
            return (ing1.name, ing1.tempStatus) == (ingredient1.name, tempStatus1) &&
                   (ing2.name, ing2.tempStatus) == (ingredient2.name, tempStatus2) ||
                   (ing2.name, ing2.tempStatus) == (ingredient1.name, tempStatus1) &&
                   (ing1.name, ing1.tempStatus) == (ingredient2.name, tempStatus2);
        }
    }
}
