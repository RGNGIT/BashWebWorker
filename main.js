const express = require("express");
const cors = require("cors");
const tools = require("./src/tools.js");
const cmd = require("./src/cmd.js");

const app = express();

const corsOpt = {
  origin: '*',
  credentials: true,
  optionSuccessStatus: 200
};

app.use(express.urlencoded({ extended: true }));
app.use(cors(corsOpt));
app.use(express.json());

app.post("/smb/delete", (req, res) => {
  var folderPath = req.body.folderPath;
  var fileName = req.body.fileName;

  if (!folderPath || !fileName) {
    res.send(tools.logger("/smb/delete no folderPath or fileName provided"));
    return;
  }

  let result = cmd.deleteFile(folderPath, fileName);
  res.send(tools.logger(result));
});

app.listen(5928, () => {
  tools.logger("Application started");
});