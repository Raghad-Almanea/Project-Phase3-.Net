using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterDelivery.ViewModels
{
    public class ProfileViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Img { get; set; }
        public int City_Id { get; set; }
        public double Wallet { get; set; }
        public int Points { get; set; }
    }
}