using ShopPCBusinessLogic.ViewModels;
using ShopPCBusinessLogic.BindingModels;
using System;
using System.Collections.Generic;
using System.Text;


namespace ShopPCBusinessLogic.Interfaces
{
    public interface IComponentLogic
    {
        List<ComponentViewModel> Read(ComponentBindingModel model);
        void CreateOrUpdate(ComponentBindingModel model);
        void Delete(ComponentBindingModel model);
    }
}