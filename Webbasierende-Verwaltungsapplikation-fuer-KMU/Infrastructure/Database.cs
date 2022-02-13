using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Helper;
using Webbasierende_Verwaltungsapplikation_fuer_KMU.Model;

namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class Database : DbContext
    {
        public Database()
        {
            Database.SetCommandTimeout((int?)new TimeSpan(0, 10, 0).TotalSeconds);
        }

        public DbSet<Document> Document => Set<Document>();
        public DbSet<Element> Element => Set<Element>();
        public DbSet<TimeRecordingElement> TimeRecordingElement => Set<TimeRecordingElement>();
        public DbSet<Order> Order => Set<Order>();
        public DbSet<Status> Status => Set<Status>();
        public DbSet<Tag> Tag => Set<Tag>();
        public DbSet<Ticket> Ticket => Set<Ticket>();
        public DbSet<Message> Message => Set<Message>();
        public DbSet<User> User => Set<User>();
        public DbSet<Address> Address => Set<Address>();
        public DbSet<Operator> Operator => Set<Operator>();
        public DbSet<PhoneNumber> PhoneNumber => Set<PhoneNumber>();

        public DbSet<Party> Party { get; set; } = default!;
        public DbSet<Company> Company { get; set; } = default!;
        public DbSet<Customer> Customer { get; set; } = default!;
        public DbSet<Employee> Employee { get; set; } = default!;

        /// <summary>
        /// Configures the context to connect to a Microsoft SQL Server database.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Webbasierende-Verwaltungsapplikation-fuer-KMU;Integrated Security=True");
        }

        /// <summary>
        /// Creates Relations for the Database.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Party>().OwnsOne(c => c.Rights);
            modelBuilder.Entity<Customer>().OwnsOne(c => c.Person);
            modelBuilder.Entity<Employee>().OwnsOne(c => c.Person);
            modelBuilder.Entity<Party>().HasMany(c => c.PhoneNumbers);
            modelBuilder.Entity<Ticket>().HasMany(c => c.TimeRecordingElements);
            modelBuilder.Entity<Ticket>().HasMany(c => c.Elements);
            modelBuilder.Entity<Party>().HasMany(c => c.Address);
            modelBuilder.Entity<Operator>().HasOne(c => c.OfficeAddress);
            modelBuilder.Entity<Order>().HasMany(o => o.Conversations).WithOne(m => m.Order!).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Ticket>().HasMany(m => m.Messages).WithOne(m => m.Ticket!).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<User>().HasOne(m => m.Party).WithOne(m => m!.User!).HasForeignKey<Party>(m => m.UserId);
            modelBuilder.Entity<Party>().HasMany(m => m.Documents);
            modelBuilder.Entity<Address>().HasOne(m => m.Party);
            modelBuilder.Entity<Party>().HasMany(m => m.Address);
            modelBuilder.Entity<Conversation>().HasMany(e => e.Messages);
        }

        public void GenerateFakeData()
        {
            Randomizer.Seed = new Random(1111);
            Database.SetCommandTimeout(60);
            Status.AddRange(new Status() { Name = "Done", Sequence = 0 }, new Status() { Name = "InProgress", Sequence = 1 }, new Status() { Name = "Backlog", Sequence = 2 });
            SaveChanges();
            Console.WriteLine("Starting...");
            List<string> MimeTypes = new() { ".pdf", ".png", ".jpg" };
            Operator.Add(new Operator()
            {
                Name = "Vaikoon",
                UID = "U72376216",
                TermsAndConditions = "this is a text :)",
                OfficeEmail = "office@testmail.com",
                Owner = "Hafner",
                PhoneNumber = "07874080746",
                OfficeAddress = new Address() { Country = "Österreich", ZipCode = "AT-1010", City = "Wien", Street = "Opernring", State = "Wien", DoorNumber = "720", HouseNumber = "E" },
            });
            SaveChanges();

            User.AddRange(new Faker<User>().Rules((f, u) =>
            {
                u.Secret = HashMethods.GenerateSecret();
                u.PasswordHash = HashMethods.HashPassword(u.Secret, "12345");
                u.RegisteredAt = f.Date.Between(new DateTime(2005, 1, 1, 12, 0, 0), new DateTime(2015, 1, 1));

                u.LoginEMail = f.Person.Email;

                #region contact
                CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                ci.NumberFormat.CurrencyDecimalSeparator = ".";
                u.Active = true;
                u.Party = f.Random.Bool() ? new Faker<Customer>().Rules((f, c) =>
                {
                    c.Rights.CanCreateNewConversation = f.Random.Bool(float.Parse("0.90", NumberStyles.Any, ci));
                    c.Rights.CanUploadFiles = f.Random.Bool(float.Parse("0.90", NumberStyles.Any, ci));
                    c.Role = Role.Customer;
                    c.Person = new Model.Person(f.Person.LastName, f.Person.FirstName, f.PickRandom<Gender>(), f.Person.DateOfBirth);
                    c.AcceptedTerms = true;
                }) : f.Random.Bool() ? new Faker<Company>().Rules((f, c) =>
                {
                    c.Rights.CanCreateNewConversation = f.Random.Bool(float.Parse("0.90", NumberStyles.Any, ci));
                    c.Rights.CanUploadFiles = f.Random.Bool(float.Parse("0.90", NumberStyles.Any, ci));
                    c.Role = Role.Company;
                    c.CompanyName = f.Company.CompanyName();
                    c.UID = f.Random.Utf16String(11, 11);
                    c.AcceptedTerms = true;
                }) : new Faker<Employee>().Rules((f, e) =>
                {
                    e.Role = Role.Employee;
                    e.Rights.CanCreateNewConversation = false;
                    e.Rights.CanUploadFiles = f.Random.Bool(float.Parse("0.95", NumberStyles.Any, ci));
                    e.Person = new Model.Person(f.Person.LastName, f.Person.FirstName, f.PickRandom<Gender>(), f.Person.DateOfBirth);
                    e.HourlyRate = f.Random.Decimal(15, 30).ToString();
                });

                u.Party.EMail = u.LoginEMail;
                u.Party.Address.AddRange(new Faker<Address>().Rules((f, a) =>
                {
                    a.Country = f.Address.Country();
                    a.State = f.Address.State();
                    a.ZipCode = f.Address.ZipCode();
                    a.City = f.Address.City();
                    a.Street = f.Address.StreetName();
                    a.StairCase = f.Random.Int(1, 10).ToString();
                    a.HouseNumber = f.Address.BuildingNumber();
                    a.DoorNumber = f.Random.Int(1, 10).ToString();
                    a.IsPrimary = false;
                }).Generate(2).ToList());
                u.Party.PhoneNumbers.AddRange(new Faker<PhoneNumber>().Rules((f, t) =>
                {
                    t.Number = f.Phone.PhoneNumber();
                    t.Type = f.PickRandom<PhoneNumberType>();

                }).Generate(2).ToList());
                #endregion

                if (u.Party.GetType() == typeof(Company) || u.Party.GetType() == typeof(Customer))
                {
                    u.Party.Orders.AddRange(new Faker<Order>().Rules((f, p) =>
                        {
                            p.OrderStatus = OrderStatus.Open;
                            p.Created = f.Date.Between(new DateTime(2005, 1, 1, 12, 0, 0), new DateTime(2015, 1, 1));
                            p.LastUpdated = f.Date.Between(new DateTime(2005, 1, 1, 12, 0, 0), new DateTime(2015, 1, 1));
                            p.BillingAddress = f.Random.ListItem(u.Party.Address);
                            p.DeliveryAddress = f.Random.ListItem(u.Party.Address);
                            p.Guid = f.Random.Guid();
                            p.Name = f.Commerce.ProductName();
                            p.Description = f.Commerce.ProductDescription();

                            p.Tickets.AddRange(new Faker<Ticket>().Rules((f, t) =>
                            {
                                t.EmployeeParty = u.Party;
                                t.Name = f.Commerce.ProductName();
                                t.Description = f.Commerce.ProductDescription();
                                t.Tags.AddRange(new Faker<Tag>().Rules((f, r) =>
                                {
                                    r.Name = f.Commerce.ProductMaterial();
                                }).Generate(f.Random.Int(0, 10)).ToList());
                                t.Created = f.Date.Between(new DateTime(2005, 1, 1, 12, 0, 0), new DateTime(2015, 1, 1));
                                t.LastUpdated = t.Created.AddYears(f.Random.Int(0, 5)).AddMonths(f.Random.Int(0, 11)).AddDays(f.Random.Int(0, 364)).AddHours(f.Random.Int(0, 24)).AddMinutes(f.Random.Int(0, 60));
                                t.Priority = f.PickRandom<Priority>();
                                t.Status = f.Random.ListItem<Status>(Status.ToList());
                                t.SendNotification = f.Random.Bool();
                                t.Elements.AddRange(new Faker<Element>().Rules((f, e) =>
                                {
                                    e.Description = f.Random.Words();
                                    e.Created = f.Date.Between(t.Created, t.Created.AddYears(f.Random.Int(0, 1)).AddMonths(f.Random.Int(0, 11)).AddDays(f.Random.Int(0, 364)).AddHours(f.Random.Int(0, 24)).AddMinutes(f.Random.Int(0, 60)));
                                    e.Ammount = f.Random.Decimal(0.50m, 20.0m).ToString();
                                    e.PricePerItem = f.Random.Int(1, 20).ToString();
                                }).Generate(f.Random.Int(0, 10)).ToList());
                                t.TimeRecordingElements.AddRange(new Faker<TimeRecordingElement>().Rules((f, t) =>
                                {
                                    t.Created = f.Date.Between(t.Created, t.Created.AddYears(f.Random.Int(0, 1)).AddMonths(f.Random.Int(0, 11)).AddDays(f.Random.Int(0, 364)).AddHours(f.Random.Int(0, 24)).AddMinutes(f.Random.Int(0, 60)));
                                    t.Description = f.Random.Utf16String(10, 20);
                                    t.Minutes = f.Random.Decimal(0.5m, 20.0m);
                                    t.PricePerHour = f.Random.Decimal(15.0m, 35.0m).ToString();
                                }).Generate(f.Random.Int(0, 10)).ToList());
                                t.Note = f.Commerce.ProductDescription();
                                t.Documents.AddRange(new Faker<Document>().Rules((f, d) =>
                                {
                                    d.Created = t.Created.AddYears(f.Random.Int(0, 1)).AddMonths(f.Random.Int(0, 11)).AddDays(f.Random.Int(0, 364)).AddHours(f.Random.Int(0, 24)).AddMinutes(f.Random.Int(0, 60));
                                    d.LastUpdated = d.Created.AddYears(f.Random.Int(0, 5)).AddMonths(f.Random.Int(0, 11)).AddDays(f.Random.Int(0, 364)).AddHours(f.Random.Int(0, 24)).AddMinutes(f.Random.Int(0, 60));
                                    d.DocumentType = f.Random.Bool() ? DocumentType.Standard : DocumentType.CompanyIntern;
                                    d.OriginalName = f.Random.Word();
                                    d.BlobFileName = (f.Random.Guid() + d.Created.ToString());
                                    d.BlobContainer = "documents";
                                    d.MimeType = f.Random.ListItem<string>(MimeTypes).ToString();
                                    d.Size = f.Random.Int(1, 10000);
                                }).Generate(f.Random.Int(0, 10)).ToList());


                            }).Generate(f.Random.Int(0, 10)).ToList());
                            p.Note = f.Random.Words(f.Random.Int(10, 30));
                            p.Documents.AddRange(new Faker<Document>().Rules((f, d) =>
                            {
                                d.Created = f.Date.Between(new DateTime(2005, 1, 1, 12, 0, 0), new DateTime(2015, 1, 1));
                                d.LastUpdated = d.Created.AddYears(f.Random.Int(0, 5)).AddMonths(f.Random.Int(0, 11)).AddDays(f.Random.Int(0, 364)).AddHours(f.Random.Int(0, 24)).AddMinutes(f.Random.Int(0, 60));
                                d.DocumentType = DocumentType.Standard;
                                d.OriginalName = f.Random.Word();
                                d.BlobFileName = (f.Random.Guid() + d.Created.ToString());
                                d.BlobContainer = "documents";
                                d.MimeType = f.Random.ListItem<string>(MimeTypes).ToString();
                                d.Size = f.Random.Int(1, 10000);
                            }).Generate(3).ToList());
                        }).Generate(3).ToList());
                }
                Console.WriteLine("Generating User");
            }).Generate(50).ToList());
            Console.WriteLine("Generated User");
            SaveChanges();

            foreach (var t in Document.ToList())
            {
                t.Party = new Randomizer().ListItem<Employee>(Employee.ToList());
            }
            Console.WriteLine("Added Employees to Documents.");
            SaveChanges();

            foreach (var t in TimeRecordingElement.ToList())
            {
                var party = new Randomizer().ListItem<Employee>(Employee.ToList());
                t.Party = party;
                t.PricePerHour = party.HourlyRate;
            }
            Console.WriteLine("Added Timerecords to Employees.");
            SaveChanges();

            foreach (var t in Ticket.ToList())
            {
                t.EmployeeParty = new Randomizer().ListItem<Employee>(Employee.ToList());
            }
            Console.WriteLine("Added Ticket to Employees.");
            SaveChanges();
            Console.WriteLine("DONE!");

            Employee.First().Role = Role.Admin;
            Console.WriteLine("Converted one employee to an admin.");
            SaveChanges();
        }

    }
}
