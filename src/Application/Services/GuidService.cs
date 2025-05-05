using System;

namespace Application.Services
{
    public class GuidService
    {
        public Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }
    }
}