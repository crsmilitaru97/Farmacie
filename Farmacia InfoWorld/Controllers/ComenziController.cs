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
        public IEnumerable<Comanda> Get()
        {
            List<Comanda> ListaComenzi = new List<Comanda>();
            string query = @"SELECT Nume + ' ' + Prenume AS NumePacient, * FROM Comanda 
                             INNER JOIN Pacient ON Pacient.ID = Comanda.ID_Pacient";
                             //INNER JOIN ComandaMedicament ON ComandaMedicament.ID_Comanda = Comanda.ID";
            //INNER JOIN Medicament ON ComandaMedicament.ID_Medicament = Medicament.ID
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

                        string queryMed = $@"SELECT DISTINCT ComandaMedicament.Cantitate, * FROM Comanda
                                             INNER JOIN ComandaMedicament ON ComandaMedicament.ID_Comanda = Comanda.ID
                                             INNER JOIN Medicament ON ComandaMedicament.ID_Medicament = Medicament.ID
                                             WHERE Comanda.ID_Pacient = {Convert.ToString(dr["ID_Pacient"])}";


                        using (SqlCommand commandMed = new SqlCommand(queryMed, myCon))
                        {
                            SqlDataReader readerMed = commandMed.ExecuteReader();
                            DataTable tableMed = new DataTable();
                            tableMed.Load(readerMed);

                            foreach (DataRow med in tableMed.Rows)
                            {
                                Medicament medicament = new Medicament();
                                medicament.Denumire = Convert.ToString(med["Denumire"]);
                                medicament.Cantitate = Convert.ToInt32(med["Cantitate"]);

                                comanda.Medicamente.Add(medicament);
                            }
                        }
                        comanda.Status = Convert.ToString(dr["Status"]);
                        ListaComenzi.Add(comanda);
                    }

                    myReader.Close();
                    myCon.Close();
                }
            }

            return ListaComenzi;
        }
    }
}