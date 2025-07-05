module.exports = { deleteFile, kerberosAuth }

const { exec } = require('child_process');

function deleteFile(folderPath, fileName) {
  let result = "Pending/No data";
  const command = `smbclient "${folderPath}" --use-kerberos=required -c "del '${fileName}'; exit;"`;
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
  // TODO: Вроде бы не надо будет если екзек из окружения сервера
}