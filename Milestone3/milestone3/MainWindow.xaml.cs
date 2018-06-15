using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
//using Microsoft.Windows.Controls.DataVisualization.Charting;

namespace mile1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string tQuery;
        public string tOpenHour;
        public string tCloseHour;
        public string tBusinessID;
        public string foropenclose;

        public List<string> tBusinessIDs = new List<string>();
        public List<string> tOpenHours = new List<string>();
        public List<string> tCloseHours = new List<string>();

        public double la;
        public double lo;
        //[ImplementPropertyChanged]

        public class Business : INotifyPropertyChanged
        {

            private int total_checkin;
            public int Total_checkin
            {
                get
                {
                    return total_checkin;
                }
                set
                {
                    UpdateProperty(ref total_checkin, value);
                }
            }

            private void UpdateProperty<T>(ref T properValue, T newValue, [CallerMemberName] string propertyName = "")
            {
                if (object.Equals(properValue, newValue))
                {
                    return;
                }
                properValue = newValue;

                OnPropertyChanged(propertyName);
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            public string name { get; set; }
            public string address { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zip_code { get; set; }
            public double distance { get; set; }
            public float stars { get; set; }
            public float avg_review_rating { get; set; }
            public int num_of_reviews { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            addStates();
            addColumns2Grid();
            addColumns2FriendsGrid();
            addColumns2ReivewGrid();
            for (int i = 0; i < 24; i++)
            {
                FromList.Items.Add(i.ToString() + ":00");
                ToList.Items.Add(i.ToString() + ":00");
            }

            dayofweeklist.Items.Add("Monday");
            dayofweeklist.Items.Add("Tuesday");
            dayofweeklist.Items.Add("Wednesday");
            dayofweeklist.Items.Add("Thursday");
            dayofweeklist.Items.Add("Friday");
            dayofweeklist.Items.Add("Saturday");
            dayofweeklist.Items.Add("Sunday");

            double num = -0.5;
            for (int i = 0; i <= 10; i++)
                ratingcomboBox.Items.Add((num += 0.5).ToString());
            ratingcomboBox.SelectedIndex = 10;

        }

        private string buildConnString()
        {
            //return "Host=localhost; username=postgres; password=abc0553; Database=milestone3db";
            return "Host=wsnip.ddns.net; port=4321; username=rantao; password=/*-/*-;Database=cpts451_milestone3db";
        }

        public void addStates()
        {
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT distinct state FROM business_address ORDER BY state;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stateslist.Items.Add(reader.GetString(0));
                        }
                    }

                }
                conn.Close();
            }
        }

        public void addColumns2Grid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Business Name";
            col1.Binding = new Binding("name");
            col1.Width = 128;
            businessGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Address";
            col2.Binding = new Binding("address");
            col2.Width = 128;
            businessGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "City";
            col3.Binding = new Binding("city");
            col3.Width = 64;
            businessGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "State";
            col4.Binding = new Binding("state");
            businessGrid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Header = "Zip Code";
            col5.Binding = new Binding("zip_code");
            businessGrid.Columns.Add(col5);

            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Header = "Distance\n(mile)";
            col6.Binding = new Binding("distance");
            col6.Width = 64;
            businessGrid.Columns.Add(col6);

            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Header = "Stars";
            col7.Binding = new Binding("stars");
            businessGrid.Columns.Add(col7);

            DataGridTextColumn col8 = new DataGridTextColumn();
            col8.Header = "Number of\nreviews";
            col8.Binding = new Binding("num_of_reviews");
            businessGrid.Columns.Add(col8);

            DataGridTextColumn col9 = new DataGridTextColumn();
            col9.Header = "Avg review\nrating";
            col9.Binding = new Binding("avg_review_rating");
            businessGrid.Columns.Add(col9);

            DataGridTextColumn col10 = new DataGridTextColumn();
            col10.Header = "Total\nCheckin";
            col10.Binding = new Binding("Total_checkin");
            businessGrid.Columns.Add(col10);
        }

        public static double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'M')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }

        private void stateslist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            businessGrid.Items.Clear();
            cityBox.Items.Clear();
            if (stateslist.SelectedIndex > -1)
            {
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {

                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT distinct name,address,city,state,postalcode,stars,reviewcount,reviewrating,numcheckins,latitude,longitude FROM business b,business_address a WHERE b.business_id=a.business_id AND state='" + stateslist.SelectedItem.ToString() + "' ORDER BY numcheckins DESC;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                double bla = reader.GetDouble(9);
                                double blo = reader.GetDouble(10);
                                double dis = DistanceTo(bla, blo, la, lo);

                                businessGrid.Items.Add(new Business()
                                {
                                    name = reader.GetString(0),
                                    address = reader.GetString(1),
                                    city = reader.GetString(2),
                                    state = reader.GetString(3),
                                    zip_code = reader.GetString(4),
                                    distance = dis,
                                    stars = reader.GetFloat(5),
                                    num_of_reviews = reader.GetInt32(6),
                                    avg_review_rating = reader.GetFloat(7),
                                    Total_checkin = reader.GetInt32(8)
                                });
                            }
                        }

                    }

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT distinct city FROM business_address WHERE state = '" + stateslist.SelectedItem.ToString() + "'ORDER BY city;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cityBox.Items.Add(reader.GetString(0));
                            }
                        }

                    }
                    conn.Close();
                }
                numofbusinesslabel.Content = businessGrid.Items.Count;
            }
        }

        private void cityBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            businessGrid.Items.Clear();
            zipBox.Items.Clear();
            if (cityBox.SelectedIndex > -1)
            {
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {

                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT distinct name,address,city,state,postalcode,stars,reviewcount,reviewrating,numcheckins,latitude,longitude FROM business b,business_address a WHERE b.business_id=a.business_id AND state='" + stateslist.SelectedItem.ToString() + "'AND city='" + cityBox.SelectedItem.ToString() + "' ORDER BY numcheckins DESC;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                double bla = reader.GetDouble(9);
                                double blo = reader.GetDouble(10);
                                double dis = DistanceTo(bla, blo, la, lo);
                                businessGrid.Items.Add(new Business()
                                {
                                    name = reader.GetString(0),
                                    address = reader.GetString(1),
                                    city = reader.GetString(2),
                                    state = reader.GetString(3),
                                    zip_code = reader.GetString(4),
                                    distance = dis,
                                    stars = reader.GetFloat(5),
                                    num_of_reviews = reader.GetInt32(6),
                                    avg_review_rating = reader.GetFloat(7),
                                    Total_checkin = reader.GetInt32(8)
                                });
                            }
                        }

                    }

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT distinct postalcode FROM business_address WHERE state = '" + stateslist.SelectedItem.ToString() + "'AND city='" + cityBox.SelectedItem.ToString() + "' ORDER BY postalcode;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                zipBox.Items.Add(reader.GetString(0));
                            }
                        }

                    }
                    conn.Close();
                }
                numofbusinesslabel.Content = businessGrid.Items.Count;
            }
        }

        private void zipBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            businessGrid.Items.Clear();
            catBox.Items.Clear();
            if (zipBox.SelectedIndex > -1)
            {
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT distinct name,address,city,state,postalcode,stars,reviewcount,reviewrating,numcheckins,latitude,longitude FROM business b,business_address a WHERE b.business_id=a.business_id AND state = '" + stateslist.SelectedItem.ToString() + "'AND city = '" + cityBox.SelectedItem.ToString() + "'AND postalcode = '" + zipBox.SelectedItem.ToString() + "' ORDER BY numcheckins DESC;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                double bla = reader.GetDouble(9);
                                double blo = reader.GetDouble(10);
                                double dis = DistanceTo(bla, blo, la, lo);
                                businessGrid.Items.Add(new Business()
                                {
                                    name = reader.GetString(0),
                                    address = reader.GetString(1),
                                    city = reader.GetString(2),
                                    state = reader.GetString(3),
                                    zip_code = reader.GetString(4),
                                    distance = dis,
                                    stars = reader.GetFloat(5),
                                    num_of_reviews = reader.GetInt32(6),
                                    avg_review_rating = reader.GetFloat(7),
                                    Total_checkin = reader.GetInt32(8)
                                });
                            }
                        }
                    }

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT distinct category FROM business b, business_address a, business_category c WHERE b.business_id=a.business_id AND b.business_id=c.business_id AND state = '" + stateslist.SelectedItem.ToString() + "'AND city='" + cityBox.SelectedItem.ToString() + "'AND postalcode = '" + zipBox.SelectedItem.ToString() + "' ORDER BY category;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                catBox.Items.Add(reader.GetString(0));
                            }
                        }
                    }
                    conn.Close();
                }
                numofbusinesslabel.Content = businessGrid.Items.Count;
            }
        }

        private void catBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string temp = "";
            if (catBox.SelectedItems.Count > 0)
            {
                foreach (var item in catBox.SelectedItems)
                {
                    temp = temp
                        + "(SELECT distinct name,address,city,state,postalcode,stars,reviewcount,reviewrating,numcheckins,b.business_id,latitude,longitude FROM business b,business_address a, business_category c WHERE b.business_id=a.business_id AND b.business_id=c.business_id AND state = '" + stateslist.SelectedItem.ToString() + "'AND city = '" + cityBox.SelectedItem.ToString() + "'AND postalcode = '" + zipBox.SelectedItem.ToString()
                        + "'AND category='" + item.ToString() + "' ORDER BY numcheckins DESC)INTERSECT";
                }
                temp = temp.Substring(0, temp.Length - 9);
            }

            businessGrid.Items.Clear();
            SelectedCategorylistBox.Items.Clear();

            foreach (var item in catBox.SelectedItems)
            {
                SelectedCategorylistBox.Items.Add(item.ToString());
            }

            //dayofweeklist.Items.Clear();
            if (catBox.SelectedIndex > -1)
            {
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = temp;
                        tQuery = cmd.CommandText;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                double bla = reader.GetDouble(10);
                                double blo = reader.GetDouble(11);
                                double dis = DistanceTo(bla, blo, la, lo);
                                businessGrid.Items.Add(new Business()
                                {
                                    name = reader.GetString(0),
                                    address = reader.GetString(1),
                                    city = reader.GetString(2),
                                    state = reader.GetString(3),
                                    zip_code = reader.GetString(4),
                                    distance = dis,
                                    stars = reader.GetFloat(5),
                                    num_of_reviews = reader.GetInt32(6),
                                    avg_review_rating = reader.GetFloat(7),
                                    Total_checkin = reader.GetInt32(8)
                                });
                            }
                        }

                    }

                    //using (var cmd = new NpgsqlCommand())
                    //{
                    //    cmd.Connection = conn;
                    //    cmd.CommandText = "SELECT distinct dayofweek FROM business_checkin c,(" + temp + ") as r1 WHERE c.business_id=r1.business_id;";
                    //    using (var reader = cmd.ExecuteReader())
                    //    {
                    //        while (reader.Read())
                    //        {
                    //            dayofweeklist.Items.Add(reader.GetString(0));
                    //        }
                    //    }

                    //}
                    conn.Close();
                }
                numofbusinesslabel.Content = businessGrid.Items.Count;
            }
        }

        private void dayofweeklist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            businessGrid.Items.Clear();
            //FromList.Items.Clear();
            tBusinessIDs.Clear();
            tOpenHours.Clear();
            tCloseHours.Clear();
            if (dayofweeklist.SelectedIndex > -1)
            {
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT distinct name,address,city,state,postalcode,stars,reviewcount,reviewrating,numcheckins,h.business_id,openhour,closehour,latitude,longitude FROM business_hour h, (" +
                            tQuery + ") as r1 WHERE h.business_id = r1.business_id AND dayofweek = '" + dayofweeklist.SelectedItem.ToString() + "' ORDER BY numcheckins DESC; ";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                double bla = reader.GetDouble(12);
                                double blo = reader.GetDouble(13);
                                double dis = DistanceTo(bla, blo, la, lo);
                                businessGrid.Items.Add(new Business()
                                {
                                    name = reader.GetString(0),
                                    address = reader.GetString(1),
                                    city = reader.GetString(2),
                                    state = reader.GetString(3),
                                    zip_code = reader.GetString(4),
                                    distance = dis,
                                    stars = reader.GetFloat(5),
                                    num_of_reviews = reader.GetInt32(6),
                                    avg_review_rating = reader.GetFloat(7),
                                    Total_checkin = reader.GetInt32(8)
                                });

                                tBusinessID = reader.GetString(9);
                                tBusinessIDs.Add(tBusinessID);

                                tOpenHour = reader.GetString(10);
                                tOpenHours.Add(tOpenHour);

                                tCloseHour = reader.GetString(11);
                                tCloseHours.Add(tCloseHour);


                            }
                        }
                    }
                    conn.Close();
                }
                numofbusinesslabel.Content = businessGrid.Items.Count;
            }
        }

        private void FromList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int count = tBusinessIDs.Count();
            List<int> opened = new List<int>();

            if (ToList.SelectedIndex > -1)
            {
                for (int i = 0; i < count; i++)
                {
                    int fromHour = Int32.Parse(FromList.SelectedItem.ToString().Substring(0, FromList.SelectedItem.ToString().Length - 3));
                    int toHour = Int32.Parse(ToList.SelectedItem.ToString().Substring(0, ToList.SelectedItem.ToString().Length - 3));
                    int closeHour = Int32.Parse(tCloseHours[i].Split(':')[0]);
                    int openHour = Int32.Parse(tOpenHours[i].Split(':')[0]);

                    if (openHour != closeHour)
                    {
                        if (fromHour < toHour) //day time
                        {
                            if (fromHour > openHour && toHour <= closeHour)
                            {
                                opened.Add(i);
                            }
                        }
                        else if (fromHour == toHour) //24hour
                        {
                            if (closeHour == openHour)
                                opened.Add(i);
                        }
                        else //night time
                        {
                            if (openHour > closeHour) //overnight place
                            {
                                if (toHour <= closeHour)
                                {
                                    opened.Add(i);
                                }
                            }
                        }
                    }
                    else
                    {
                        opened.Add(i);
                    }

                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    int fromHour = Int32.Parse(FromList.SelectedItem.ToString().Substring(0, FromList.SelectedItem.ToString().Length - 3));
                    int openHour = Int32.Parse(tOpenHours[i].Split(':')[0]);
                    if (fromHour >= openHour)
                    {
                        opened.Add(i);
                    }
                }
            }
            string tcmd = "";

            if (opened.Count > 0)
            {
                tcmd = "SELECT distinct name,address,city,state,postalcode,stars,reviewcount,reviewrating,numcheckins,h.business_id,latitude,longitude FROM business_hour h, (" +
                                tQuery + ") as r1 WHERE h.business_id = r1.business_id AND dayofweek = '" + dayofweeklist.SelectedItem.ToString() + "' AND (";

                for (int i = 0; i < opened.Count; i++)
                {
                    tcmd = tcmd + " h.business_id='" + tBusinessIDs[opened[i]] + "' OR";
                }

                tcmd = tcmd.Substring(0, tcmd.Length - 2);

                tcmd = tcmd + ");";
            }
            else
            {
                tcmd = "SELECT distinct name,address,city,state,postalcode,stars,reviewcount,reviewrating,numcheckins,h.business_id,latitude,longitude FROM business_hour h, (" +
                                tQuery + ") as r1 WHERE h.business_id = r1.business_id AND dayofweek = '" + dayofweeklist.SelectedItem.ToString() + "' AND (h.business_id='null');";
            }

            businessGrid.Items.Clear();
            //ToList.Items.Clear();
            if (FromList.SelectedIndex > -1)
            {
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = tcmd;
                        foropenclose = cmd.CommandText;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                double bla = reader.GetDouble(10);
                                double blo = reader.GetDouble(11);
                                double dis = DistanceTo(bla, blo, la, lo);
                                businessGrid.Items.Add(new Business()
                                {
                                    name = reader.GetString(0),
                                    address = reader.GetString(1),
                                    city = reader.GetString(2),
                                    state = reader.GetString(3),
                                    zip_code = reader.GetString(4),
                                    distance = dis,
                                    stars = reader.GetFloat(5),
                                    num_of_reviews = reader.GetInt32(6),
                                    avg_review_rating = reader.GetFloat(7),
                                    Total_checkin = reader.GetInt32(8)
                                });
                            }
                        }
                    }
                    conn.Close();
                }
                numofbusinesslabel.Content = businessGrid.Items.Count;
            }
        }

        private void ToList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int count = tBusinessIDs.Count();
            List<int> closed = new List<int>();

            for (int i = 0; i < count; i++)
            {
                int fromHour = Int32.Parse(FromList.SelectedItem.ToString().Substring(0, FromList.SelectedItem.ToString().Length - 3));
                int toHour = Int32.Parse(ToList.SelectedItem.ToString().Substring(0, ToList.SelectedItem.ToString().Length - 3));
                int closeHour = Int32.Parse(tCloseHours[i].Split(':')[0]);
                int openHour = Int32.Parse(tOpenHours[i].Split(':')[0]);

                if (openHour != closeHour)
                {
                    if (fromHour < toHour) //day time
                    {
                        if (toHour <= closeHour)
                        {
                            closed.Add(i);
                        }
                    }
                    else if (fromHour == toHour) //24hour
                    {
                        if (closeHour == openHour)
                            closed.Add(i);
                    }
                    else //night time
                    {
                        if (openHour > closeHour) //overnight place
                        {
                            if (toHour <= closeHour)
                            {
                                closed.Add(i);
                            }
                        }
                    }
                }
                else
                {
                    closed.Add(i);
                }

            }

            string tcmd = "";

            if (closed.Count > 0)
            {
                tcmd = foropenclose.Substring(0, foropenclose.Length - 1) + " AND (";

                for (int i = 0; i < closed.Count; i++)
                {
                    tcmd = tcmd + " h.business_id='" + tBusinessIDs[closed[i]] + "' OR";
                }

                tcmd = tcmd.Substring(0, tcmd.Length - 2);

                tcmd = tcmd + ") ORDER BY numcheckins DESC;";
            }
            else
            {
                tcmd = foropenclose.Substring(0, foropenclose.Length - 1) + " AND (h.business_id='null');";
            }

            businessGrid.Items.Clear();
            if (FromList.SelectedIndex > -1)
            {
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = tcmd;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                double bla = reader.GetDouble(10);
                                double blo = reader.GetDouble(11);
                                double dis = DistanceTo(bla, blo, la, lo);
                                businessGrid.Items.Add(new Business()
                                {
                                    name = reader.GetString(0),
                                    address = reader.GetString(1),
                                    city = reader.GetString(2),
                                    state = reader.GetString(3),
                                    zip_code = reader.GetString(4),
                                    distance = dis,
                                    stars = reader.GetFloat(5),
                                    num_of_reviews = reader.GetInt32(6),
                                    avg_review_rating = reader.GetFloat(7),
                                    Total_checkin = reader.GetInt32(8)
                                });
                            }
                        }
                    }
                    conn.Close();
                }
                numofbusinesslabel.Content = businessGrid.Items.Count;
            }
        }

        private void UserNametextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserIdlistBox.Items.Clear();
            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT distinct user_id,name,averagerating,fans,yelpingsince,funny,cool,useful FROM users WHERE name='" + UserNametextBox.Text + "';";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserIdlistBox.Items.Add(reader.GetString(0));
                            NametextBox.Text = reader.GetString(1);
                            StarstextBox.Text = reader.GetFloat(2).ToString();
                            FanstextBox.Text = reader.GetInt32(3).ToString();
                            SincetextBox.Text = reader.GetString(4);
                            FunnytextBox.Text = reader.GetInt32(5).ToString();
                            CooltextBox.Text = reader.GetInt32(6).ToString();
                            UsefultextBox.Text = reader.GetInt32(7).ToString();
                        }
                    }
                }
                conn.Close();
            }
        }

        public class Friends
        {
            public string name { get; set; }
            public float averagerating { get; set; }
            public string yelpingsince { get; set; }
            public int fans { get; set; }
            public int reivewcount { get; set; }
            public int funny { get; set; }
            public int useful { get; set; }
            public int cool { get; set; }
            public string user_id { get; set; }
        }

        public void addColumns2FriendsGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Name";
            col1.Binding = new Binding("name");
            FriendslistdataGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Avg Stars";
            col2.Binding = new Binding("averagerating");
            FriendslistdataGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "Yelping Since";
            col3.Binding = new Binding("yelpingsince");
            FriendslistdataGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "Fans";
            col4.Binding = new Binding("fans");
            FriendslistdataGrid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Header = "reivew count";
            col5.Binding = new Binding("reivewcount");
            FriendslistdataGrid.Columns.Add(col5);

            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Header = "Funny";
            col6.Binding = new Binding("funny");
            FriendslistdataGrid.Columns.Add(col6);

            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Header = "Useful";
            col7.Binding = new Binding("useful");
            FriendslistdataGrid.Columns.Add(col7);

            DataGridTextColumn col8 = new DataGridTextColumn();
            col8.Header = "Cool";
            col8.Binding = new Binding("cool");
            FriendslistdataGrid.Columns.Add(col8);

            DataGridTextColumn col9 = new DataGridTextColumn();
            col9.Header = "User id";
            col9.Binding = new Binding("user_id");
            FriendslistdataGrid.Columns.Add(col9);
        }

        public class Reivew
        {
            public string name { get; set; }
            public string business { get; set; }
            public string city { get; set; }
            public string text { get; set; }
            public float rating { get; set; }
            public string date { get; set; }
            public string address { get; set; }
        }

        public void addColumns2ReivewGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Name";
            col1.Binding = new Binding("name");
            ReivewdataGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Business";
            col2.Binding = new Binding("business");
            ReivewdataGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "City";
            col3.Binding = new Binding("city");
            ReivewdataGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "Text";
            col4.Binding = new Binding("text");
            ReivewdataGrid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Header = "Rating";
            col5.Binding = new Binding("rating");
            ReivewdataGrid.Columns.Add(col5);

            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Header = "Date";
            col6.Binding = new Binding("date");
            ReivewdataGrid.Columns.Add(col6);

            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Header = "Address";
            col7.Binding = new Binding("address");
            ReivewdataGrid.Columns.Add(col7);

        }

        private void UserIdlistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NametextBox.Clear();
            StarstextBox.Clear();
            FanstextBox.Clear();
            SincetextBox.Clear();
            FunnytextBox.Clear();
            CooltextBox.Clear();
            UsefultextBox.Clear();
            FriendslistdataGrid.Items.Clear();
            ReivewdataGrid.Items.Clear();
            if (UserIdlistBox.SelectedIndex > -1)
            {
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT distinct user_id,name,averagerating,fans,yelpingsince,funny,cool,useful FROM users WHERE name='"
                            + UserNametextBox.Text + "' AND user_id='" + UserIdlistBox.SelectedItem.ToString() + "';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                NametextBox.Text = reader.GetString(1);
                                StarstextBox.Text = reader.GetFloat(2).ToString();
                                FanstextBox.Text = reader.GetInt32(3).ToString();
                                SincetextBox.Text = reader.GetString(4);
                                FunnytextBox.Text = reader.GetInt32(5).ToString();
                                CooltextBox.Text = reader.GetInt32(6).ToString();
                                UsefultextBox.Text = reader.GetInt32(7).ToString();
                            }
                        }
                    }

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT distinct Name,averagerating,yelpingsince,fans,reivewcount,funny,useful,cool,u.user_id FROM users u, (SELECT distinct friend_id FROM users u, user_friend f  WHERE u.user_id='" + UserIdlistBox.SelectedItem.ToString() + "' and u.user_id=f.user_id) as r1 WHERE friend_id=u.user_id;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                FriendslistdataGrid.Items.Add(new Friends()
                                {
                                    name = reader.GetString(0),
                                    averagerating = reader.GetFloat(1),
                                    yelpingsince = reader.GetString(2),
                                    fans = reader.GetInt32(3),
                                    reivewcount = reader.GetInt32(4),
                                    funny = reader.GetInt32(5),
                                    useful = reader.GetInt32(6),
                                    cool = reader.GetInt32(7),
                                    user_id = reader.GetString(8)
                                });
                            }
                        }
                    }

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT u.name,b.name,a.city,r.text,r.rating,r.date,a.address FROM review r,business b,users u,business_address a, (SELECT distinct friend_id  FROM users u, user_friend f WHERE u.user_id='" + UserIdlistBox.SelectedItem.ToString() + "' and u.user_id=f.user_id) as r1 WHERE friend_id=r.user_id AND b.business_id=r.business_id AND r.user_id=u.user_id AND b.business_id=a.business_id ORDER BY date DESC;";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReivewdataGrid.Items.Add(new Reivew()
                                {
                                    name = reader.GetString(0),
                                    business = reader.GetString(1),
                                    city = reader.GetString(2),
                                    text = reader.GetString(3),
                                    rating = reader.GetFloat(4),
                                    date = reader.GetString(5),
                                    address = reader.GetString(6)
                                });
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }

        private void SetLocationbutton_Click(object sender, RoutedEventArgs e)
        {
            la = float.Parse(LattitudetextBox.Text);
            lo = float.Parse(LongitudetextBox.Text);
        }

        private void removefriendbutton_Click(object sender, RoutedEventArgs e)
        {
            string uid = UserIdlistBox.SelectedItem.ToString();
            Friends t = (Friends)FriendslistdataGrid.SelectedCells[0].Item;
            string fid = t.user_id;

            using (var conn = new NpgsqlConnection(buildConnString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "DELETE FROM user_friend WHERE user_id='" + uid + "' AND friend_id='" + fid + "';";
                    cmd.ExecuteNonQuery();
                    FriendslistdataGrid.Items.Remove(t);
                }
                conn.Close();
            }
        }

        private void checkinbutton_Click(object sender, RoutedEventArgs e)
        {
            //PropertyChangedEventHandler handler = PropertyChanged;
            if (businessGrid.SelectedIndex > -1)
            {
                Business tb = (Business)businessGrid.SelectedCells[0].Item;
                tb.Total_checkin++;

                //tb.PropertyChanged += handler;
                //PropertyChanged++;
                //businessGrid.NotifyCurrentCellDirty(true);
                //this.businessGrid.DataContextChanged;

                string businessname = tb.name;
                string businessaddress = tb.address;

                string currentWeek = DateTime.Now.DayOfWeek.ToString();
                string currentTime = DateTime.Now.ToString("H:mm");
                int currentHour = Int32.Parse(currentTime.Substring(0, currentTime.Length - 3));
                string business_id = "";

                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT b.business_id FROM business b,business_checkin c,business_address a WHERE b.business_id=c.business_id AND b.business_id=a.business_id AND b.name='" + businessname + "' AND a.address='" + businessaddress + "' AND c.dayofweek='" + currentWeek + "';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                business_id = reader.GetString(0);
                            }
                        }
                    }
                    conn.Close();
                }

                if (6 <= currentHour && currentHour < 12) //morning
                {
                    using (var conn = new NpgsqlConnection(buildConnString()))
                    {
                        conn.Open();
                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = "UPDATE business_checkin SET morning=morning+1 WHERE business_id='" + business_id + "' AND dayofweek='" + currentWeek + "'";
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
                else if (12 <= currentHour && currentHour < 17) //afternoon
                {
                    using (var conn = new NpgsqlConnection(buildConnString()))
                    {
                        conn.Open();
                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = "UPDATE business_checkin SET afternoon=afternoon+1 WHERE business_id='" + business_id + "' AND dayofweek='" + currentWeek + "'";
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
                else if (17 <= currentHour && currentHour < 23) //evening
                {
                    using (var conn = new NpgsqlConnection(buildConnString()))
                    {
                        conn.Open();
                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = "UPDATE business_checkin SET evening=evening+1 WHERE business_id='" + business_id + "' AND dayofweek='" + currentWeek + "'";
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
                else //night
                {
                    using (var conn = new NpgsqlConnection(buildConnString()))
                    {
                        conn.Open();
                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = "UPDATE business_checkin SET night=night+1 WHERE business_id='" + business_id + "' AND dayofweek='" + currentWeek + "'";
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
            }
        }

        private void businessGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (businessGrid.SelectedIndex > -1)
                businessnametextBox.Text = ((Business)businessGrid.SelectedCells[0].Item).name;
        }
        //cite from net.
        Random m_rnd = new Random();
        public char getRandomChar()
        {
            int ret = m_rnd.Next(122);
            while (ret < 48 || (ret > 57 && ret < 65) || (ret > 90 && ret < 97))
            {
                ret = m_rnd.Next(122);
            }
            return (char)ret;
        }

        public string getRandomString(int length)
        {
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(getRandomChar());
            }
            return sb.ToString();
        }

        private void reviewbutton_Click(object sender, RoutedEventArgs e)
        {
            if (businessGrid.SelectedIndex > -1 && UserIdlistBox.SelectedIndex > -1)
            {
                Business tb = (Business)businessGrid.SelectedCells[0].Item;

                //businessGrid.NotifyCurrentCellDirty(true);
                //this.businessGrid.DataContextChanged;

                string businessname = tb.name;
                string businessaddress = tb.address;

                string business_id = "";

                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT b.business_id FROM business b,business_address a WHERE b.business_id=a.business_id AND b.name='" + businessname + "' AND a.address='" + businessaddress + "';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                business_id = reader.GetString(0);
                            }
                        }
                    }
                    conn.Close();
                }

                string user_id = UserIdlistBox.SelectedItem.ToString();
                //business_id
                string review_id = getRandomString(22);
                string reviewtext = reviewtextBox.Text;
                string rating = ratingcomboBox.SelectedItem.ToString();
                string date = DateTime.Now.ToString("yyyy-MM-dd");

                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO review(user_id,business_id,review_id,text,rating,date,funny,useful,cool) VALUES ('"
                            + user_id + "','" + business_id + "','" + review_id + "','" + reviewtext + "'," + rating + ",'" + date + "',0,0,0);";
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                reviewtextBox.Text = "Done, review successfully added";
            }
        }

        private void ShowCheckinButton_Click(object sender, RoutedEventArgs e)
        {
            if (businessGrid.SelectedIndex > -1)
            {
                Business tb = (Business)businessGrid.SelectedCells[0].Item;

                string businessname = tb.name;
                string businessaddress = tb.address;

                string business_id = "";

                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT b.business_id FROM business b,business_address a WHERE b.business_id=a.business_id AND b.name='" + businessname + "' AND a.address='" + businessaddress + "';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                business_id = reader.GetString(0);
                            }
                        }
                    }
                    conn.Close();
                }

                int morning = 0, afternoon = 0, evening = 0, night = 0;
                string dayofweek = "";
                List<KeyValuePair<string, int>> valueList = new List<KeyValuePair<string, int>>();
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT dayofweek,morning,afternoon,evening,night FROM business_checkin WHERE business_id='" + business_id + "';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dayofweek = reader.GetString(0);
                                morning = reader.GetInt32(1);
                                afternoon = reader.GetInt32(2);
                                evening = reader.GetInt32(3);
                                night = reader.GetInt32(4);

                                valueList.Add(new KeyValuePair<string, int>(dayofweek, morning + afternoon + evening + night));
                            }
                        }
                    }
                    conn.Close();
                }

                //columnChart.ti = "";
                Window1 win1 = new Window1();
                win1.columnChart.DataContext = valueList;
                win1.ShowDialog();
            }
        }
        public void addColumns2reviewonNewWindowGrid(reviewWindow rWin)
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "Date";
            col1.Binding = new Binding("date");
            rWin.reviewdataGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "User name";
            col2.Binding = new Binding("name");
            rWin.reviewdataGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "Star";
            col3.Binding = new Binding("rating");
            rWin.reviewdataGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "Text";
            col4.Binding = new Binding("text");
            rWin.reviewdataGrid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Header = "Funny";
            col5.Binding = new Binding("funny");
            rWin.reviewdataGrid.Columns.Add(col5);

            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Header = "Useful";
            col6.Binding = new Binding("useful");
            rWin.reviewdataGrid.Columns.Add(col6);

            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Header = "Cool";
            col7.Binding = new Binding("cool");
            rWin.reviewdataGrid.Columns.Add(col7);


            //rWin.reviewdataGrid.MaxWidth = 256;
        }
        public class ReivewOnNewWindow
        {
            public string date { get; set; }
            public string name { get; set; }
            public float rating { get; set; }
            public string text { get; set; }
            public int funny { get; set; }
            public int useful { get; set; }
            public int cool { get; set; }
        }

        private void ShowReviewsButton_Click(object sender, RoutedEventArgs e)
        {
            if (businessGrid.SelectedIndex > -1)
            {
                Business tb = (Business)businessGrid.SelectedCells[0].Item;
                reviewWindow rWin = new reviewWindow();
                addColumns2reviewonNewWindowGrid(rWin);
                string businessname = tb.name;
                string businessaddress = tb.address;

                string business_id = "";

                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT b.business_id FROM business b,business_address a WHERE b.business_id=a.business_id AND b.name='" + businessname + "' AND a.address='" + businessaddress + "';";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                business_id = reader.GetString(0);
                            }
                        }
                    }
                    conn.Close();
                }
                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT r1.date,u.name,r1.rating,r1.text,r1.funny,r1.useful,r1.cool FROM users u, (SELECT * FROM review WHERE business_id='" + business_id + "') as r1 WHERE r1.user_id=u.user_id ORDER BY r1.date DESC";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rWin.reviewdataGrid.Items.Add(
                                    new ReivewOnNewWindow()
                                    {
                                        date = reader.GetString(0),
                                        name = reader.GetString(1),
                                        rating = reader.GetFloat(2),
                                        text = reader.GetString(3),
                                        funny = reader.GetInt32(4),
                                        useful = reader.GetInt32(5),
                                        cool = reader.GetInt32(6)
                                    });
                            }
                        }
                    }
                    conn.Close();
                }
                rWin.ShowDialog();
            }
        }

        private void NumOfBusPZipButton_Click(object sender, RoutedEventArgs e)
        {
            if (cityBox.SelectedIndex > -1)
            {
                PerZipcodeWindow zipWin = new PerZipcodeWindow();
                string city = cityBox.SelectedItem.ToString();
                List<KeyValuePair<int, int>> valueList = new List<KeyValuePair<int, int>>();

                using (var conn = new NpgsqlConnection(buildConnString()))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "SELECT postalcode,COUNT(postalcode) FROM business_address a, business b WHERE city = '" + city + "' AND a.business_id = b.business_id GROUP BY postalcode";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                valueList.Add(new KeyValuePair<int, int>(reader.GetInt32(0), reader.GetInt32(1)));
                            }
                        }
                    }
                    conn.Close();
                }
                zipWin.columnChart.DataContext = valueList;
                zipWin.ShowDialog();
            }
        }
    }
}