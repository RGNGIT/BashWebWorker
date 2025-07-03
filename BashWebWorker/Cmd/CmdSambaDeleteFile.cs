using BashWebWorker.Cmd.Interfaces;
using BashWebWorker.Tools;
using System.Diagnostics;

namespace BashWebWorker.Cmd
{
    public class CmdSambaDeleteFile: ICommand
    {
        public void Run(params object[] parameters) 
        {
            if (parameters.Length < 2)
            {
                Logger.Error("CmdSambaDeleteFile. Not enough params.");
                return;
            }

            var folderPath = parameters[0] as string;
            var fileName = parameters[1] as string;
            DeleteFileFromNetworkFolder(folderPath!, fileName!);
        }

        private void DeleteFileFromNetworkFolder(string folderPath, string fileName)
        {
            var unc = folderPath.TrimEnd('\\').Replace(@"\\", string.Empty);
            var parts = unc.Split('\\');
            if (parts.Length < 2)
            {
                Logger.Error($"DeleteFileFromNetworkFolder. Invalid network folder format: {folderPath}");
                return;
            }

            var server = parts[0];
            var share = parts[1];
            var pathInShare = string.Join("/", parts.Skip(2));

            try
            {
                var kinitProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/usr/bin/kinit",
                        Arguments = "-k -t /container/kerberos/krb5.keytab HTTP/hq-co1-dirtest.hq.corp.russneft.ru@HQ.CORP.RUSSNEFT.RU",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                kinitProcess.StartInfo.EnvironmentVariables["KRB5CCNAME"] = "FILE:/tmp/krb5cc";

                kinitProcess.Start();
                kinitProcess.WaitForExit();

                if (kinitProcess.ExitCode != 0)
                {
                    var error = kinitProcess.StandardError.ReadToEnd();
                    Logger.Error($"DeleteFileFromNetworkFolder. Kerberos auth failed: {error}");
                    return;
                }

                string EscapeSmbString(string s)
                {
                    return s.Replace("'", "'\"'\"'");
                }

                var escapedFileName = EscapeSmbString(fileName);
                var smbCommands = "";

                if (!string.IsNullOrEmpty(pathInShare))
                {
                    var escapedPath = EscapeSmbString(pathInShare);
                    smbCommands += $"cd '{escapedPath}'; ";
                }

                smbCommands += $"del '{escapedFileName}'; exit;";

                var smbService = $"//{server}/{share}";
                var smbProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/usr/bin/smbclient",
                        Arguments = $"\"{smbService}\" --use-kerberos=required --use-krb5-ccache=FILE:/tmp/krb5cc -c \"{smbCommands}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                smbProcess.Start();
                var output = smbProcess.StandardOutput.ReadToEnd();
                var errorOutput = smbProcess.StandardError.ReadToEnd();
                smbProcess.WaitForExit();

                if (smbProcess.ExitCode == 0)
                    Logger.Debug($"DeleteFileFromNetworkFolder. File deleted successfully via smbclient: {fileName}");
                else
                    Logger.Error($"DeleteFileFromNetworkFolder. smbclient failed to delete file '{fileName}'. Error: {errorOutput}");
            }
            catch (Exception ex)
            {
                Logger.Error($"DeleteFileFromNetworkFolder. Error deleting file: {fileName}. Exception: {ex.ToString()}");
            }
        }
    }
}
