using ShopPCBusinessLogic.BindingModels;
using ShopPCBusinessLogic.Interfaces;
using ShopPCBusinessLogic.ViewModels;
using ShopPCListImplement.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopPCListImplement.Implements
{
    public class ProductLogic : IProductLogic
    {
        private readonly DataListSingleton source;
        public ProductLogic()
        {
            source = DataListSingleton.GetInstance();
        }
        public void CreateOrUpdate(ProductBindingModel model)
        {
            Product tempProduct = model.Id.HasValue ? null : new Product { Id = 1 };
            foreach (var product in source.Products)
            {
                if (product.ProductName == model.ProductName && product.Id != model.Id)
                {
                    throw new Exception("Уже есть изделие с таким названием");
                }
                if (!model.Id.HasValue && product.Id >= tempProduct.Id)
                {
                    tempProduct.Id = product.Id + 1;
                }
                else if (model.Id.HasValue && product.Id == model.Id)
                {
                    tempProduct = product;
                }
            }
            if (model.Id.HasValue)
            {
                if (tempProduct == null)
                {
                    throw new Exception("Элемент не найден");
                }
                CreateModel(model, tempProduct);
            }
            else
            {
                source.Products.Add(CreateModel(model, tempProduct));
            }
        }
        public void Delete(ProductBindingModel model)
        {
            for (int i = 0; i < source.ProductComponents.Count; ++i)
            {
                if (source.ProductComponents[i].ProductId == model.Id)
                {
                    source.ProductComponents.RemoveAt(i--);
                }
            }
            for (int i = 0; i < source.Products.Count; ++i)
            {
                if (source.Products[i].Id == model.Id)
                {
                    source.Products.RemoveAt(i);
                    return;
                }
            }
            throw new Exception("Элемент не найден");
        }
        private Product CreateModel(ProductBindingModel model, Product product)
        {
            product.ProductName = model.ProductName;
            product.Price = model.Price;
            int maxSFId = 0;
            for (int i = 0; i < source.ProductComponents.Count; ++i)
            {
                if (source.ProductComponents[i].Id > maxSFId)
                {
                    maxSFId = source.ProductComponents[i].Id;
                }
                if (source.ProductComponents[i].ProductId == product.Id)
                {
                    if
                    (model.ProductComponents.ContainsKey(source.ProductComponents[i].ComponentId))
                    {
                        source.ProductComponents[i].Count =
                        model.ProductComponents[source.ProductComponents[i].ComponentId].Item2;
                        model.ProductComponents.Remove(source.ProductComponents[i].ComponentId);
                    }
                    else
                    {
                        source.ProductComponents.RemoveAt(i--);
                    }
                }
            }
            foreach (var sf in model.ProductComponents)
            {
                source.ProductComponents.Add(new ProductComponent
                {
                    Id = ++maxSFId,
                    ProductId = product.Id,
                    ComponentId = sf.Key,
                    Count = sf.Value.Item2
                });
            }
            return product;
        }
        public List<ProductViewModel> Read(ProductBindingModel model)
        {
            List<ProductViewModel> result = new List<ProductViewModel>();
            foreach (var component in source.Products)
            {
                if (model != null)
                {
                    if (component.Id == model.Id)
                    {
                        result.Add(CreateViewModel(component));
                        break;
                    }
                    continue;
                }
                result.Add(CreateViewModel(component));
            }
            return result;
        }
        private ProductViewModel CreateViewModel(Product Product)
        {
            Dictionary<int, (string, int)> ProductComponents = new Dictionary<int, (string, int)>();
            foreach (var sf in source.ProductComponents)
            {
                if (sf.ProductId == Product.Id)
                {
                    string ComponentName = string.Empty;
                    foreach (var Component in source.Components)
                    {
                        if (sf.ComponentId == Component.Id)
                        {
                            ComponentName = Component.ComponentName;
                            break;
                        }
                    }
                    ProductComponents.Add(sf.ComponentId, (ComponentName, sf.Count));
                }
            }
            return new ProductViewModel
            {
                Id = Product.Id,
                ProductName = Product.ProductName,
                Price = Product.Price,
                ProductComponents = ProductComponents
            };
        }
    }
}