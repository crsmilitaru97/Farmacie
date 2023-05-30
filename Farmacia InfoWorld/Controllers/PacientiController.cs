using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Farmacia_InfoWorld.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class PacientiController : ControllerBase
    {
        private readonly ILogger<PacientiController> _logger;
        private readonly IConfiguration _configuration;

        public PacientiController(ILogger<PacientiController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("/listapacienti")]
        public IEnumerable<Pacient> Get()
        {
            List<Pacient> ListaPacienti = new List<Pacient>();
            string query = @"SELECT * FROM Pacient";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");
            SqlDataReader myReader;
            using (var myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using var myCommand = new SqlCommand(query, myCon);
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                foreach (DataRow dr in table.Rows)
                {
                    Pacient pacient = new Pacient();
                    pacient.ID = Convert.ToInt32(dr["ID"]);
                    pacient.Nume = Convert.ToString(dr["Nume"]);
                    pacient.Prenume = Convert.ToString(dr["Prenume"]);
                    pacient.CNP = Convert.ToString(dr["CNP"]);
                    pacient.Data_Nastere = Convert.ToDateTime(dr["Data_Nastere"]);
                    pacient.Telefon = Convert.ToString(dr["Telefon"]);
                    pacient.Email = Convert.ToString(dr["Email"]);
                    ListaPacienti.Add(pacient);
                }

                myReader.Close();
                myCon.Close();
            }

            return ListaPacienti;
        }

        [HttpPost("/adaugapacient")]
        public JsonResult AdaugaPacient([FromBody] Pacient pacient)
        {
            string query = @"INSERT INTO Pacient (Nume, Prenume, Data_Nastere, CNP, Telefon, Email) 
                     VALUES (@Nume, @Prenume, @Data_Nastere, @CNP, @Telefon, @Email)";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@Nume", pacient.Nume);
                myCommand.Parameters.AddWithValue("@Prenume", pacient.Prenume);
                myCommand.Parameters.AddWithValue("@CNP", pacient.CNP);
                myCommand.Parameters.AddWithValue("@Data_Nastere", pacient.Data_Nastere);
                myCommand.Parameters.AddWithValue("@Telefon", pacient.Telefon);
                myCommand.Parameters.AddWithValue("@Email", pacient.Email);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }
            return new JsonResult("Updated Successfully");
        }
    }
}