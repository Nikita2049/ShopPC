using ShopPCBusinessLogic.BindingModels;
using ShopPCBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopPCBusinessLogic.Interfaces
{
    public interface IOrderLogic
    {
        List<OrderViewModel> Read(OrderBindingModel model);
        void CreateOrUpdate(OrderBindingModel model);
        void Delete(OrderBindingModel model);
    }
}