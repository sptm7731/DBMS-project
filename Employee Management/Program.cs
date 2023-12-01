using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

class Program
{
    static string connectionString = "Data Source=DESKTOP-34HOJHD\\SQLEXPRESS2;Initial Catalog=test1;Integrated Security=True;Encrypt=False";

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. Search Employee by Emp_ID");
            Console.WriteLine("3. List All Employees");
            Console.WriteLine("4. Delete Employee by Emp_ID");
            Console.WriteLine("5. Update Employee Information");
            Console.WriteLine("6. List Employees and Salaries (Descending)");
            Console.WriteLine("7. List Employees Grouped by District");
            Console.WriteLine("8. Exit");

            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Invalid choice. Please enter a number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    AddEmployee();
                    break;
                case 2:
                    SearchEmployee();
                    break;
                case 3:
                    ListAllEmployees();
                    break;
                case 4:
                    DeleteEmployee();
                    break;
                case 5:
                    UpdateEmployee();
                    break;
                case 6:
                    ListEmployeesAndSalariesDescending();
                    break;
                case 7:
                    ListEmployeesGroupedByDistrict();
                    break;
                case 8:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please enter a valid option.");
                    break;
            }
        }
    }

    static void AddEmployee()
    {
        // Collect employee data from the user
        Console.Write("Enter Emp_ID: ");
        int empId = int.Parse(Console.ReadLine());

        Console.Write("Enter First Name: ");
        string firstName = Console.ReadLine();

        Console.Write("Enter Last Name: ");
        string lastName = Console.ReadLine();

        Console.Write("Enter Salary: ");
        int salary = int.Parse(Console.ReadLine());

        Console.Write("Enter Position: ");
        string position = Console.ReadLine();

        // Collect address data
        Console.Write("Enter City: ");
        string city = Console.ReadLine();

        Console.Write("Enter District: ");
        string district = Console.ReadLine();

        Console.Write("Enter Street: ");
        string street = Console.ReadLine();

        // Collect emp_info data
        Console.Write("Enter Email: ");
        string email = Console.ReadLine();

        Console.Write("Enter Phone: ");
        string phone = Console.ReadLine();

        Console.Write("Enter Field: ");
        string field = Console.ReadLine();

        Console.Write("Enter Hire Date (yyyy-MM-dd): ");
        DateTime hireDate = DateTime.Parse(Console.ReadLine());

        Console.Write("Enter Work Experience: ");
        int workExp = int.Parse(Console.ReadLine());

        // Insert data into the database
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                // Insert data into the employee table
                cmd.CommandText = "INSERT INTO employee (Emp_ID, F_Name, L_Name, Salary, Position) " +
                                 "VALUES (@Emp_ID, @F_Name, @L_Name, @Salary, @Position);";
                cmd.Parameters.AddWithValue("@Emp_ID", empId);
                cmd.Parameters.AddWithValue("@F_Name", firstName);
                cmd.Parameters.AddWithValue("@L_Name", lastName);
                cmd.Parameters.AddWithValue("@Salary", salary);
                cmd.Parameters.AddWithValue("@Position", position);
                cmd.ExecuteNonQuery();

                // Get the auto-generated serial ID
                cmd.CommandText = "SELECT @@IDENTITY";
                int serial = Convert.ToInt32(cmd.ExecuteScalar());

                // Insert data into the Address table
                cmd.CommandText = "INSERT INTO Address (ID, City, District, Street) " +
                                 "VALUES (@ID, @City, @District, @Street);";
                cmd.Parameters.AddWithValue("@ID", serial);
                cmd.Parameters.AddWithValue("@City", city);
                cmd.Parameters.AddWithValue("@District", district);
                cmd.Parameters.AddWithValue("@Street", street);
                cmd.ExecuteNonQuery();

                // Insert data into the Emp_info table
                cmd.CommandText = "INSERT INTO Emp_info (ID, email, phone, field, hire_date, work_exp) " +
                                 "VALUES (@ID2, @email, @phone, @field, @hire_date, @work_exp);";
                cmd.Parameters.AddWithValue("@ID2", serial);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@field", field);
                cmd.Parameters.AddWithValue("@hire_date", hireDate);
                cmd.Parameters.AddWithValue("@work_exp", workExp);
                cmd.ExecuteNonQuery();

                Console.WriteLine("Employee added successfully.");
            }
        }
    }

    static void SearchEmployee()
    {
        Console.Write("Enter Emp_ID: ");
        int empId = int.Parse(Console.ReadLine());

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT e.Emp_ID, e.F_Name, e.L_Name, e.Salary, e.Position, a.City, a.District, a.Street, ei.email, ei.phone, ei.field, ei.hire_date, ei.work_exp " +
                                 "FROM employee e " +
                                 "JOIN Address a ON e.serial = a.ID " +
                                 "JOIN Emp_info ei ON e.serial = ei.ID " +
                                 "WHERE e.Emp_ID = @Emp_ID";
                cmd.Parameters.AddWithValue("@Emp_ID", empId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine("Employee Details:");
                        Console.WriteLine($"Emp_ID: {reader["Emp_ID"],-10}");
                        Console.WriteLine($"First Name: {reader["F_Name"],-15}");
                        Console.WriteLine($"Last Name: {reader["L_Name"],-15}");
                        Console.WriteLine($"Salary: {reader["Salary"],-10}");
                        Console.WriteLine($"Position: {reader["Position"],-20}");
                        Console.WriteLine($"City: {reader["City"],-15}");
                        Console.WriteLine($"District: {reader["District"],-15}");
                        Console.WriteLine($"Street: {reader["Street"],-15}");
                        Console.WriteLine($"Email: {reader["email"],-20}");
                        Console.WriteLine($"Phone: {reader["phone"],-20}");
                        Console.WriteLine($"Field: {reader["field"],-15}");
                        Console.WriteLine($"Hire Date: {reader["hire_date"],-10}");
                        Console.WriteLine($"Work Experience: {reader["work_exp"],-10}");
                    }
                    else
                    {
                        Console.WriteLine($"Employee with Emp_ID {empId} not found.");
                    }
                }
            }
        }
    }

    static void ListAllEmployees()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT e.Emp_ID, e.F_Name, e.L_Name, e.Salary, e.Position, a.City, a.District, a.Street, ei.email, ei.phone, ei.field, ei.hire_date, ei.work_exp " +
                                 "FROM employee e " +
                                 "JOIN Address a ON e.serial = a.ID " +
                                 "JOIN Emp_info ei ON e.serial = ei.ID";

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("All Employees:");
                    Console.WriteLine("{0,-10}{1,-15}{2,-15}{3,-10}{4,-20}{5,-15}{6,-15}{7,-15}{8,-20}{9,-20}{10,-15}{11,-10}{12,-10}",
                        "Emp_ID", "First Name", "Last Name", "Salary", "Position", "City", "District", "Street", "Email", "Phone", "Field", "Hire Date", "Work Exp");
                    while (reader.Read())
                    {
                        Console.WriteLine("{0,-10}{1,-15}{2,-15}{3,-10}{4,-20}{5,-15}{6,-15}{7,-15}{8,-20}{9,-20}{10,-15}{11,-10}{12,-10}",
                            reader["Emp_ID"], reader["F_Name"], reader["L_Name"], reader["Salary"], reader["Position"],
                            reader["City"], reader["District"], reader["Street"], reader["email"], reader["phone"], reader["field"],
                            reader["hire_date"], reader["work_exp"]);
                    }
                }
            }
        }
    }

    static void DeleteEmployee()
    {
        Console.Write("Enter Emp_ID to delete: ");
        int empId = int.Parse(Console.ReadLine());

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM employee WHERE Emp_ID = @Emp_ID";
                cmd.Parameters.AddWithValue("@Emp_ID", empId);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Employee with Emp_ID {empId} deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"Employee with Emp_ID {empId} not found.");
                }
            }
        }
    }

static void UpdateEmployee()
{
    Console.Write("Enter Emp_ID to update: ");
    int empId = int.Parse(Console.ReadLine());

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();

        using (SqlCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT e.serial, e.F_Name, e.L_Name, e.Salary, e.Position, " +
                              "a.City, a.District, a.Street, " +
                              "ei.email, ei.phone, ei.field, ei.hire_date, ei.work_exp " +
                              "FROM employee e " +
                              "JOIN Address a ON e.serial = a.ID " +
                              "JOIN Emp_info ei ON e.serial = ei.ID " +
                              "WHERE e.Emp_ID = @Emp_ID";
            cmd.Parameters.AddWithValue("@Emp_ID", empId);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    Console.WriteLine("Current Employee Details:");
                    Console.WriteLine($"Emp_ID: {reader["serial"]}");
                    Console.WriteLine($"1. First Name: {reader["F_Name"]}");
                    Console.WriteLine($"2. Last Name: {reader["L_Name"]}");
                    Console.WriteLine($"3. Salary: {reader["Salary"]}");
                    Console.WriteLine($"4. Position: {reader["Position"]}");
                    Console.WriteLine($"5. City: {reader["City"]}");
                    Console.WriteLine($"6. District: {reader["District"]}");
                    Console.WriteLine($"7. Street: {reader["Street"]}");
                    Console.WriteLine($"8. Email: {reader["email"]}");
                    Console.WriteLine($"9. Phone: {reader["phone"]}");
                    Console.WriteLine($"10. Field: {reader["field"]}");
                    Console.WriteLine($"11. Hire Date: {reader["hire_date"]}");
                    Console.WriteLine($"12. Work Experience: {reader["work_exp"]}");

                    Console.WriteLine("\nEnter the number of the information you want to update or '0' to update all:");
                    int updateChoice;
                    if (int.TryParse(Console.ReadLine(), out updateChoice))
                    {
                        switch (updateChoice)
                        {
                            case 1:
                                UpdateField(empId, "F_Name", "First Name");
                                break;
                            case 2:
                                UpdateField(empId, "L_Name", "Last Name");
                                break;
                            case 3:
                                UpdateField(empId, "Salary", "Salary");
                                break;
                            case 4:
                                UpdateField(empId, "Position", "Position");
                                break;
                            case 5:
                                UpdateField(empId, "City", "City");
                                break;
                            case 6:
                                UpdateField(empId, "District", "District");
                                break;
                            case 7:
                                UpdateField(empId, "Street", "Street");
                                break;
                            case 8:
                                UpdateField(empId, "email", "Email");
                                break;
                            case 9:
                                UpdateField(empId, "phone", "Phone");
                                break;
                            case 10:
                                UpdateField(empId, "field", "Field");
                                break;
                            case 11:
                                UpdateField(empId, "hire_date", "Hire Date");
                                break;
                            case 12:
                                UpdateField(empId, "work_exp", "Work Experience");
                                break;
                            case 0:
                                UpdateAllFields(empId);
                                break;
                            default:
                                Console.WriteLine("Invalid choice.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                    }
                }
                else
                {
                    Console.WriteLine($"Employee with Emp_ID {empId} not found.");
                }
            }
        }
    }
}

static void UpdateField(int empId, string field, string fieldName)
{
    Console.Write($"Enter new {fieldName}: ");
    string newValue = Console.ReadLine();

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();

        using (SqlCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = $"UPDATE employee SET {field} = @Value WHERE Emp_ID = @Emp_ID";
            cmd.Parameters.AddWithValue("@Value", newValue);
            cmd.Parameters.AddWithValue("@Emp_ID", empId);
            cmd.ExecuteNonQuery();

            Console.WriteLine($"{fieldName} updated successfully.");
        }
    }
}

static void UpdateAllFields(int empId)
{
    // Collect all employee data from the user
    Console.Write("Enter new First Name: ");
    string firstName = Console.ReadLine();

    Console.Write("Enter new Last Name: ");
    string lastName = Console.ReadLine();

    Console.Write("Enter new Salary: ");
    int salary = int.Parse(Console.ReadLine());

    Console.Write("Enter new Position: ");
    string position = Console.ReadLine();

    // Collect new address data
    Console.Write("Enter new City: ");
    string city = Console.ReadLine();

    Console.Write("Enter new District: ");
    string district = Console.ReadLine();

    Console.Write("Enter new Street: ");
    string street = Console.ReadLine();

    // Collect new emp_info data
    Console.Write("Enter new Email: ");
    string email = Console.ReadLine();

    Console.Write("Enter new Phone: ");
    string phone = Console.ReadLine();

    Console.Write("Enter new Field: ");
    string field = Console.ReadLine();

    Console.Write("Enter new Hire Date (yyyy-MM-dd): ");
    DateTime hireDate = DateTime.Parse(Console.ReadLine());

    Console.Write("Enter new Work Experience: ");
    int workExp = int.Parse(Console.ReadLine());

    // Update all data in the database
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();

        using (SqlCommand cmd = connection.CreateCommand())
        {
            cmd.CommandText = "UPDATE employee " +
                             "SET F_Name = @F_Name, L_Name = @L_Name, Salary = @Salary, Position = @Position " +
                             "WHERE Emp_ID = @Emp_ID";
            cmd.Parameters.AddWithValue("@Emp_ID", empId);
            cmd.Parameters.AddWithValue("@F_Name", firstName);
            cmd.Parameters.AddWithValue("@L_Name", lastName);
            cmd.Parameters.AddWithValue("@Salary", salary);
            cmd.Parameters.AddWithValue("@Position", position);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "UPDATE Address " +
                             "SET City = @City, District = @District, Street = @Street " +
                             "WHERE ID = (SELECT serial FROM employee WHERE Emp_ID = @Emp_ID)";
            cmd.Parameters.AddWithValue("@City", city);
            cmd.Parameters.AddWithValue("@District", district);
            cmd.Parameters.AddWithValue("@Street", street);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "UPDATE Emp_info " +
                             "SET email = @email, phone = @phone, field = @field, hire_date = @hire_date, work_exp = @work_exp " +
                             "WHERE ID = (SELECT serial FROM employee WHERE Emp_ID = @Emp_ID)";
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@phone", phone);
            cmd.Parameters.AddWithValue("@field", field);
            cmd.Parameters.AddWithValue("@hire_date", hireDate);
            cmd.Parameters.AddWithValue("@work_exp", workExp);
            cmd.ExecuteNonQuery();

            Console.WriteLine("All information updated successfully.");
        }
    }
}


    static void ListEmployeesAndSalariesDescending()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT e.Emp_ID, e.F_Name, e.L_Name, e.Salary " +
                                 "FROM employee e " +
                                 "ORDER BY e.Salary DESC";

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Employees and Salaries (Descending):");
                    Console.WriteLine("{0,-10}{1,-15}{2,-15}{3,-10}", "Emp_ID", "First Name", "Last Name", "Salary");
                    while (reader.Read())
                    {
                        Console.WriteLine("{0,-10}{1,-15}{2,-15}{3,-10}",
                            reader["Emp_ID"], reader["F_Name"], reader["L_Name"], reader["Salary"]);
                    }
                }
            }
        }
    }

    static void ListEmployeesGroupedByDistrict()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT e.Emp_ID, e.F_Name, e.L_Name, a.District " +
                                 "FROM employee e " +
                                 "JOIN Address a ON e.serial = a.ID " +
                                 "ORDER BY a.District";

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Employees Grouped by District:");
                    Console.WriteLine("{0,-10}{1,-15}{2,-15}{3,-15}", "Emp_ID", "First Name", "Last Name", "District");
                    string currentDistrict = string.Empty;
                    while (reader.Read())
                    {
                        if (currentDistrict != reader["District"].ToString())
                        {
                            Console.WriteLine($"District: {reader["District"]}");
                            currentDistrict = reader["District"].ToString();
                        }
                        Console.WriteLine("{0,-10}{1,-15}{2,-15}{3,-15}",
                            reader["Emp_ID"], reader["F_Name"], reader["L_Name"], reader["District"]);
                    }
                }
            }
        }
    }
}
