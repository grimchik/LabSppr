using Bar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bar.Domain.Models
{
    public class Cart
    {
        /// <summary>
        /// Список объектов в корзине: ключ - ID продукта
        /// </summary>
        public Dictionary<int, CartItem> CartItems { get; set; } = new();

        /// <summary>
        /// Добавить продукт в корзину
        /// </summary>
        /// <param name="product">Добавляемый продукт</param>
        public virtual void AddToCart(Product product)
        {
            if (CartItems.ContainsKey(product.ID))
            {
                CartItems[product.ID].Count++;
            }
            else
            {
                CartItems[product.ID] = new CartItem { Item = product, Count = 1 };
            }
        }

        /// <summary>
        /// Удалить продукт из корзины
        /// </summary>
        /// <param name="id">ID удаляемого продукта</param>
        public virtual void RemoveItems(int id)
        {
            if (CartItems.ContainsKey(id))
            {
                CartItems.Remove(id);
            }
        }

        /// <summary>
        /// Очистить корзину
        /// </summary>
        public virtual void ClearAll()
        {
            CartItems.Clear();
        }

        /// <summary>
        /// Количество продуктов в корзине
        /// </summary>
        public int ItemCount => CartItems.Sum(item => item.Value.Count);

        /// <summary>
        /// Общая стоимость продуктов
        /// </summary>
        public decimal TotalAmount => CartItems.Sum(item => item.Value.Item.Price * item.Value.Count);
    }
}
