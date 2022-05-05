using UnityEngine;
using UnityEngine.UI;

namespace AlchemyAR.Alchemy
{
    public class Order : MonoBehaviour
    {
        public GameObject potion;
        public float timeLeft;
        public Slider orderDisplay;

        public static Order AddOrder(GameObject to, GameObject potion, float timeForOrder)
        {
            var order = to.AddComponent<Order>();
            order.potion = potion;
            order.timeLeft = timeForOrder;
            order.orderDisplay = Instantiate(GameManager.Instance.orderDisplay, GameManager.Instance.ordersPanel.transform);
            return order;
        }

        private void Start()
        {
            orderDisplay.minValue = 0;
            orderDisplay.maxValue = GameManager.Instance.timeForOneOrder;
            orderDisplay.value = timeLeft;
            
            var image = orderDisplay.transform.GetChild(2).GetComponent<Image>();
            if (image != null)
                image.sprite = GameManager.Instance.Icons[potion.name];
        }

        private void Update()
        {
            timeLeft -= Time.deltaTime;
            orderDisplay.value = timeLeft;

            if (timeLeft <= 0 || GameManager.Instance.gameOver)
            {
                GameManager.Instance.activeOrders.Remove(this);
                DestroyOrder();
            }
        }

        public void DestroyOrder()
        {
            Destroy(orderDisplay.gameObject);
            Destroy(this);
        }
    }
}