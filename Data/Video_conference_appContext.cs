using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Video_conference_app.Models;

namespace Video_conference_app.Data
{
    public class Video_conference_appContext : DbContext
    {
        public Video_conference_appContext (DbContextOptions<Video_conference_appContext> options)
            : base(options)
        {
        }

        public DbSet<Video_conference_app.Models.User> User { get; set; } = default!;
    }
}
