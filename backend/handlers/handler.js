const Document = require('./../document/document');
const MerkleTree = require('./../merkletree/merkletree');

async function addDocument(req, res) {
    const { documentOwner,
            documentPartner,
            hash,
            timestampOwner,
            signatureOwner,
            nicknameOwner,
            nicknamePartner,
            name } = req.body;
    const doc = await Document.addDocument(
        name,
        nicknameOwner,
        nicknamePartner,
        documentOwner,
        documentPartner,
        hash,
        2,
        signatureOwner,
        timestampOwner
        );
    const id = doc._id;
    if (id) {
        res.send({
            result: id,
            error: null
        });
    } else {
        res.send({
            result: null,
            error: "Can not add document"
        });
    }
}

async function getDocument(req, res) {
    const { id } = req.params;
    const doc = await Document.getDocumentByID(id);
    if (doc !== null) {
        res.set({
            result: {
                document: doc
            },
            error: null
        });
    } else {
        res.set({
            result: null,
            error: "Document does not exist"
        });
    }
}

async function updateSignatures(req, res) {
    const { id } = req.params;
    const {
        signaturePartner,
        timestampPartner
    } = req.body;
    await Document.updateDocumentSignatureAndTimestamp(id, signaturePartner, timestampPartner);
    const doc = await Document.getDocumentByID(id);
    await Document.pushTreeToDB(id, doc);
    if (doc !== null) {
        res.send({
            result: {
                signatureOwner: doc.signatureOwner,
                timestampOwner: doc.timestampOwner
            },
            error: null
        });
    } else {
        res.send({
            result: null,
            error: "Document does not exist"
        });
    }
}

async function getOwnerDocuments(req, res) {
    const { id } = req.params;
    const docs = Document.getDocumentByNicknameOwner(id);
    if (docs.length !== 0) {
        res.send({
            result: {
                documents: docs
            },
            error: null
        });
    } else {
        res.send({
            result: null,
            error: "User haven't documents"
        });
    }
}

async function getPartnerDocuments(req, res) {
    const { id } = req.params;
    const docs = Document.getDocumentByNicknamePartner(id);
    if (docs.length !== 0) {
        res.send({
            result: {
                documents: docs
            },
            error: null
        });
    } else {
        res.send({
            result: null,
            error: "User haven't ever sign documents"
        });
    }
}


async function getProof(req, res) {
    const { txNumber, hash } = req.params;
    const doc = await Document.getDocumentByHash(hash);
    if (doc === null) {
        res.send({
            result: null,
            error: "Document does not exist"
        });
        return;
    }
    const concatSignatures = doc.signatures.reduce((acc, val) => acc + val);
    const leaf = MerkleTree.SHA256(hash + concatSignatures);
    const { tree } = Document.getTree();
    const proof = MerkleTree.getProof(tree, leaf);
    if (txNumber == doc.index) {
        res.send({
            result: {
                proof: proof,
                document: doc
            }
        });
    } else {
        res.send({
            result: null,
            error: "Invalid tx number"
        });
    }
}

module.exports = {
  getDocument: getDocument,
  addDocument: addDocument,
  getProof: getProof,
  updateSignatures: updateSignatures,
  getOwnerDocuments: getOwnerDocuments,
  getPartnerDocuments: getPartnerDocuments
};