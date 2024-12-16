using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Description;
using E_SalesAPI.Models;

namespace E_SalesAPI.Controllers
{
    public class CustomersController : ApiController
    {
        private E_SalesEntities db = new E_SalesEntities();

        /// <summary>
        /// Получает список всех клиентов из базы данных.
        /// </summary>
        // GET: api/Customers
        public IHttpActionResult GetCustomers()
        {
            return Ok(db.Customers.ToList());
        }

        /// <summary>
        /// Получает данные о клиенте по его идентификатору.
        /// </summary>
        /// <param name="id">Id клиента</param>
        // GET: api/Customers/5
        [ResponseType(typeof(Customer))]
        public IHttpActionResult GetCustomer(int id)
        {
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        /// <summary>
        /// Обновляет данные клиента, включая проверку формата номера телефона.
        /// </summary>
        /// <param name="id">Id клиента</param>
        /// <returns></returns>
        // PUT: api/Customers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCustomer(int id, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            //Регулярное выражение для номера телефона
            if (!Regex.IsMatch(customer.Phone, @"^\d{3}-\d{3}-\d{4}$"))
            {
                return BadRequest("Phone number format is invalid. Use xxx-xxx-xxxx.");
            }

            var existingCustomer = db.Customers.Find(id);
            if (existingCustomer == null)
            {
                return NotFound();
            }

            db.SaveChanges();
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Добавляет нового клиента в базу данных. Проверяет формат номера телефона и длину имени.
        /// </summary>
        // POST: api/Customers
        [ResponseType(typeof(Customer))]
        public IHttpActionResult PostCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Регулярное выражение для номера телефона
            if (!Regex.IsMatch(customer.Phone, @"^\d{3}-\d{3}-\d{4}$"))
            {
                return BadRequest("Phone number format is invalid. Use xxx-xxx-xxxx.");
            }

            if (customer.Name.Length > 99)
            {
                return BadRequest("Name cannot exceed 99 characters.");
            }

            db.Customers.Add(customer);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = customer.CustomerId }, customer);
        }

        /// <summary>
        /// Удаляет клиента из базы данных по его идентификатору.
        /// </summary>
        /// <param name="id">Id клиента</param>
        // DELETE: api/Customers/5
        [ResponseType(typeof(Customer))]
        public IHttpActionResult DeleteCustomer(int id)
        {
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            db.Customers.Remove(customer);
            db.SaveChanges();

            return Ok(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(int id)
        {
            return db.Customers.Count(e => e.CustomerId == id) > 0;
        }
    }
}