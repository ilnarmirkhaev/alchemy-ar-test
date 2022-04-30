using UnityEngine;

namespace AlchemyAR.Alchemy
{
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Alchemy/Recipe")]
    public class Recipe : ScriptableObject
    {
        public GameObject ingredient1;
        public Ingredient.TemperatureStatus tempStatus1;
        
        public GameObject ingredient2;
        public Ingredient.TemperatureStatus tempStatus2;
        
        public GameObject result;

        public bool IsCorrect(Ingredient ing1, Ingredient ing2)
        {
            return (ing1.name, ing1.TempStatus) == (ingredient1.name, tempStatus1) &&
                   (ing2.name, ing2.TempStatus) == (ingredient2.name, tempStatus2) ||
                   (ing2.name, ing2.TempStatus) == (ingredient1.name, tempStatus1) &&
                   (ing1.name, ing1.TempStatus) == (ingredient2.name, tempStatus2);
        }
    }
}
