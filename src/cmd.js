module.exports = { deleteFile, kerberosAuth }

const { exec } = require('child_process');
const tools = require("./tools.js");
require("dotenv").config();

function deleteFile(rootFolderPath, personalFolderPath, fileName) {
  let result = "Pending/No data";
  const appDomainUser = process.env.APP_DOMAIN_USER;
  const command = `smbclient -k '${rootFolderPath}' -U ${appDomainUser} -c 'cd "${personalFolderPath}"; del "${fileName}"; exit;'`;
  exec(command, (error, stdout, stderr) => {
    if (error) {
      result = `exec error: ${error.message}`;
      return;
    }

    if (stderr) {
      result = `stderr:\n${stderr}`;
      return;
    }

    result = `stdout:\n${stdout}`;
  });

  return result;
}

function kerberosAuth() {
  const keytab = process.env.KRB_KEYTAB;
  const principal = process.env.KRB_PRINCIPAL;

  const command = `kinit -k -t ${keytab} ${principal}`;
  exec(command, (error, stdout, stderr) => {
    if (stdout)
      tools.logger(stdout.message);
    if (stderr)
      tools.logger(stderr.message);
    if (error)
      tools.logger(error.message);
  });
}
