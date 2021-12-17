using System;
using System.Data;
using System.Data.SqlClient;

namespace BudgetCalculatorServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();

            Console.WriteLine("Starting authentication server for Budget calculator app");
            Console.WriteLine("Auth server running");

            server.ExecuteServer();



        }
    }
}
