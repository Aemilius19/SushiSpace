﻿using AutoMapper;
using SushiSpace.Application.Data.Context;
using SushiSpace.Application.Repositories.Abstractions;
using SushiSpace.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiSpace.Application.Repositories.Implementations
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(SushiSpaceDb context, IMapper mapper = null) : base(context, mapper)
        {
        }
    }
}