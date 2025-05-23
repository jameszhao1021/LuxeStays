﻿using LuxeStays.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxeStays.Application.Common.Interfaces
{
    public interface IVillaNumberRepository: IRepository<VillaNumber>
    {
        void Update(VillaNumber entity);
    }
}
