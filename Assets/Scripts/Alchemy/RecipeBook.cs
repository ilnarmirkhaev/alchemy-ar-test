using System;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace AlchemyAR.Alchemy
{
    public class RecipeBook : MonoBehaviour
    {
        [SerializeField] private Recipe[] recipes;
        [SerializeField] private GameObject bookPanel;
        [SerializeField] private TMP_Text book;
        private bool _isOpen;
        private const string ColdColor = "#00C8FF";
        private const string HotColor = "#FF6060";

        private void Awake()
        {
            if (bookPanel == null || book == null) return;

            _isOpen = bookPanel.activeSelf;
            
            var bookText = new StringBuilder(book.text);
            
            foreach (var recipe in recipes)
            {
                var sb = new StringBuilder();
                sb.Append(recipe.result.name);
                sb.Append(" = ");
                sb.Append(ColorText(recipe.ingredient1.name, recipe.tempStatus1));
                sb.Append(" + ");
                sb.Append(ColorText(recipe.ingredient2.name, recipe.tempStatus2));
                
                bookText.Append(sb);
                bookText.AppendLine();
            }

            book.text = bookText.ToString();
        }

        public void ToggleBook()
        {
            if (GameManager.Instance.gameOver) return;
            bookPanel.SetActive(!_isOpen);
            _isOpen = bookPanel.activeSelf;
        }

        private static string ColorText(string word, Ingredient.TemperatureStatus temperatureStatus)
        {
            if (temperatureStatus == Ingredient.TemperatureStatus.Warm) return word;

            var color= temperatureStatus == Ingredient.TemperatureStatus.Cold ? ColdColor : HotColor;
            
            return $"<color={color}>{word}</color>";
        }
    }
}
