using LuxeStays.Application.Common.Interfaces;
using LuxeStays.Domain.Entities;
using LuxeStays.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxeStays.Infrastructure.Repository
{
    public class AmenityRepository: Repository<Amenity>,IAmenityRepository
    {
        private readonly ApplicationDbContext _db;
        public AmenityRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Amenity entity)
        {
            _db.Amentities.Update(entity);
        }
    }
}
