using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;   
  
namespace WaterDelivery.Models.TableDb  
{      
    public class AddressUser    
    {      
        public int Id { get; set; }   
        public int fk_userID { get; set; }   
        public string title { get; set; }
        public string address { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public bool is_used { get; set; }
        public bool is_active { get; set; }  
        public virtual Client fk_user { get; set; }
    }
}