module.exports = { deleteFile, kerberosAuth }

const { exec } = require('child_process');
const tools = require("./tools.js");
require("dotenv").config();

function deleteFile(rootFolderPath, personalFolderPath, fileName) {
  let result = "OK";
  const appDomainUser = "Svc-dir@HQ.CORP.RUSSNEFT.RU";

  tools.logger(`deleteFile. Deleting file ${rootFolderPath}/${personalFolderPath}/${fileName} DomainUser ${appDomainUser}`);
  const command = `smbclient -k '${rootFolderPath}' -U ${appDomainUser} -c 'cd "${personalFolderPath}"; del "${fileName}"; exit;'`;
  exec(command, (error, stdout, stderr) => {
    if (error)
      tools.logger(`exec error: ${error}`);
    if (stderr)
      tools.logger(`stderr: ${stderr}`);
    if (stdout)
      tools.logger(`stdout: ${stdout}`);
  });

  return result;
}

function kerberosAuth() {
  const keytab = "/etc/krb5.keytab";
  const principal = "HTTP/hq-co1-dirtest.hq.corp.russneft.ru@HQ.CORP.RUSSNEFT.RU";

  tools.logger(`kerberosAuth. Logging in with Keytab ${keytab} Principal ${principal}`);
  const command = `kinit -k -t ${keytab} ${principal}`;
  exec(command, (error, stdout, stderr) => {
    if (stdout)
      tools.logger(stdout);
    if (stderr)
      tools.logger(stderr);
    if (error)
      tools.logger(error);
  });
}
