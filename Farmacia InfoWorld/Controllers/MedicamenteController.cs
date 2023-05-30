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
        public IEnumerable<Medicament> Get()
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
                        medicament.Gramaj = Convert.ToInt32(dr["Gramaj"]);
                        medicament.Forma = Convert.ToString(dr["Forma"]);
                        medicament.Descriere = Convert.ToString(dr["Descriere"]);
                        ListaMedicamente.Add(medicament);
                    }

                    myReader.Close();
                    myCon.Close();
                }
            }

            return ListaMedicamente;
        }
    }
}