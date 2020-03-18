using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAL
{
    public class Init
    {
        public static string AppDataFolder { get; set; }
        public static string ErrorDataFolder { get; set; }

        public Init(string app_data_folder, string error_data_folder)
        {
            DirectoryInfo dirData = new DirectoryInfo(app_data_folder);
            if (!dirData.Exists)
            {
                dirData.Create();
            }

            DirectoryInfo dirError = new DirectoryInfo(error_data_folder);
            if (!dirError.Exists)
            {
                dirError.Create();
            }

            AppDataFolder = app_data_folder;
            ErrorDataFolder = error_data_folder;
        }

    }
}
