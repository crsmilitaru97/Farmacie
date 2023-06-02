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
        public IEnumerable<Pacient> GetPacient()
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
                    pacient.Adrese = GetAdrese(myCon, pacient.ID);

                    ListaPacienti.Add(pacient);
                }

                myReader.Close();
                myCon.Close();
            }

            return ListaPacienti;
        }

        private static List<Adresa> GetAdrese(SqlConnection myCon, int ID_Pacient)
        {
            List<Adresa> adrese = new List<Adresa>();
            string queryMed = $@"SELECT * FROM Adresa
                                 WHERE Adresa.ID_Pacient = {ID_Pacient}";

            using (SqlCommand commandMed = new SqlCommand(queryMed, myCon))
            {
                SqlDataReader readerMed = commandMed.ExecuteReader();
                DataTable tableMed = new DataTable();
                tableMed.Load(readerMed);

                foreach (DataRow med in tableMed.Rows)
                {
                    Adresa adresa = new Adresa(); 
                    adresa.ID = Convert.ToInt32(med["ID"]);
                    adresa.Tip_Adresa = Convert.ToInt32(med["Tip_Adresa"]);
                    adresa.Linie_Adresa = Convert.ToString(med["Linie_Adresa"]);
                    adresa.Localitate = Convert.ToString(med["Localitate"]);
                    adresa.Judet = Convert.ToString(med["Judet"]);
                    adresa.Cod_Postal = Convert.ToString(med["Cod_Postal"]);
                    adrese.Add(adresa);
                }
            }
            return adrese;
        }


        [HttpPost("/pacient/adauga")]
        public JsonResult AdaugaPacient([FromBody] Pacient pacient)
        {
            string query = @"INSERT INTO Pacient (Nume, Prenume, Data_Nastere, CNP, Telefon, Email) 
                             OUTPUT inserted.ID
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
                pacient.ID = Convert.ToInt32(myCommand.ExecuteScalar());
                myCon.Close();
            }
            foreach (var adr in pacient.Adrese)
            {
                AdaugaAdresa(adr, pacient.ID);
            }

            return new JsonResult("Updated Successfully");
        }

        private JsonResult AdaugaAdresa(Adresa adresa, int ID_Pacient)
        {
            string query = @"INSERT INTO Adresa (Tip_Adresa, Linie_Adresa, Localitate, Judet, Cod_Postal, ID_Pacient) 
                             VALUES (@Tip_Adresa, @Linie_Adresa, @Localitate, @Judet, @Cod_Postal, @ID_Pacient)";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@Tip_Adresa", adresa.Tip_Adresa);
                myCommand.Parameters.AddWithValue("@Linie_Adresa", adresa.Linie_Adresa);
                myCommand.Parameters.AddWithValue("@Localitate", adresa.Localitate);
                myCommand.Parameters.AddWithValue("@Judet", adresa.Judet);
                myCommand.Parameters.AddWithValue("@Cod_Postal", adresa.Cod_Postal);
                myCommand.Parameters.AddWithValue("@ID_Pacient", ID_Pacient);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }
            return new JsonResult("Updated Successfully");
        }


        [HttpPost("/pacient/modifica")]
        public JsonResult ModificaPacient([FromBody] Pacient pacient)
        {
            string query = @"UPDATE Pacient SET Nume = @Nume, Prenume = @Prenume, Data_Nastere = @Data_Nastere, CNP=@CNP, Telefon = @Telefon, Email = @Email 
                             WHERE ID = @ID";

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
                myCommand.Parameters.AddWithValue("@ID", pacient.ID);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }
            foreach (var adr in pacient.Adrese)
            {
                if (adr.ID != 0)
                    ModificaAdresa(adr);
                else
                    AdaugaAdresa(adr, pacient.ID);
            }

            return new JsonResult("Updated Successfully");
        }

        private JsonResult ModificaAdresa(Adresa adresa)
        {
            string query = @"UPDATE Adresa SET Tip_Adresa=@Tip_Adresa, Linie_Adresa=@Linie_Adresa, Localitate=@Localitate, Judet=@Judet, Cod_Postal=@Cod_Postal 
                             WHERE ID=@ID";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@Tip_Adresa", adresa.Tip_Adresa);
                myCommand.Parameters.AddWithValue("@Linie_Adresa", adresa.Linie_Adresa);
                myCommand.Parameters.AddWithValue("@Localitate", adresa.Localitate);
                myCommand.Parameters.AddWithValue("@Judet", adresa.Judet);
                myCommand.Parameters.AddWithValue("@Cod_Postal", adresa.Cod_Postal);
                myCommand.Parameters.AddWithValue("@ID", adresa.ID);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }
            return new JsonResult("Updated Successfully");
        }


        [HttpPost("/pacient/sterge")]
        public JsonResult StergePacient([FromBody] Pacient pacient)
        {     
            foreach (var adr in pacient.Adrese)
            {
                StergeAdresa(adr);
            }
            string query = @"DELETE FROM Pacient
                             WHERE ID = @ID";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@ID", pacient.ID);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }


            return new JsonResult("Deleted Successfully");
        }

        private JsonResult StergeAdresa(Adresa adresa)
        {
            string query = @"DELETE FROM Adresa
                             WHERE ID = @ID";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@ID", adresa.ID);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }

            return new JsonResult("Deleted Successfully");
        }
    }
}