using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Data.SqlClient;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;

public class Categories
{
    public string Name;
}
public class Cities
{
    public string Name;
}
public class Clients
{
    public string FullName;
    public string DateOfBith;
    public string Gender;
    public string Email;
    public int CountryId;
    public int CityId;
}
public class Countries
{
    public string Name;
}
public class InterestedBuyers
{
    public int ClientId;
    public int CategoryId;
}
public class Products
{
    public string Name;
    public int CategoryId;
}
public class Promotions
{
    public int Percent;
    public string StartDate;
    public string EndDate;
    public int CountryId;
    public int ProducId;
}

class Repository
{
    readonly string connectionString =
        @"Data Source=DESKTOP-VKP7RF3\SQLEXPRESS01;Initial Catalog=MailingsDb;Integrated Security=True";

    public List<Categories> GetCategories()
    {
        using var db = new SqlConnection(connectionString);
        db.Open();

        var query = "SELECT * FROM Categories";
        var columns = db.Query<Categories>(query);

        return columns.ToList();
    }
    public List<Cities> GetCities()
    {
        using var db = new SqlConnection(connectionString);
        db.Open();

        var query = "SELECT * FROM Cities";
        var columns = db.Query<Cities>(query);

        return columns.ToList();
    }
    public List<Clients> GetClients()
    {
        using var db = new SqlConnection(connectionString);
        db.Open();

        var query = "SELECT * FROM Clients";
        var columns = db.Query<Clients>(query);

        return columns.ToList();
    }
    public List<Countries> GetCountries()
    {
        using var db = new SqlConnection(connectionString);
        db.Open();

        var query = "SELECT * FROM Countries";
        var columns = db.Query<Countries>(query);

        return columns.ToList();
    }
    public List<InterestedBuyers> GetInterestedBuyers()
    {
        using var db = new SqlConnection(connectionString);
        db.Open();

        var query = "SELECT * FROM InterestedBuyers";
        var columns = db.Query<InterestedBuyers>(query);

        return columns.ToList();
    }
    public List<Products> GetProducts()
    {
        using var db = new SqlConnection(connectionString);
        db.Open();

        var query = "SELECT * FROM Products";
        var columns = db.Query<Products>(query);

        return columns.ToList();
    }
    public List<Promotions> GetPromotions()
    {
        using var db = new SqlConnection(connectionString);
        db.Open();

        var query = "SELECT * FROM Promotions";
        var columns = db.Query<Promotions>(query);

        return columns.ToList();
    }

    public void InsertValues(string table, string name)
    {
        using var db = new SqlConnection(connectionString);
        db.Open();

        var query = $"INSERT INTO {table} VALUES ('{name}')";
        SqlCommand command = new SqlCommand(query, db);
        command.ExecuteNonQuery();
    }
    public void InsertProducts(string name, int index)
    {
        using var db = new SqlConnection(connectionString);
        db.Open();

        var query = "INSERT INTO Products (Name, CategoryId) VALUES (@name, @index)";
        var columns = db.Query<Products>(query, new { name, index });
    }
    
    public void UpdateValues(string table, string name, int index)
    {
        using var db = new SqlConnection(connectionString);
        db.Open();

        var query = $"UPDATE {table} SET Name = '{name}' WHERE Id = {index}";
        SqlCommand command = new SqlCommand(query, db);
        command.ExecuteNonQuery();
    }

    public void DeleteValues(string table, string name)
    {
        using var db = new SqlConnection(connectionString);
        db.Open();

        var query = $"DELETE {table} WHERE Name = '{name}'";
        SqlCommand command = new SqlCommand(query, db);
        command.ExecuteNonQuery();
    }
}

public class Programm
{
    private static Repository repository;

    public static void Main()
    {
        repository = new Repository();
        StartServer();
        Console.Read();
    }

    static async Task StartServer()
    {
        HttpListener server = new HttpListener();
        server.Prefixes.Add("http://127.0.0.1:8080/");
        server.Start();

        Console.WriteLine("Server started ...");

        while (true)
        {
            var context = await server.GetContextAsync();
            var request = context.Request;
            var response = context.Response;

            using var output = new StreamWriter(context.Response.OutputStream);

            if (request.RawUrl == "/getCategories")
            {
                foreach (var item in repository.GetCategories())
                {
                    await output.WriteLineAsync(item.Name);
                }
            }
            else if (request.RawUrl == "/getCities")
            {
                foreach (var item in repository.GetCities())
                {
                    await output.WriteLineAsync(item.Name);
                }
            }
            else if (request.RawUrl == "/getClients")
            {
                foreach (var item in repository.GetClients())
                {
                    await output.WriteLineAsync(item.FullName + " " 
                        + item.DateOfBith + " " 
                        + item.Gender + " " 
                        + item.Email + " "
                        + item.CountryId + " "
                        + item.CityId);
                }
            }
            else if (request.RawUrl == "/getCountries")
            {
                foreach (var item in repository.GetCountries())
                {
                    await output.WriteLineAsync(item.Name);
                }
            }
            else if (request.RawUrl == "/getInterestedBuyers")
            {
                foreach (var item in repository.GetInterestedBuyers())
                {
                    await output.WriteLineAsync(item.ClientId + " " + item.CategoryId);
                }
            }
            else if (request.RawUrl == "/getProducts")
            {
                foreach (var item in repository.GetProducts())
                {
                    await output.WriteLineAsync(item.Name + " " + item.CategoryId);
                }
            }
            else if (request.RawUrl == "/getPromotions")
            {
                foreach (var item in repository.GetPromotions())
                {
                    await output.WriteLineAsync(item.Percent + " " 
                        + item.StartDate + " " 
                        + item.EndDate + " " 
                        + item.CountryId + " "
                        + item.ProducId);
                }
            }
            else if (request.QueryString.GetValues("name") != null)
            {
                string name = request.QueryString.GetValues("name")[0].ToString();

                if (request.RawUrl.StartsWith("/InsertCountries")) 
                    repository.InsertValues((request.QueryString.GetValues("table")[0]).ToString(), name);
                else if (request.RawUrl.StartsWith("/InsertCities"))
                    repository.InsertValues((request.QueryString.GetValues("table")[0]).ToString(), name);
                else if (request.RawUrl.StartsWith("/InsertCategories"))
                    repository.InsertValues((request.QueryString.GetValues("table")[0]).ToString(), name);
                else if (request.RawUrl.StartsWith("/InsertProducts")) repository.InsertProducts(name,
                    Convert.ToInt32(request.QueryString.GetValues("index")[0]));
                else if(request.RawUrl.StartsWith("/UpdateCountries")) 
                    repository.UpdateValues(request.QueryString.GetValues("table")[0].ToString(), name, 
                    Convert.ToInt32(request.QueryString.GetValues("index")[0]));
                else if (request.RawUrl.StartsWith("/UpdateCities")) 
                    repository.UpdateValues(request.QueryString.GetValues("table")[0].ToString(), name, 
                    Convert.ToInt32(request.QueryString.GetValues("index")[0]));
                else if (request.RawUrl.StartsWith("/UpdateCategories")) 
                    repository.UpdateValues(request.QueryString.GetValues("table")[0].ToString(), name,
                    Convert.ToInt32(request.QueryString.GetValues("index")[0]));
                else if (request.RawUrl.StartsWith("/UpdateProducts")) 
                    repository.UpdateValues(request.QueryString.GetValues("table")[0].ToString(), name,
                    Convert.ToInt32(request.QueryString.GetValues("index")[0]));
                else if (request.RawUrl.StartsWith("/UpdateClients")) 
                    repository.UpdateValues(request.QueryString.GetValues("table")[0].ToString(), name,
                    Convert.ToInt32(request.QueryString.GetValues("index")[0]));
                else if (request.RawUrl.StartsWith("/DeleteCountries")) 
                    repository.DeleteValues(request.QueryString.GetValues("table")[0].ToString(), name);
                else if (request.RawUrl.StartsWith("/DeleteCities")) 
                    repository.DeleteValues(request.QueryString.GetValues("table")[0].ToString(), name);
                else if (request.RawUrl.StartsWith("/DeleteCategories")) 
                    repository.DeleteValues(request.QueryString.GetValues("table")[0].ToString(), name);
                else if (request.RawUrl.StartsWith("/DeleteProducts")) 
                    repository.DeleteValues(request.QueryString.GetValues("table")[0].ToString(), name);
                else if (request.RawUrl.StartsWith("/DeleteClients")) 
                    repository.DeleteValues(request.QueryString.GetValues("table")[0].ToString(), name);
            }
        }

        server.Stop();
    }
}

