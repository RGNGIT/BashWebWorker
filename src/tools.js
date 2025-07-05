module.exports = { logger }

function logger(log, dt = true) {
  var today = new Date();
  var date = today.getFullYear() + '-' + (
    today.getMonth() + 1
  ) + '-' + today.getDate();
  var time = today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();
  var dateTime = date + ' ' + time;
  console.log((dt ? '<' + dateTime + '> ' : '') + log);
  return log;
}