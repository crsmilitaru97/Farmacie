using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Farmacia_InfoWorld.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class MedicamenteController : ControllerBase
    {
        private readonly ILogger<MedicamenteController> _logger;
        private readonly IConfiguration _configuration;

        public MedicamenteController(ILogger<MedicamenteController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("/listamedicamente")]
        public IEnumerable<Medicament> GetMedicament()
        {
            List<Medicament> ListaMedicamente = new List<Medicament>();
            string query = @"SELECT * FROM Medicament";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    foreach (DataRow dr in table.Rows)
                    {
                        Medicament medicament = new Medicament();
                        medicament.ID = Convert.ToInt32(dr["ID"]);
                        medicament.Denumire = Convert.ToString(dr["Denumire"]);
                        medicament.Forma = Convert.ToString(dr["Forma"]);
                        medicament.Descriere = Convert.ToString(dr["Descriere"]);
                        medicament.Pret = Convert.ToInt32(dr["Pret"]);
                        ListaMedicamente.Add(medicament);
                    }

                    myReader.Close();
                    myCon.Close();
                }
            }

            return ListaMedicamente;
        }

        [HttpGet("/medicament/loturi")]
        public IEnumerable<Lot> GetLoturi([FromQuery] decimal id_medicament)
        {
            List<Lot> loturi = new List<Lot>();
            string queryMed = $@"SELECT * FROM Lot
                                 WHERE Lot.ID_Medicament = {id_medicament}"; 
            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand commandMed = new SqlCommand(queryMed, myCon))
                {
                    SqlDataReader readerMed = commandMed.ExecuteReader();
                    DataTable tableMed = new DataTable();
                    tableMed.Load(readerMed);

                    foreach (DataRow row in tableMed.Rows)
                    {
                        Lot lot = new Lot();
                        lot.ID = Convert.ToInt32(row["ID"]);
                        lot.Data_Expirare = Convert.ToDateTime(row["Data_Expirare"]);
                        lot.Cantitate = Convert.ToInt32(row["Cantitate"]);
                        lot.ID_Medicament = Convert.ToInt32(row["ID_Medicament"]);
                        loturi.Add(lot);
                    }
                    readerMed.Close();
                    myCon.Close();
                }
            }
            return loturi;
        }

        [HttpPost("/medicament/adauga")]
        public JsonResult AdaugaMedicament([FromBody] Medicament medicament)
        {
            string query = @"INSERT INTO Medicament (Denumire, Pret, Forma, Descriere) 
                             VALUES (@Denumire, @Pret, @Forma, @Descriere)";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@Denumire", medicament.Denumire);
                myCommand.Parameters.AddWithValue("@Forma", medicament.Forma);
                myCommand.Parameters.AddWithValue("@Descriere", medicament.Descriere);
                myCommand.Parameters.AddWithValue("@Pret", medicament.Pret);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }
            return new JsonResult("Updated Successfully");
        }


        [HttpPost("/medicament/modifica")]
        public JsonResult ModificaPacient([FromBody] Medicament medicament)
        {
            string query = @"UPDATE Medicament SET Denumire = @Denumire, Forma = @Forma, Descriere = @Descriere, Pret=@Pret
                             WHERE ID = @ID";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@Denumire", medicament.Denumire);
                myCommand.Parameters.AddWithValue("@Forma", medicament.Forma);
                myCommand.Parameters.AddWithValue("@Descriere", medicament.Descriere);
                myCommand.Parameters.AddWithValue("@Pret", medicament.Pret);
                myCommand.Parameters.AddWithValue("@ID", medicament.ID);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }

            return new JsonResult("Updated Successfully");
        }


        [HttpPost("/medicament/sterge")]
        public JsonResult StergePacient([FromBody] Medicament medicament)
        {
            string query = @"DELETE FROM Medicament
                             WHERE ID = @ID";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@ID", medicament.ID);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }


            return new JsonResult("Deleted Successfully");
        }
    }
}