const express = require("express");
const cors = require("cors");
const tools = require("./src/tools.js");
const cmd = require("./src/cmd.js");

const app = express();
require('dotenv').config();

const corsOpt = {
  origin: '*',
  credentials: true,
  optionSuccessStatus: 200
};

app.use(express.urlencoded({ extended: true }));
app.use(cors(corsOpt));
app.use(express.json());

app.post("/smb/delete", (req, res) => {
  const rootFolderPath = req.body.rootFolderPath;
  const personalFolderPath = req.body.personalFolderPath;
  const fileName = req.body.fileName;

  if (!rootFolderPath || !personalFolderPath || !fileName) {
    res.send(tools.logger("/smb/delete no rootFolderPath, personalFolderPath or fileName provided"));
    return;
  }

  let result = cmd.deleteFile(rootFolderPath, personalFolderPath, fileName);
  res.send(tools.logger(result));
});

app.listen(process.env.PORT, () => {
  cmd.kerberosAuth();
  tools.logger("Application started");
});