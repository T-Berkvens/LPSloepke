using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace LPSloepke
{
    public static class Database
    {
        private static string _connectionstring = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=fhictora01.fhict.local)(PORT=1521)))
                                                   (CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=fhictora)));User ID=dbi336483;PASSWORD=ytcazk;";
        /// <summary>
        /// Slaat een contract op in de database
        /// </summary>
        public static bool InsertContract(Huurcontract contract)
        {
            string query = @"InsertContract";
            int contractID = 0;
            using (OracleConnection connection = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = connection;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    cmd.Parameters.Add(new OracleParameter("p_email", contract.Email));
                    cmd.Parameters.Add(new OracleParameter("p_naam", contract.Naam));
                    cmd.Parameters.Add(new OracleParameter("p_begindate", contract.Begin));
                    cmd.Parameters.Add(new OracleParameter("p_einddate", contract.Einde));
                    cmd.Parameters.Add(new OracleParameter("return_value", OracleDbType.Int32, System.Data.ParameterDirection.ReturnValue));
                    try
                    {
                        connection.Open();
                        OracleDataReader reader = cmd.ExecuteReader();
                        contractID = Convert.ToInt32(cmd.Parameters["return_value"].Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }

            foreach (Boot boot in contract.Artikelen.Where(x => x is Boot))
            {
                query = @"INSERT INTO HUURBOOT (HuurContract_ID, Boot_Naam)
                        VALUES (:p_contractID, :p_boot_naam)";
                using (OracleConnection connection = new OracleConnection(_connectionstring))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.CommandText = query;
                        cmd.Connection = connection;
                        cmd.Parameters.Add(new OracleParameter(":p_contractID", contractID));
                        cmd.Parameters.Add(new OracleParameter(":p_boot_naam", boot.Naam));
                        try
                        {
                            connection.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    }
                }
            }

            foreach (Accessoire accessoire in contract.Artikelen.Where(x => x is Accessoire))
            {
                query = @"INSERT INTO HuurAccessoire (HuurContract_ID, Accessoire_Naam)
                        VALUES (:p_contractID, :p_accessoire_naam)";
                for (int i = 0; i < accessoire.Aantal; i++)
                {
                    using (OracleConnection connection = new OracleConnection(_connectionstring))
                    {
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            cmd.CommandText = query;
                            cmd.Connection = connection;
                            cmd.Parameters.Add(new OracleParameter(":p_contractID", contractID));
                            cmd.Parameters.Add(new OracleParameter(":p_accessoire_naam", accessoire.Naam));
                            try
                            {
                                connection.Open();
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Haalt alle beschikbare boten op uit de database
        /// </summary>
        /// <returns>Lijst met beschikbare boten</returns>
        public static List<Boot> LaadBoten()
        {
            List<Boot> boten = new List<Boot>();
            string query = @"SELECT b1.Naam, bt1.Naam, bt1.Prijs, bt1.Motor, bt1.TankInhoud
                            FROM Boot b1, BootType bt1
                            WHERE b1.BootType_Naam = bt1.Naam
                            AND b1.Naam NOT IN (SELECT b.Naam
                            FROM Boot b, BootType bt, HuurBoot hb, HuurContract hc
                            WHERE b.BootType_Naam = bt.Naam
                            AND hb.Boot_Naam = b.Naam
                            AND hb.HuurContract_ID = hc.ID
                            AND sysdate BETWEEN hc.BeginDate AND hc.EindDate)";

            using (OracleConnection connection = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = connection;
                    try
                    {
                        connection.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Boot boot = new Boot(reader.GetValue(0).ToString(), reader.GetDouble(2), (BootType)Enum.Parse(typeof(BootType), reader.GetValue(1).ToString()),
                                    Convert.ToBoolean(reader.GetInt32(3)), (reader.GetValue(4) != DBNull.Value ? Convert.ToDouble(reader.GetInt32(4)) : 0));
                                boten.Add(boot);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return boten;
                    }
                }
            }
            return boten;
        }

        /// <summary>
        /// Haalt alle beschikbare accessoires op uit de database
        /// </summary>
        /// <returns>Lijst met beschikbare accessoires</returns>
        public static List<Accessoire> LaadAccessoires()
        {
            List<Accessoire> accessoires = new List<Accessoire>();
            string query = @"SELECT a1.Naam, a1.Prijs, a1.Aantal - (SELECT COUNT(a.Naam) 
                            FROM Accessoire a, HuurAccessoire ha, HuurContract hc
                            WHERE a.Naam = ha.Accessoire_Naam
                            AND a.Naam = a1.Naam
                            AND ha.HuurContract_ID = hc.ID
                            AND sysdate BETWEEN hc.BeginDate AND EindDate) AS Aantal
                            FROM accessoire a1";

            using (OracleConnection connection = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = connection;
                    try
                    {
                        connection.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Accessoire accessoire = new Accessoire(reader.GetValue(0).ToString(), reader.GetDouble(1), Convert.ToInt32(reader.GetValue(2)));
                                accessoires.Add(accessoire);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return accessoires;
                    }
                }
            }
            return accessoires;
        }

        /// <summary>
        /// Haalt alle huurcontracten op uit de database
        /// </summary>
        /// <returns>Lijst met alle huurcontracten</returns>
        public static List<Huurcontract> LaadContracten()
        {
            List <Huurcontract> lst = new List<Huurcontract>();
            string query = @"SELECT hc.ID, p.Naam, p.Email, hc.BeginDate, hc.EindDate
                            FROM HuurContract hc, Persoon p
                            WHERE hc.Persoon_Email = p.Email";

            using (OracleConnection connection = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = connection;
                    try
                    {
                        connection.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Huurcontract hc = new Huurcontract();
                                hc.ID = Convert.ToInt32(reader.GetValue(0));
                                hc.Naam = reader.GetValue(1).ToString();
                                hc.Email = reader.GetValue(2).ToString();
                                hc.Begin = reader.GetDateTime(3);
                                hc.Einde = reader.GetDateTime(4);
                                lst.Add(hc);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return lst;
                    }
                }
            }
            return lst;
        }

        /// <summary>
        /// Haalt alle gehuurde artikelen op van huurcontract
        /// </summary>
        /// <param name="id">ID van huurcontract waarvan je alle gehuurde artikelen wil weten</param>
        /// <returns>Lijst met gehuurde artikelen</returns>
        public static List<Artikel> GetHuurContractDetails(int id)
        {
            List<Artikel> lst = new List<Artikel>();
            string query = @"SELECT DISTINCT b.Naam, bt.Naam, bt.Prijs, bt.Motor, bt.Tankinhoud
                            FROM HuurContract hc, Boot b, BootType bt, HuurBoot hb
                            WHERE hc.ID = :pID
                            AND hb.HuurContract_ID = hc.ID
                            AND hb.Boot_Naam = b.Naam
                            AND b.BootType_Naam = bt.Naam";

            using (OracleConnection connection = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = connection;
                    cmd.Parameters.Add(":pID", id);
                    try
                    {
                        connection.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Boot boot = new Boot(reader.GetValue(0).ToString(), reader.GetDouble(2), (BootType)Enum.Parse(typeof(BootType), reader.GetValue(1).ToString()),
                                                    Convert.ToBoolean(reader.GetInt32(3)), (reader.GetValue(4) != DBNull.Value ? Convert.ToDouble(reader.GetInt32(4)) : 0));
                                lst.Add(boot);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return lst;
                    }
                }
            }

            query = @"SELECT DISTINCT a1.Naam, a1.Prijs, (SELECT COUNT(a.Naam)
                                                          FROM Accessoire a, HuurAccessoire ha, HuurContract hc
                                                          WHERE hc.ID = :pID
                                                          AND ha.HuurContract_ID = hc.ID
                                                          AND ha.Accessoire_Naam = a.Naam
                                                          AND a1.Naam = a.Naam
                                                          GROUP BY a.Naam) AS Aantal
                    FROM HuurContract hc1, Accessoire a1, HuurAccessoire ha1
                    WHERE hc1.ID = :pID
                    AND ha1.HuurContract_ID = hc1.ID
                    AND ha1.Accessoire_Naam = a1.Naam";

            using (OracleConnection connection = new OracleConnection(_connectionstring))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandText = query;
                    cmd.Connection = connection;
                    cmd.Parameters.Add(":pID", id);
                    cmd.Parameters.Add(":pID", id);
                    try
                    {
                        connection.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Accessoire accessoire = new Accessoire(reader.GetValue(0).ToString(), reader.GetDouble(1), Convert.ToInt32(reader.GetValue(2)));
                                lst.Add(accessoire);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return lst;
                    }
                }
            }
            return lst;
        }
    }
}
