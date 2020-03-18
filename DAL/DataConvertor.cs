using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace DAL
{
    public static class DataConvertor
    {
        //Ref: http://www.codeproject.com/Tips/784090/Conversion-Between-DataTable-and-List-in-Csharp
        #region :Converts List To DataTable 
        /// <summary>
        /// List To Table Ex:
        /// List<cl_Student> ls_stu = cl_ReadData.getStudents;
        /// DataTable dt = cl_DataConversion.ToDataTable(ls_stu);
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<TSource>(this IList<TSource> data)
        {
            DataTable dataTable = new DataTable(typeof(TSource).Name);
            PropertyInfo[] props = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ??
                    prop.PropertyType);
            }

            foreach (TSource item in data)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        // List To Table - Usage ex:
        // DataTable dt = cl_DataConversion.ToDataTable(cl_ReadData.getStudents);
        // or
        // List<cl_Student> ls_stu = cl_ReadData.getStudents;
        // DataTable dt = cl_DataConversion.ToDataTable(ls_stu);
        //-----------------------------------------------------------------
        #endregion

        #region :Converts DataTable To List
            /*
        /// <summary>
        /// Table To List Ex:
        /// DataTable tb_emp = cl_ReadData.getEmployees;
        /// List<cl_Employee> ls = cl_DataConversion.ToList<cl_Employee>(tb_emp);
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static List<TSource> ToList<TSource>(this DataTable dataTable) where TSource : new()
        {
            var dataList = new List<TSource>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            var objFieldNames = (from PropertyInfo aProp in typeof(TSource).GetProperties(flags)
                                 select new
                                 {
                                     Name = aProp.Name,
                                     Type = Nullable.GetUnderlyingType(aProp.PropertyType) ??
                             aProp.PropertyType
                                 }).ToList();
            var dataTblFieldNames = (from DataColumn aHeader in dataTable.Columns
                                     select new
                                     {
                                         Name = aHeader.ColumnName,
                                         Type = aHeader.DataType
                                     }).ToList();
            var commonFields = objFieldNames.Intersect(dataTblFieldNames).ToList();

            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var aTSource = new TSource();
                foreach (var aField in commonFields)
                {
                    PropertyInfo propertyInfos = aTSource.GetType().GetProperty(aField.Name);
                    var value = (dataRow[aField.Name] == DBNull.Value) ?
                    null : dataRow[aField.Name]; //if database field is nullable
                    propertyInfos.SetValue(aTSource, value, null);
                }
                dataList.Add(aTSource);
            }
            return dataList;
        }
        */
        // Table To List - Usage ex:
        // List<cl_Employee> ls = cl_DataConversion.ToList<cl_Employee>(cl_ReadData.getEmployees);
        // or
        // DataTable tb_emp = cl_ReadData.getEmployees;
        // List<cl_Employee> ls = cl_DataConversion.ToList<cl_Employee>(tb_emp);
        //-----------------------------------------------------------------
        #endregion

        #region :Convert Data List To JSON
        // List To JSON - Requires using Newtonsoft.Json from Nuget. Somex reference from Assemblies --> Extensions 
        // For Dynamic Table on page

        /// <summary>
        /// List To Json - Dynamic Table Display. Usage Ex:
        /// List<cl_Student> ls_stud = cl_ReadData.getStudents;
        /// JsonData = cl_DataConversion.ToJSONforDynamicTB(ls_stud);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string ToJSONforDynamicTB<T>(List<T> items)
        {
            string s = JsonConvert.SerializeObject(items);
            return s;
        }

        // List To Json Dynamic Table Display - Usage ex:
        // List<cl_Student> ls_stud = cl_ReadData.getStudents;
        // JsonData = cl_DataConversion.ToJSONforDynamicTB(ls_stud);
        //-----------------------------------------------------------------

        // For Manual Table on page
        /// <summary>
        /// List To Json - Manual Table Display. Usage ex:
        /// List<cl_Student> ls_st = cl_ReadData.getStudents;
        /// JsonData_Manual = cl_DataConversion.ToJSONforManualTB(ls_st);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string ToJSONforManualTB<T>(List<T> items)
        {
            // Ref: http://stackoverflow.com/questions/35654350/converting-list-of-objects-to-json-array
            string s = JsonConvert.SerializeObject(new { instance = items });
            return s;
        }

        // List To Json - Manual Table Display - Usage ex:
        // List<cl_Student> ls_st = cl_ReadData.getStudents;
        // JsonData_Manual = cl_DataConversion.ToJSONforManualTB(ls_st);
        //-----------------------------------------------------------------
        #endregion

        #region :DataTable to DataSet to DataTable
        public static DataSet dtToDataSet(DataTable dt)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        public static DataTable dsToDataTable(DataSet ds)
        {
            DataTable dt = new DataTable();
            dt = ds.Tables[0];
            return dt;
        }
        #endregion

        #region :Convert List to CSV File
        //Ref: http://stackoverflow.com/questions/17698228/generic-list-to-csv-string
        /// <summary>
        /// Ex: List<bel_Memo> memo = bal_Memo.getMemos();
        /// string str = ListToCSVFile(memo);
        /// </summary>
        public static string ListToCSVFile<T>(List<T> ls)
        {
            var properties = typeof(T).GetProperties();
            var result = new StringBuilder();

            foreach (var row in ls)
            {
                var values = properties.Select(p => p.GetValue(row, null))
                                       .Select(v => StringToCSVCell(Convert.ToString(v)));
                var line = string.Join(",", values);
                result.AppendLine(line);
            }

            return result.ToString();
        }

        private static string StringToCSVCell(string str)
        {
            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in str)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }
                sb.Append("\"");
                return sb.ToString();
            }

            return str;
        }
        #endregion

        #region :Convert List to XML File
        public static string ListToXML<T>(T ls)
        {
            using (StringWriter sw = new StringWriter(new StringBuilder()))
            {
                XmlSerializer sz = new XmlSerializer(typeof(T));
                sz.Serialize(sw, ls);
                return sw.ToString();
            }
        }
        #endregion

        #region :Convert Excel File to XML File
        /*
        //Create xml file from excel file
        public static void ExcelToXML(string excel_file_path_in, string xml_file_path_out)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = dal_DAL.getExcelData_ds(excel_file_path_in);
                ds.WriteXml(xml_file_path_out);
                //Console.WriteLine("Aiports xml file created successfully.");
            }
            catch (System.Exception ex)
            {
                string err = "Error: " + ex.Message;
                //Console.WriteLine("Unable to create airport xml file: \n" + ex.Message);
            }
        }
        */
        #endregion
    }
}
