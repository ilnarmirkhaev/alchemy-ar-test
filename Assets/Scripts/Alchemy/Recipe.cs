using UnityEngine;

namespace AlchemyAR.Alchemy
{
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Alchemy/Recipe")]
    public class Recipe : ScriptableObject
    {
        public string ingredient1;
        public string ingredient2;
        
        public GameObject result;
    }
}
