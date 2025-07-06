using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxeStays.Application.Common.Interfaces
{
    public interface IS3Service
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    }

}
