using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace ConsoleApplication1
{
    public class Geolocation
    {
        
        static string baseUri = "http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";
        static string result;

        public static void RetrieveFormatedAddress(string lat, string lng)
        {
            string requestUri = string.Format(baseUri, lat, lng);

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.GetEncoding("utf-8");
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                wc.DownloadStringAsync(new Uri(requestUri));
            }
        }
        static void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var xmlElm = XElement.Parse(e.Result);

            var status = (from elm in xmlElm.Descendants()
                          where elm.Name == "status"
                          select elm).FirstOrDefault();
            if (status.Value.ToLower() == "ok")
            {
                var res = (from elm in xmlElm.Descendants()
                           where elm.Name == "formatted_address"
                           select elm).FirstOrDefault();
                DataTableSave(res.Value);
            }
            else
            {
                DataTableSave(result = "No Address Found");
            }
        }
        public static void DataTableSave(string content)
        {
            string con = ConfigurationManager.ConnectionStrings["Test"].ToString();
            SqlConnection connection = new SqlConnection(con);
            connection.Open();
            string query = "INSERT INTO Location (Address) VALUES (@Address)";
            SqlCommand cmd = new SqlCommand(query, connection);
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                cmd.Parameters.AddWithValue("@Address", content);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                connection.Close();
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
        public static void DataTableRow()
        {
            string con = ConfigurationManager.ConnectionStrings["Test"].ToString();
            SqlConnection connection = new SqlConnection(con);
            connection.Open();

            string query = "Select * from Coordinate";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader dataReader = command.ExecuteReader();
            try
            {
                if (dataReader.HasRows)
                    while (dataReader.Read())
                    {
                        string lat = dataReader["Latitude"].ToString();
                        string lon = dataReader["Longitude"].ToString();
                        RetrieveFormatedAddress(lat, lon);
                    }
            }
            catch
            {
                throw;
            }
            finally
            {
                dataReader.Close();
                connection.Close();
                connection.Dispose();
            }
        }
    }
}
