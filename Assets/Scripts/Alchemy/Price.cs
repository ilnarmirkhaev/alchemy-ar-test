using UnityEngine;

namespace AlchemyAR.Alchemy
{
    [CreateAssetMenu(fileName = "New Price", menuName = "Alchemy/Price")]
    public class Price : ScriptableObject
    {
        public GameObject potion;
        public int price;
    }
}