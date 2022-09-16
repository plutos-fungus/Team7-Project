using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SurfsUp.Models;

namespace SurfsUp.Areas.Identity.Data
{
    public class SurfUpUser : IdentityUser
    {
        public List<Surfboard> Surfboards { get; set; }

    }
}
