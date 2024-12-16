using E_SalesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace E_SalesAPI.Controllers
{
    public class ProductsController : ApiController
    {
        private E_SalesEntities db = new E_SalesEntities();

        // GET: api/Products
        public IHttpActionResult GetProducts()
        {
            return Ok(db.Products.ToList());
        }

    }
}
