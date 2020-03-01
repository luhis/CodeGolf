// eslint-disable-next-line @typescript-eslint/no-var-requires
const path = require("path");

// eslint-disable-next-line functional/immutable-data
module.exports = {
  process: (_, filename, __, ___) => "module.exports = " + JSON.stringify(path.basename(filename)) + ";"
};
