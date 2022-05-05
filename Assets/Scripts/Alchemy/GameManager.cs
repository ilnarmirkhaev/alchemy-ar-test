using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AlchemyAR.Alchemy
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            
            // DontDestroyOnLoad(gameObject);
            Init();
        }
        
        // Items and prices
        [SerializeField] private GameObject[] potions;
        [SerializeField] private Price[] prices;
        private readonly Dictionary<string, int> _priceList = new Dictionary<string, int>();
        public readonly Dictionary<string, Sprite> Icons = new Dictionary<string, Sprite>();

        public List<Order> activeOrders = new List<Order>();
        public List<GameObject> readyPotions;
        
        // Time parameters
        [SerializeField] private float timeBetweenOrders = 25f;
        public float timeForOneOrder = 30f;
        private float _timeFromLastOrder;

        // Delay start
        private bool _waitingToStart = true;
        
        // Money
        [SerializeField] private int gold;
        [SerializeField] private int levelGoal = 1000;
        
        // UI
        [SerializeField] private TMP_Text goldDisplay;
        public GameObject ordersPanel;
        public Slider orderDisplay;
        [SerializeField] private GameObject bookPanel;

        // Game over
        [SerializeField] private GameObject winPanel;
        public bool gameOver;

        private int Gold
        {
            get => gold;
            set
            {
                gold = value;
                goldDisplay.text = $"{gold} / {levelGoal}";
                
                if (gold >= levelGoal)
                    Win();
            }
        }

        private void Init()
        {
            goldDisplay.text = $"{gold} / {levelGoal}";
            
            // Create priceList
            foreach (var price in prices)
            {
                _priceList.Add(price.potion.name, price.price);
            }
            
            // Get potion icons
            foreach (var potion in potions)
            {
                Icons.Add(potion.name, Resources.Load<Sprite>($"Icons/{potion.name}"));
            }
        }

        private IEnumerator Start()
        {
            gameOver = false;
            yield return new WaitForSeconds(2);
            
            _waitingToStart = false;
            _timeFromLastOrder = timeBetweenOrders;
        }

        private void Update()
        {
            if (_waitingToStart || gameOver) return;
            
            _timeFromLastOrder += Time.deltaTime;
            
            if (_timeFromLastOrder >= timeBetweenOrders || activeOrders.Count == 0)
            {
                NewOrder();
                _timeFromLastOrder = -1f;
            }
        }

        private void NewOrder()
        {
            var potion = potions[Random.Range(0, potions.Length)];
            var order = Order.AddOrder(gameObject, potion, timeForOneOrder);
            
            activeOrders.Add(order);
        }

        public void Sell()
        {
            if (gameOver) return;
            if (readyPotions.Count == 0 || activeOrders.Count == 0) return;

            var income = 0;

            // Reverse search and removal
            for (var i = readyPotions.Count - 1; i >= 0; i--)
            {
                var potion = readyPotions[i];
                var found = false;
                
                for (var j = 0; j < activeOrders.Count; j++)
                {
                    var order = activeOrders[j];
                    
                    if (potion.name != order.potion.name) continue;
                    
                    found = true;
                    activeOrders.RemoveAt(j);
                    order.DestroyOrder();
                    break;
                }

                if (!found) continue;
                
                readyPotions.RemoveAt(i);
                income += _priceList[potion.name];
                potion.SetActive(false);
            }
            
            if (income > 0)
                Gold += income;
        }

        public void ClearReadyPotions()
        {
            readyPotions.Clear();
        }

        private void Win()
        {
            gameOver = true;
            bookPanel.SetActive(false);
            winPanel.SetActive(true);
        }

        public void PlayAgain()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
