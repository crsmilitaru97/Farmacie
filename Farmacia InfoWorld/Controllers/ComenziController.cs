using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Farmacia_InfoWorld.Controllers
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
                        comanda.Medicamente = GetMedicamente(myCon, comanda.ID_Pacient);
                        ListaComenzi.Add(comanda);
                    }

                    myReader.Close();
                    myCon.Close();
                }
            }

            return ListaComenzi;
        }

        private static List<Medicament> GetMedicamente(SqlConnection myCon, int ID_Pacient)
        {
            List<Medicament> meds = new List<Medicament>();
            string queryMed = $@"SELECT DISTINCT ComandaMedicament.Cantitate,ComandaMedicament.ID_Medicament, * FROM Comanda
                                 INNER JOIN ComandaMedicament ON ComandaMedicament.ID_Comanda = Comanda.ID
                                 INNER JOIN Medicament ON ComandaMedicament.ID_Medicament = Medicament.ID
                                 WHERE Comanda.ID_Pacient = {ID_Pacient}";

            using (SqlCommand commandMed = new SqlCommand(queryMed, myCon))
            {
                SqlDataReader readerMed = commandMed.ExecuteReader();
                DataTable tableMed = new DataTable();
                tableMed.Load(readerMed);

                foreach (DataRow med in tableMed.Rows)
                {
                    Medicament medicament = new Medicament();
                    medicament.ID = Convert.ToInt32(med["ID_Medicament"]);
                    medicament.Denumire = Convert.ToString(med["Denumire"]);
                    medicament.Forma = Convert.ToString(med["Forma"]);
                    medicament.Cantitate = Convert.ToInt32(med["Cantitate"]);
                    medicament.Pret = Convert.ToDecimal(med["Pret"]);

                    meds.Add(medicament);
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
            foreach (var med in comanda.Medicamente)
            {
                AdaugaMedicamente(med, comanda.ID);
            }

            return new JsonResult("Updated Successfully");
        }

        private JsonResult AdaugaMedicamente(Medicament medicament, int ID_Comanda)
        {
            string query = @"INSERT INTO ComandaMedicament (ID_Comanda, ID_Medicament, Cantitate) 
                             VALUES (@ID_Comanda, @ID_Medicament, @Cantitate)";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@ID_Comanda", ID_Comanda);
                myCommand.Parameters.AddWithValue("@ID_Medicament", medicament.ID);
                myCommand.Parameters.AddWithValue("@Cantitate", medicament.Cantitate);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }
            return new JsonResult("Updated Successfully");
        }


        [HttpPost("/comanda/modifica")]
        public JsonResult ModificaComanda([FromBody] Comanda comanda)
        {
            string query = @"UPDATE Comanda SET Status = @Status, Data = @Data, Pret = @ID_Pacient, Pret=@ID_Pacient
                             WHERE ID = @ID";

            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            using (var myCon = new SqlConnection(sqlDataSource))
            {
                using var myCommand = new SqlCommand(query, myCon);
                myCommand.Parameters.AddWithValue("@Status", comanda.Status);
                myCommand.Parameters.AddWithValue("@Data", comanda.Data);
                myCommand.Parameters.AddWithValue("@Pret", comanda.Pret);
                myCommand.Parameters.AddWithValue("@ID_Pacient", comanda.ID_Pacient);
                myCommand.Parameters.AddWithValue("@ID", comanda.ID);

                myCon.Open();
                myCommand.ExecuteNonQuery();
                myCon.Close();
            }

            return new JsonResult("Updated Successfully");
        }


        [HttpPost("/comanda/aproba")]
        public ActionResult AprobaComanda([FromBody] Comanda comanda)
        {
            string sqlDataSource = _configuration.GetConnectionString("farmacieConnectionString");

            foreach (var med in comanda.Medicamente)
            {
                List<Lot> loturi = GetLoturi(med.ID);
                if (loturi.Count == 0)
                {
                    return new JsonResult(new { status = "error", message = "Nu exista stoc pentru acest medicament!" });
                }
                int i = 0;

                while (med.Cantitate > 0)
                {
                    if (i > loturi.Count)
                    {
                        return new JsonResult(new { status = "error", message = "Nu exista stoc destul pentru acest medicament!" });
                    }
                    var lot = loturi[i];

                    if (lot.Cantitate <= med.Cantitate)
                    {
                        med.Cantitate -= lot.Cantitate;
                        StergeLot(lot.ID);
                    }
                    else
                    {
                        lot.Cantitate -= med.Cantitate;
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


            return new JsonResult("Deleted Successfully");
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

            return new JsonResult("Updated Successfully");
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

            return new JsonResult("Deleted Successfully");
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

            return new JsonResult("Deleted Successfully");
        }
    }
}