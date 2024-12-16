using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using E_SalesAPI.Models;

namespace E_SalesAPI.Controllers
{
    public class OrdersController : ApiController
    {
        private E_SalesEntities db = new E_SalesEntities();

        /// <summary>
        /// Получает список заказов с информацией о продукте и клиенте.
        /// </summary>
        // GET: api/Orders
        public IHttpActionResult GetOrders()
        {
            var orders = db.Orders
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    Product = o.Product.Name,
                    Customer = o.Customer.Name,
                    Quantity = o.Quantity,
                    OrderDate = o.OrderDate
                }).ToList();

            return Ok(orders);
        }

        /// <summary>
        /// Получает данные о конкретном заказе по его идентификатору. 
        /// </summary>
        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            var order = db.Orders
                .Where(o => o.OrderId == id)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    o.Quantity,
                    Product = o.Product.Name,
                    Customer = o.Customer.Name
                })
                .FirstOrDefault();

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        /// <summary>
        /// Обновляет данные заказа, включая количество, дату, продукт и клиента.
        /// </summary>
        /// <param name="id">Id заказа</param>
        /// <param name="orderDto"></param>
        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orderDto.OrderId)
            {
                return BadRequest("Order ID mismatch.");
            }

            var existingOrder = db.Orders.Find(id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // Обновляем поля в существующем заказе
            existingOrder.Quantity = orderDto.Quantity;
            existingOrder.OrderDate = orderDto.OrderDate;

            // Получаем соответствующие связи из базы
            var product = db.Products.FirstOrDefault(p => p.Name == orderDto.Product);
            var customer = db.Customers.FirstOrDefault(c => c.Name == orderDto.Customer);

            if (product == null || customer == null)
            {
                return BadRequest("Invalid Product or Customer.");
            }

            existingOrder.ProductId = product.ProductId;
            existingOrder.CustomerId = customer.CustomerId;

            db.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Добавляет новый заказ в базу данных. Проверяет, что клиент, продукт и количество указаны корректно.
        /// </summary>
        /// <param name="order">Объект заказа</param>
        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Проверка на существование ProductId
            if (!db.Products.Any(p => p.ProductId == order.ProductId))
            {
                return BadRequest("Invalid ProductId.");
            }

            //Проверка на существование CustomerId
            if (!db.Customers.Any(c => c.CustomerId == order.CustomerId))
            {
                return BadRequest("Invalid CustomerId.");
            }

            //Проверка количества добавляемых товаров
            if (order.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than 0.");
            }

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.OrderId }, order);
        }

        /// <summary>
        /// Удаляет заказ из базы данных по его идентификатору.
        /// </summary>
        /// <param name="id">Id заказа</param>
        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
            db.SaveChanges();

            return Ok(order);
        }

        /// <summary>
        /// Освобождает ресурсы, используемые контроллером.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Проверяет существование заказа с указанным идентификатором.
        /// </summary>
        /// <param name="id">Id заказа</param>
        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.OrderId == id) > 0;
        }
    }
}