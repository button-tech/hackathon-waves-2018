function getDocument(req, res) {

}

function addDocument(req, res) {

}

async function getProof(req, res) {
    const body = req.params;

    const txNumber = body.txNumber;
    const hash = body.hash;
    const timestamp = body.timestamp;

    // res.statusCode = 200;
    // res.send(proof);
    //
    // res.statusCode = 400;
    // res.send(undefined);

}

module.exports = {
  getDocument: getDocument,
  addDocument: addDocument,
  getProof: getProof
};