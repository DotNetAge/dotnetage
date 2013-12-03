using DNA.Web.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace DNA.Web.ServiceModel.Jobs
{
    public class BackupToFtp : Job
    {
        public override string Title
        {
            get
            {
                return "Backup to FTP";
            }
        }

        public override string Descritpion
        {
            get
            {
                return "Backup DotNetAge data to FTP server.";
            }
        }

        public string SourcePath { get; set; }

        public string FTPServer { get; set; }

        public string FTPUser { get; set; }

        public string FTPPwd { get; set; }

        public string FTPRemoteFolder { get; set; }

        protected override void OnExecute()
        {
            //foreach (var 
            //UploadFtp(SourcePath 
        }

        public static int UploadFtp(string filePath, string filename, string ftpServerIP, string ftpUserID, string ftpPassword)
        {

            FileInfo fileInf = new FileInfo(filePath + "\\" + filename);
            string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
            FtpWebRequest reqFTP;

            // Create FtpWebRequest object from the Uri provided 
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" + fileInf.Name));

            try
            {

                // Provide the WebPermission Credintials 
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

                // By default KeepAlive is true, where the control connection is not closed 
                // after a command is executed. 
                reqFTP.KeepAlive = false;

                // Specify the command to be executed. 
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

                // Specify the data transfer type. 
                reqFTP.UseBinary = true;

                // Notify the server about the size of the uploaded file 
                reqFTP.ContentLength = fileInf.Length;

                // The buffer size is set to 2kb 
                int buffLength = 2048;

                byte[] buff = new byte[buffLength];
                int contentLen;

                // Opens a file stream (System.IO.FileStream) to read the file to be uploaded 
                //FileStream fs = fileInf.OpenRead(); 
                FileStream fs = fileInf.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // Stream to which the file to be upload is written 
                Stream strm = reqFTP.GetRequestStream();

                // Read from the file stream 2kb at a time 
                contentLen = fs.Read(buff, 0, buffLength);

                // Till Stream content ends 
                while (contentLen != 0)
                {
                    // Write Content from the file stream to the FTP Upload Stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }

                // Close the file stream and the Request Stream 
                strm.Close();
                fs.Close();
                return 0;
            }
            catch (Exception ex)
            {

                reqFTP.Abort();
                Logger.Error(ex);
                return -2;
            }

        }
    }
}
