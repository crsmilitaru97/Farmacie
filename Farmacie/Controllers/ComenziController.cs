using Farmacie.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Farmacie.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class ComenziController : ControllerBase
    {
        private readonly ILogger<ComenziController> _logger;
        private readonly IConfiguration _configuration;
        enum Status { Neaprobată, Aprobata };

        public ComenziController(ILogger<ComenziController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("/listacomenzi")]
        public IEnumerable<Comanda> GetComenzi()
        {
            List<Comanda> ListaComenzi = new List<Comanda>();
            string query = @"SELECT Nume + ' ' + Prenume AS NumePacient, * FROM Comanda 
                             INNER JOIN Pacient ON Pacient.ID = Comanda.ID_Pacient";
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
                        Comanda comanda = new Comanda();
                        comanda.ID = Convert.ToInt32(dr["ID"]);
                        comanda.NumePacient = Convert.ToString(dr["NumePacient"]);
                        comanda.Status = Convert.ToInt32(dr["Status"]);
                        comanda.Data = Convert.ToDateTime(dr["Data"]);
                        comanda.Pret = Convert.ToDecimal(dr["Pret"]);
                        comanda.ID_Pacient = Convert.ToInt32(dr["ID_Pacient"]);
                        comanda.ComandaMedicamente = GetMedicamente(myCon, comanda.ID);
                        ListaComenzi.Add(comanda);
                    }

                    myReader.Close();
                    myCon.Close();
                }
            }

            return ListaComenzi;
        }

        private static List<ComandaMedicament> GetMedicamente(SqlConnection myCon, int id_comanda)
        {
            List<ComandaMedicament> meds = new List<ComandaMedicament>();
            string queryMed = $@"SELECT DISTINCT ComandaMedicament.Cantitate,ComandaMedicament.ID_Medicament, * FROM Comanda
                                 INNER JOIN ComandaMedicament ON ComandaMedicament.ID_Comanda = Comanda.ID
                                 INNER JOIN Medicament ON ComandaMedicament.ID_Medicament = Medicament.ID
                                 WHERE Comanda.ID = {id_comanda}";

            using (SqlCommand commandMed = new SqlCommand(queryMed, myCon))
            {
                SqlDataReader readerMed = commandMed.ExecuteReader();
                DataTable tableMed = new DataTable();
                tableMed.Load(readerMed);

                foreach (DataRow med in tableMed.Rows)
                {
                    ComandaMedicament comandaMedicament = new ComandaMedicament();
                    comandaMedicament.ID = Convert.ToInt32(med["ID"]);
                    comandaMedicament.Cantitate = Convert.ToInt32(med["Cantitate"]);
                    comandaMedicament.ID_Medicament = Convert.ToInt32(med["ID_Medicament"]);
                    comandaMedicament.ID_Comanda = Convert.ToInt32(med["ID_Comanda"]);

                    comandaMedicament._medicament = new Medicament();
                    comandaMedicament._medicament.Denumire = Convert.ToString(med["Denumire"]);
                    comandaMedicament._medicament.Forma = Convert.ToString(med["Forma"]);
                    comandaMedicament._medicament.Pret = Convert.ToDecimal(med["Pret"]);

                    meds.Add(comandaMedicament);
                }
            }
            return meds;
        }


        [HttpPost("/comanda/adauga")]
        public JsonResult AdaugaComanda([FromBody] Comanda comanda)
        {
            string query = @"INSERT INTO Comanda (Status, Data, Pret, ID_Pacient)   
                             OUTPUT inserted.ID
                             VALUES (@Status, @Data, @Pret, @ID_Pacient)";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@Status", comanda.Status);
                myCommand.Parameters.AddWithValue("@Data", comanda.Data);
                myCommand.Parameters.AddWithValue("@Pret", comanda.Pret);
                myCommand.Parameters.AddWithValue("@ID_Pacient", comanda.ID_Pacient);

                myCon.Open();
                comanda.ID = Convert.ToInt32(myCommand.ExecuteScalar());
                myCon.Close();
            }
            foreach (var med in comanda.ComandaMedicamente)
            {
                AdaugaMedicamente(med, comanda.ID);
            }

            return new JsonResult("Comanda adaugata cu succes");
        }

        private JsonResult AdaugaMedicamente(ComandaMedicament comandaMedicament, int ID_Comanda)
        {
            string query = @"INSERT INTO ComandaMedicament (ID_Comanda, ID_Medicament, Cantitate) 
                             VALUES (@ID_Comanda, @ID_Medicament, @Cantitate)";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@ID_Comanda", ID_Comanda);
                myCommand.Parameters.AddWithValue("@ID_Medicament", comandaMedicament.ID_Medicament);
                myCommand.Parameters.AddWithValue("@Cantitate", comandaMedicament.Cantitate);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }
            return new JsonResult("Relatie comanda-medicamente adaugata cu succes");
        }


        [HttpPost("/comanda/modifica")]
        public JsonResult ModificaComanda([FromBody] Comanda comanda)
        {
            foreach (var comandaMedicament in comanda.ComandaMedicamente)
            {
                if (comandaMedicament._status == ComandaMedicament.Status.nemodificat)
                    continue;

                string medComQuery = string.Empty;

                if (comandaMedicament._status == ComandaMedicament.Status.nou)
                {
                    medComQuery = @"INSERT INTO ComandaMedicament (ID_Comanda, ID_Medicament, Cantitate)
                                    VALUES (@ID_Comanda, @ID_Medicament, @Cantitate)";
                }
                else if (comandaMedicament._status == ComandaMedicament.Status.modificat)
                {
                    medComQuery = $@"UPDATE ComandaMedicament SET Cantitate=@Cantitate
                                     WHERE ID = {comandaMedicament.ID}";
                }

                if (!string.IsNullOrEmpty(medComQuery))
                {
                    AdaugaSauModificaRelatieComandaMedicamente(medComQuery, comandaMedicament, comanda.ID);
                }
            }

            string query = @"UPDATE Comanda SET Pret=@Pret
                             WHERE ID = @ID";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@Pret", comanda.Pret);
                myCommand.Parameters.AddWithValue("@ID", comanda.ID);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }

            return new JsonResult("Comanda modificata cu succes");

        }

        private JsonResult AdaugaSauModificaRelatieComandaMedicamente(string query, ComandaMedicament comandaMedicament, int id_comanda)
        {
            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@ID_Comanda", id_comanda);
                myCommand.Parameters.AddWithValue("@ID_Medicament", comandaMedicament.ID_Medicament);
                myCommand.Parameters.AddWithValue("@Cantitate", comandaMedicament.Cantitate);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }

            return new JsonResult("Relatie comanda-medicamente adaugata cu succes");
        }


        [HttpPost("/comanda/aproba")]
        public ActionResult AprobaComanda([FromBody] Comanda comanda)
        {
            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            foreach (var comMed in comanda.ComandaMedicamente)
            {
                List<Lot> loturi = GetLoturi(comMed.ID_Medicament);
                if (loturi.Count == 0)
                {
                    return new JsonResult(new { status = "error", message = "Nu exista stoc pentru acest medicament!" });
                }
                int i = 0;

                while (comMed.Cantitate > 0)
                {
                    if (comMed.Cantitate > loturi.Sum(lot => lot.Cantitate))
                    {
                        return new JsonResult(new { status = "error", message = "Nu exista stoc suficient pentru acest medicament!" });
                    }
                    var lot = loturi[i];

                    if (lot.Cantitate <= comMed.Cantitate)
                    {
                        comMed.Cantitate -= lot.Cantitate;
                        StergeLot(lot.ID);
                    }
                    else
                    {
                        lot.Cantitate -= comMed.Cantitate;
                        ModificaLot(lot.ID, lot.Cantitate);
                        break;
                    }

                    i++;
                }
            }

            string query = $@"UPDATE Comanda SET Status = 1
                             WHERE ID = {comanda.ID}";

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }

            return new JsonResult("Comanda a fost aprobata cu succes!");
        }

        #region Loturi
        public JsonResult StergeLot(int id_lot)
        {
            string query = @"DELETE FROM Lot
                             WHERE ID = @ID";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@ID", id_lot);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }


            return new JsonResult("Lot sters");
        }

        public JsonResult ModificaLot(int id_lot, int cantitateNoua)
        {
            string query = @"UPDATE Lot SET Cantitate = @Cantitate
                             WHERE ID = @ID";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@Cantitate", cantitateNoua);
                myCommand.Parameters.AddWithValue("@ID", id_lot);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }

            return new JsonResult("Lot modificat");
        }

        public List<Lot> GetLoturi(int id_medicament)
        {
            List<Lot> loturi = new List<Lot>();
            string queryMed = $@"SELECT ID, Cantitate FROM Lot
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
                        lot.Cantitate = Convert.ToInt32(row["Cantitate"]);
                        loturi.Add(lot);
                    }
                    readerMed.Close();
                    myCon.Close();
                }
            }
            return loturi;
        }
        #endregion

        [HttpPost("/comanda/sterge")]
        public JsonResult StergeComanda([FromBody] Comanda comanda)
        {
            StergeRelatieComandaMedicamente(comanda.ID);

            string query = @"DELETE FROM Comanda
                             WHERE ID = @ID";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@ID", comanda.ID);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }

            return new JsonResult("Comanda stearsa");
        }

        private JsonResult StergeRelatieComandaMedicamente(int id_comanda)
        {
            string query = @"DELETE FROM ComandaMedicament
                             WHERE ID_Comanda = @ID_Comanda";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@ID_Comanda", id_comanda);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }

            return new JsonResult("Relatie comanda-medicamente stearsa cu succes");
        }
    }
}