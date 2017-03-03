using Bogus;
using System;
using System.Linq;

namespace DemoApi
{
    public class DemoData
    {
        public static IQueryable<DemoPerson> GetDemoData(int quantity)
        {
            Randomizer.Seed = new Random(3897234);
            var userId = 0;
            var users = new Faker<DemoPerson>()
                .RuleFor(u => u.firstName, f => f.Name.FirstName())
                .RuleFor(u => u.lastName, f => f.Name.LastName())
                .RuleFor(u => u.hairColor, f => f.Commerce.Color())
                .RuleFor(u => u.eyeColor, f => f.Commerce.Color())
                .RuleFor(u => u.id, f => userId++)
                .RuleFor(u => u.height, f => f.Random.Int(135, 250))
                .RuleFor(u => u.team, f => f.Random.Int(1, 13))
                .RuleFor(u => u.address, f => new Address { Unit = f.Random.Int(1, 20), City = f.Address.City(), Address1 = f.Address.StreetAddress() })
                .Generate(quantity);

            return users.AsQueryable();
        }
    }
}