using System.Data.SqlClient;
using Voting_;

static void ADONet()
{
    // ЗАДАНИЕ 1
    var connection = new SqlConnection((@"Server = DESKTOP-GTAIH6J; Database = ConsignmentADO; Trusted_Connection = True"));
    connection.Open();
    Console.WriteLine("Список мужчин и женщин, отсортированных по возрасту:");
    var command1 = connection.CreateCommand();
    command1.CommandText = "select v.Name, c.Name from Vote v join Consignment c on v.ConsignmentId = c.Id order by  v.Gender desc, v.Date";
    var reader = command1.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"{reader.GetString(0)} {reader.GetString(1)}");

    }
    reader.Close();

    // зАДАНИЕ 2
    Console.WriteLine("\nСписок проголосовавших за КПРФ, родившихся позже 92:");
    var command2 = connection.CreateCommand();
    command2.CommandText = " select v.Name from Vote v join Consignment c on v.ConsignmentId = c.Id where c.Name = 'КПРФ' and v.Date > '1992-01-01'";
    reader = command2.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"Name: {reader.GetString(reader.GetOrdinal("Name"))}");
    }
    reader.Close();

    // ЗАДАНИЕ 3
    var command3 = connection.CreateCommand();
    command3.CommandText = "select count(*) from Vote";
    reader = command3.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"\nОбщее количество проголосовавших: {reader.GetValue(0)}");
    }
    reader.Close();
    var command3_2 = connection.CreateCommand();
    command3_2.CommandText = "select Gender, cast(count(*) as real) / cast((select count(*) from Vote) as real) as Количество from Vote group by Gender";
    reader = command3_2.ExecuteReader();
    Console.WriteLine("\nПроцент мужчин и женщин:");
    while (reader.Read())
    {
        Console.WriteLine($"{reader.GetBoolean(0)} : {reader.GetValue(1)}");
    }
    reader.Close();


    // Задание 4
    // вопрос - как отслеживать изменяющийся результат другой команды? 
    var command4_1 = connection.CreateCommand();
    var command4_2 = connection.CreateCommand();
    command4_1.CommandText = "insert into Vote(Name, Gender, Date, ConsignmentId) values(@Name, @Gender, GETDATE(), 3)";
    command4_2.CommandText = "select cast(count(*) as real) / cast((select count(*) from Vote) as real) as Количество from Vote where ConsignmentId = 3";
    reader = command4_2.ExecuteReader();
    while (reader.Read() && reader.GetFloat(0) < 70)
    {
        command4_1.Parameters.AddWithValue("Пупкин В.", 1);
        int res = command4_1.ExecuteNonQuery();
        reader = command4_2.ExecuteReader();
    }
    reader.Close();


    // ЗАДАНИЕ 5
    var command5 = connection.CreateCommand();
    command5.CommandText = "select c.Name, cast(count(*) as real) / cast((select count(*) from Vote) as real) from Vote v join Consignment c on c.Id = v.ConsignmentId group by c.Name ";
    reader = command5.ExecuteReader();
    Console.WriteLine("\nПроцент победы партий:");
    while (reader.Read())
    {
        Console.WriteLine($"{reader.GetString(0)} : {reader.GetValue(1)}");
    }
    reader.Close();
    connection.Close();

    // ЗАДАНИЕ 6

    var command_6 = connection.CreateCommand();
    var command_6_2 = connection.CreateCommand();
    command_6.CommandText = "delete from Vote where ConsignmentId = 2";
    command_6_2.CommandText = "delete from Consignment where Name = 'Яблоко'";
    int res1 =  command_6.ExecuteNonQuery();
    int res2 = command_6_2.ExecuteNonQuery();


}

static double persent(double all, double target)
{
    return target / all * 100;
}
static void EF()
{
    ConsignmentContex db = new ConsignmentContex();
    db.CreateDbIfNotExist();

    // ЗАДАНИЕ 1
    Console.WriteLine("Список мужчин и женщин, отсортированных по возрасту:");
    foreach (var item in db.Votes.OrderByDescending(x => x.Genger).ThenBy(x => x.Date).Select(x => new {name = x.Name, cons = x.сonsignment.Name}))
    {
        // не дает обращаться к {item.сonsignment.Name}
        Console.WriteLine($"{item.name} : {item.cons}");
    }

    // ЗАДАНИЕ 2
    Console.WriteLine("Список людей, родившихся после 92-ого года и проголосовавших за КПРФ:");
    foreach (var item in db.Votes.Where(x => x.Date > new DateTime(1992, 01, 01, 00, 00, 00) && x.сonsignment.Name == "КПРФ"))
    {
        Console.WriteLine($"{item.Name}");
    }

    double all = db.Votes.Count();
    // ЗАДАНИЕ 3
    Console.WriteLine("Общее количество проголосовавших, процент женщин и мужчин:");
    {
        double man = persent(all, db.Votes.Count(x => x.Genger == true));
        double woman = persent(all, db.Votes.Count(x => x.Genger == false));
        Console.WriteLine($"Всего: {all}");
        Console.WriteLine($"% Мужчин: {man}");
        Console.WriteLine($"% Женщин: {woman}");
    }

    //ЗАДАНИЕ 4
    Console.WriteLine("Добавление голосов в единую партию до 70%");
    while (persent(all, db.Votes.Count(x => x.сonsignment.Name == "Единая Россия")) < 70){
        db.AddNewVote(
            "Киселов В. В.", true,
            db.Consignments.Where(x => x.Name == "Единая Россия").First().Id,
            DateTime.Now);
        all = db.Votes.Count();
    }

    // ЗАДАНИЕ 5
    double  all2 = db.Votes.Count();
    Console.WriteLine("Процент победы партий:");
    foreach (var item in db.Votes.GroupBy(x => x.сonsignment.Name).Select(x => new { name = x.Key, count = x.Count() / all2 * 100 }))
    {
        Console.WriteLine($"{item.name} : {item.count}");
    }

    // ЗАДАНИЕ 6 
    Console.WriteLine("Удаление голосов за партию Яблоко и самой партии из списка");
    foreach(var item in db.Votes.Where(x => x.сonsignment.Name == "Яблоко")
                                .Select(x => new {idVote = x.Id, idCons = x.сonsignment.Id}))
    {
        db.DeleteVote(item.idVote);
       
    }
    db.DeleteCons(db.Consignments.Where(x => x.Name == "Яблоко").First().Id);

    foreach (var item in db.Votes.Select(x => new {cons = x.сonsignment.Name}))
    {
        Console.WriteLine(item.cons);
    }
}
EF();
//ADONet();