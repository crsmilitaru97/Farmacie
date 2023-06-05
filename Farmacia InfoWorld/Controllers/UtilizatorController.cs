using Farmacia_InfoWorld.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace Farmacia_InfoWorld.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class UtilizatorController : ControllerBase
    {
        private readonly ILogger<ComenziController> _logger;
        private readonly IConfiguration _configuration;
        enum Status { Neaprobată, Aprobata };

        public UtilizatorController(ILogger<ComenziController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("/inregistrare")]
        public int Inregistrare([FromBody] Utilizator utilizator)
        {
            string query = @"INSERT INTO Utilizator (Email, Parola, Tip)  
                             OUTPUT inserted.ID
                             VALUES (@Email, @Parola, @Tip)";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@Email", utilizator.Email);
                myCommand.Parameters.AddWithValue("@Parola", HashPassword(utilizator.Parola));
                myCommand.Parameters.AddWithValue("@Tip", utilizator.Tip);

                myCon.Open();
                utilizator.ID = Convert.ToInt32(myCommand.ExecuteScalar());
                myCon.Close();
            }

            return utilizator.ID;
        }

        [HttpGet("/conectare")]
        public Utilizator Conectare([FromQuery] string email, [FromQuery] string parola)
        {
            string query = @"SELECT * FROM Utilizator
                             WHERE Email = @Email";
            string connectionString = _configuration.GetConnectionString("farmacieConnectionString");
            Utilizator utilizator = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);

                using SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (Convert.ToString(reader["Parola"]) == HashPassword(parola))
                    {
                        utilizator = new Utilizator();
                        utilizator.Parola = Convert.ToString(reader["Parola"]);
                        utilizator.ID = Convert.ToInt32(reader["ID"]);
                        utilizator.Email = Convert.ToString(reader["Email"]);
                        utilizator.Tip = Convert.ToString(reader["Tip"]);
                        utilizator.ID_Pacient = Convert.ToInt32(reader["ID_Pacient"]);
                    }
                }
            }

            return utilizator;
        }

        [HttpPost("/utilizator/modifica")]
        public JsonResult ModificaUtilizator([FromBody] Utilizator utilizator)
        {
            string query = @"UPDATE Utilizator SET Email = @Email, Parola = @Parola, Tip = @Tip, ID_Pacient=@ID_Pacient
                             WHERE ID = @ID";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@ID", utilizator.ID);
                myCommand.Parameters.AddWithValue("@Email", utilizator.Email);
                myCommand.Parameters.AddWithValue("@Parola", HashPassword(utilizator.Parola));
                myCommand.Parameters.AddWithValue("@Tip", utilizator.Tip);
                myCommand.Parameters.AddWithValue("@ID_Pacient", utilizator.ID_Pacient);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }

            return new JsonResult("Utilizator modificat");
        }

        public string HashPassword(string parola)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(parola);

                byte[] hashedPasswordBytes = sha256.ComputeHash(passwordBytes);

                string hashedPassword = BitConverter.ToString(hashedPasswordBytes).Replace("-", string.Empty);

                return hashedPassword;
            }
        }
    }
}