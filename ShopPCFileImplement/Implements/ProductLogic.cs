using ShopPCBusinessLogic.BindingModels;
using ShopPCBusinessLogic.Interfaces;
using ShopPCBusinessLogic.ViewModels;
using ShopPCFileImplement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopPCFileImplement.Implements
{
    public class ProductLogic : IProductLogic
    {
        private readonly FileDataListSingleton source;
        public ProductLogic()
        {
            source = FileDataListSingleton.GetInstance();
        }
        public void CreateOrUpdate(ProductBindingModel model)
        {
            Product element = source.Products.FirstOrDefault(rec => rec.ProductName ==
           model.ProductName && rec.Id != model.Id);
            if (element != null)
            {
                throw new Exception("Уже есть системный блок с таким названием");
            }
            if (model.Id.HasValue)
            {
                element = source.Products.FirstOrDefault(rec => rec.Id == model.Id);
                if (element == null)
                {
                    throw new Exception("Элемент не найден");
                }
            }
            else
            {
                int maxId = source.Products.Count > 0 ? source.Components.Max(rec =>
               rec.Id) : 0;
                element = new Product { Id = maxId + 1 };
                source.Products.Add(element);
            }
            element.ProductName = model.ProductName;
            element.Price = model.Price;
            // удалили те, которых нет в модели
            source.ProductComponents.RemoveAll(rec => rec.ProductId == model.Id &&
           !model.ProductComponents.ContainsKey(rec.ComponentId));
            // обновили количество у существующих записей
            var updateComponents = source.ProductComponents.Where(rec => rec.ProductId ==
           model.Id && model.ProductComponents.ContainsKey(rec.ComponentId));
            foreach (var updateComponent in updateComponents)
            {
                updateComponent.Count =
               model.ProductComponents[updateComponent.ComponentId].Item2;
                model.ProductComponents.Remove(updateComponent.ComponentId);
            }
            // добавили новые
            int maxFSId = source.ProductComponents.Count > 0 ?
           source.ProductComponents.Max(rec => rec.Id) : 0;
            foreach (var pc in model.ProductComponents)
            {
                source.ProductComponents.Add(new ProductComponent
                {
                    Id = ++maxFSId,
                    ProductId = element.Id,
                    ComponentId = pc.Key,
                    Count = pc.Value.Item2
                });
            }
        }
        public void Delete(ProductBindingModel model)
        {
            // удаяем записи по компонентам при удалении изделия
            source.ProductComponents.RemoveAll(rec => rec.ProductId == model.Id);
            Product element = source.Products.FirstOrDefault(rec => rec.Id == model.Id);
            if (element != null)
            {
                source.Products.Remove(element);
            }
            else
            {
                throw new Exception("Элемент не найден");
            }
        }
        public List<ProductViewModel> Read(ProductBindingModel model)
        {
            return source.Products
            .Where(rec => model == null || rec.Id == model.Id)
            .Select(rec => new ProductViewModel
            {
                Id = rec.Id,
                ProductName = rec.ProductName,
                Price = rec.Price,
                ProductComponents = source.ProductComponents
            .Where(recFS => recFS.ProductId == rec.Id)
           .ToDictionary(recFS => recFS.ComponentId, recFS =>
            (source.Components.FirstOrDefault(recC => recC.Id ==
           recFS.ComponentId)?.ComponentName, recFS.Count))
            })
            .ToList();
        }
    }
}