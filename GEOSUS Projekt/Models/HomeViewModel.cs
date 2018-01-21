using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using System.Data;
using System.Text;
using System.Web.Mvc;

namespace GEOSUS_Projekt.Models
{
    public class Services
    {
        public List<SelectListItem> DohvatiListuGradova()
        {
            System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/MyWebSiteRoot");
            List<Grad> gradovi = new List<Grad>();

            // Connect to a PostgreSQL database
            using (NpgsqlConnection conn = new NpgsqlConnection(rootWebConfig.ConnectionStrings.ConnectionStrings["GeoSusConnectionString"].ToString()))
            {
                conn.Open();

                // Start a transaction as it is required to work with result sets (cursors) in PostgreSQL
                using (NpgsqlTransaction tran = conn.BeginTransaction())
                {

                    // Vraca kursor
                    NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM gradovi", conn);
                    command.CommandType = CommandType.Text;

                    NpgsqlDataReader dr = command.ExecuteReader();
                    // Output rows 
                    while (dr.Read())
                    {
                        Grad result = new Models.Grad(dr[1].ToString(), Int32.Parse(dr[0].ToString()));
                        gradovi.Add(result);
                    }
                    dr.Close();

                    tran.Commit();
                }

                conn.Close();
            }

            var rezultat = new List<SelectListItem>();
            foreach(var grad in gradovi)
            {
                rezultat.Add(new SelectListItem { Value = grad.gradId.ToString(), Text = grad.Naziv });
            }
            return rezultat;
        }

        public List<Odasiljac> VratiOdasiljace(int gradId)
        {
            System.Configuration.Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/MyWebSiteRoot");
            List<Odasiljac> rezultat = new List<Odasiljac>();

            // Connect to a PostgreSQL database
            using (NpgsqlConnection conn = new NpgsqlConnection(rootWebConfig.ConnectionStrings.ConnectionStrings["GeoSusConnectionString"].ToString()))
            {
                conn.Open();

                // Start a transaction as it is required to work with result sets (cursors) in PostgreSQL
                using (NpgsqlTransaction tran = conn.BeginTransaction())
                {

                    // Vraca kursor
                    NpgsqlCommand command = new NpgsqlCommand("nadjiOdasiljace", conn);
                    command.CommandType = CommandType.StoredProcedure;
                    //command.Parameters.AddWithValue(gradId);

                    NpgsqlParameter q = new NpgsqlParameter();
                    //q.ParameterName = "@gradId";
                    q.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer;
                    q.Direction = ParameterDirection.Input;
                    q.Value = gradId;
                    command.Parameters.Add(q);

                    NpgsqlParameter p = new NpgsqlParameter();
                    //p.ParameterName = "@ref";
                    p.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Refcursor;
                    p.Direction = ParameterDirection.InputOutput;
                    p.Value = "ref";
                    command.Parameters.Add(p);
                    command.ExecuteNonQuery();

                    // Dohvaca sve iz kursora
                    command.CommandText = "fetch all in \"ref\"";
                    command.CommandType = CommandType.Text;

                    NpgsqlDataReader dr = command.ExecuteReader();
                    // Output rows 
                    while (dr.Read())
                    {
                        Odasiljac result = new Models.Odasiljac(dr[0].ToString(), double.Parse(dr[1].ToString()), double.Parse(dr[2].ToString()), dr[3].ToString(), double.Parse(dr[4].ToString()), double.Parse(dr[5].ToString()), Int32.Parse(dr[6].ToString()), double.Parse(dr[7].ToString()), double.Parse(dr[8].ToString()), Int32.Parse(dr[9].ToString()), Int32.Parse(dr[10].ToString()));
                        rezultat.Add(result);
                    }
                    dr.Close();

                    tran.Commit();
                }

                conn.Close();
            }

            return rezultat;
        }
    }

    public class Odasiljac
    {
        public Odasiljac(string gradNaziv, double gradXpos, double gradYpos, string odasiljacNaziv, double odasiljacXpos, double odasiljacYpos, int frekvencija, double udaljenost, double snaga, int izvornaSnaga, int radijus)
        {
            grad = gradNaziv;
            gradX = gradXpos;
            gradY = gradYpos;
            odasiljac = odasiljacNaziv;
            odasiljacX = odasiljacXpos;
            odasiljacY = odasiljacYpos;
            this.frekvencija = frekvencija;
            this.udaljenost = udaljenost;
            this.snaga = snaga;
            this.izvornaSnaga = izvornaSnaga;
            this.radijus = radijus;
        }
        public string grad { get; set; }
        public double gradX { get; set; }
        public double gradY { get; set; }

        public string odasiljac { get; set; }
        public double odasiljacX { get; set; }
        public double odasiljacY { get; set; }

        public int frekvencija { get; set; }
        public double udaljenost { get; set; }
        public double snaga { get; set; }
        public int izvornaSnaga { get; set; }
        public int radijus { get; set; }
    }

    public class Grad
    {
        public Grad(string naziv, int idgrad)
        {
            Naziv = naziv;
            gradId = idgrad;
        }
        public string Naziv { get; set; }
        public int gradId { get; set; }
    }

    public class HomeViewModel
    {
        public IEnumerable<SelectListItem> gradovi { get; set; }
        public List<Odasiljac> odasiljaci { get; set; }
    }
}