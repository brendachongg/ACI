using GeneralLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;


namespace DataLayer
{
    public class DB_Access_Control
    {
        private Database_Connection dbConnection = new Database_Connection();

        public bool checkUserAccess(int userId, string functionName)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select accessId from aci_access_rights where userId=@usr and defunct='N' and FunctionId=(select FunctionId from aci_functions where functionName=@funct)";

                cmd.Parameters.AddWithValue("@usr", userId);
                cmd.Parameters.AddWithValue("@funct", functionName);

                return dbConnection.executeScalar(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Access_Control.cs", "checkUserAccess()", ex.Message, -1);

                return false;
            }
        }

        //get all the functionId each user has
        public DataTable getUserAccess(int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "SELECT a.functionId, f.functionName from aci_access_rights a inner join aci_functions f on a.functionId=f.functionId WHERE a.userId = @userId and a.defunct='N'";
                cmd.Parameters.AddWithValue("@userId", userId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Access_Control.cs", "getFunctionOfUser()", ex.Message, -1);

                return null;
            }
        }


        public bool assignAccessRights(List<int> selFunction, int staffId, int userId)
        {
            //get existing access rights first
            DataTable dt = getUserAccess(staffId);
            if (dt == null) return false;
            List<int> existAr = new List<int>();
            foreach (DataRow dr in dt.Rows) existAr.Add((int)dr["functionId"]);

            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                sqlConn.Open();
                SqlTransaction trans = sqlConn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConn;
                cmd.Transaction = trans;

                try
                {
                    List<SqlParameter> arPrams = new List<SqlParameter>();
                    string selAr = "";
                    List<int> newAr = new List<int>();
                    for (int i = 0; i < selFunction.Count; i++)
                    {
                        selAr += "@fid" + i + ",";
                        arPrams.Add(new SqlParameter("@fid" + i, selFunction[i]));

                        if (!existAr.Exists(x => x == selFunction[i])) newAr.Add(selFunction[i]);
                    }
                    if (selAr != "") selAr = selAr.Substring(0, selAr.Length - 1);

                    cmd.CommandText = "update aci_access_rights set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where userId=@staff " + (selAr == "" ? "" : "and functionId not in (" + selAr + ")");
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@staff", staffId);
                    if (selAr != "") cmd.Parameters.AddRange(arPrams.ToArray());

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    //add those new access rights
                    cmd.CommandText = "insert into aci_access_rights(userId, functionId, createdBy) values (@staff, @fid, @uid)";
                    foreach (int fid in newAr)
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@staff", staffId);
                        cmd.Parameters.AddWithValue("@fid", fid);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Bundle.cs", "updateBundle()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
  
        }

        //get all the functions from database
        public DataTable getAccessFunctions()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "SELECT cr.codeValueDisplay, cr.codeValue, f.functionName, f.functionId, f.functionDesc "
                    + "FROM code_reference cr INNER JOIN aci_functions f ON cr.codeValue = f.functionGrp and f.defunct='N' and cr.codeType='FCTGRP' ORDER BY f.functionName";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Access_Control.cs", "getAccessFunctions()", ex.Message, -1);

                return null;
            }
        }


        //check for existing function name
        public bool checkExistingFunction(string function)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "SELECT count(*) FROM aci_functions WHERE defunct='N' and UPPER(functionName) = @n ";
                cmd.Parameters.AddWithValue("@n", function);

                return dbConnection.executeScalarInt(cmd) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Access_Control.cs", "checkExistingFunction()", ex.Message, -1);

                return true;
            }
        }

        //get all the functions from database filter by group
        public DataTable getFunctionByGroup(string group)
        {
            try
            {
                string sqlStmt = "SELECT functionId, functionName, functionDesc FROM aci_functions WHERE functionGrp = @group and defunct='N'";
                SqlCommand cmd = new SqlCommand(sqlStmt);
                cmd.Parameters.AddWithValue("@group", group);
                DataTable dtStructure = dbConnection.getDataTable(cmd);
                return dtStructure;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Access_Control.cs", "getFunctionByGroup()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getAllFunctionGrps()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "SELECT codeValueDisplay, CodeValue FROM code_reference where codeType='FCTGRP' and defunct='N' ORDER BY codeOrder";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Access_Control.cs", "getAllFunctionGrps()", ex.Message, -1);

                return null;
            }
        }

        public bool deleteFunction(int functionId, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update aci_functions set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where functionId=@fid";
                cmd.Parameters.AddWithValue("@fid", functionId);
                cmd.Parameters.AddWithValue("@uid", userId);
                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Access_Control.cs", "deleteFunction()", ex.Message, -1);

                return false;
            }
        }


        //add new function
        public bool addFunction(string group, String function, String desc, int userId)
        {
            try
            {
                SqlCommand  cmd = new SqlCommand();
                cmd.CommandText = "INSERT aci_functions(functionName, functionGrp, functionDesc, createdBy) VALUES(@f, @fg, @desc, @uid)";
                cmd.Parameters.AddWithValue("@f", function);
                cmd.Parameters.AddWithValue("@desc", desc);
                cmd.Parameters.AddWithValue("@fg", group);
                cmd.Parameters.AddWithValue("@uid", userId);
                dbConnection.executeNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Access_Control.cs", "addFunction()", ex.Message, -1);

                return false;
            }
        }


        //update the function
        public bool updateFunction(String functionGrp, int functionId, string desc, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "UPDATE aci_functions SET functionGrp = @fg, functionDesc = @desc, lastModifiedBy=@userid, lastModifiedDate=getdate() WHERE functionId = @functionId";

                cmd.Parameters.AddWithValue("@fg", functionGrp);
                cmd.Parameters.AddWithValue("@desc", desc);
                cmd.Parameters.AddWithValue("@functionId", functionId);
                cmd.Parameters.AddWithValue("@userid", userId);

                dbConnection.executeNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Access_Control.cs", "updateFunction()", ex.Message, -1);

                return false;
            }
        }

    }
}
