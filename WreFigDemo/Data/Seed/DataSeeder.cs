using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WreFigDemo.Identity;
using WreFigDemo.Models.Entities;

namespace WreFigDemo.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.MigrateAsync();

        await SeedRolesAsync(roleMgr);
        await SeedBranchDataAsync(db);
        await SeedUsersAsync(userMgr, db);
    }

    // ── Roles ──────────────────────────────────────────────────────────────────

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleMgr)
    {
        foreach (var role in AppRoles.All)
        {
            if (!await roleMgr.RoleExistsAsync(role))
                await roleMgr.CreateAsync(new IdentityRole(role));
        }
    }

    // ── Regions & Branches ─────────────────────────────────────────────────────

    private static async Task SeedBranchDataAsync(AppDbContext db)
    {
        // ── Regions & Branches (skip if already seeded) ────────────────────────
        if (!await db.Regions.AnyAsync())
        {
        var regions = new List<Region>
        {
            new() { Name = "North" },
            new() { Name = "Mid-Atlantic" },
            new() { Name = "Mid-South" },
            new() { Name = "South" }
        };
        db.Regions.AddRange(regions);
        await db.SaveChangesAsync();

        var north      = regions[0];
        var midAtlantic = regions[1];
        var midSouth   = regions[2];
        var south      = regions[3];

        var branches = new List<Branch>
        {
            // North (10)
            new() { Name = "Highgate Montpelier - VT", City = "Highgate Montpelier", State = "VT", RegionId = north.Id },
            new() { Name = "Bow - NH",                 City = "Bow",                 State = "NH", RegionId = north.Id },
            new() { Name = "Holbrook - MA",            City = "Holbrook",            State = "MA", RegionId = north.Id },
            new() { Name = "Gloucester - MA",          City = "Gloucester",          State = "MA", RegionId = north.Id },
            new() { Name = "Acton - MA",               City = "Acton",               State = "MA", RegionId = north.Id },
            new() { Name = "Bridgewater - MA",         City = "Bridgewater",         State = "MA", RegionId = north.Id },
            new() { Name = "Carver - MA",              City = "Carver",              State = "MA", RegionId = north.Id },
            new() { Name = "Old Lyme - CT",            City = "Old Lyme",            State = "CT", RegionId = north.Id },
            new() { Name = "Springfield - MA",         City = "Springfield",         State = "MA", RegionId = north.Id },
            new() { Name = "Monroe - CT",              City = "Monroe",              State = "CT", RegionId = north.Id },

            // Mid-Atlantic (10)
            new() { Name = "Swedesboro - NJ",          City = "Swedesboro",  State = "NJ", RegionId = midAtlantic.Id },
            new() { Name = "Vernon - NJ",              City = "Vernon",      State = "NJ", RegionId = midAtlantic.Id },
            new() { Name = "Bayville - NJ",            City = "Bayville",    State = "NJ", RegionId = midAtlantic.Id },
            new() { Name = "Salunga - PA",             City = "Salunga",     State = "PA", RegionId = midAtlantic.Id },
            new() { Name = "Ivyland - PA",             City = "Ivyland",     State = "PA", RegionId = midAtlantic.Id },
            new() { Name = "Honesdale - PA",           City = "Honesdale",   State = "PA", RegionId = midAtlantic.Id },
            new() { Name = "Washington - PA",          City = "Washington",  State = "PA", RegionId = midAtlantic.Id, IsAcquisition = true },
            new() { Name = "Loretto - PA",             City = "Loretto",     State = "PA", RegionId = midAtlantic.Id, IsAcquisition = true },
            new() { Name = "Crofton - MD",             City = "Crofton",     State = "MD", RegionId = midAtlantic.Id },
            new() { Name = "Jamaica - VA",             City = "Jamaica",     State = "VA", RegionId = midAtlantic.Id },

            // Mid-South (5)
            new() { Name = "Stanley - NC",             City = "Stanley",     State = "NC", RegionId = midSouth.Id },
            new() { Name = "Boone - NC",               City = "Boone",       State = "NC", RegionId = midSouth.Id },
            new() { Name = "Durham - NC",              City = "Durham",      State = "NC", RegionId = midSouth.Id },
            new() { Name = "Hiram - GA",               City = "Hiram",       State = "GA", RegionId = midSouth.Id },
            new() { Name = "Arlington - TN",           City = "Arlington",   State = "TN", RegionId = midSouth.Id },

            // South (6)
            new() { Name = "Fort Myers - FL",          City = "Fort Myers",  State = "FL", RegionId = south.Id },
            new() { Name = "Gainesville - FL",         City = "Gainesville", State = "FL", RegionId = south.Id },
            new() { Name = "Jacksonville - FL",        City = "Jacksonville",State = "FL", RegionId = south.Id },
            new() { Name = "Largo - FL",               City = "Largo",       State = "FL", RegionId = south.Id },
            new() { Name = "Stuart - FL",              City = "Stuart",      State = "FL", RegionId = south.Id },
            new() { Name = "Orlando - FL",             City = "Orlando",     State = "FL", RegionId = south.Id },
        };

        db.Branches.AddRange(branches);
        await db.SaveChangesAsync();
        } // end regions/branches guard

        // ── Employees (skip if already seeded) ────────────────────────────────
        if (!await db.Employees.AnyAsync())
        {
        var B = (await db.Branches.ToListAsync()).ToDictionary(b => b.City);

        var employees = new List<Employee>
        {
            // ── North ──────────────────────────────────────────────────────────
            new() { Name = "Frank Ouellet",  DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#VT1001", ManagerName = "J. Desroches", JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Highgate Montpelier"].Id },
            new() { Name = "Diane Larose",   DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#VT1002", ManagerName = "J. Desroches", JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Highgate Montpelier"].Id },
            new() { Name = "Paul Tetreault", DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#VT1003", ManagerName = "J. Desroches", JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Highgate Montpelier"].Id },

            new() { Name = "Kevin Morse",    DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#NH1001", ManagerName = "T. Bouchard",  JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Bow"].Id },
            new() { Name = "Carla Jennings", DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#NH1002", ManagerName = "T. Bouchard",  JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Bow"].Id },
            new() { Name = "A. Sullivan",    DefaultShift = "AM", TruckAssignment = null,     TruckId = null,      ManagerName = "T. Bouchard",  JobTitle = "Driver Trainee", ResourceCategory = "Training",  BranchId = B["Bow"].Id },

            new() { Name = "Mike Ferrara",   DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#MA1001", ManagerName = "R. Costa",     JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Holbrook"].Id },
            new() { Name = "Janet Ramos",    DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#MA1002", ManagerName = "R. Costa",     JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Holbrook"].Id },
            new() { Name = "Steve Correia",  DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#MA1003", ManagerName = "R. Costa",     JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Holbrook"].Id },

            new() { Name = "Dan Leary",      DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#MA2001", ManagerName = "C. Favazza",   JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Gloucester"].Id },
            new() { Name = "Tina Palazzi",   DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#MA2002", ManagerName = "C. Favazza",   JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Gloucester"].Id },
            new() { Name = "Greg Santos",    DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#MA2003", ManagerName = "C. Favazza",   JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Gloucester"].Id },

            new() { Name = "Bill Hennessy",  DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#MA3001", ManagerName = "S. Flynn",     JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Acton"].Id },
            new() { Name = "Rosa Vega",      DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#MA3002", ManagerName = "S. Flynn",     JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Acton"].Id },
            new() { Name = "T. Callahan",    DefaultShift = "AM", TruckAssignment = null,     TruckId = null,      ManagerName = "S. Flynn",     JobTitle = "Driver Trainee", ResourceCategory = "Training",  BranchId = B["Acton"].Id },

            new() { Name = "Ed Shaughnessy", DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#MA4001", ManagerName = "P. Melo",      JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Bridgewater"].Id },
            new() { Name = "Amy Teixeira",   DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#MA4002", ManagerName = "P. Melo",      JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Bridgewater"].Id },
            new() { Name = "Luis Pinto",     DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#MA4003", ManagerName = "P. Melo",      JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Bridgewater"].Id },

            new() { Name = "Tom Furtado",    DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#MA5001", ManagerName = "D. Sylvia",    JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Carver"].Id },
            new() { Name = "Maria Pacheco",  DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#MA5002", ManagerName = "D. Sylvia",    JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Carver"].Id },

            new() { Name = "Scott Pelletier",DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#CT1001", ManagerName = "L. Desanti",   JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Old Lyme"].Id },
            new() { Name = "Debra Riccio",   DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#CT1002", ManagerName = "L. Desanti",   JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Old Lyme"].Id },
            new() { Name = "J. Marinelli",   DefaultShift = "AM", TruckAssignment = null,     TruckId = null,      ManagerName = "L. Desanti",   JobTitle = "Driver Trainee", ResourceCategory = "Training",  BranchId = B["Old Lyme"].Id },

            new() { Name = "Carlos Rivera",  DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#MA6001", ManagerName = "A. Ortega",    JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Springfield"].Id },
            new() { Name = "Donna Reyes",    DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#MA6002", ManagerName = "A. Ortega",    JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Springfield"].Id },
            new() { Name = "Ray Colon",      DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#MA6003", ManagerName = "A. Ortega",    JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Springfield"].Id },

            new() { Name = "Phil Tremblay",  DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#CT2001", ManagerName = "G. Ferrante",  JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Monroe"].Id },
            new() { Name = "Kim Albano",     DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#CT2002", ManagerName = "G. Ferrante",  JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Monroe"].Id },

            // ── Mid-Atlantic ───────────────────────────────────────────────────
            new() { Name = "TJ Martinez",    DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#PJ1709", ManagerName = "Kyle Narkum",   JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Swedesboro"].Id },
            new() { Name = "Caleb Lucas",    DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#PJ1204", ManagerName = "Kyle Narkum",   JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Swedesboro"].Id },
            new() { Name = "Robert Hall",    DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#PJ0882", ManagerName = "Kyle Narkum",   JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Swedesboro"].Id },
            new() { Name = "M. Rodriguez",   DefaultShift = "AM", TruckAssignment = null,     TruckId = null,      ManagerName = "Kyle Narkum",   JobTitle = "Driver Trainee", ResourceCategory = "Training",  BranchId = B["Swedesboro"].Id },

            new() { Name = "Dave Kowalski",  DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#NJ1001", ManagerName = "B. Mancini",   JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Vernon"].Id },
            new() { Name = "Lori Gallo",     DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#NJ1002", ManagerName = "B. Mancini",   JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Vernon"].Id },
            new() { Name = "Mark Esposito",  DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#NJ1003", ManagerName = "B. Mancini",   JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Vernon"].Id },

            new() { Name = "Tony Soriano",   DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#NJ2001", ManagerName = "F. Caputo",    JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Bayville"].Id },
            new() { Name = "Sandy Greco",    DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#NJ2002", ManagerName = "F. Caputo",    JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Bayville"].Id },
            new() { Name = "R. DeSimone",    DefaultShift = "AM", TruckAssignment = null,     TruckId = null,      ManagerName = "F. Caputo",    JobTitle = "Driver Trainee", ResourceCategory = "Training",  BranchId = B["Bayville"].Id },

            new() { Name = "Wayne Stoltzfus",DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#PA1001", ManagerName = "H. Zimmerman", JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Salunga"].Id },
            new() { Name = "Ruth Beiler",    DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#PA1002", ManagerName = "H. Zimmerman", JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Salunga"].Id },
            new() { Name = "Earl Kreider",   DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#PA1003", ManagerName = "H. Zimmerman", JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Salunga"].Id },

            new() { Name = "Gary Harrington",DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#PA2001", ManagerName = "C. Moyer",     JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Ivyland"].Id },
            new() { Name = "Nancy Seidel",   DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#PA2002", ManagerName = "C. Moyer",     JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Ivyland"].Id },

            new() { Name = "Joe Kishbaugh",  DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#PA3001", ManagerName = "T. Wentz",     JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Honesdale"].Id },
            new() { Name = "Barb Labar",     DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#PA3002", ManagerName = "T. Wentz",     JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Honesdale"].Id },
            new() { Name = "K. Sterchak",    DefaultShift = "AM", TruckAssignment = null,     TruckId = null,      ManagerName = "T. Wentz",     JobTitle = "Driver Trainee", ResourceCategory = "Training",  BranchId = B["Honesdale"].Id },

            new() { Name = "Chuck Malone",   DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#PA4001", ManagerName = "D. Petrosky",  JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Washington"].Id },
            new() { Name = "Patty Sebek",    DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#PA4002", ManagerName = "D. Petrosky",  JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Washington"].Id },

            new() { Name = "Brian Svonavec", DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#PA5001", ManagerName = "M. Gindlesperger", JobTitle = "Driver",     ResourceCategory = "Pumping",   BranchId = B["Loretto"].Id },
            new() { Name = "Angie Plummer",  DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#PA5002", ManagerName = "M. Gindlesperger", JobTitle = "Driver",     ResourceCategory = "IG/Grease", BranchId = B["Loretto"].Id },

            new() { Name = "Marcus Webb",    DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#MD1001", ManagerName = "T. Hutchins",  JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Crofton"].Id },
            new() { Name = "Sharon Diggs",   DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#MD1002", ManagerName = "T. Hutchins",  JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Crofton"].Id },
            new() { Name = "Leon Watkins",   DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#MD1003", ManagerName = "T. Hutchins",  JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Crofton"].Id },

            new() { Name = "Roy Hundley",    DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#VA1001", ManagerName = "S. Garnett",   JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Jamaica"].Id },
            new() { Name = "Faye Tanner",    DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#VA1002", ManagerName = "S. Garnett",   JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Jamaica"].Id },

            // ── Mid-South ──────────────────────────────────────────────────────
            new() { Name = "Derek Shaw",     DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#NC1001", ManagerName = "Mickey Henson", JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Stanley"].Id },
            new() { Name = "Lisa Monroe",    DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#NC1002", ManagerName = "Mickey Henson", JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Stanley"].Id },
            new() { Name = "James Pryor",    DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#NC1003", ManagerName = "Mickey Henson", JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Stanley"].Id },

            new() { Name = "Sara Whitfield", DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#NC2001", ManagerName = "Mickey Henson", JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Boone"].Id },
            new() { Name = "Alan Bridges",   DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#NC2002", ManagerName = "Mickey Henson", JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Boone"].Id },
            new() { Name = "B. Combs",       DefaultShift = "AM", TruckAssignment = null,     TruckId = null,      ManagerName = "Mickey Henson", JobTitle = "Driver Trainee", ResourceCategory = "Training",  BranchId = B["Boone"].Id },

            new() { Name = "Pat Nguyen",     DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#NC3001", ManagerName = "Mickey Henson", JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Durham"].Id },
            new() { Name = "Chris Talbot",   DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#NC3002", ManagerName = "Mickey Henson", JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Durham"].Id },
            new() { Name = "Nina Park",      DefaultShift = "AM", TruckAssignment = null,     TruckId = null,      ManagerName = "Mickey Henson", JobTitle = "Driver Trainee", ResourceCategory = "Training",  BranchId = B["Durham"].Id },

            new() { Name = "Dwight Puckett", DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#GA1001", ManagerName = "R. Poole",     JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Hiram"].Id },
            new() { Name = "Cheryl Mahone",  DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#GA1002", ManagerName = "R. Poole",     JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Hiram"].Id },
            new() { Name = "Troy Hicks",     DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#GA1003", ManagerName = "R. Poole",     JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Hiram"].Id },

            new() { Name = "Wanda Colson",   DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#TN1001", ManagerName = "J. Vickers",   JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Arlington"].Id },
            new() { Name = "Brent Lester",   DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#TN1002", ManagerName = "J. Vickers",   JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Arlington"].Id },
            new() { Name = "D. Whitmore",    DefaultShift = "AM", TruckAssignment = null,     TruckId = null,      ManagerName = "J. Vickers",   JobTitle = "Driver Trainee", ResourceCategory = "Training",  BranchId = B["Arlington"].Id },

            // ── South ──────────────────────────────────────────────────────────
            new() { Name = "Ray Delgado",    DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#FL1001", ManagerName = "C. Ibarra",    JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Fort Myers"].Id },
            new() { Name = "Iris Castillo",  DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#FL1002", ManagerName = "C. Ibarra",    JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Fort Myers"].Id },
            new() { Name = "Marco Suarez",   DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#FL1003", ManagerName = "C. Ibarra",    JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Fort Myers"].Id },

            new() { Name = "Phil Okafor",    DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#FL2001", ManagerName = "D. Meeks",     JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Gainesville"].Id },
            new() { Name = "Tammy Byrd",     DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#FL2002", ManagerName = "D. Meeks",     JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Gainesville"].Id },
            new() { Name = "S. Weatherly",   DefaultShift = "AM", TruckAssignment = null,     TruckId = null,      ManagerName = "D. Meeks",     JobTitle = "Driver Trainee", ResourceCategory = "Training",  BranchId = B["Gainesville"].Id },

            new() { Name = "Andre Thomas",   DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#FL3001", ManagerName = "K. Hollis",    JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Jacksonville"].Id },
            new() { Name = "Gwen Crosby",    DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#FL3002", ManagerName = "K. Hollis",    JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Jacksonville"].Id },
            new() { Name = "Ben Strickland", DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#FL3003", ManagerName = "K. Hollis",    JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Jacksonville"].Id },

            new() { Name = "Hector Fuentes", DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#FL4001", ManagerName = "P. Escamilla", JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Largo"].Id },
            new() { Name = "Diana Rojas",    DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#FL4002", ManagerName = "P. Escamilla", JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Largo"].Id },
            new() { Name = "E. Quinones",    DefaultShift = "AM", TruckAssignment = null,     TruckId = null,      ManagerName = "P. Escamilla", JobTitle = "Driver Trainee", ResourceCategory = "Training",  BranchId = B["Largo"].Id },

            new() { Name = "Walt Kimball",   DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#FL5001", ManagerName = "N. Branson",   JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Stuart"].Id },
            new() { Name = "Connie Aldrich", DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#FL5002", ManagerName = "N. Branson",   JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Stuart"].Id },

            new() { Name = "Jesse Morales",  DefaultShift = "AM", TruckAssignment = "3,800g", TruckId = "#FL6001", ManagerName = "A. Varga",     JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Orlando"].Id },
            new() { Name = "Kim Dupont",     DefaultShift = "AM", TruckAssignment = "1,000g", TruckId = "#FL6002", ManagerName = "A. Varga",     JobTitle = "Driver",         ResourceCategory = "Pumping",   BranchId = B["Orlando"].Id },
            new() { Name = "Pete Salazar",   DefaultShift = "PM", TruckAssignment = "4,800g", TruckId = "#FL6003", ManagerName = "A. Varga",     JobTitle = "Driver",         ResourceCategory = "IG/Grease", BranchId = B["Orlando"].Id },
        };

        db.Employees.AddRange(employees);
        await db.SaveChangesAsync();

        // Seed April 2026 schedule for all employees
        await SeedAprilScheduleAsync(db, employees);
        } // end employees guard
    }

    private static async Task SeedAprilScheduleAsync(AppDbContext db, List<Employee> employees)
    {
        var year = 2026; var month = 4;
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var entries = new List<ScheduleEntry>();

        foreach (var emp in employees)
        {
            for (int d = 1; d <= daysInMonth; d++)
            {
                var date = new DateOnly(year, month, d);
                var dow = date.DayOfWeek;
                string code;
                if (dow == DayOfWeek.Saturday || dow == DayOfWeek.Sunday)
                    code = "—";
                else if (emp.ResourceCategory == "Training")
                    code = d <= 14 ? "TR" : "WA";
                else
                    code = emp.DefaultShift == "PM" ? "WP" : "WA";

                // Sprinkle realistic variations
                if (emp.Name == "TJ Martinez"  && d == 17) code = "PTO";
                if (emp.Name == "TJ Martinez"  && d == 11 && dow == DayOfWeek.Saturday) code = "OC";
                if (emp.Name == "Caleb Lucas"  && d == 7)  code = "CO";
                if (emp.Name == "Caleb Lucas"  && d == 24) code = "PTO";
                if (emp.Name == "Robert Hall"  && d == 11 && dow == DayOfWeek.Saturday) code = "OC";

                entries.Add(new ScheduleEntry
                {
                    EmployeeId = emp.Id,
                    Date       = date,
                    StatusCode = code,
                    CreatedBy  = "system",
                    CreatedAt  = DateTime.UtcNow
                });
            }
        }

        db.ScheduleEntries.AddRange(entries);
        await db.SaveChangesAsync();
    }

    // ── Users ──────────────────────────────────────────────────────────────────

    private static async Task SeedUsersAsync(UserManager<AppUser> userMgr, AppDbContext db)
    {
        if (await userMgr.Users.AnyAsync()) return;

        var swedesboroId = db.Branches.First(b => b.City == "Swedesboro").Id;
        var stanleyId    = db.Branches.First(b => b.City == "Stanley").Id;
        var booneId      = db.Branches.First(b => b.City == "Boone").Id;
        var durhamId     = db.Branches.First(b => b.City == "Durham").Id;

        var seedUsers = new (AppUser user, string password, string role, int[] branchIds)[]
        {
            (new AppUser { UserName = "admin@wre.com",     Email = "admin@wre.com",     FullName = "Admin User"      }, "Admin@123!",   AppRoles.PlannerDashboard,   []),
            (new AppUser { UserName = "planner@wre.com",   Email = "planner@wre.com",   FullName = "Sarah Planner"   }, "Planner@123!", AppRoles.PlannerDashboard,   []),
            (new AppUser { UserName = "fsup@wre.com",      Email = "fsup@wre.com",      FullName = "Kyle Narkum"     }, "FSuper@123!",  AppRoles.FieldSupervisor,    [swedesboroId]),
            (new AppUser { UserName = "dsup@wre.com",      Email = "dsup@wre.com",      FullName = "Maria Dispatch"  }, "DSuper@123!",  AppRoles.DispatchSupervisor, []),
            (new AppUser { UserName = "mickey@wre.com",    Email = "mickey@wre.com",    FullName = "Mickey Henson"   }, "Mickey@123!",  AppRoles.Planner,            [stanleyId, booneId, durhamId]),
            (new AppUser { UserName = "dispatch@wre.com",  Email = "dispatch@wre.com",  FullName = "Tom Dispatcher"  }, "Disp@123!",    AppRoles.Dispatcher,         [swedesboroId]),
            (new AppUser { UserName = "employee@wre.com",  Email = "employee@wre.com",  FullName = "Jane Employee"   }, "Emp@123!",     AppRoles.OtherEmployee,      [swedesboroId]),
        };

        foreach (var (user, password, role, branchIds) in seedUsers)
        {
            var result = await userMgr.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userMgr.AddToRoleAsync(user, role);
                foreach (var bid in branchIds)
                    db.UserBranches.Add(new AppUserBranch { UserId = user.Id, BranchId = bid });
            }
        }

        await db.SaveChangesAsync();
    }
}
