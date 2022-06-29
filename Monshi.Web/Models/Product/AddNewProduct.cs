using System.ComponentModel.DataAnnotations;
using Monshi.Domain.Products.Entities;

namespace Monshi.Web.Models.Product
{
    public class AddNewProduct
    {
        [Required(ErrorMessage = "لطفا نام کالا را وارد کنید")]
        public string Name { get; set; }

        [Required(ErrorMessage = "لطفا نوع محصول یا خدمت را وارد کنید")]
        public ProductType ProductType { get; set; }

        [Required(ErrorMessage = "لطفا قیمت کالا را وارد کنید")]
        public int Price { get; set; }

        [Required(ErrorMessage = "لطفا توضیحات کالا را وارد کنید")]
        public string Description { get; set; }

        [Required(ErrorMessage = "لطفا تعداد کالا را وارد کنید")]
        public int Quantity { get; set; }
        
        public string? Message { get; set; }
    }
}
