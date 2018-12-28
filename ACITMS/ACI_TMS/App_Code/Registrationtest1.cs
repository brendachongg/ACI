using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public class Registrationtest1
{
  string strConnectionString = ConfigurationManager.ConnectionStrings["tmsdbConnection"].ConnectionString;

    private int _id = 0;
    private string _desc = "";
    private string _user = "";
    private string _item_type = "";


    public Registrationtest1()
    {

    }
    public Registrationtest1(int id, string desc, string user, string item_type)
    {
        this.id = id;
        this.desc = desc;
        this.user = user;
        this.item_type = item_type;

    }

    public int id
    {
        get { return _id; }
        set { _id = value; }
    }
    public string desc
    {
        get { return _desc; }
        set { _desc = value; }
    }
    public string user
    {
        get { return _user; }
        set { _user = value; }
    }
    public string item_type
    {
        get { return _item_type; }
        set { _item_type = value; }
    }

    public Registrationtest1 Registration(int id)
    {
        Registrationtest1 Registrationdetail = null;

        string desc, user, item_type;

        string queryStr = "SELECT * FROM batchreg WHERE id = @id";
        SqlConnection conn = new SqlConnection(strConnectionString);
        SqlCommand cmd = new SqlCommand(queryStr, conn);
        cmd.Parameters.AddWithValue("@id", id);
        conn.Open();
        SqlDataReader dr = cmd.ExecuteReader();

        if (dr.Read())
        {
            desc = dr["desc"].ToString();
            user = dr["user"].ToString();
            item_type = dr["item_type"].ToString();


            Registrationdetail = new Registrationtest1(id, desc, user, item_type);
        }
        else
        {
            Registrationdetail = null;
        }
        conn.Close();
        dr.Close();

        return Registrationdetail;

    }

    public List<Registrationtest1> getRegistration()
    {
        List<Registrationtest1> getRegistration = new List<Registrationtest1>();

        string desc, user, item_type;
        int id;

        string queryStr = "SELECT * FROM batchreg";
        SqlConnection conn = new SqlConnection(strConnectionString);
        SqlCommand cmd = new SqlCommand(queryStr, conn);
        conn.Open();
        SqlDataReader dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            desc = dr["desc"].ToString();
            user = dr["user"].ToString();
            item_type = dr["item_type"].ToString();
            id = int.Parse(dr["id"].ToString());
            Registrationtest1 create = new Registrationtest1(id, desc, user, item_type);
            getRegistration.Add(create);

        }
        conn.Close();
        dr.Close();


        return getRegistration;
    }
}
