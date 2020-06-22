using ShopPCBusinessLogic.ViewModels;
using ShopPCBusinessLogic.BindingModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopPCBusinessLogic.Interfaces
{
    public interface IProductLogic
    {
        List<ProductViewModel> Read(ProductBindingModel model);
        void CreateOrUpdate(ProductBindingModel model);
        void Delete(ProductBindingModel model);
    }
}