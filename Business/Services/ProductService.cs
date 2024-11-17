﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abstraction.IEntities;
using Abstraction.IRepositories;
using Abstraction.IServices;
using Abstraction.Models;
using AutoMapper;
using Business.Validation;

namespace Business.Services
{
    public class ProductService : AbstractService<ProductModel, IProduct>, IProductService
    {
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper, unitOfWork.ProductRepository)
        {
        }

        public virtual async Task<IEnumerable<ProductModel>> GetAllAsync()
        {
            var entities = await this.UnitOfWork.ProductRepository.GetAllWithDetailsAsync();
            return this.Mapper.Map<IEnumerable<ProductModel>>(entities);
        }

        public virtual async Task<ProductModel> GetByIdAsync(int id)
        {
            var entity = await this.UnitOfWork.ProductRepository.GetByIdWithDetailsAsync(id);
            return this.Mapper.Map<ProductModel>(entity);
        }

        public async Task AddCategoryAsync(ProductCategoryModel categoryModel)
        {
            Validation(categoryModel);
            var entity = this.Mapper.Map<IProductCategory>(categoryModel);
            await this.UnitOfWork.ProductCategoryRepository.AddAsync(entity);
            await this.UnitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<ProductCategoryModel>> GetAllProductCategoriesAsync()
        {
            var entities = await this.UnitOfWork.ProductCategoryRepository.GetAllAsync();
            return this.Mapper.Map<IEnumerable<ProductCategoryModel>>(entities);
        }

        public async Task<IEnumerable<ProductModel>> GetByFilterAsync(FilterSearchModel filterSearch)
        {
            var product = await this.UnitOfWork.ProductRepository.GetAllWithDetailsAsync();
            var filterProduct = product.Where(p =>
                (filterSearch.MinPrice == null || p.Price >= filterSearch.MinPrice) &&
                (filterSearch.MaxPrice == null || p.Price <= filterSearch.MaxPrice) &&
                (filterSearch.CategoryId == null || p.ProductCategoryId == filterSearch.CategoryId));
            return this.Mapper.Map<IEnumerable<ProductModel>>(filterProduct);
        }

        public async Task RemoveCategoryAsync(int categoryId)
        {
            await this.UnitOfWork.ProductCategoryRepository.DeleteByIdAsync(categoryId);
            await this.UnitOfWork.SaveAsync();
        }

        public async Task UpdateCategoryAsync(ProductCategoryModel categoryModel)
        {
            Validation(categoryModel);
            var entity = this.Mapper.Map<IProductCategory>(categoryModel);
            this.UnitOfWork.ProductCategoryRepository.Update(entity);
            await this.UnitOfWork.SaveAsync();
        }

        protected static void Validation(ProductCategoryModel model)
        {
            if (model == null
                || string.IsNullOrWhiteSpace(model.CategoryName))
            {
                throw new MarketException();
            }
        }

        protected override void Validation(ProductModel model)
        {
            if (model == null
                || string.IsNullOrWhiteSpace(model.ProductName)
                || model.Price < 0)
            {
                throw new MarketException();
            }
        }
    }
}
