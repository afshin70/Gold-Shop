﻿using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Entities;
using Gold.SharedKernel.DTO.OperationResult;

namespace Gold.Domain.Contract.IRepositories
{
    public interface IProductRepository : IBaseRepository<Product, long>
    {
       
    }
}