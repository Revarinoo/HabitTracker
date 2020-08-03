using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HabitTracker;
using HabitTracker.Database;
using Npgsql;
using HabitTracker.Gainer;
using HabitTracker.HabitAggregate;
using Abc.HabitTracker.Api;
namespace Abc.HabitTracker.Api.Controllers
{
    [ApiController]
    public class BadgesController : ControllerBase
    {
        private readonly ILogger<BadgesController> _logger;
        private string connString;
        public BadgesController(ILogger<BadgesController> logger)
        {
            _logger = logger;
            connString = "Host=localhost;Username=Habit;Password=revarino123;Database=HabitTracker;Port=5432";
        }

        [HttpGet("api/v1/users/{userID}/badges")]
        public ActionResult<IEnumerable<Badges>> All(Guid userID)
        {
            NpgsqlConnection _connection = new NpgsqlConnection(connString);
            _connection.Open();
            IHabitRepository repo1 = new HabitRepository(_connection, null);
            List<Guid> badgeID = new List<Guid>();
            List<Badge> badge = new List<Badge>();
            List<Badges> badges = new List<Badges>();
            foreach(Guid x in repo1.GetAllBadge(userID)){
                badgeID.Add(x);
            }

            foreach(Guid y in badgeID){
                badge.Add(repo1.FindBadge(userID,y));
            }

            foreach(Badge z in badge){
                Badges bg = new Badges(){
                    ID = z.ID,
                    name = z.name,
                    description = z.description,
                    user_id = z.users,
                    created_at = z.created_at
                };
                badges.Add(bg);
            }
            return badges;
        }
    }
}
