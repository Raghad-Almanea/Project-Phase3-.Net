using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WaterDelivery.Models.TableDb;

namespace WaterDelivery.ViewModels
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            Categorys = new List<CategoryViewModel>();
            Sliders = new List<SliderViewModel>();
        }
        public List<CategoryViewModel> Categorys { get; set; }     
        public List<SliderViewModel> Sliders { get; set; }
    }

    public class CategoryViewModel
    {
        public CategoryViewModel()
        {
            Products = new List<ProductHomeViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public List<ProductHomeViewModel> Products { get; set; }
    }

    public class ProductHomeViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImg { get; set; }
        public double Price { get; set; }
        public int QtyCart { get; set; }
        public bool IsFavourite { get; set; }
    }

    public class SliderViewModel
    {
        public int SliderId { get; set; }
        public string Img { get; set; }
        public int type  { get; set; }
    }
}