using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voting_
{
    internal class ConsignmentContex : DbContext
    {
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Consignment> Consignments { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server = DESKTOP-GTAIH6J; Database = ConsignmentEF; Trusted_Connection = True");
            base.OnConfiguring(optionsBuilder);
        }
        public void CreateDbIfNotExist()
        {
            this.Database.EnsureCreated();

        }

        public void AddNewVote(string name, bool g, int cons, DateTime d)
        {
            ConsignmentContex context = new ConsignmentContex();

            // Создать нового покупателя
            Vote vote = new Vote
            {
                Name = name,
                Date = d,
                Genger = g,
                сonsignment = context.Consignments.Find(cons)
            };

            // Добавить в DbSet
            context.Votes.AddRange(vote);

            // Сохранить изменения в базе данных
            context.SaveChanges();
        }

        public void DeleteVote(int id)
        {
            ConsignmentContex context = new ConsignmentContex();
            Vote vote = context.Votes
                        .Where(o => o.Id == id)
                        .FirstOrDefault();

            context.Votes.Remove(vote);
            context.SaveChanges();
        }
        public void DeleteCons(int id)
        {
            ConsignmentContex context = new ConsignmentContex();
            Consignment cons = context.Consignments
                        .Where(o => o.Id == id)
                        .FirstOrDefault();

            context.Consignments.Remove(cons);
            context.SaveChanges();
        }

        //не работает 


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    List<Vote> votes;
        //    List<Consignment> consignments;
        //    consignments = new List<Consignment>()
        //    {
        //        new Consignment { Id = 1, Name = "КПРФ" },
        //        new Consignment { Id = 2, Name = "Яблоко" },
        //        new Consignment { Id = 3, Name = "Единая Россия" },
        //    };
        //    votes = new List<Vote>()
        //    {
        //        new Vote { Id = 1, Name = "Иванов А. А.", Date = new DateTime(1987, 7, 21, 0, 0, 0), сonsignment = consignments[0], Genger = true },
        //            new Vote { Id = 2, Name = "Иванова Н. В.", Date = new DateTime(1992, 4, 15, 0, 0, 0), сonsignment = consignments[0], Genger = false },
        //            new Vote { Id = 3, Name = "Петрова М. А.", Date = new DateTime(1980, 8, 16, 0, 0, 0), сonsignment = consignments[0], Genger = false },
        //            new Vote { Id = 4, Name = "Клементьев А. А.", Date = new DateTime(1999, 9, 9, 0, 0, 0), сonsignment = consignments[1], Genger = true },
        //            new Vote { Id = 5, Name = "Комаров В. В.", Date = new DateTime(2001, 1, 15, 0, 0, 0), сonsignment = consignments[1], Genger = true },
        //            new Vote { Id = 6, Name = "Рыков А. А.", Date = new DateTime(1965, 4, 16, 0, 0, 0), сonsignment = consignments[1], Genger = true },
        //            new Vote { Id = 7, Name = "Семенов Р. А.", Date = new DateTime(1975, 6, 12, 0, 0, 0), сonsignment = consignments[1], Genger = true },
        //            new Vote { Id = 8, Name = "Мухина В. Д.", Date = new DateTime(1989, 5, 30, 0, 0, 0), сonsignment = consignments[2], Genger = false },
        //            new Vote { Id = 9, Name = "Романова Т. Л.", Date = new DateTime(2002, 12, 30, 0, 0, 0), сonsignment = consignments[2], Genger = false },
        //            new Vote { Id = 10, Name = "Краков В. Д.", Date = new DateTime(1998, 8, 15, 0, 0, 0), сonsignment = consignments[2], Genger = true },
        //            new Vote { Id = 11, Name = "Тимофеев Р. М.", Date = new DateTime(1987, 1, 3, 0, 0, 0), сonsignment = consignments[2], Genger = true }
        //    };
        //    modelBuilder.Entity<Consignment>().HasData(consignments);
        //    modelBuilder.Entity<Vote>().HasData(votes);
        //}


    }


    public class Consignment
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }

    }

    public class Vote
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public bool Genger { get; set; }
        public DateTime Date { get; set; }
        public Consignment сonsignment { get; set; }
    }

   

}
