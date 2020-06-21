using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ShopPCBusinessLogic.ViewModels
{
    public class ComponentViewModel
    {
        public int Id { get; set; }
        [DisplayName("Название продукта")]
        public string ComponentName { get; set; }
    }
}