using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SocialNetwork : System.Web.UI.Page
{
    SqlConnection konekcija = new SqlConnection(WebConfigurationManager.ConnectionStrings["SocialKon"].ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Popuni();
        }
    }

    public void Popuni()
    {
        DropDownList1.Items.Clear();
        SqlCommand komanda = new SqlCommand();
        komanda.Connection = konekcija;
        komanda.CommandText = "Select ID, firstName, surname from Users";
        SqlDataReader citac;
        konekcija.Open();
        citac = komanda.ExecuteReader();
        while (citac.Read())
        {
            ListItem novi = new ListItem();
            novi.Text = citac["firstName"] + " " + citac["surname"];
            novi.Value = citac["ID"].ToString();
            DropDownList1.Items.Add(novi);
        }
        citac.Close();
        konekcija.Close();
    }

    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        ListBox1.Items.Clear();
        lbfof.Items.Clear();
        lbsug.Items.Clear();

        //Friends
        string naredba = "select u.ID, u.firstName, u.surname from Users as u "+
            "join User_Friend as uf "+
            "on u.ID = uf.IDfr "+
            "where uf.ID = '" + DropDownList1.SelectedItem.Value + "' "+
            "union "+
            "select u.ID, u.firstName, u.surname from Users as u "+
            "join User_Friend as uf "+
            "on u.ID = uf.ID "+
            "where uf.IDfr = '" + DropDownList1.SelectedItem.Value + "'";

        SqlCommand komanda = new SqlCommand(naredba, konekcija);
        SqlDataAdapter adapter = new SqlDataAdapter(komanda);

        DataTable dtFriends = new DataTable();
        konekcija.Open();
        adapter.Fill(dtFriends);
        konekcija.Close();
        foreach (DataRow red in dtFriends.Rows)
        {
            ListItem novi = new ListItem();
            novi.Text = red["firstName"] +" "+ red["surname"];
            novi.Value = red["ID"].ToString();
            ListBox1.Items.Add(novi);
        }

        //Friends of friends (bez zajednickih prijatelja)
        string naredba1 = "select u.ID, u.firstName, u.surname " +
        "from Users as u " +
        "join User_Friend as uf " +
        "on u.ID = uf.IDfr and uf.Idfr <> '" + DropDownList1.SelectedItem.Value + "' and uf.IDfr not in (select * from fn_FriendsOfSelected('" + DropDownList1.SelectedItem.Value + "')) " +
        "where uf.ID in (select * from fn_FriendsOfSelected('" + DropDownList1.SelectedItem.Value + "')) " +
        "union " +
        "select u.ID, u.firstName, u.surname " +
        "from Users as u " +
        "join User_Friend as uf " +
        "on u.ID = uf.ID and uf.ID <> '" + DropDownList1.SelectedItem.Value + "' and uf.ID not in (select * from fn_FriendsOfSelected('" + DropDownList1.SelectedItem.Value + "')) " +
        "where uf.IDfr in (select * from fn_FriendsOfSelected('" + DropDownList1.SelectedItem.Value + "'))";

        SqlCommand komanda1 = new SqlCommand(naredba1, konekcija);
        SqlDataAdapter adapter1 = new SqlDataAdapter(komanda1);

        DataTable dtFof = new DataTable();
        konekcija.Open();
        adapter1.Fill(dtFof);
        konekcija.Close();
        foreach (DataRow red in dtFof.Rows)
        {
            ListItem novi = new ListItem();
            novi.Text = red["firstName"] + " " + red["surname"];
            novi.Value = red["ID"].ToString();
            lbfof.Items.Add(novi);
        }

        //Suggested friends
        string naredba2 = "select u.ID, u.firstName, u.surname " +
                          "from " +
                          "(select f1.IDfr from fn_FriendsOfFriends('" + DropDownList1.SelectedItem.Value + "') as f1 " +
                           "where exists(select numidfr " +
                                        "from(select count(*) as numidfr " +
                                             "from(select IDfr from fn_FriendsOfSelected(f1.IDfr) as ffof " +
                                                  "where ffof.IDfr in (select * from fn_FriendsOfSelected('" + DropDownList1.SelectedItem.Value + "'))) as derived) as d2 " +
                                        "where numidfr >= 2)) as sugfr " +
                          "join Users as u " +
                          "on sugfr.IDfr = u.ID";

        SqlCommand komanda2 = new SqlCommand(naredba2, konekcija);
        SqlDataAdapter adapter2 = new SqlDataAdapter(komanda2);

        DataTable dtsug = new DataTable();
        konekcija.Open();
        adapter2.Fill(dtsug);
        konekcija.Close();
        foreach (DataRow red in dtsug.Rows)
        {
            ListItem novi = new ListItem();
            novi.Text = red["firstName"] + " " + red["surname"];
            novi.Value = red["ID"].ToString();
            lbsug.Items.Add(novi);
        }
    }
}